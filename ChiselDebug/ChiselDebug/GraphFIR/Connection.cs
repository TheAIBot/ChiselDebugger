using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR
{
    public class Connection
    {
        public readonly Output From = null;
        public readonly HashSet<Input> To = new HashSet<Input>();
        public ValueType Value;

        public Connection(Output from)
        {
            this.From = from;
        }

        public void ConnectToInput(Input input, bool isConditional)
        {
            if (!isConditional && input.IsConnected())
            {
                if (input.Con != null)
                {
                    input.Con.DisconnectInput(input);
                }
            }

            To.Add(input);
            input.Connect(this, isConditional);
        }

        public void DisconnectInput(Input input)
        {
            To.Remove(input);
            input.Disconnect(this);
        }

        public bool IsUsed()
        {
            return To.Count > 0;
        }

        public void SetDefaultvalue()
        {
            if (From.Type == null)
            {

            }
            Value = new ValueType(From.Type);
        }
    }
}
