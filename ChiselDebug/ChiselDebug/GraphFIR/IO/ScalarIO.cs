using ChiselDebug.GraphFIR.Components;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VCDReader;
#nullable enable

namespace ChiselDebug.GraphFIR.IO
{
    public abstract class ScalarIO : FIRIO
    {
        public GroundType? Type { get; private set; }
        private bool IsInferingTypeNow = false;
        public ValueType? Value;

        public ScalarIO(FIRRTLNode? node, string? name, IFIRType? type) : base(node, name)
        {
            if (type is GroundType ground && ground.IsTypeFullyKnown())
            {
                this.Type = ground;
            }
        }

        public abstract bool IsConnectedToAnything();

        public abstract bool IsConnectedToAnythingPlaceable();

        public override List<ScalarIO> Flatten(List<ScalarIO> list)
        {
            list.Add(this);
            return list;
        }

        internal override void FlattenOnly<T>(ref Span<T> list)
        {
            if (this is T thisT)
            {
                list[0] = thisT;
                list = list.Slice(1);
            }
        }

        internal override void FlattenTo<T>(ref Span<T> list)
        {
            list[0] = (T)this;
            list = list.Slice(1);
        }

        public override int GetScalarsCount()
        {
            return 1;
        }

        public override int GetScalarsCountOfType<T>()
        {
            return this is T ? 1 : 0;
        }

        public override bool TryGetIO(string ioName, [NotNullWhen(true)] out IContainerIO? container)
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
            if (Value == null)
            {
                Value = new ValueType(Type);
            }
        }

        public ref BinaryVarValue GetValue()
        {
            if (Value == null)
            {
                throw new InvalidOperationException("Value was not set.");
            }

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
            return Value != null;
        }

        public abstract ScalarIO? GetPaired();
        public abstract void SetPaired(ScalarIO paired);
    }
}
