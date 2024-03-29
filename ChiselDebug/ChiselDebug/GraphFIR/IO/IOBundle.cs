﻿using ChiselDebug.GraphFIR.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
#nullable enable

namespace ChiselDebug.GraphFIR.IO
{
    public class IOBundle : AggregateIO
    {
        private readonly List<FIRIO> OrderedIO;
        private readonly Dictionary<string, FIRIO> IO = new Dictionary<string, FIRIO>();

        public IOBundle(FIRRTLNode? node, string? name, List<FIRIO> io) : base(node, name)
        {
            this.OrderedIO = io.ToList();

            foreach (var firIO in io)
            {
                if (firIO.Name == null)
                {
                    throw new InvalidOperationException("IO must have a name when connected to a bundle.");
                }
                IO.Add(firIO.Name, firIO);
                firIO.SetParentIO(this);
            }
        }

        public IOBundle(FIRRTLNode? node, string name, List<FIRIO> io, List<string> ioNames, bool twoWayRelationship = true) : base(node, name)
        {
            this.OrderedIO = io.ToList();

            foreach (var (firIO, ioName) in io.Zip(ioNames))
            {
                IO.Add(ioName, firIO);
            }

            if (twoWayRelationship)
            {
                foreach (var firIO in IO.Values)
                {
                    firIO.SetParentIO(this);
                }
            }
        }

        public override FIRIO[] GetIOInOrder()
        {
            return OrderedIO.ToArray();
        }

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false, Source? condition = null)
        {
            if (input is not IOBundle)
            {
                throw new Exception("Bundle can only connect to other bundle.");
            }
            IOBundle bundle = (IOBundle)input;

            if (asPassive && !IsPassiveOfType<Source>())
            {
                throw new Exception("Bundle must be a passive output bundle but it was not.");
            }

            if (asPassive && !bundle.IsPassiveOfType<Sink>())
            {
                throw new Exception("Bundle must connect to a passive input bundle.");
            }

            if (!allowPartial && IO.Count != bundle.IO.Count)
            {
                throw new Exception("Trying to fully connect two bundles that don't match.");
            }

            IEnumerable<string> ioConnectNames = IO.Keys;
            if (allowPartial)
            {
                ioConnectNames = ioConnectNames.Intersect(bundle.IO.Keys);
            }

            foreach (var ioName in ioConnectNames)
            {
                var a = GetIO(ioName);
                var b = bundle.GetIO(ioName);

                if (a is Source aOut && b is Sink bIn)
                {
                    aOut.ConnectToInput(bIn, allowPartial, asPassive, condition);
                }
                else if (a is Sink aIn && b is Source bOut)
                {
                    bOut.ConnectToInput(aIn, allowPartial, asPassive, condition);
                }
                else if (a is IOBundle aBundle && b is IOBundle bBundle)
                {
                    aBundle.ConnectToInput(bBundle, allowPartial, asPassive, condition);
                }
                else if (a is Vector aVec && b is Vector bVec)
                {
                    aVec.ConnectToInput(bVec, allowPartial, asPassive, condition);
                }
                else
                {
                    throw new Exception($"Can't connect IO of type {a.GetType()} to {b.GetType()}.");
                }
            }

            base.ConnectToInput(input, allowPartial, asPassive, condition);
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode? node)
        {
            List<FIRIO> changedFlow = OrderedIO.Select(x => x.ToFlow(flow, node)).ToList();
            return new IOBundle(node, Name, changedFlow);
        }

        public override List<ScalarIO> Flatten(List<ScalarIO> list)
        {
            foreach (var io in OrderedIO)
            {
                io.Flatten(list);
            }

            return list;
        }

        internal override void FlattenOnly<T>(ref Span<T> list)
        {
            foreach (var io in OrderedIO)
            {
                io.FlattenOnly(ref list);
            }
        }

        internal override void FlattenTo<T>(ref Span<T> list)
        {
            foreach (var io in OrderedIO)
            {
                io.FlattenTo(ref list);
            }
        }

        public override int GetScalarsCount()
        {
            int scalars = 0;
            foreach (var io in OrderedIO)
            {
                scalars += io.GetScalarsCount();
            }

            return scalars;
        }

        public override int GetScalarsCountOfType<T>()
        {
            int scalars = 0;
            foreach (var io in OrderedIO)
            {
                scalars += io.GetScalarsCountOfType<T>();
            }

            return scalars;
        }

        public override bool IsPassiveOfType<T>()
        {
            foreach (var io in IO.Values)
            {
                if (!io.IsPassiveOfType<T>())
                {
                    return false;
                }
            }

            return true;
        }

        public override bool TryGetIO(string ioName, [NotNullWhen(true)] out IContainerIO? container)
        {
            if (IO.TryGetValue(ioName, out FIRIO? innerIO))
            {
                container = innerIO;
                return true;
            }

            container = null;
            return false;
        }

        protected void AddIO(string? name, FIRIO io)
        {
            ArgumentNullException.ThrowIfNull(name);

            io.SetParentIO(this);
            OrderedIO.Add(io);
            IO.Add(name, io);
        }
    }
}
