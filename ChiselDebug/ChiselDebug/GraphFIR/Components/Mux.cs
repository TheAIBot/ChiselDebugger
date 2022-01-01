﻿using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VCDReader;

namespace ChiselDebug.GraphFIR.Components
{
    public sealed class Mux : PairedIOFIRRTLNode
    {
        public readonly FIRIO[] Choises;
        public readonly Sink Decider;
        public readonly FIRIO Result;
        public readonly bool IsVectorIndexer;
        public readonly Vector ChoisesVec;

        private readonly Sink[] ChoiseInputs;
        private readonly Source[] ResultOutputs;
        private readonly BinaryVarValue[] UnknownOutputs;
        private bool CanOverrideResult = false;

        public Mux(List<FIRIO> choises, Source decider, FirrtlNode defNode, bool isVectorIndexer = false) : base(defNode)
        {
            choises = choises.Select(x => x.GetSource()).ToList();
            if (!choises.All(x => x.IsPassiveOfType<Source>()))
            {
                throw new Exception("Inputs to mux must all be passive output types.");
            }

            Choises = choises.Select(x => x.Flip(this)).ToArray();
            Decider = new Sink(this, decider.Type);
            Result = choises.First().Copy(this);
            IsVectorIndexer = isVectorIndexer;
            foreach (var res in Result.Flatten())
            {
                res.RemoveType();
            }

            // IO need to be connected to a parent vector so
            // aggregate io lines can be drawn to a mux
            ChoisesVec = new Vector(this, string.Empty, Choises.ToArray());

            for (int i = 0; i < Choises.Length; i++)
            {
                choises[i].ConnectToInput(Choises[i]);
            }
            decider.ConnectToInput(Decider);

            AddOneToManyPairedIO(Result, Choises.ToList());

            ChoiseInputs = Choises.SelectMany(x => x.Flatten().Cast<Sink>()).ToArray();
            ResultOutputs = Result.Flatten().Cast<Source>().ToArray();
            UnknownOutputs = new BinaryVarValue[ResultOutputs.Length];
        }

        public override Sink[] GetSinks()
        {
            List<Sink> inputs = new List<Sink>();
            inputs.Add(Decider);
            inputs.AddRange(ChoiseInputs);
            return inputs.ToArray();
        }

        public override Source[] GetSources()
        {
            return ResultOutputs.ToArray();
        }

        public override IEnumerable<FIRIO> GetIO()
        {
            yield return ChoisesVec;
            yield return Decider;
            yield return Result;
        }

        public override void Compute()
        {
            ref BinaryVarValue deciderValue = ref Decider.UpdateValueFromSourceFast();

            //If decidor isn't binary then output can't be chosen
            //so therefore it's set to undecided
            if (!deciderValue.IsValidBinary)
            {
                SetResultToUnknown();
                return;
            }



            ReadOnlySpan<Sink> ChosenInputs;
            if (!IsVectorIndexer)
            {
                Debug.Assert(Choises.Length <= 2, "Only support multiplexer with two choises");

                if (deciderValue.Bits[0] == BitState.One)
                {
                    ChosenInputs = ChoiseInputs.AsSpan(0, ResultOutputs.Length);
                }
                else
                {
                    //Conditionally valid
                    if (Choises.Length == 1)
                    {
                        SetResultToUnknown();
                        return;
                    }

                    ChosenInputs = ChoiseInputs.AsSpan(ResultOutputs.Length);
                }
            }
            else
            {
                int index = deciderValue.AsInt();
                ChosenInputs = ChoiseInputs.AsSpan(ResultOutputs.Length * index, ResultOutputs.Length);
            }

            if (CanOverrideResult)
            {
                for (int i = 0; i < ResultOutputs.Length; i++)
                {
                    ResultOutputs[i].Value.OverrideValue(ref ChosenInputs[i].UpdateValueFromSourceFast());
                }
            }
            else
            {
                for (int i = 0; i < ResultOutputs.Length; i++)
                {
                    ref BinaryVarValue fromBin = ref ChosenInputs[i].UpdateValueFromSourceFast();
                    ref BinaryVarValue toBin = ref ResultOutputs[i].GetValue();

                    toBin.SetBitsAndExtend(ref fromBin, ChosenInputs[i].Value.IsSigned);
                }
            }
        }

        private void SetResultToUnknown()
        {
            if (CanOverrideResult)
            {
                for (int i = 0; i < ResultOutputs.Length; i++)
                {
                    ResultOutputs[i].Value.OverrideValue(ref UnknownOutputs[i]);
                }
            }
            else
            {
                foreach (Source output in ResultOutputs)
                {
                    output.GetValue().SetAllUnknown();
                }
            }
        }

        internal override void InferType()
        {
            foreach (Sink input in GetSinks())
            {
                input.InferType();
            }
            foreach (Source output in GetSources())
            {
                output.InferType();
            }

            for (int i = 0; i < UnknownOutputs.Length; i++)
            {
                UnknownOutputs[i] = new BinaryVarValue(ResultOutputs[i].Type.Width, false);
                UnknownOutputs[i].SetAllUnknown();
            }

            //If no input is extended to fit its output then it's possible
            //to directly point to the input values instead of copying
            //its value to the output
            bool noExtension = true;
            for (int i = 0; i < ResultOutputs.Length; i++)
            {
                for (int x = 0; x < Choises.Length; x++)
                {
                    if (ResultOutputs[i].Type.Width != ChoiseInputs[i + x * ResultOutputs.Length].Type.Width)
                    {
                        noExtension = false;
                        break;
                    }
                }
            }

            if (noExtension)
            {
                CanOverrideResult = true;
            }
        }
    }
}
