using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR
{
    public class Mux : FIRRTLNode
    {
        public readonly FIRIO[] Choises;
        public readonly Input Decider;
        public readonly FIRIO Result;

        public Mux(List<FIRIO> choises, Output decider, IFIRType outType)
        {
            choises = choises.Select(x => x.GetOutput()).ToList();
            choises.ForEach(x => x.Flatten().ToList().ForEach(y => y.InferType()));
            if (!choises.All(x => x.IsPassiveOfType<Output>()))
            {
                throw new Exception("Inputs to mux must all be passive output types.");
            }
            if (!choises.All(x => x.SameIO(choises.First())))
            {
                throw new Exception("All inputs to mux must be of the same type.");
            }

            this.Choises = choises.Select(x => x.Flip(this)).ToArray();
            this.Decider = new Input(this, new FIRRTL.UIntType(1));
            this.Result = choises.First().Copy(this);
            Result.SetName(null);

            for (int i = 0; i < Choises.Length; i++)
            {
                Choises[i].SetName(null);
                choises[i].ConnectToInput(Choises[i]);
            }
            decider.ConnectToInput(Decider);
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

        public override void InferType()
        {
            foreach (Input input in GetInputs())
            {
                input.InferType();
            }
        }
    }
}
