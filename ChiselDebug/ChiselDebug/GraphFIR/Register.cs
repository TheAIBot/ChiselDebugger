using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.GraphFIR
{
    public class Register : FIRRTLNode
    {
        public readonly string Name;
        public readonly Input In;
        public readonly Input? Clock;
        public readonly Input? Reset;
        public readonly Input Init;
        public readonly Output Result;

        public Register(string name, Output clock, Output? reset, Output? init, IFIRType outType)
        {
            this.Name = name;
            this.In = new Input(this, name + "/in", outType);
            this.Clock = new Input(this, clock.Type);
            clock.ConnectToInput(Clock);

            if (reset != null)
            {
                this.Reset = new Input(this, reset.Type);
                reset.ConnectToInput(Reset);
            }

            if (init != null)
            {
                this.Init = new Input(this, init.Type);
                init.ConnectToInput(Init);
            }

            this.Result = new Output(this, Name, outType);
        }

        internal IOBundle GetIOAsBundle()
        {
            return new RegisterIO(Name, In, Result);
        }

        public override ScalarIO[] GetInputs()
        {
            List<ScalarIO> inputs = new List<ScalarIO>();
            inputs.Add(In);
            inputs.Add(Clock);
            if (Reset != null)
            {
                inputs.Add(Reset);
            }
            if (Init != null)
            {
                inputs.Add(Init);
            }
            return inputs.ToArray();
        }

        public override ScalarIO[] GetOutputs()
        {
            return new ScalarIO[] { Result };
        }

        public override FIRIO[] GetIO()
        {
            List<FIRIO> io = new List<FIRIO>();
            io.Add(In);
            io.Add(Clock);
            if (Reset != null)
            {
                io.Add(Reset);
            }
            if (Init != null)
            {
                io.Add(Init);
            }
            io.Add(Result);

            return io.ToArray();
        }

        public override void InferType()
        { }
    }

    public class RegisterIO : IOBundle
    {
        private readonly FIRIO RegIn;
        private readonly FIRIO RegOut;

        public RegisterIO(string name, FIRIO regIn, FIRIO regOut) : base(name, new List<FIRIO>() { regIn, regOut }, false)
        {
            this.RegIn = regIn;
            this.RegOut = regOut;
        }

        public override FIRIO GetInput()
        {
            return RegIn;
        }

        public override FIRIO GetOutput()
        {
            return RegOut;
        }
    }
}
