using ChiselDebug;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Timeline;
using FIRRTL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using VCDReader;

[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]

namespace ChiselDebugTests
{
    internal static class TestTools
    {
        internal static CircuitGraph VerifyCanCreateGraph(string firrtl, CircuitGraph lowFirGraph = null)
        {
            Circuit circuit = FIRRTL.Parse.FromString(firrtl);
            CircuitGraph graph = CircuitToGraph.GetAsGraph(circuit, lowFirGraph);

            return graph;
        }

        private static CircuitGraph VerifyCanCreateGraphFromFile(string firrtlPath, CircuitGraph lowFirGraph = null)
        {
            using FileStream fileStream = new FileStream(firrtlPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            Circuit circuit = FIRRTL.Parse.FromStream(fileStream);
            CircuitGraph graph = CircuitToGraph.GetAsGraph(circuit, lowFirGraph);

            return graph;
        }

        internal static void VerifyChiselTest(string moduleName, string extension)
        {
            const string modulePath = "ChiselTests";

            CircuitGraph lowFirGraph = null;
            if (extension == "fir")
            {
                string loFirPath = Path.Combine(modulePath, $"{moduleName}.lo.fir");
                lowFirGraph = VerifyCanCreateGraphFromFile(loFirPath);
            }

            string firPath = Path.Combine(modulePath, $"{moduleName}.{extension}");
            CircuitGraph graph = VerifyCanCreateGraphFromFile(firPath, lowFirGraph);

            using VCD vcd = VCDReader.Parse.FromFile($"{modulePath}/{moduleName}.vcd");
            VCDTimeline timeline = new VCDTimeline(vcd);

            VerifyCircuitState(graph, timeline, isLoFIRRTL(extension), false);
        }

        private static VCDTimeline MakeTimeline(string moduleName, string modulePath)
        {
            using VCD vcd = LoadVCD(moduleName, modulePath);
            return new VCDTimeline(vcd);
        }

        private static VCD LoadVCD(string moduleName, string modulePath)
        {
            return VCDReader.Parse.FromFile($"{modulePath}/{moduleName}.vcd");
        }

        internal static CircuitGraph VerifyMakeGraph(string moduleName, string extension, string modulePath)
        {
            CircuitGraph lowFirGraph = null;
            if (extension == "fir")
            {
                string loFirPath = Path.Combine(modulePath, $"{moduleName}.lo.fir");
                lowFirGraph = VerifyCanCreateGraphFromFile(loFirPath);
            }

            string firPath = Path.Combine(modulePath, $"{moduleName}.{extension}");
            return VerifyCanCreateGraphFromFile(firPath, lowFirGraph);
        }

        internal static void VerifyInferTypes(string moduleName, string extension, bool isVerilogVCD, string modulePath)
        {
            CircuitGraph graph = VerifyMakeGraph(moduleName, extension, modulePath);
            using VCD vcd = LoadVCD(moduleName, modulePath);

            foreach (var variables in vcd.Variables)
            {
                foreach (var variable in variables)
                {
                    ScalarIO varCon = graph.GetConnection(variable);
                    if (varCon == null)
                    {
                        continue;
                    }

                    ref BinaryVarValue actual = ref varCon.GetValue();
                    if (!varCon.HasValue())
                    {
                        continue;
                    }
                    if (variable.Size != actual.Bits.Length)
                    {
                        Console.WriteLine($"Ref: {variable.Reference}, Expected: {variable.Size}, Actual: {actual.Bits.Length}");
                    }
                    Assert.AreEqual(variable.Size, actual.Bits.Length);
                }
            }
        }

        internal static void VerifyComputeGraph(string moduleName, string extension, bool isVerilogVCD, string modulePath)
        {
            CircuitGraph graph = VerifyMakeGraph(moduleName, extension, modulePath);
            VCDTimeline timeline = MakeTimeline(moduleName, modulePath);

            VerifyCircuitState(graph, timeline, isLoFIRRTL(extension), isVerilogVCD);
        }

        private static bool isLoFIRRTL(string extension)
        {
            return extension == "treadle.lo.fir";
        }

        public readonly struct VarIDToIO
        {
            public readonly VarDef VariableDef;
            public readonly ScalarIO IO;

            public VarIDToIO(VarDef variable, ScalarIO io)
            {
                this.VariableDef = variable;
                this.IO = io;
            }
        }

        internal static VarIDToIO[] GetUsedVars(CircuitGraph graph, VCDTimeline timeline, bool isLoFIRRTL, bool isVerilogVCD, WireIgnoreTracker ignoreTracker)
        {
            List<VarIDToIO> varIDToIO = new List<VarIDToIO>();
            CircuitState firstState = timeline.GetFirstState();
            foreach (BinaryVarValue expected in firstState.VariableValues.Values)
            {
                foreach (var variable in expected.Variables)
                {
                    ScalarIO varCon = graph.GetConnection(variable);
                    if (varCon == null)
                    {
                        ignoreTracker.IgnoreBecauseNotExist(variable);
                        continue;
                    }

                    //Input to memory read port is delayed by passing it through registers but this
                    //step is only done as the last step in the firrtl transformation and therefore
                    //is only present in loFIRRTL. Inputs to en and addr for read port can therefore
                    //only be compared hen running loFIRRTL because other wise the delay is not added
                    //and thus not simulated here.
                    if (!isLoFIRRTL && varCon.IsPartOfAggregateIO &&
                        ((varCon.ParentIO is MemReadPort && (varCon.Name == "en" || varCon.Name == "addr")) ||
                         (varCon.ParentIO is MemRWPort && (varCon.Name == "en" || varCon.Name == "addr" || varCon.Name == "wmode"))))
                    {
                        ignoreTracker.IgnorebecauseMemReadDelay(variable);
                        continue;
                    }

                    varIDToIO.Add(new VarIDToIO(variable, varCon));
                }
            }
            return varIDToIO.ToArray();
        }

        internal static void VerifyCircuitState(CircuitGraph graph, VCDTimeline timeline, bool isLoFIRRTL, bool isVerilogVCD)
        {
            WireIgnoreTracker ignoreTracker = new WireIgnoreTracker();
            VarIDToIO[] varsToCheck = GetUsedVars(graph, timeline, isLoFIRRTL, isVerilogVCD, ignoreTracker);

            List<string> stateErrors = new List<string>();
            foreach (var state in timeline.GetAllDistinctStates())
            {
                graph.SetState(state);
                if (state.Time == timeline.TimeInterval.StartInclusive)
                {
                    continue;
                }
                if (state.Time == timeline.TimeInterval.InclusiveEnd())
                {
                    break;
                }

                graph.ComputeRemainingGraphFast();
                for (int z = 0; z < varsToCheck.Length; z++)
                {
                    ref var expected = ref CollectionsMarshal.GetValueRefOrNullRef(state.VariableValues, varsToCheck[z].VariableDef.ID);
                    ScalarIO varCon = varsToCheck[z].IO;

                    ref BinaryVarValue actual = ref (varCon is Input input ? ref input.UpdateValueFromSourceFast() : ref varCon.GetValue());
                    if (expected.IsValidBinary && actual.IsValidBinary)
                    {
                        if (!expected.SameValue(ref actual))
                        {
                            stateErrors.Add($"\nTime: {state.Time.ToString("N0")}\nName: {varCon.GetFullName()}\nExpected: {expected.BitsToString()}\nActual:   {actual.BitsToString()}\n");
                        }
                    }
                    else
                    {
                        for (int i = 0; i < expected.Bits.Length; i++)
                        {
                            if (!actual.Bits[i].IsBinary())
                            {
                                ignoreTracker.IgnoreBecauseUnknown(varsToCheck[z].VariableDef);
                                break;
                            }

                            if (expected.Bits[i] != actual.Bits[i])
                            {
                                //graph.SetStateFast(state, isVerilogVCD);
                                stateErrors.Add($"\nTime: {state.Time.ToString("N0")}\nName: {varCon.GetFullName()}\nExpected: {expected.BitsToString()}\nActual:   {actual.BitsToString()}\n");
                                break;
                            }
                        }
                    }

                    if (stateErrors.Count > 0)
                    {
                        ignoreTracker.WriteToConsole();
                        Console.WriteLine();
                        Console.WriteLine(graph.StateToString());
                        Assert.Fail(string.Join('\n', stateErrors));
                    }
                }
            }

            ignoreTracker.WriteToConsole();
        }
    }
}
