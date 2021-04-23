using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public class Input : ScalarIO
    {
        private HashSet<Connection> CondCons = null;

        public Input(FIRRTLNode node, IFIRType type) : this(node, string.Empty, type)
        { }

        public Input(FIRRTLNode node, string name, IFIRType type) : base(node, name, type)
        { }

        public override bool IsConnected()
        {
            return Con != null || (CondCons != null && CondCons.Count > 0);
        }

        public override bool IsConnectedToAnything()
        {
            return Con != null || (CondCons != null && CondCons.Count > 0);
        }

        public Connection[] GetAllConnections()
        {
            List<Connection> cons = new List<Connection>();
            if (Con != null)
            {
                cons.Add(Con);
            }
            if (CondCons != null)
            {
                cons.AddRange(CondCons);
            }

            return cons.ToArray();
        }

        public override FIRIO GetInput()
        {
            return this;
        }

        internal Connection[] GetConditionalConnections()
        {
            if (CondCons == null)
            {
                return Array.Empty<Connection>();
            }
            return CondCons.ToArray();
        }

        public void Disconnect(Connection toDisconnect)
        {
            if (Con == toDisconnect)
            {
                Con = null;
            }
            else
            {
                if (CondCons == null || !CondCons.Remove(toDisconnect))
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

            if (CondCons != null)
            {
                foreach (var con in CondCons.ToArray())
                {
                    con.DisconnectInput(this);
                }
            }
        }

        public void Connect(Connection con, bool isConditional)
        {
            if (isConditional)
            {
                if (CondCons == null)
                {
                    CondCons = new HashSet<Connection>();
                }
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

        public Connection GetEnabledCon()
        {
            if (CondCons != null)
            {
                foreach (var condCon in CondCons)
                {
                    if (condCon.From.IsEnabled)
                    {
                        return condCon;
                    }
                }
            }

            if (Con != null)
            {
                return Con;
            }

            //No connection is enabled.
            //This should only happen when circuit state isn't set yet.
            //Just return random connection as they should all have the
            //same value.
            return CondCons.First();
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
            if (CondCons != null)
            {
                foreach (var condCon in CondCons)
                {
                    condCon.From.InferType();
                    SetType(condCon.From.Type);
                }
            }
        }
    }
}
