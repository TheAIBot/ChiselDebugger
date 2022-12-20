using ChiselDebug.GraphFIR.Components;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VCDReader;
#nullable enable

namespace ChiselDebug.GraphFIR.IO
{
    public sealed class Source : ScalarIO
    {
        private List<Sink>? To = null;
        private Sink? Paired = null;


        public Source(FIRRTLNode? node, string? name, IFIRType? type) : base(node, name, type)
        { }

        public override bool IsConnectedToAnything()
        {
            return To != null && To.Count > 0;
        }

        public override bool IsConnectedToAnythingPlaceable()
        {
            return IsConnectedToAnything() && To.Any(x => x.Node is not INoPlaceAndRoute);
        }

        public override FIRIO GetSource()
        {
            return this;
        }

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false, Source? condition = null)
        {
            if (input is Sink ioIn)
            {
                ioIn.Connect(this, condition);
                ConnectOnlyOutputSide(ioIn);
            }
            else
            {
                throw new Exception("Output can only be connected to input.");
            }
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode? node)
        {
            return flow switch
            {
                FlowChange.Source => new Source(node, Name, Type),
                FlowChange.Sink => new Sink(node, Name, Type),
                FlowChange.Flipped => new Sink(node, Name, Type),
                FlowChange.Preserve => new Source(node, Name, Type),
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

        public void DisconnectAll()
        {
            foreach (var input in GetConnectedInputs().ToArray())
            {
                input.DisconnectAllFrom(this);
            }
        }

        internal void DisconnectOnlyOutputSide(Sink input)
        {
            To.Remove(input);
        }

        internal void ConnectOnlyOutputSide(Sink input)
        {
            if (To == null)
            {
                To = new List<Sink>();
            }
            To.Add(input);
        }

        public IEnumerable<Sink> GetConnectedInputs()
        {
            return To ?? Enumerable.Empty<Sink>();
        }

        public override ref BinaryVarValue FetchValue()
        {
            return ref Value.Value;
        }

        public void SetFromBigInt(in BigInteger newValue)
        {
            Value.SetFromBigInt(in newValue);
        }

        public override Sink GetPaired()
        {
            return Paired;
        }

        public override void SetPaired(ScalarIO paired)
        {
            Paired = (Sink)paired;
        }
    }
}
