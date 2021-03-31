using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR
{
    public class Wire : FIRRTLNode
    {
        public readonly string Name;
        public readonly FIRIO In;
        public readonly FIRIO Result;

        public Wire(string name, FIRIO inputType)
        {
            this.Name = name;
            this.In = inputType.Copy(this);
            this.Result = inputType.Flip(this);

            In.SetName(Name + "/in");
            Result.SetName(Name);
        }

        internal void BypassWireIO()
        {
            IOHelper.BypassIO(In, Result);
        }

        internal IOBundle GetIOAsBundle()
        {
            return new WireIO(Name, In, Result);
        }

        public override ScalarIO[] GetInputs()
        {
            return In.Flatten().ToArray();
        }

        public override ScalarIO[] GetOutputs()
        {
            return Result.Flatten().ToArray();
        }

        public override FIRIO[] GetIO()
        {
            return new FIRIO[] { In, Result };
        }

        public override void InferType()
        {
            foreach (Input input in GetInputs())
            {
                input.InferType();
            }
        }
    }

        public class WireIO : IOBundle
    {
        private readonly FIRIO WireIn;
        private readonly FIRIO WireOut;

        public WireIO(string name, FIRIO wireIn, FIRIO wireOut) : base(name, new List<FIRIO>() { wireIn, wireOut }, false)
        {
            this.WireIn = wireIn;
            this.WireOut = wireOut;
        }

        public override FIRIO GetInput()
        {
            return WireIn;
        }

        public override FIRIO GetOutput()
        {
            return WireOut;
        }
    }
}
