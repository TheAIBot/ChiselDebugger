﻿using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Timeline;
using System;
using System.Collections.Generic;
using VCDReader;

namespace ChiselDebug
{
    public class CircuitState
    {
        public readonly Dictionary<VarDef, BinaryVarValue> VariableValues = new Dictionary<VarDef, BinaryVarValue>(new VarDefComparar());
        public ulong Time { get; private set; }

        private CircuitState(CircuitState copyFrom)
        {
            foreach (var keyValue in copyFrom.VariableValues)
            {
                VariableValues.Add(keyValue.Key, keyValue.Value);
            }

            this.Time = copyFrom.Time;
        }

        public CircuitState(DumpVars initVarValues)
        {
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
            foreach (var variables in varDefs)
            {
                foreach (var variable in variables)
                {
                    BitState[] bits = new BitState[variable.Size];
                    Array.Fill(bits, BitState.Zero);

                    VariableValues.Add(variable, new BinaryVarValue(bits, variables));
                }
            }

            this.Time = 0;
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

            Time = changes.Time;
        }

        public CircuitState Copy()
        {
            return new CircuitState(this);
        }
    }
}
