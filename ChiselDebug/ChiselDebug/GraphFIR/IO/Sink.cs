﻿using ChiselDebug.GraphFIR.Components;
using ChiselDebug.Utilities;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using VCDReader;

namespace ChiselDebug.GraphFIR.IO
{
    public sealed class Sink : ScalarIO
    {
        private Source Con;
        private RefEnabledList<Connection> CondCons = new RefEnabledList<Connection>();
        private Source Paired = null;

        public Sink(FIRRTLNode node, IFIRType type) : this(node, null, type)
        { }

        public Sink(FIRRTLNode node, string name, IFIRType type) : base(node, name, type)
        { }

        public void ReplaceConnection(Connection replace, Source replaceWith)
        {
            ReplaceConnection(replace, new Connection(replaceWith, replace.Condition));
        }

        public void ReplaceConnection(Connection replace, Connection replaceWith)
        {
            if (replace.Condition == null)
            {
                if (Con != replace.From)
                {
                    throw new Exception("Input is not connected to the given output.");
                }

                replace.From.DisconnectOnlyOutputSide(this);
                replaceWith.From.ConnectOnlyOutputSide(this);
                Con = replaceWith.From;
            }
            else
            {
                for (int i = 0; i < CondCons.Count; i++)
                {
                    if (CondCons[i] == replace)
                    {
                        replace.From.DisconnectOnlyOutputSide(this);
                        replaceWith.From.ConnectOnlyOutputSide(this);
                        CondCons[i] = replaceWith;
                        return;
                    }
                }

                throw new Exception("Input is not conditionally connected to the given output.");
            }
        }

        public override bool IsConnectedToAnything()
        {
            return Con != null || CondCons.Count > 0;
        }

        public override bool IsConnectedToAnythingPlaceable()
        {
            return (Con != null && Con.Node is not INoPlaceAndRoute) ||
                   (CondCons.Count > 0 && CondCons.ToArray().Any(x => x.From.Node is not INoPlaceAndRoute));
        }

        public Connection[] GetConnections()
        {
            List<Connection> cons = new List<Connection>();
            if (Con != null)
            {
                cons.Add(new Connection(Con));
            }

            for (int i = 0; i < CondCons.Count; i++)
            {
                cons.Add(CondCons[i]);
            }

            return cons.ToArray();
        }

        public Connection GetConnection(Source from, Source condition)
        {
            if (!TryGetConnection(from, condition, out var connection))
            {
                throw new Exception("Input is not connected to the given output.");
            }

            return connection.Value;
        }

        public bool TryGetConnection(Source from, Source condition, [NotNullWhen(true)] out Connection? connection)
        {
            if (condition == null && Con == from)
            {
                connection = new Connection(from);
                return true;
            }
            else
            {
                for (int i = 0; i < CondCons.Count; i++)
                {
                    if (CondCons[i].From == from && CondCons[i].Condition == condition)
                    {
                        connection = CondCons[i];
                        return true;
                    }
                }
            }

            connection = null;
            return false;
        }

        public override FIRIO GetSink()
        {
            return this;
        }

        internal void TransferConnectionsTo(Sink input)
        {
            if (Con != null)
            {
                Con.ConnectToInput(input);
            }

            foreach (var condCon in CondCons.ToArray())
            {
                condCon.From.ConnectToInput(input, false, false, condCon.Condition);
            }

            DisconnectAll();
        }

        public void Disconnect(Connection toDisconnect)
        {
            if (new Connection(Con) == toDisconnect)
            {
                Con.DisconnectOnlyOutputSide(this);
                Con = null;
            }
            else
            {
                bool didDisconnect = false;
                for (int i = 0; i < CondCons.Count; i++)
                {
                    if (CondCons[i] == toDisconnect)
                    {
                        CondCons[i].From.DisconnectOnlyOutputSide(this);
                        CondCons.RemoveAt(i);
                        didDisconnect = true;
                        break;
                    }
                }

                if (!didDisconnect)
                {
                    throw new Exception("Can't disconnect from a connection is wasn't connected to.");
                }
            }
        }

