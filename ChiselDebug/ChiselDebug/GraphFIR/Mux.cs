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

        public Mux(List<FIRIO> choises, Output decider, FirrtlNode defNode) : base(defNode)
        {
            choises = choises.Select(x => x.GetOutput()).ToList();
            if (!choises.All(x => x.IsPassiveOfType<Output>()))
            {
                throw new Exception("Inputs to mux must all be passive output types.");
            }

            this.Choises = choises.Select(x => x.Flip(this)).ToArray();
            this.Decider = new Input(this, decider.Type);
            this.Result = choises.First().Copy(this);
            Result.SetName(null);

            for (int i = 0; i < Choises.Length; i++)
            {
                Choises[i].SetName(null);
                choises[i].ConnectToInput(Choises[i]);
            }
            decider.ConnectToInput(Decider);

            AddOneToManyPairedIO(Result, Choises.ToList());
        }

        public override ScalarIO[] GetInputs()
        {
            List<ScalarIO> inputs = new List<ScalarIO>();
            inputs.AddRange(Choises.SelectMany(x => x.Flatten()));
            inputs.Add(Decider);
            return inputs.ToArray();
        }

        public override ScalarIO[] GetOutputs()
        {
            return Result.Flatten().ToArray();
        }

        public override FIRIO[] GetIO()
        {
            List<FIRIO> io = new List<FIRIO>();
            io.AddRange(Choises);
            io.Add(Decider);
            io.Add(Result);

            return io.ToArray();
        }

        public override void Compute()
        {
            Debug.Assert(Choises.Length <= 2, "Only support multiplexer with two choises");

            FIRIO ChosenInput;
            if (Decider.GetEnabledCon().Value.IsTrue())
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
                        BinaryVarValue binValue = output.Con.Value.GetValue();
                        Array.Fill(binValue.Bits, BitState.X);
                    }

                    return;
                }

                ChosenInput = Choises.Last();
            }

            Input[] from = ChosenInput.Flatten().Cast<Input>().ToArray();
            Output[] to = Result.Flatten().Cast<Output>().ToArray();
            Debug.Assert(from.Length == to.Length);

            for (int i = 0; i < from.Length; i++)
            {
                BinaryVarValue fromBin = from[i].GetEnabledCon().Value.GetValue();
                BinaryVarValue toBin = to[i].Con.Value.GetValue();

                toBin.SetBitsAndExtend(fromBin, from[i].Type is SIntType);
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
