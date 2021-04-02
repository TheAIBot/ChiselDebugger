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
            this.Value = new ValueType(From.Type);
        }

        public void ConnectToInput(Input input)
        {
            if (input.IsConnected())
            {
                input.Disconnect();
            }

            To.Add(input);
            input.Con = this;
        }

        public void DisconnectInput(Input input)
        {
            To.Remove(input);
            input.Con = null;
        }

        public bool IsUsed()
        {
            return To.Count > 0;
        }
    }
}