        public void DisconnectAll()
        {
            if (Con != null)
            {
                Disconnect(new Connection(Con));
            }

            for (int i = 0; i < CondCons.Count; i++)
            {
                CondCons[i].From.DisconnectOnlyOutputSide(this);
            }
            CondCons.Clear();
        }

        public void DisconnectAllFrom(Source from)
        {
            if (Con == from)
            {
                Con.DisconnectOnlyOutputSide(this);
                Con = null;
            }

            for (int i = 0; i < CondCons.Count; i++)
            {
                if (CondCons[i].From == from)
                {
                    CondCons[i].From.DisconnectOnlyOutputSide(this);
                    CondCons.RemoveAt(i);
                    break;
                }
            }
        }

        public void Connect(Source con, Source condition)
        {
            if (condition != null)
            {
                /*
                 * handle this
                 * 
                 * when en:
                 *      a <= b
                 *      a <= c
                 *      skip
                 *      
                 * Check if connection exists with same condition
                 * and remove it so it's replaced with the new one.
                 */
                for (int i = 0; i < CondCons.Count; i++)
                {
                    var condCon = CondCons[i];
                    if (condCon.Condition == condition)
                    {
                        Disconnect(condCon);
                        break;
                    }
                }

                CondCons.Add(new Connection(con, condition));
            }
            else
            {
                DisconnectAll();
                Con = con;
            }
        }

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false, Source condition = null)
        {
            throw new Exception("Input can't be connected to output. Flow is reversed.");
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node)
        {
            return flow switch
            {
                FlowChange.Source => new Source(node, Name, Type),
                FlowChange.Sink => new Sink(node, Name, Type),
                FlowChange.Flipped => new Source(node, Name, Type),
                FlowChange.Preserve => new Sink(node, Name, Type),
                var error => throw new Exception($"Unknown flow. Flow: {flow}")
            };
        }

        public override bool IsPassiveOfType<T>()
        {
            return this is T;
        }

        public Source GetEnabledSource()
        {
            for (int i = CondCons.Count - 1; i >= 0; i--)
            {
                var condCon = CondCons[i];
                if (condCon.IsEnabled())
                {
                    return condCon.From;
                }
            }

            if (Con != null)
            {
                return Con;
            }

            return null;
        }

        public void UpdateValueFromSource()
        {
            ValueType enabledValue = FetchValueFromSourceFast();
            if (enabledValue != Value)
            {
                Value.UpdateFrom(enabledValue);
            }
        }

        public ValueType FetchValueFromSourceFast()
        {
            for (int i = CondCons.Count - 1; i >= 0; i--)
            {
                ref readonly var condCon = ref CondCons[i];
                if (condCon.IsEnabled())
                {
                    return condCon.From.Value;
                }
            }

            if (Con != null)
            {
                return Con.Value;
            }

            Value.Value.SetAllUnknown();
            return Value;
        }

        public ref BinaryVarValue UpdateValueFromSourceFast()
        {
            ValueType enabledValue = FetchValueFromSourceFast();
            if (enabledValue.Value.Length != Value.Value.Length)
            {
                Value.UpdateFrom(enabledValue);
                return ref GetValue();
            }

            return ref enabledValue.Value;
        }

        public BigInteger GetValueAsBigInt()
        {
            return FetchValueFromSourceFast().GetAsBigInt();
        }

        public override void InferGroundType()
        {
            if (Type is GroundType ground && ground.IsTypeFullyKnown())
            {
                return;
            }

            List<Source> endPoints = new List<Source>();
            if (Con != null)
            {
                endPoints.Add(Con);
            }
            for (int i = 0; i < CondCons.Count; i++)
            {
                endPoints.Add(CondCons[i].From);
            }

            SetType(TypeHelper.InferMaxWidthType(endPoints.ToArray()));
        }

        public override ref BinaryVarValue FetchValue()
        {
            return ref UpdateValueFromSourceFast();
        }

        public override Source GetPaired()
        {
            return Paired;
        }

        public override void SetPaired(ScalarIO paired)
        {
            Paired = (Source)paired;
        }
    }
}