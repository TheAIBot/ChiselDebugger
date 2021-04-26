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
        public readonly Dictionary<VarDef, VarValue> VariableValues = new Dictionary<VarDef, VarValue>();

        private CircuitState(CircuitState copyFrom)
        {
            foreach (var keyValue in copyFrom.VariableValues)
            {
                VariableValues.Add(keyValue.Key, keyValue.Value);
            }
        }

        public CircuitState(DumpVars initVarValues)
        {
            foreach (var initValue in initVarValues.InitialValues)
            {
                foreach (var variable in initValue.Variables)
                {
                    VariableValues.Add(variable, initValue);
                }
            }
        }

        public CircuitState(List<List<VarDef>> varDefs)
        {
            foreach (var variables in varDefs)
            {
                foreach (var variable in variables)
                {
                    BitState[] bits = new BitState[variable.Size];
                    Array.Fill(bits, BitState.Zero);

                    VariableValues.Add(variable, new BinaryVarValue(bits, variables));
                }
            }
        }

        internal void AddChanges(TimeStepChanges changes)
        {
            foreach (var change in changes.Changes)
            {
                foreach (var variable in change.Variables)
                {
                    VariableValues[variable] = change;
                }
            }
        }

        public CircuitState Copy()
        {
            return new CircuitState(this);
        }
    }
}
