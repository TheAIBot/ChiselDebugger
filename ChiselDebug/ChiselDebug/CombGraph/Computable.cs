﻿using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System;
using VCDReader;

namespace ChiselDebug.CombGraph
{
    public readonly struct Computable
    {
        private readonly FIRRTLNode Node;
        private readonly Connection Con;
        private readonly BinaryVarValue OldValue;

        public Computable(FIRRTLNode node)
        {
            this.Node = node;
            this.Con = null;
            this.OldValue = null;
        }

        public Computable(Connection con)
        {
            this.Node = null;
            this.Con = con;
            this.OldValue = new BinaryVarValue(Con.Value.GetValue().Bits.Length);
            Array.Fill(OldValue.Bits, BitState.X);
        }

        public Connection Compute()
        {
            if (Node != null)
            {
                Node.Compute();
            }
            else
            {
                //Copy value from other side of module
                if (Con.From.Node is Module mod)
                {
                    Input input = (Input)mod.GetPairedIO(Con.From);
                    if (input.IsConnectedToAnything())
                    {
                        Connection copyFrom = input.GetEnabledCon();
                        Con.Value.UpdateFrom(copyFrom.Value);
                    }
                }

                //Did connection value change?
                if (!OldValue.SameValue(Con.Value.GetValue()))
                {
                    OldValue.SetBitsAndExtend(Con.Value.GetValue(), false);
                    Con.Value.UpdateValueString();
                    return Con;
                }
            }

            return null;
        }
    }
}
