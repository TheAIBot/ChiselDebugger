using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCDReader;

namespace ChiselDebug.GraphFIR
{
    public sealed class VectorAssign : FIRRTLNode
    {
        private readonly Vector VecIn;
        private readonly Input Index;
        private readonly FIRIO Value;
        private readonly Vector VecOut;
        private readonly Input[] VecInputs;
        private readonly Output[] VecOutputs;
        private readonly Input[] ValueInputs;

        public VectorAssign(Vector input, Output index, Output condition, FirrtlNode defNode) : base(defNode)
        {
            if (!input.IsPassiveOfType<Input>())
            {
                throw new Exception("Vector assign input must be a passive input type.");
            }

            this.VecIn = (Vector)input.Copy(this);
            this.Index = (Input)index.Flip(this);
            this.Value = VecIn.GetIndex(0).Copy(this);
            this.VecOut = (Vector)input.Flip(this);

            this.VecInputs = VecIn.Flatten().Cast<Input>().ToArray();
            this.VecOutputs = VecOut.Flatten().Cast<Output>().ToArray();
            this.ValueInputs = Value.Flatten().Cast<Input>().ToArray();

            Input[] inputs = input.Flatten().Cast<Input>().ToArray();
            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i].TransferConnectionsTo(VecInputs[i]);
                VecOutputs[i].ConnectToInput(inputs[i], false, false, condition);
            }

            index.ConnectToInput(Index);

            Index.SetName("Index");
            Value.SetName("Value");
        }

        public override void Compute()
        {
            //Copy input values to outputs
            for (int i = 0; i < VecInputs.Length; i++)
            {
                if (VecInputs[i].IsConnectedToAnything())
                {
                    ref BinaryVarValue binValue = ref VecInputs[i].UpdateValueFromSourceFast();
                    VecOutputs[i].Value.UpdateValue(ref binValue);
                }
                else
                {
                    VecOutputs[i].Value.Value.SetAllUnknown();
                }
            }

            //Can't assign to index if index is unknown
            ref BinaryVarValue binIndex = ref Index.UpdateValueFromSourceFast();
            if (!binIndex.IsValidBinary)
            {
                return;
            }

            //Can't assign to index if it's out of bounds
            int index = binIndex.AsInt();
            if (index >= VecIn.Length || index < 0)
            {
                return;
            }

            //Override output values at index with new values
            for (int i = 0; i < ValueInputs.Length; i++)
            {
                if (ValueInputs[i].IsConnectedToAnything())
                {
                    ref BinaryVarValue binValue = ref ValueInputs[i].UpdateValueFromSourceFast();
                    VecOutputs[index * ValueInputs.Length + i].Value.UpdateValue(ref binValue);
                }
            }
        }

        public FIRIO GetAssignIO()
        {
            return Value;
        }

        public override Input[] GetInputs()
        {
            List<Input> inputs = new List<Input>();
            inputs.AddRange(VecInputs);
            inputs.Add(Index);
            inputs.AddRange(ValueInputs);

            return inputs.ToArray();
        }

        public override IEnumerable<FIRIO> GetIO()
        {
            yield return VecIn;
            yield return Index;
            yield return Value;
            yield return VecOut;
        }

        public override Output[] GetOutputs()
        {
            return VecOutputs.ToArray();
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
