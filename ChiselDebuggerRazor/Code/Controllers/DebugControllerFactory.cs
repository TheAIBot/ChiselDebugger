using ChiselDebug.GraphFIR.Circuit;
using ChiselDebug.GraphFIR.Circuit.Converter;
using ChiselDebuggerRazor.Code.Templates;
using FIRRTL;
using System.IO;

namespace ChiselDebuggerRazor.Code.Controllers
{
    public sealed class DebugControllerFactory
    {
        private readonly IWorkLimiter WorkLimiter;

        public DebugControllerFactory(IWorkLimiter workLimiter)
        {
            WorkLimiter = workLimiter;
        }

        public DebugController Create(Stream? loFirStream, Stream hiFirStream)
        {
            CircuitGraph? loGraph = null;
            if (loFirStream != null)
            {
                Circuit locircuit = Parse.FromStream(loFirStream);
                loGraph = CircuitToGraph.GetAsGraph(locircuit);
            }

            Circuit circuit = Parse.FromStream(hiFirStream);
            CircuitGraph graph = CircuitToGraph.GetAsGraph(circuit, loGraph);

            return new DebugController(graph, new PlacementTemplator(WorkLimiter), new RouteTemplator(WorkLimiter), new SeqWorkOverrideOld<ulong>(WorkLimiter));
        }
    }
}
