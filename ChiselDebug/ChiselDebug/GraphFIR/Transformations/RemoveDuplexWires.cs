using ChiselDebug.GraphFIR.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.GraphFIR.Transformations
{
    internal class RemoveDuplexWires
    {
        public static void Transform(Module mainModule)
        {
            foreach (var mod in mainModule.GetAllNestedNodesOfType<Module>())
            {
                //In a truely stupid move, FIRRTL supports connecting
                //Sinks to other sinks. In order to support that case
                //a sink can pretend to be a source. It's important 
                //that they stop pretending after the module graph
                //has been made because this hack shouldn't be
                //visible outside of graph creation. Everything else
                //should still work on the assumption that only
                //connections from a source to a sink are possible.
                mod.RemoveAllDuplexWires();
            }
        }
    }
}
