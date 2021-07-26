using System;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR.IO
{
    public class DuplexIO : AggregateIO
    {
        private readonly FIRIO InIO;
        private readonly FIRIO OutIO;

        public DuplexIO(FIRRTLNode node, string name, FIRIO inIO, FIRIO outIO) : base(node, name)
        {
            this.InIO = inIO;
            this.OutIO = outIO;
        }

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false, Output condition = null)
        {
            throw new Exception("Duplex can't be connected to anything.");
        }

        public override IEnumerable<ScalarIO> Flatten()
        {
            foreach (var io in InIO.Flatten())
            {
                yield return io;
            }
            foreach (var io in OutIO.Flatten())
            {
                yield return io;
            }
        }

        public override List<ScalarIO> Flatten(List<ScalarIO> list)
        {
            InIO.Flatten(list);
            OutIO.Flatten(list);

            return list;
        }

        public override FIRIO GetInput()
        {
            return InIO;
        }

        public override FIRIO GetOutput()
        {
            return OutIO;
        }

        public override FIRIO[] GetIOInOrder()
        {
            return new FIRIO[] { InIO, OutIO };
        }

        public override bool IsPassiveOfType<T>()
        {
            throw new Exception("Duplex can't be passive of any type.");
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

        public override IEnumerable<FIRIO> WalkIOTree()
        {
            yield return this;

            foreach (var nested in InIO.WalkIOTree())
            {
                yield return nested;
            }
            foreach (var nested in OutIO.WalkIOTree())
            {
                yield return nested;
            }
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
