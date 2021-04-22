﻿using ChiselDebug;
using ChiselDebug.GraphFIR;
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

        internal static void VerifyChiselTest(string moduleName, string extension, bool testVCD)
        {
            CircuitGraph lowFirGraph = null;
            if (extension == "fir")
            {
                lowFirGraph = VerifyCanCreateGraph(File.ReadAllText($"ChiselTests/{moduleName}.lo.fir"));
            }

            CircuitGraph graph = VerifyCanCreateGraph(File.ReadAllText($"ChiselTests/{moduleName}.{extension}"), lowFirGraph);

            if (testVCD)
            {
                using VCD vcd = VCDReader.Parse.FromFile($"ChiselTests/{moduleName}.vcd");
                VCDTimeline timeline = new VCDTimeline(vcd);

                VerifyCircuitState(graph, timeline);
            }
        }

        internal static void VerifyCircuitState(CircuitGraph graph, VCDTimeline timeline)
        {
            foreach (var time in timeline.GetAllSimTimes())
            {
                CircuitState state = timeline.GetStateAtTime(time);
                graph.SetState(state);

                foreach (BinaryVarValue expected in state.VariableValues.Values)
                {
                    Connection varCon = graph.GetConnection(expected.Variable);
                    if (varCon == null)
                    {
                        continue;
                    }

                    BinaryVarValue actual = varCon.Value.GetValue();
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
                }
            }
        }
    }
}
