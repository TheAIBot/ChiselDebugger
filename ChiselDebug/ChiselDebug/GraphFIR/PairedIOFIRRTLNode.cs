using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.GraphFIR
{
    public abstract class PairedIOFIRRTLNode : FIRRTLNode
    {
        private readonly Dictionary<FIRIO, FIRIO> IOPairs = new Dictionary<FIRIO, FIRIO>();

        public PairedIOFIRRTLNode(FirrtlNode defNode) : base(defNode)
        { }

        internal void AddPairedIO(FIRIO io, FIRIO ioFlipped)
        {
            IOHelper.PairIO(IOPairs, io, ioFlipped);
        }

        public FIRIO GetPairedIO(FIRIO io)
        {
            return IOPairs[io];
        }

        public bool IsPartOfPair(FIRIO io)
        {
            return IOPairs.ContainsKey(io);
        }
    }
}
