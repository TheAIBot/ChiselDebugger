using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public class Output : ScalarIO
    {
        private HashSet<Input> To = null;
        public ValueType Value;

        public Output(FIRRTLNode node, IFIRType type) : this(node, string.Empty, type)
        { }

        public Output(FIRRTLNode node, string name, IFIRType type) : base(node, name, type)
        { }

        public override bool IsConnected()
        {
            return true;
        }

        public override bool IsConnectedToAnything()
        {
            return IsUsed();
        }

        public override void DisconnectAll()
        {
            Input[] inputs = GetConnectedInputs().ToArray();
            foreach (var input in inputs)
            {
                DisconnectInput(input);
            }
        }

        public override void SetType(IFIRType type)
        {
            base.SetType(type);
        }

        public override FIRIO GetOutput()
        {
            return this;
        }

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false, bool isConditional = false)
        {
            if (input is Input ioIn)
            {
                ConnectToInput(ioIn, isConditional);
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
                if (pairedIO is Mux mux)
                {
                    SetType(TypeHelper.InferMuxOutputType(this, mux));
                }
                else
                {
                    foreach (Input paired in pairedIO.GetAllPairedIO(this).OfType<Input>())
                    {
                        paired.InferType();
                        SetType(paired.Type);
                    }
                }
            }
            else
            {
                Node.InferType();
            }
        }

        public void ConnectToInput(Input input, bool isConditional)
        {
            if (!isConditional && input.IsConnected())
            {
                if (input.Con != null)
                {
                    input.Con.DisconnectInput(input);
                }
            }

            if (To == null)
            {
                To = new HashSet<Input>();
            }
            To.Add(input);
            input.Connect(this, isConditional);
        }

        public void DisconnectInput(Input input)
        {
            To.Remove(input);
            input.Disconnect(this);
        }

        public bool IsUsed()
        {
            return To != null && To.Count > 0;
        }

        public IEnumerable<Input> GetConnectedInputs()
        {
            return To ?? Enumerable.Empty<Input>();
        }


        public void SetDefaultvalue()
        {
            Value = new ValueType(Type);
        }
    }
}
