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
        public readonly Dictionary<string, BinaryVarValue> VariableValues;
        public ulong Time { get; private set; }

        private CircuitState(CircuitState copyFrom)
        {
            this.VariableValues = new Dictionary<string, BinaryVarValue>(copyFrom.VariableValues);
            this.Time = copyFrom.Time;
        }

        public CircuitState(DumpVars initVarValues)
        {
            VariableValues = new Dictionary<string, BinaryVarValue>(initVarValues.InitialValues.Count);
            foreach (var initValue in initVarValues.InitialValues)
            {
                var variable = initValue.Variables[0];
                VariableValues.Add(variable.ID, (BinaryVarValue)initValue);
            }

            this.Time = 0;
        }

        public CircuitState(List<List<VarDef>> varDefs)
        {
            VariableValues = new Dictionary<string, BinaryVarValue>(varDefs.Count);
            foreach (var variables in varDefs)
            {
                var variable = variables[0];
                BitState[] bits = new BitState[variable.Size];
                Array.Fill(bits, BitState.Zero);

                VariableValues.Add(variable.ID, new BinaryVarValue(bits, variables, true));
            }

            this.Time = 0;
        }

        internal void AddChanges(ReadOnlySpan<BinaryVarValue> changes, ulong time)
        {
            foreach (var change in changes)
            {
                var variable = change.Variables[0];
                VariableValues[variable.ID] = change;
            }

            Time = Time;
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
