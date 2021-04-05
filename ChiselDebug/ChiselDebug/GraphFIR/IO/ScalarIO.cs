using FIRRTL;
using System;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR.IO
{
    public abstract class ScalarIO : FIRIO
    {
        public readonly FIRRTLNode Node;
        public IFIRType Type { get; protected set; }
        public Connection Con = null;
        private Connection EnabledCond = null;
        public bool IsEnabled => EnabledCond == null || EnabledCond.Value.IsTrue();

        public ScalarIO(FIRRTLNode node, IFIRType type) : this(node, string.Empty, type)
        { }

        public ScalarIO(FIRRTLNode node, string name, IFIRType type) : base(name)
        {
            this.Node = node;
            this.Type = type;
        }

        public void SetEnabledCondition(Connection enabledCond)
        {
            EnabledCond = enabledCond;
        }

        public virtual bool IsConnected()
        {
            return Con != null;
        }

        public virtual bool IsConnectedToAnything()
        {
            return Con != null && Con.To.Count > 0;
        }

        public override IEnumerable<ScalarIO> Flatten()
        {
            yield return this;
        }

        public override bool TryGetIO(string ioName, bool modulesOnly, out IContainerIO container)
        {
            throw new Exception("Scalar IO can't contain additional io.");
        }

        public abstract void SetType(IFIRType type);

        public abstract void InferType();
    }
}
