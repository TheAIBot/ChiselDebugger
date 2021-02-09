using System.Collections.Generic;

namespace ChiselDebug.FIRRTL
{
    public class FIRRTLContainer : FIRRTLNode
    {
        public readonly List<Input> ExternalInputs = new List<Input>();
        public readonly List<Output> ExternalOutputs = new List<Output>();

        public readonly List<Input> InternalInputs = new List<Input>();
        public readonly List<Output> InternalOutputs = new List<Output>();       

        public void AddExternalInput(string inputName)
        {
            ExternalInputs.Add(new Input(inputName));
            InternalOutputs.Add(new Output(inputName));
        }

        public void AddExternalOutput(string outputName)
        {
            ExternalOutputs.Add(new Output(outputName));
            InternalInputs.Add(new Input(outputName));
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
    }
}
