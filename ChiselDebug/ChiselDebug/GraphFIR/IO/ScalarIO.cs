using FIRRTL;
using System;

namespace ChiselDebug.GraphFIR.IO
{
    public abstract class ScalarIO : FIRIO
    {
        public readonly FIRRTLNode Node;
        public IFIRType Type { get; protected set; }
        public Connection Con = null;

        public ScalarIO(FIRRTLNode node, IFIRType type) : this(node, string.Empty, type)
        { }

        public ScalarIO(FIRRTLNode node, string name, IFIRType type) : base(name)
        {
            this.Node = node;
            this.Type = type;
        }

        public bool IsConnected()
        {
            return Con != null;
        }

        public override bool TryGetIO(string ioName, bool modulesOnly, out IContainerIO container)
        {
            throw new Exception("Scalar IO can't contain additional io.");
        }

        public abstract void SetType(IFIRType type);
    }
}
