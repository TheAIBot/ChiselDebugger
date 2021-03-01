using System.Collections.Generic;

namespace ChiselDebug
{
    public class CircuitGraph
    {
        public readonly string Name;
        public readonly List<GraphFIR.Module> Modules = new List<GraphFIR.Module>();

        public CircuitGraph(string name, List<GraphFIR.Module> modules)
        {
            this.Name = name;
            this.Modules = modules;
        }

        public void InferTypes()
        {
            foreach (var mod in Modules)
            {
                mod.InferType();
            }
        }
    }
}
