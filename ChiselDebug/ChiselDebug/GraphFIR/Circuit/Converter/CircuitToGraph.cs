using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.Transformations;
using System;
using System.Linq;

namespace ChiselDebug.GraphFIR.Circuit.Converter
{
    public static partial class CircuitToGraph
    {
        public static CircuitGraph GetAsGraph(FIRRTL.Circuit circuit, CircuitGraph graphLowFir = null)
        {
            VisitHelper helper = new VisitHelper(null, graphLowFir);
            foreach (var moduleDef in circuit.Modules)
            {
                helper.ModuleRoots.Add(moduleDef.Name, moduleDef);
            }

            FIRRTL.DefModule mainModDef = circuit.Modules.SingleOrDefault(x => x.Name == circuit.Main);
            if (mainModDef == null)
            {
                throw new ChiselDebugException("Circuit does not contain a module with the circuits name.");
            }
            Module mainModule = VisitModule(helper, null, mainModDef);

            RemoveDuplexWires.Transform(mainModule);
            MakeAggregateConnections.Transform(mainModule);
            BypassCondModBorderConnections.Transform(mainModule);
            FlattenConditionalStructure.Transform(mainModule);

            return new CircuitGraph(circuit.Main, mainModule);
        }

        private static Module VisitModule(VisitHelper parentHelper, string moduleInstanceName, FIRRTL.DefModule moduleDef)
        {
            if (moduleDef is FIRRTL.Module mod)
            {
                VisitHelper helper = parentHelper.ForNewModule(mod.Name, moduleInstanceName, mod);
                foreach (var port in mod.Ports)
                {
                    VisitPort(helper, port);
                }

                VisitStatement(helper, mod.Body);

                return helper.Mod;
            }
            else if (moduleDef is FIRRTL.ExtModule extMod)
            {
                VisitHelper helper = parentHelper.ForNewModule(extMod.Name, moduleInstanceName, extMod);
                foreach (var port in extMod.Ports)
                {
                    VisitPort(helper, port);
                }

                return helper.Mod;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
