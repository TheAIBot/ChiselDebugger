﻿using ChiselDebug.GraphFIR.Components;
using System;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR.IO
{
    public sealed class DuplexIO : AggregateIO
    {
        private readonly FIRIO InIO;
        private readonly FIRIO OutIO;

        public DuplexIO(FIRRTLNode node, string name, FIRIO inIO, FIRIO outIO) : base(node, name)
        {
            this.InIO = inIO;
            this.OutIO = outIO;
        }

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false, Source condition = null)
        {
            throw new Exception("Duplex can't be connected to anything.");
        }

        internal override void FlattenOnly<T>(ref Span<T> list)
        {
            InIO.FlattenOnly(ref list);
            OutIO.FlattenOnly(ref list);
        }

        public override List<T> FlattenTo<T>(List<T> list)
        {
            InIO.FlattenTo(list);
            OutIO.FlattenTo(list);

            return list;
        }

        public override int GetScalarsCount()
        {
            return InIO.GetScalarsCount() + OutIO.GetScalarsCount();
        }

        public override int GetScalarsCountOfType<T>()
        {
            return InIO.GetScalarsCountOfType<T>() + OutIO.GetScalarsCountOfType<T>();
        }

        public override FIRIO GetSink()
        {
            return InIO;
        }

        public override FIRIO GetSource()
        {
            return OutIO;
        }

        public override FIRIO[] GetIOInOrder()
        {
            return new FIRIO[] { InIO, OutIO };
        }

        public override bool IsPassiveOfType<T>()
        {
            return false;
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node)
        {
            return flow switch
            {
                FlowChange.Source => OutIO.ToFlow(flow, node),
                FlowChange.Sink => InIO.ToFlow(flow, node),
                FlowChange.Flipped => new DuplexIO(node, Name, InIO.ToFlow(flow, node), OutIO.ToFlow(flow, node)),
                FlowChange.Preserve => new DuplexIO(node, Name, InIO.ToFlow(flow, node), OutIO.ToFlow(flow, node)),
                var error => throw new Exception($"Unknown flow. Flow: {flow}")
            };
        }

        public override bool TryGetIO(string ioName, out IContainerIO container)
        {
            if (InIO.Name == ioName)
            {
                container = InIO;
                return true;
            }
            else if (OutIO.Name == ioName)
            {
                container = OutIO;
                return true;
            }

            container = null;
            return false;
        }
    }
}
