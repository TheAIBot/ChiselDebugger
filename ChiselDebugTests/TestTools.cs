using ChiselDebug;
using ChiselDebug.Timeline;
using FIRRTL;
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
                VCD vcd = VCDReader.Parse.FromFile($"ChiselTests/{moduleName}.vcd");
                VCDTimeline timeline = new VCDTimeline(vcd);

                graph.SetState(timeline.GetStateAtTime(0));
            }
        }
    }
}
