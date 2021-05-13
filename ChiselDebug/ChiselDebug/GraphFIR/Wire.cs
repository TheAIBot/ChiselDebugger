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
            IOHelper.BypassIO(In, Result);
            Debug.Assert(In.Flatten().All(x => !x.IsConnectedToAnything()));
            Debug.Assert(Result.Flatten().All(x => !x.IsConnectedToAnything()));
        }

        internal bool CanBypassWire()
        {
            int portCount = 0;
            portCount += In.GetAllIOOfType<IPortsIO>().Select(x => x.GetAllPorts().Length).Sum();
            portCount += Result.GetAllIOOfType<IPortsIO>().Select(x => x.GetAllPorts().Length).Sum();

            //No idea how to visualize a Wire with a vector as input/output and a vectoraccess
            //on the input that changes an index and a vectoracces on the output that selects
            //a single index from the vector.
            return portCount == 0;
        }

        internal DuplexIO GetAsDuplex()
        {
            return new DuplexIO(this, Name, In, Result);
        }

        public override Input[] GetInputs()
        {
            List<Input> inputs = new List<Input>();
            inputs.AddRange(In.Flatten().OfType<Input>());
            inputs.AddRange(Result.Flatten().OfType<Input>());
            return inputs.ToArray();
        }

        public override Output[] GetOutputs()
        {
            List<Output> outputs = new List<Output>();
            outputs.AddRange(In.Flatten().OfType<Output>());
            outputs.AddRange(Result.Flatten().OfType<Output>());
            return outputs.ToArray();
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
