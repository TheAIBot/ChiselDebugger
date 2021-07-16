using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VCDReader;

namespace ChiselDebug.GraphFIR
{
    public class Mux : PairedIOFIRRTLNode
    {
        public readonly FIRIO[] Choises;
        public readonly Input Decider;
        public readonly FIRIO Result;
        public readonly bool IsVectorIndexer;

        private readonly Input[] ChoiseInputs;
        private readonly bool[] ChoiseIsSInt;
        private readonly Output[] ResultOutputs;

        public Mux(List<FIRIO> choises, Output decider, FirrtlNode defNode, bool isVectorIndexer = false) : base(defNode)
        {
            choises = choises.Select(x => x.GetOutput()).ToList();
            if (!choises.All(x => x.IsPassiveOfType<Output>()))
            {
                throw new Exception("Inputs to mux must all be passive output types.");
            }

            this.Choises = choises.Select(x => x.Flip(this)).ToArray();
            this.Decider = new Input(this, decider.Type);
            this.Result = choises.First().Copy(this);
            this.IsVectorIndexer = isVectorIndexer;
            Result.SetName(null);
            foreach (var res in Result.Flatten())
            {
                res.RemoveType();
            }

            for (int i = 0; i < Choises.Length; i++)
            {
                Choises[i].SetName(null);
                choises[i].ConnectToInput(Choises[i]);
            }
            decider.ConnectToInput(Decider);

            AddOneToManyPairedIO(Result, Choises.ToList());

            this.ChoiseInputs = Choises.SelectMany(x => x.Flatten().Cast<Input>()).ToArray();
            this.ResultOutputs = Result.Flatten().Cast<Output>().ToArray();
            this.ChoiseIsSInt = new bool[ResultOutputs.Length];
        }

        public override Input[] GetInputs()
        {
            List<Input> inputs = new List<Input>();
            inputs.Add(Decider);
            inputs.AddRange(ChoiseInputs);
            return inputs.ToArray();
        }

        public override Output[] GetOutputs()
        {
            return ResultOutputs.ToArray();
        }

        public override IEnumerable<FIRIO> GetIO()
        {
            foreach (var io in Choises)
            {
                yield return io;
            }

            yield return Decider;
            yield return Result;
        }

        public override IEnumerable<FIRIO> GetVisibleIO()
        {
            yield return Result;
        }

        public override void Compute()
        {
            ref BinaryVarValue deciderValue = ref Decider.UpdateValueFromSourceFast();

            //If decidor isn't binary then output can't be chosen
            //so therefore it's set to undecided
            if (!deciderValue.IsValidBinary)
            {
                foreach (var output in ResultOutputs)
                {
                    output.GetValue().SetAllUnknown();
                }
                return;
            }



            ReadOnlySpan<Input> ChosenInputs;
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
                        foreach (Output output in ResultOutputs)
                        {
                            output.GetValue().SetAllUnknown();
                        }

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

            for (int i = 0; i < ResultOutputs.Length; i++)
            {
                ref BinaryVarValue fromBin = ref ChosenInputs[i].UpdateValueFromSourceFast();
                ref BinaryVarValue toBin = ref ResultOutputs[i].GetValue();

                toBin.SetBitsAndExtend(ref fromBin, ChoiseIsSInt[i]);
            }
        }

        internal override void InferType()
        {
            foreach (Input input in GetInputs())
            {
                input.InferType();
            }

            var firstChoise = Choises.First().Flatten().Cast<Input>().ToArray();
            for (int i = 0; i < firstChoise.Length; i++)
            {
                ChoiseIsSInt[i] = firstChoise[i].Type is SIntType;
            }
        }
    }
}
