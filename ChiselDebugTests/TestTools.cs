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

        internal static void VerifyChiselTest(string moduleName, string extension, bool testVCD, string modulePath = "ChiselTests", bool isVerilogVCD = true)
        {
            CircuitGraph lowFirGraph = null;
            if (extension == "fir")
            {
                string loFirPath = Path.Combine(modulePath, $"{moduleName}.lo.fir");
                lowFirGraph = VerifyCanCreateGraphFromFile(loFirPath);
            }

            string firPath = Path.Combine(modulePath, $"{moduleName}.{extension}");
            CircuitGraph graph = VerifyCanCreateGraphFromFile(firPath, lowFirGraph);

            if (testVCD)
            {
                using VCD vcd = VCDReader.Parse.FromFile($"{modulePath}/{moduleName}.vcd");
                VCDTimeline timeline = new VCDTimeline(vcd);

                VerifyCircuitState(graph, timeline, isVerilogVCD);
            }
        }

        private static VCDTimeline MakeTimeline(string moduleName, string modulePath)
        {
            using VCD vcd = VCDReader.Parse.FromFile($"{modulePath}/{moduleName}.vcd");
            return new VCDTimeline(vcd);
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

            VCDTimeline timeline = MakeTimeline(moduleName, modulePath);
            CircuitState state = timeline.GetStateAtTime(timeline.TimeInterval.StartInclusive);

            foreach (BinaryVarValue expected in state.VariableValues.Values)
            {
                foreach (var variable in expected.Variables)
                {
                    ScalarIO varCon = graph.GetConnection(variable, isVerilogVCD);
                    if (varCon == null)
                    {
                        continue;
                    }

                    BinaryVarValue actual = varCon.Value.GetValue();
                    Assert.AreEqual(expected.Bits.Length, actual.Bits.Length);
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
            foreach (var state in timeline.GetAllDistinctStates())
            {
                graph.SetState(state, isVerilogVCD);

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

                        BinaryVarValue actual = varCon.Value.GetValue();
                        if (expected.Bits.Length != actual.Bits.Length)
                        {

                        }
                        Assert.AreEqual(expected.Bits.Length, actual.Bits.Length);
                        for (int i = 0; i < expected.Bits.Length; i++)
                        {
                            if (!actual.Bits[i].IsBinary())
                            {
                                continue;
                            }

                            if (expected.Bits[i] != actual.Bits[i])
                            {

                            }
                            Assert.AreEqual(expected.Bits[i], actual.Bits[i]);
                        }

                        //Console.WriteLine($"Name: {expected.Variables[0].Reference}\nExpected: {expected.BitsToString()}\nActual:   {actual.BitsToString()}\n");
                    }
                }
            }
        }
    }
}
