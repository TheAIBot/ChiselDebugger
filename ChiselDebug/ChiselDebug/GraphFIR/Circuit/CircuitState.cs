using System;
using System.Collections.Generic;
using VCDReader;

namespace ChiselDebug.GraphFIR.Circuit
{
    public sealed class CircuitState
    {
        public readonly Dictionary<string, BinaryVarValue> VariableValues;
        public ulong Time { get; private set; }

        private CircuitState(CircuitState copyFrom)
        {
            VariableValues = new Dictionary<string, BinaryVarValue>(copyFrom.VariableValues);
            Time = copyFrom.Time;
        }

        public CircuitState(DumpVars initVarValues)
        {
            VariableValues = new Dictionary<string, BinaryVarValue>(initVarValues.InitialValues.Count);
            foreach (VarValue initValue in initVarValues.InitialValues)
            {
                var variable = initValue.Variables[0];
                VariableValues.Add(variable.ID, (BinaryVarValue)initValue);
            }

            Time = 0;
        }

        public CircuitState(List<List<VarDef>> varDefs)
        {
            VariableValues = new Dictionary<string, BinaryVarValue>(varDefs.Count);
            foreach (List<VarDef> variables in varDefs)
            {
                var variable = variables[0];
                BitState[] bits = new BitState[variable.Size];
                Array.Fill(bits, BitState.Zero);

                VariableValues.Add(variable.ID, new BinaryVarValue(bits, variables, true));
            }

            Time = 0;
        }

        internal void AddChanges(ReadOnlySpan<BinaryVarValue> changes, ulong time)
        {
            for (int i = 0; i < changes.Length; i++)
            {
                ref readonly var change = ref changes[i];
                var variable = change.Variables[0];
                VariableValues[variable.ID] = change;
            }

            Time = time;
        }

        internal void AddChange(BinaryVarValue value)
        {
            var variable = value.Variables[0];
            VariableValues[variable.ID] = value;
        }

        public CircuitState Copy()
        {
            return new CircuitState(this);
        }
    }
}
