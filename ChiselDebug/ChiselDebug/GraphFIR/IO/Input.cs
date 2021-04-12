﻿using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public class Input : ScalarIO
    {
        private Output SinkSource = null;
        private readonly HashSet<Connection> CondCons = new HashSet<Connection>();

        public Input(FIRRTLNode node, IFIRType type) : this(node, string.Empty, type)
        { }

        public Input(FIRRTLNode node, string name, IFIRType type) : base(node, name, type)
        { }

        public override bool IsConnected()
        {
            return Con != null || CondCons.Count > 0;
        }

        public override bool IsConnectedToAnything()
        {
            return Con != null || CondCons.Count > 0;
        }

        public Connection[] GetAllConnections()
        {
            List<Connection> cons = new List<Connection>();
            if (Con != null)
            {
                cons.Add(Con);
            }
            cons.AddRange(CondCons);

            return cons.ToArray();
        }

        public override FIRIO GetInput()
        {
            return this;
        }

        public override FIRIO GetOutput()
        {
            if (SinkSource == null)
            {
                SinkSource = (Output)Flip();
            }

            return SinkSource;
        }

        internal Connection[] GetConditionalConnections()
        {
            return CondCons.ToArray();
        }

        public bool HasSinkSource()
        {
            return SinkSource != null;
        }

        public void MakeSinkOnly()
        {
            if (SinkSource == null)
            {
                return;
            }

            //if (!SinkSource.Con.IsUsed())
            //{
            //    throw new Exception("Probably an error when a source is created in a sink but it's not connected to anything.");
            //}

            if (!IsConnected())
            {
                SinkSource.DisconnectAll();
                return;
                throw new Exception("Sink must be connected when it's also used as a source.");
            }

            IOHelper.BypassIO(SinkSource, Con.From);
            foreach (var condCond in CondCons)
            {
                IOHelper.BypassIO(SinkSource, condCond.From);
            }
            SinkSource = null;
        }

        public void Disconnect(Connection toDisconnect)
        {
            if (Con == toDisconnect)
            {
                Con = null;
            }
            else
            {
                if (!CondCons.Remove(toDisconnect))
                {
                    throw new Exception("Can't disconnect from a connection is wasn't connected to.");
                }
            }
        }

        public override void DisconnectAll()
        {
            if (Con != null)
            {
                Con.DisconnectInput(this);
            }

            foreach (var con in CondCons.ToArray())
            {
                con.DisconnectInput(this);
            }
        }

        public void Connect(Connection con, bool isConditional)
        {
            if (isConditional)
            {
                CondCons.Add(con);
            }
            else
            {
                Con = con;
            }
        }

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false, bool isConditional = false)
        {
            throw new Exception("Input can't be connected to output. Flow is reversed.");
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node = null)
        {
            return flow switch
            {
                FlowChange.Source => new Output(node ?? Node, Name, Type),
                FlowChange.Sink => new Input(node ?? Node, Name, Type),
                FlowChange.Flipped => new Output(node ?? Node, Name, Type),
                FlowChange.Preserve => new Input(node ?? Node, Name, Type),
                var error => throw new Exception($"Unknown flow. Flow: {flow}")
            };
        }

        public override bool IsPassiveOfType<T>()
        {
            return this is T;
        }

        public override bool SameIO(FIRIO other)
        {
            return other is Input otherIn && 
                   Type.Equals(otherIn.Type);
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
            if (Type is GroundType ground && ground.IsTypeFullyKnown())
            {
                return;
            }
            if (Con != null)
            {
                Con.From.InferType();
                SetType(Con.From.Type);
            }
            foreach (var condCon in CondCons)
            {
                condCon.From.InferType();
                SetType(condCon.From.Type);
            }
        }
    }
}
