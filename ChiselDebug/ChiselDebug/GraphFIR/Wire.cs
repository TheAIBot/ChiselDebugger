using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChiselDebug.GraphFIR
{
    public class Wire : PairedIOFIRRTLNode
    {
        public readonly string Name;
        public readonly FIRIO In;
        public readonly FIRIO Result;

        public Wire(string name, FIRIO inputType, FirrtlNode defNode) : base(defNode)
        {
            if (!inputType.IsPassiveOfType<Input>())
            {
                throw new Exception("Wire input type must be a passive input type.");
            }

            this.Name = name;
            this.In = inputType.Copy(this);
            this.Result = inputType.Flip(this);
            AddPairedIO(In, Result);

            In.SetName(Name + "/in");
            Result.SetName(Name);
        }

        internal void BypassWireIO()
        {
            IOHelper.BypassWire(GetInputs(), GetOutputs());
            Debug.Assert(In.Flatten().All(x => !x.IsConnectedToAnything()));
            Debug.Assert(Result.Flatten().All(x => !x.IsConnectedToAnything()));
        }

        internal DuplexIO GetAsDuplex()
        {
            return new DuplexIO(this, Name, In, Result);
        }

        public override Input[] GetInputs()
        {
            return In.Flatten().Cast<Input>().ToArray();
        }

        public override Output[] GetOutputs()
        {
            return Result.Flatten().Cast<Output>().ToArray();
        }

        public override IEnumerable<FIRIO> GetIO()
        {
            yield return In;
            yield return Result;
        }

        public override void Compute()
        {
            //throw new Exception("This node is not computable");
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
