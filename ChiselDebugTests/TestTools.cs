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
        internal static CircuitGraph VerifyCanCreateGraph(string firrtl)
        {
            Circuit circuit = FIRRTL.Parse.FromString(firrtl);
            CircuitGraph graph = CircuitToGraph.GetAsGraph(circuit);
            graph.InferTypes();

            return graph;
        }

        internal static void VerifyChiselTest(string moduleName, string extension, bool testVCD)
        {
            CircuitGraph graph = TestTools.VerifyCanCreateGraph(File.ReadAllText($"ChiselTests/{moduleName}.{extension}"));

            if (testVCD)
            {
                VCD vcd = VCDReader.Parse.FromFile($"ChiselTests/{moduleName}.vcd");
                VCDTimeline timeline = new VCDTimeline(vcd);

                graph.SetState(timeline.GetStateAtTime(0));
            }
        }
    }
}
