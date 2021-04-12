using FIRRTL;
using System;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR.IO
{
    public abstract class ScalarIO : FIRIO
    {
        public IFIRType Type { get; private set; }
        public Connection Con = null;
        private Connection EnabledCond = null;
        private bool IsInferingTypeNow = false;
        public bool IsEnabled => EnabledCond == null || EnabledCond.Value.IsTrue();

        public ScalarIO(FIRRTLNode node, IFIRType type) : this(node, string.Empty, type)
        { }

        public ScalarIO(FIRRTLNode node, string name, IFIRType type) : base(node, name)
        {
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

        public override IEnumerable<FIRIO> WalkIOTree()
        {
            yield return this;
        }

        public override bool TryGetIO(string ioName, bool modulesOnly, out IContainerIO container)
        {
            throw new Exception("Scalar IO can't contain additional io.");
        }

        public virtual void SetType(IFIRType type)
        {
            Type = type;
        }

        public abstract void DisconnectAll();

        public void InferType()
        {
            if (IsInferingTypeNow)
            {
                return;
            }

            IsInferingTypeNow = true;
            InferGroundType();
            IsInferingTypeNow = false;
        }

        public abstract void InferGroundType();
    }
}
