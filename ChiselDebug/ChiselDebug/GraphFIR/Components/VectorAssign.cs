using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using VCDReader;

namespace ChiselDebug.GraphFIR.Components
{
    public sealed class VectorAssign : FIRRTLNode
    {
        private readonly Vector VecIn;
        private readonly Sink Index;
        private readonly FIRIO Value;
        private readonly Vector VecOut;
        private readonly Sink[] VecInputs;
        private readonly Source[] VecOutputs;
        private readonly Sink[] ValueInputs;

        public VectorAssign(Vector input, Source index, Source condition, FirrtlNode defNode) : base(defNode)
        {
            if (!input.IsPassiveOfType<Sink>())
            {
                throw new Exception("Vector assign input must be a passive input type.");
            }

            VecIn = (Vector)input.Copy(this);
            Index = (Sink)index.Flip(this);
            Value = VecIn.GetIndex(0).Copy(this);
            VecOut = (Vector)input.Flip(this);

            VecInputs = VecIn.FlattenTo<Sink>();
            VecOutputs = VecOut.FlattenTo<Source>();
            ValueInputs = Value.FlattenTo<Sink>();

            Sink[] inputs = input.FlattenTo<Sink>();
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

        public override Sink[] GetSinks()
        {
            List<Sink> inputs = new List<Sink>();
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

        public override Source[] GetSources()
        {
            return VecOutputs.ToArray();
        }

        internal override void InferType()
        {
            foreach (var input in GetSinks())
            {
                input.InferType();
            }
            foreach (var output in GetSources())
            {
                output.InferType();
            }
        }
    }
}
