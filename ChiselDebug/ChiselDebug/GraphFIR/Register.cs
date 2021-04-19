using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.GraphFIR
{
    public class Register : PairedIOFIRRTLNode
    {
        public readonly string Name;
        public readonly FIRIO In;
        public readonly FIRIO Result;
        public readonly Input? Clock;
        public readonly Input? Reset;
        public readonly FIRIO? Init;

        public Register(string name, FIRIO inputType, Output clock, Output? reset, FIRIO? init, FirrtlNode defNode) : base(defNode)
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

            this.Clock = new Input(this, "clock", clock.Type);
            clock.ConnectToInput(Clock);

            if (reset != null)
            {
                this.Reset = new Input(this, "reset", reset.Type);
                reset.ConnectToInput(Reset);
            }

            if (init != null)
            {
                if (!init.IsPassiveOfType<Output>())
                {
                    throw new Exception("Register init must be of a passive output type.");
                }

                this.Init = init.Flip(this);
                Init.SetName("init");
                init.ConnectToInput(Init);
            }

            List<FIRIO> pairs = new List<FIRIO>();
            pairs.Add(In);
            if (Init != null)
            {
                pairs.Add(Init);
            }
            AddOneToManyPairedIO(Result, pairs);
        }

        internal DuplexIO GetAsDuplex()
        {
            return new DuplexIO(this, Name, In, Result);
        }

        private List<FIRIO> GetAllIO()
        {
            List<FIRIO> io = new List<FIRIO>();
            io.Add(GetAsDuplex());
            io.Add(Clock);
            if (Reset != null)
            {
                io.Add(Reset);
            }
            if (Init != null)
            {
                io.Add(Init);
            }

            return io;
        }

        public override ScalarIO[] GetInputs()
        {
            return GetAllIO().SelectMany(x => x.Flatten())
                             .OfType<Input>()
                             .ToArray();
        }

        public override ScalarIO[] GetOutputs()
        {
            return GetAllIO().SelectMany(x => x.Flatten())
                             .OfType<Output>()
                             .ToArray();
        }

        public override FIRIO[] GetIO()
        {
            return GetAllIO().ToArray();
        }

        public override void Compute()
        {
            throw new Exception("This node is not computable");
        }

        public override void InferType()
        { }
    }
}
