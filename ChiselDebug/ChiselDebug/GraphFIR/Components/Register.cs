#nullable enable

using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.Components
{
    public sealed class Register : PairedIOFIRRTLNode, IStatePreserving
    {
        public readonly string Name;
        public readonly FIRIO In;
        public readonly FIRIO Result;
        public readonly Input Clock;
        public readonly Input? Reset;
        public readonly FIRIO? Init;

        public Register(string name, FIRIO inputType, Output clock, Output? reset, FIRIO? init, FirrtlNode defNode) : base(defNode)
        {
            if (!inputType.IsPassiveOfType<Input>())
            {
                throw new Exception("Register must be a passive input type.");
            }

            Name = name;
            In = inputType.Copy(this);
            In.SetName(Name + "/in");

            Result = In.Flip(this);
            Result.SetName(Name);

            Clock = new Input(this, "clock", clock.Type);
            clock.ConnectToInput(Clock);

            if (reset != null)
            {
                Reset = new Input(this, "reset", reset.Type);
                reset.ConnectToInput(Reset);
            }

            if (init != null)
            {
                if (!init.IsPassiveOfType<Output>())
                {
                    throw new Exception("Register init must be of a passive output type.");
                }

                Init = init.Flip(this);
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

        public IEnumerable<FIRIO> GetAllIO()
        {
            yield return GetAsDuplex();
            yield return Clock;
            if (Reset != null)
            {
                yield return Reset;
            }
            if (Init != null)
            {
                yield return Init;
            }
        }

        public override Input[] GetInputs()
        {
            return GetAllIO().SelectMany(x => x.Flatten())
                             .OfType<Input>()
                             .ToArray();
        }

        public override Output[] GetOutputs()
        {
            return GetAllIO().SelectMany(x => x.Flatten())
                             .OfType<Output>()
                             .ToArray();
        }

        public override IEnumerable<FIRIO> GetIO()
        {
            foreach (var io in GetAllIO())
            {
                yield return io;
            }
        }

        public override IEnumerable<FIRIO> GetVisibleIO()
        {
            yield return new DuplexIO(this, In.Name, In, Result);
            yield return new DuplexIO(this, Result.Name, In, Result);
        }

        public override void Compute()
        {
            throw new Exception("This node is not computable");
        }

        internal override void InferType()
        {
            foreach (var input in GetInputs())
            {
                input.InferType();
            }
            foreach (var output in GetOutputs())
            {
                output.InferType();
            }
        }
    }
}
