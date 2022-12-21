using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#nullable enable

namespace ChiselDebug.GraphFIR.Components
{
    public sealed class Wire : PairedIOFIRRTLNode
    {
        public readonly string Name;
        public readonly FIRIO In;
        public readonly FIRIO Result;

        public Wire(string name, FIRIO inputType, FirrtlNode? defNode) : base(defNode)
        {
            if (!inputType.IsPassiveOfType<Sink>())
            {
                throw new Exception("Wire input type must be a passive input type.");
            }

            Name = name;
            In = inputType.Copy(this);
            Result = inputType.Flip(this);
            AddPairedIO(In, Result);

            In.SetName(Name + "/in");
            Result.SetName(Name);
        }

        internal void BypassWireIO()
        {
            IOHelper.BypassWire(GetSinks(), GetSources());
            Debug.Assert(In.Flatten().All(x => !x.IsConnectedToAnything()));
            Debug.Assert(Result.Flatten().All(x => !x.IsConnectedToAnything()));
        }

        internal DuplexIO GetAsDuplex()
        {
            return new DuplexIO(this, Name, In, Result);
        }

        public override Sink[] GetSinks()
        {
            return In.FlattenTo<Sink>();
        }

        public override Source[] GetSources()
        {
            return Result.FlattenTo<Source>();
        }

        public override IEnumerable<FIRIO> GetIO()
        {
            yield return In;
            yield return Result;
        }

        public override IEnumerable<FIRIO> GetVisibleIO()
        {
            yield return GetAsDuplex();
        }

        public override void Compute()
        {
            //throw new Exception("This node is not computable");
        }

        internal override void InferType()
        {
            foreach (Sink input in GetSinks())
            {
                input.InferType();
            }
        }
    }
}
