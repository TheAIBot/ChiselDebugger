using FIRRTL;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR
{
    public class FIRRTLContainer : FIRRTLNode
    {
        public readonly List<Input> ExternalInputs = new List<Input>();
        public readonly List<Output> ExternalOutputs = new List<Output>();

        public readonly List<Input> InternalInputs = new List<Input>();
        public readonly List<Output> InternalOutputs = new List<Output>();       

        public void AddExternalInput(string inputName, IFIRType type)
        {
            ExternalInputs.Add(new Input(inputName, type));
            InternalOutputs.Add(new Output(inputName, type));
        }

        public void AddExternalOutput(string outputName, IFIRType type)
        {
            ExternalOutputs.Add(new Output(outputName, type));
            InternalInputs.Add(new Input(outputName, type));
        }

        public void PropagateSignals()
        {
            for (int i = 0; i < ExternalInputs.Count; i++)
            {
                InternalOutputs[i].Con.Value = ExternalInputs[i].Con.Value;
            }

            for (int i = 0; i < ExternalOutputs.Count; i++)
            {
                ExternalOutputs[i].Con.Value = InternalInputs[i].Con.Value;
            }
        }

        public override Input[] GetInputs()
        {
            return ExternalInputs.ToArray();
        }

        public override Output[] GetOutputs()
        {
            return ExternalOutputs.ToArray();
        }
    }
}
