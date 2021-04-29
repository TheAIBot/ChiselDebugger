using FIRRTL;
using System;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR.IO
{
    public abstract class ScalarIO : FIRIO
    {
        public GroundType Type { get; private set; }
        private Output EnabledCond = null;
        private bool IsInferingTypeNow = false;
        public ValueType Value;
        public bool IsEnabled => EnabledCond == null || EnabledCond.Value.IsTrue();

        public ScalarIO(FIRRTLNode node, string name, IFIRType type) : base(node, name)
        {
            if (type is GroundType ground && ground.IsTypeFullyKnown())
            {
                this.Type = ground;
            }
        }

        public void SetEnabledCondition(Output enabledCond)
        {
            EnabledCond = enabledCond;
        }

        public Output GetConditional()
        {
            return EnabledCond;
        }

        public bool IsConditional()
        {
            return EnabledCond != null;
        }

        public abstract bool IsConnected();

        public abstract bool IsConnectedToAnything();

        public override IEnumerable<ScalarIO> Flatten()
        {
            yield return this;
        }

        public override List<ScalarIO> Flatten(List<ScalarIO> list)
        {
            list.Add(this);

            return list;
        }

        public override IEnumerable<FIRIO> WalkIOTree()
        {
            yield return this;
        }

        public override bool TryGetIO(string ioName, bool modulesOnly, out IContainerIO container)
        {
            container = null;
            return false;
        }

        public void SetType(IFIRType type)
        {
            if (type is GroundType ground && ground.IsTypeFullyKnown())
            {
                Type = ground;
            }
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

        public void SetDefaultvalue()
        {
            Value = new ValueType(Type);
        }
    }
}
