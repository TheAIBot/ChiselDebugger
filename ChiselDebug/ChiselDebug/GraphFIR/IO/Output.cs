using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public class Output : ScalarIO
    {
        public Output(FIRRTLNode node, IFIRType type) : this(node, string.Empty, type)
        { }

        public Output(FIRRTLNode node, string name, IFIRType type) : base(node, name, type)
        {
            this.Con = new Connection(this);
        }

        public override bool IsConnected()
        {
            return Con != null;
        }

        public override bool IsConnectedToAnything()
        {
            return Con != null && Con.To.Count > 0;
        }

        public override void DisconnectAll()
        {
            Input[] inputs = Con.To.ToArray();
            foreach (var input in inputs)
            {
                Con.DisconnectInput(input);
            }
        }

        public override void SetType(IFIRType type)
        {
            base.SetType(type);
            Con.Value = new ValueType(type);
        }

        public override FIRIO GetOutput()
        {
            return this;
        }

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false, bool isConditional = false)
        {
            if (input is Input ioIn)
            {
                Con.ConnectToInput(ioIn, isConditional);
            }
            else
            {
                throw new Exception("Output can only be connected to input.");
            }
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node = null)
        {
            return flow switch
            {
                FlowChange.Source => new Output(node ?? Node, Name, Type),
                FlowChange.Sink => new Input(node ?? Node, Name, Type),
                FlowChange.Flipped => new Input(node ?? Node, Name, Type),
                FlowChange.Preserve => new Output(node ?? Node, Name, Type),
                var error => throw new Exception($"Unknown flow. Flow: {flow}")
            };
        }

        public override bool IsPassiveOfType<T>()
        {
            return this is T;
        }

        public override bool SameIO(FIRIO other)
        {
            return other is Output otherOut &&
                   Type.Equals(otherOut.Type);
        }

        public override IEnumerable<T> GetAllIOOfType<T>()
        {
            if (this is T thisIsT)
            {
                yield return thisIsT;
            }
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
                foreach (Input paired in pairedIO.GetAllPairedIO(this).OfType<Input>())
                {
                    paired.InferType();
                    SetType(paired.Type);
                }
            }
            else
            {
                Node.InferType();
            }
        }
    }
}
