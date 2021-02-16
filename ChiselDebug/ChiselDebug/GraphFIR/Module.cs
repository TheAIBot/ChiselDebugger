using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR
{
    public class Module : FIRRTLContainer
    {
        public readonly string Name;
        public List<FIRRTLPrimOP> PrimOps = new List<FIRRTLPrimOP>();

        public Module(string name)
        {
            this.Name = name;
        }

        public void AddPrimOp(FIRRTLPrimOP op)
        {
            PrimOps.Add(op);
        }

        private Output[] FindOutputs(string[] outputNames)
        {
            Output[] outputs = new Output[outputNames.Length];
            for (int i = 0; i < outputs.Length; i++)
            {
                string outputName = outputNames[i];

                Output modOutput = InternalOutputs.FirstOrDefault(x => x.Name == outputName);
                if (modOutput != default(Output))
                {
                    outputs[i] = modOutput;
                    continue;
                }

                Output opOutput = PrimOps.FirstOrDefault(x => x.Result.Name == outputName)?.Result;
                if (opOutput != default(Output))
                {
                    outputs[i] = opOutput;
                    continue;
                }

                throw new Exception($"Failed to find output with name {outputName}.");
            }

            return outputs;
        }

        private Input[] FindInputs(string[] inputNames)
        {
            Input[] inputs = new Input[inputNames.Length];
            for (int i = 0; i < inputNames.Length; i++)
            {
                string inputName = inputNames[i];

                Input modInput = InternalInputs.FirstOrDefault(x => x.Name == inputName);
                if (modInput != default(Input))
                {
                    inputs[i] = modInput;
                    continue;
                }

                throw new Exception($"Failed to find input with name {inputName}.");
            }

            return inputs;
        }

        public void Connect(string[] from, FIRRTLPrimOP op)
        {
            op.ConnectFrom(FindOutputs(from));
        }

        public void Connect(string[] from, string[] to)
        {
            Output[] outputs = FindOutputs(from);
            Input[] inputs = FindInputs(to);

            for (int i = 0; i < outputs.Length; i++)
            {
                outputs[i].ConnectToInput(inputs[i]);
            }
        }

        public List<Connection> GetAllModuleConnections()
        {
            List<Connection> connestions = new List<Connection>();
            foreach (var output in InternalOutputs)
            {
                if (output.Con.IsUsed())
                {
                    connestions.Add(output.Con);
                }
            }

            foreach (var op in PrimOps)
            {
                if (op.Result.Con.IsUsed())
                {
                    connestions.Add(op.Result.Con);
                }
            }

            return connestions;
        }

        public FIRRTLNode[] GetAllNodes()
        {
            FIRRTLNode[] nodes = new FIRRTLNode[PrimOps.Count];
            for (int i = 0; i < PrimOps.Count; i++)
            {
                nodes[i] = PrimOps[i];
            }

            return nodes;
        }
    }
}
