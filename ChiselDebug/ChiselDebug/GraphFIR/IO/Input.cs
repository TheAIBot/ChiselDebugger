using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using VCDReader;

namespace ChiselDebug.GraphFIR.IO
{
    public class Input : ScalarIO
    {
        private Output Con;
        private List<Connection> CondCons = null;

        public Input(FIRRTLNode node, IFIRType type) : this(node, null, type)
        { }

        public Input(FIRRTLNode node, string name, IFIRType type) : base(node, name, type)
        { }

        public void ReplaceConnection(Connection replace, Output replaceWith)
        {
            if (replace.Condition == null)
            {
                if (Con != replace.From)
                {
                    throw new Exception("Input is not connected to the given output.");
                }

                replace.From.DisconnectOnlyOutputSide(this);
                replaceWith.ConnectOnlyOutputSide(this);
                Con = replaceWith;
            }
            else
            {
                if (CondCons != null)
                {
                    for (int i = 0; i < CondCons.Count; i++)
                    {
                        if (CondCons[i] == replace)
                        {
                            replace.From.DisconnectOnlyOutputSide(this);
                            replaceWith.ConnectOnlyOutputSide(this);
                            CondCons[i] = new Connection(replaceWith, replace.Condition);
                            return;
                        }
                    }
                }

                throw new Exception("Input is not conditionally connected to the given output.");
            }
        }

        public override bool IsConnectedToAnything()
        {
            return Con != null || (CondCons != null && CondCons.Count > 0);
        }

        public Connection[] GetConnections()
        {
            List<Connection> cons = new List<Connection>();
            if (Con != null)
            {
                cons.Add(new Connection(Con));
            }
            if (CondCons != null)
            {
                for (int i = 0; i < CondCons.Count; i++)
                {
                    cons.Add(CondCons[i]);
                }
            }

            return cons.ToArray();
        }

        public override FIRIO GetInput()
        {
            return this;
        }

        internal void TransferConnectionsTo(Input input)
        {
            if (Con != null)
            {
                Con.ConnectToInput(input);
            }
            if (CondCons != null)
            {
                foreach (var condCon in CondCons.ToArray())
                {
                    condCon.From.ConnectToInput(input, false, false, condCon.Condition);
                }
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
                if (CondCons != null)
                {
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

            if (CondCons != null)
            {
                foreach (var condCon in CondCons)
                {
                    condCon.From.DisconnectOnlyOutputSide(this);
                }
                CondCons.Clear();
            }
        }

        public void Connect(Output con, Output condition)
        {
            if (condition != null)
            {
                if (CondCons == null)
                {
                    CondCons = new List<Connection>();
                }

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

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false, Output condition = null)
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

        public Output GetEnabledSource()
        {
            if (CondCons != null)
            {
                for (int i = CondCons.Count - 1; i >= 0; i--)
                {
                    var condCon = CondCons[i];
                    if (condCon.IsEnabled())
                    {
                        return condCon.From;
                    }
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
            if (CondCons != null)
            {
                for (int i = CondCons.Count - 1; i >= 0; i--)
                {
                    var condCon = CondCons[i];
                    if (condCon.IsEnabled())
                    {
                        Value.UpdateFrom(ref condCon.From.Value);
                        return;
                    }
                }
            }

            if (Con != null)
            {
                Value.UpdateFrom(ref Con.Value);
                return;
            }

            Value.Value.SetAllUnknown();
        }

        public ref BinaryVarValue UpdateValueFromSourceFast()
        {
            if (CondCons != null)
            {
                for (int i = CondCons.Count - 1; i >= 0; i--)
                {
                    var condCon = CondCons[i];
                    if (condCon.IsEnabled())
                    {
                        if (condCon.From.Type.Width == Type.Width)
                        {
                            return ref condCon.From.GetValue();
                        }

                        Value.UpdateFrom(ref condCon.From.Value);
                        return ref GetValue();
                    }
                }
            }

            if (Con != null)
            {
                if (Con.Type.Width == Type.Width)
                {
                    return ref Con.GetValue();
                }

                Value.UpdateFrom(ref Con.Value);
                return ref GetValue();
            }

            Value.Value.SetAllUnknown();
            return ref GetValue();
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
                    condCon.From.InferType();
                    SetType(condCon.From.Type);
                }
            }
        }
    }
}
