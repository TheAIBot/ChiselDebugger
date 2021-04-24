using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public class Input : ScalarIO
    {
        internal Output Con;
        private HashSet<Output> CondCons = null;

        public Input(FIRRTLNode node, IFIRType type) : this(node, null, type)
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

        public Output[] GetAllConnections()
        {
            List<Output> cons = new List<Output>();
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

        internal Output[] GetConditionalConnections()
        {
            if (CondCons == null)
            {
                return Array.Empty<Output>();
            }
            return CondCons.ToArray();
        }

        public void Disconnect(Output toDisconnect)
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

        public void Connect(Output con, bool isConditional)
        {
            if (isConditional)
            {
                if (CondCons == null)
                {
                    CondCons = new HashSet<Output>();
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

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node)
        {
            return flow switch
            {
                FlowChange.Source => new Output(node, Name, Type),
                FlowChange.Sink => new Input(node, Name, Type),
                FlowChange.Flipped => new Output(node, Name, Type),
                FlowChange.Preserve => new Input(node, Name, Type),
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

        public Output GetEnabledCon()
        {
            if (CondCons != null)
            {
                foreach (var condCon in CondCons)
                {
                    if (condCon.IsEnabled)
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
                Con.InferType();
                SetType(Con.Type);
            }
            if (CondCons != null)
            {
                foreach (var condCon in CondCons)
                {
                    condCon.InferType();
                    SetType(condCon.Type);
                }
            }
        }
    }
}
