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
        public readonly FIRIO In;
        public readonly FIRIO Result;
        public readonly Input? Clock;
        public readonly Input? Reset;
        public readonly FIRIO? Init;

        public Register(string name, FIRIO inputType, Output clock, Output? reset, FIRIO? init)
        {
            if (!inputType.IsPassiveOfType<Input>())
            {
                throw new Exception("Register must be a passive input type.");
            }

            this.Name = name;
            this.In = inputType.Copy(this);
            In.SetName(Name + "/in");

            this.Result = In.Flip(this);
            Result.SetName(Name);

            this.Clock = new Input(this, clock.Type);
            clock.ConnectToInput(Clock);

            if (reset != null)
            {
                this.Reset = new Input(this, reset.Type);
                reset.ConnectToInput(Reset);
            }

            if (init != null)
            {
                if (!init.IsPassiveOfType<Output>())
                {
                    throw new Exception("Register init must be of a passive output type.");
                }

                this.Init = init.Flip(this);
                init.ConnectToInput(Init);
            }
        }

        internal IOBundle GetIOAsBundle()
        {
            return new RegisterIO(Name, In, Result);
        }

        public override ScalarIO[] GetInputs()
        {
            List<ScalarIO> inputs = new List<ScalarIO>();
            inputs.AddRange(In.Flatten());
            inputs.Add(Clock);
            if (Reset != null)
            {
                inputs.Add(Reset);
            }
            if (Init != null)
            {
                inputs.AddRange(Init.Flatten());
            }
            return inputs.ToArray();
        }

        public override ScalarIO[] GetOutputs()
        {
            return Result.Flatten().ToArray();
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
