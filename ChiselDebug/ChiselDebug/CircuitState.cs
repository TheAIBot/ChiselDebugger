using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Timeline;
using System;
using System.Collections.Generic;
using VCDReader;

namespace ChiselDebug
{
    public class CircuitState
    {
        public readonly Dictionary<VarDef, BinaryVarValue> VariableValues;
        public ulong Time { get; private set; }

        private CircuitState(CircuitState copyFrom)
        {
            this.VariableValues = new Dictionary<VarDef, BinaryVarValue>(copyFrom.VariableValues, new VarDefComparar());
            this.Time = copyFrom.Time;
        }

        public CircuitState(DumpVars initVarValues)
        {
            VariableValues = new Dictionary<VarDef, BinaryVarValue>(initVarValues.InitialValues.Count, new VarDefComparar());
            foreach (var initValue in initVarValues.InitialValues)
            {
                foreach (var variable in initValue.Variables)
                {
                    VariableValues.Add(variable, (BinaryVarValue)initValue);
                }
            }

            this.Time = 0;
        }

        public CircuitState(List<List<VarDef>> varDefs)
        {
            VariableValues = new Dictionary<VarDef, BinaryVarValue>(varDefs.Count, new VarDefComparar());
            foreach (var variables in varDefs)
            {
                foreach (var variable in variables)
                {
                    BitState[] bits = new BitState[variable.Size];
                    Array.Fill(bits, BitState.Zero);

                    VariableValues.Add(variable, new BinaryVarValue(bits, variables, true));
                }
            }

            this.Time = 0;
        }

        internal void AddChanges(ReadOnlySpan<BinaryVarValue> changes, ulong time)
        {
            foreach (var change in changes)
            {
                foreach (var variable in change.Variables)
                {
                    VariableValues[variable] = change;
                }
            }

            Time = Time;
        }

        internal void AddChange(BinaryVarValue value)
        {
            foreach (var variable in value.Variables)
            {
                VariableValues[variable] = value;
            }
        }

        public CircuitState Copy()
        {
            return new CircuitState(this);
        }
    }
}
