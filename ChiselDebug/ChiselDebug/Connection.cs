using System;
using System.Collections.Generic;

namespace ChiselDebug
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

        public void ConnectToInput(Input input)
        {
            if (input.IsConnected())
            {
                throw new Exception("An input can't be connected to two wires at once.");
            }

            To.Add(input);
            input.Con = this;
        }

        public bool IsUsed()
        {
            return To.Count > 0;
        }
    }
}
