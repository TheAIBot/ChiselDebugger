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
            this.In = new Input();
            this.Clock = new Input(clock.Type);
            if (reset != null)
            {
                this.Reset = new Input(reset.Type);
            }
            if (init != null)
            {
                this.Init = new Input(init.Type);
            }
            this.Result = new Output(Name, outType);
        }

        public override Input[] GetInputs()
        {
            List<Input> inputs = new List<Input>();
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

        public override Output[] GetOutputs()
        {
            return new Output[] { Result };
        }
    }
}
