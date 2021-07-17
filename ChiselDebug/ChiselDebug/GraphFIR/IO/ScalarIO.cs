using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using VCDReader;

namespace ChiselDebug.GraphFIR.IO
{
    public abstract class ScalarIO : FIRIO
    {
        public GroundType Type { get; private set; }
        private bool IsInferingTypeNow = false;
        public ValueType Value;

        public ScalarIO(FIRRTLNode node, string name, IFIRType type) : base(node, name)
        {
            if (type is GroundType ground && ground.IsTypeFullyKnown())
            {
                this.Type = ground;
            }
        }

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

        public override bool TryGetIO(string ioName, out IContainerIO container)
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

        internal void RemoveType()
        {
            Type = null;
        }

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

        public ref BinaryVarValue GetValue()
        {
            return ref Value.Value;
        }

        public string GetGroundTypeName()
        {
            if (Type is UIntType)
            {
                return $"UInt<{Type.Width}>";
            }
            else if (Type is SIntType)
            {
                return $"SInt<{Type.Width}>";
            }
            else if (Type is ClockType)
            {
                return $"Clock";
            }
            else if (Type is AsyncResetType)
            {
                return $"AsyncReset";
            }
            else if (Type is ResetType)
            {
                return $"Reset";
            }
            else if (Type is AnalogType)
            {
                return $"Analog<{Type.Width}>";
            }
            else
            {
                throw new Exception("Unknown type.");
            }
        }

        public abstract ref BinaryVarValue FetchValue();

        public bool HasValue()
        {
            return Value.IsInitialized();
        }

        public abstract ScalarIO GetPaired();
        public abstract void SetPaired(ScalarIO paired);
    }
}
