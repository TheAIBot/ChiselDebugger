using ChiselDebug;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebugTests
{
    internal static class TestTools
    {
        internal static void VerifyCanCreateGraph(string firrtl)
        {
            Circuit circuit = FIRRTL.Parse.FromString(firrtl);
            CircuitGraph graph = CircuitToGraph.GetAsGraph(circuit);
            graph.InferTypes();
        }
    }
}
