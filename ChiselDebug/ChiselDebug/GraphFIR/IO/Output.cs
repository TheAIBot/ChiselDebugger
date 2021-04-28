using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public class Output : ScalarIO
    {
        private HashSet<Input> To = null;


        public Output(FIRRTLNode node, string name, IFIRType type) : base(node, name, type)
        { }

        public override bool IsConnected()
        {
            return true;
        }

        public override bool IsConnectedToAnything()
        {
            return To != null && To.Count > 0;
        }

        public override void DisconnectAll()
        {
            Input[] inputs = GetConnectedInputs().ToArray();
            foreach (var input in inputs)
            {
                DisconnectInput(input);
            }
        }

        public override FIRIO GetOutput()
        {
            return this;
        }

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false, bool isConditional = false)
        {
            if (input is Input ioIn)
            {
                if (!isConditional && ioIn.IsConnected())
                {
                    if (ioIn.Con != null)
                    {
                        ioIn.Con.DisconnectInput(ioIn);
                    }
                }

                if (To == null)
                {
                    To = new HashSet<Input>();
                }
                To.Add(ioIn);
                ioIn.Connect(this, isConditional);
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

        public override List<T> GetAllIOOfType<T>(List<T> list)
        {
            if (this is T tVal)
            {
                list.Add(tVal);
            }

            return list;
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

        public void DisconnectInput(Input input)
        {
            To.Remove(input);
            input.Disconnect(this);
        }

        public IEnumerable<Input> GetConnectedInputs()
        {
            return To ?? Enumerable.Empty<Input>();
        }
    }
}
