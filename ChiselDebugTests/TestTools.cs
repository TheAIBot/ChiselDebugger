using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Timeline;
using FIRRTL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCDReader;

//[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]

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
            using TextReader textStream = new StreamReader(fileStream);

            Circuit circuit = FIRRTL.Parse.FromStream(textStream);
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

            VerifyCircuitState(graph, timeline, false);
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
                    ScalarIO varCon = graph.GetConnection(variable, isVerilogVCD);
                    if (varCon == null)
                    {
                        continue;
                    }

                    ref BinaryVarValue  actual = ref varCon.GetValue();
                    if (!varCon.Value.IsInitialized())
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

            VerifyCircuitState(graph, timeline, isVerilogVCD);
        }

        internal static void VerifyCircuitState(CircuitGraph graph, VCDTimeline timeline, bool isVerilogVCD)
        {
            int totalWireStates = 0;
            int ignoredWireStates = 0;
            List<string> stateErrors = new List<string>();
            foreach (var state in timeline.GetAllDistinctStates())
            {
                graph.SetState(state, isVerilogVCD);
                if (state.Time == timeline.TimeInterval.StartInclusive)
                {
                    continue;
                }
                if (state.Time == timeline.TimeInterval.InclusiveEnd())
                {
                    break;
                }

                graph.ComputeRemainingGraphFast();
                foreach (BinaryVarValue expected in state.VariableValues.Values)
                {
                
                    foreach (var variable in expected.Variables)
                    {
                        ScalarIO varCon = graph.GetConnection(variable, isVerilogVCD);
                        if (varCon is Input input)
                        {
                            input.UpdateValueFromSource();
                        }
                        if (varCon == null)
                        {
                            continue;
                        }

                        totalWireStates++;
                        ref BinaryVarValue actual = ref varCon.GetValue();

                        for (int i = 0; i < Math.Min(expected.Bits.Length, actual.Bits.Length); i++)
                        {
                            if (!actual.Bits[i].IsBinary())
                            {
                                ignoredWireStates++;
                                break;
                            }

                            if (expected.Bits[i] != actual.Bits[i])
                            {
                                //graph.SetStateFast(state, isVerilogVCD);
                                stateErrors.Add($"\nTime: {state.Time.ToString("N0")}\nName: {expected.Variables[0].Reference}\nExpected: {expected.BitsToString()}\nActual:   {actual.BitsToString()}\n");
                                goto skipCheckRest;
                            }
                        }
                    }
                skipCheckRest:
                    int q = 0;
                }

                if (stateErrors.Count > 0)
                {
                    Console.WriteLine($"Wire states: {totalWireStates.ToString("N0")}\nIgnored: {ignoredWireStates.ToString("N0")}");
                    Console.WriteLine();
                    Console.WriteLine(graph.StateToString());
                    Assert.Fail(string.Join('\n', stateErrors));
                }
            }

            Console.WriteLine($"Wire states: {totalWireStates.ToString("N0")}\nIgnored: {ignoredWireStates.ToString("N0")}");
        }
    }
}
