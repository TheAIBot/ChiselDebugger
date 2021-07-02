using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using VCDReader;

namespace ChiselDebug.GraphFIR.IO
{
    public class Output : ScalarIO
    {
        private List<Input> To = null;


        public Output(FIRRTLNode node, string name, IFIRType type) : base(node, name, type)
        { }

        public override bool IsConnectedToAnything()
        {
            return To != null && To.Count > 0;
        }

        public override FIRIO GetOutput()
        {
            return this;
        }

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false, Output condition = null)
        {
            if (input is Input ioIn)
            {
                ioIn.Connect(this, condition);
                ConnectOnlyOutputSide(ioIn);
            }
            else
            {
                throw new Exception("Output can only be connected to input.");
            }
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node)
        {
            return flow switch
            {
                FlowChange.Source => new Output(node, Name, Type),
                FlowChange.Sink => new Input(node, Name, Type),
                FlowChange.Flipped => new Input(node, Name, Type),
                FlowChange.Preserve => new Output(node, Name, Type),
                var error => throw new Exception($"Unknown flow. Flow: {flow}")
            };
        }

        public override bool IsPassiveOfType<T>()
        {
            return this is T;
        }

        public override void InferGroundType()
        {
            if (Node == null)
            {
                return;
            }
            if (Type is GroundType ground && ground.IsTypeFullyKnown())
            {
                return;
            }

            if (Node is PairedIOFIRRTLNode pairedIO)
            {
                SetType(TypeHelper.InferMaxWidthType(this, pairedIO));
            }
            else
            {
                Node.InferType();
            }
        }

        internal void DisconnectOnlyOutputSide(Input input)
        {
            To.Remove(input);
        }

        internal void ConnectOnlyOutputSide(Input input)
        {
            if (To == null)
            {
                To = new List<Input>();
            }
            To.Add(input);
        }

        public IEnumerable<Input> GetConnectedInputs()
        {
            return To?.Distinct() ?? Enumerable.Empty<Input>();
        }

        public override ref BinaryVarValue FetchValue()
        {
            return ref Value.Value;
        }
    }
}
