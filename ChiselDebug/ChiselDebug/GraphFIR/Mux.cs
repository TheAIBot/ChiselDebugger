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
        }

        public override Input[] GetInputs()
        {
            List<Input> inputs = new List<Input>();
            inputs.AddRange(Choises.SelectMany(x => x.Flatten()).Cast<Input>());
            inputs.Add(Decider);
            return inputs.ToArray();
        }

        public override Output[] GetOutputs()
        {
            return Result.Flatten().Cast<Output>().ToArray();
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

        public override void Compute()
        {
            //First pull all results toward the mux
            Decider.UpdateValueFromSource();
            List<Input> foundInputs = new List<Input>();
            foreach (var choise in Choises)
            {
                foundInputs.Clear();
                foreach (var input in choise.GetAllIOOfType(foundInputs))
                {
                    input.UpdateValueFromSource();
                }
            }

            //If decidor isn't binary then output can't be chosen
            //so therefore it's set to undecided
            if (!Decider.GetValue().IsValidBinary())
            {
                foreach (var output in Result.Flatten())
                {
                    output.GetValue().Bits.Fill(BitState.X);
                }
                return;
            }



            FIRIO ChosenInput;
            if (!IsVectorIndexer)
            {
                Debug.Assert(Choises.Length <= 2, "Only support multiplexer with two choises");

                if (Decider.Value.IsTrue())
                {
                    ChosenInput = Choises.First();
                }
                else
                {
                    //Conditionally valid
                    if (Choises.Length == 1)
                    {
                        foreach (Output output in Result.Flatten())
                        {
                            ref readonly BinaryVarValue binValue = ref output.GetValue();
                            binValue.Bits.Fill(BitState.X);
                        }

                        return;
                    }

                    ChosenInput = Choises.Last();
                }
            }
            else
            {
                ChosenInput = Choises[Decider.GetValue().AsInt()];
            }

            Input[] from = ChosenInput.Flatten().Cast<Input>().ToArray();
            Output[] to = Result.Flatten().Cast<Output>().ToArray();
            Debug.Assert(from.Length == to.Length);

            for (int i = 0; i < from.Length; i++)
            {
                from[i].UpdateValueFromSource();
                ref readonly BinaryVarValue fromBin = ref from[i].GetValue();
                ref readonly BinaryVarValue toBin = ref to[i].GetValue();

                toBin.SetBitsAndExtend(in fromBin, from[i].Type is SIntType);
            }
        }

        internal override void InferType()
        {
            foreach (Input input in GetInputs())
            {
                input.InferType();
            }
        }
    }
}
