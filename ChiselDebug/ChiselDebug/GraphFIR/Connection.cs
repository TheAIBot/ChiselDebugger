using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR
{
    public class Connection
    {
        public readonly Output From = null;
        private HashSet<Input> To = null;
        public ValueType Value;

        public Connection(Output from)
        {
            Enumerable.Empty<Input>();
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

            if (To == null)
            {
                To = new HashSet<Input>();
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
            return To != null && To.Count > 0;
        }

        public IEnumerable<Input> GetConnectedInputs()
        {
            return To ?? Enumerable.Empty<Input>();
        }


        public void SetDefaultvalue()
        {
            Value = new ValueType(From.Type);
        }
    }
}
