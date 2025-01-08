using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR.Circuit.Converter
{
    internal sealed class RootVisitHelper : IVisitHelper
    {
        private readonly CircuitGraph? LowFirGraph;
        private long UniqueNumber = 0;

        public Module Mod => throw new InvalidOperationException();
        public bool IsConditionalModule => throw new InvalidOperationException();

        public Source ScopeEnabledCond => throw new InvalidOperationException();
        public Dictionary<string, FIRRTL.IDefModule> ModuleRoots { get; } = new();

        public RootVisitHelper(CircuitGraph? lowFirGraph)
        {
            LowFirGraph = lowFirGraph;
            ModuleRoots = new Dictionary<string, FIRRTL.IDefModule>();
        }

        public void AddNodeToModule(FIRRTLNode node)
        {
            throw new InvalidOperationException();
        }

        public void EnterEnabledScope(Source enableCond)
        {
            throw new InvalidOperationException();
        }

        public void ExitEnabledScope()
        {
            throw new InvalidOperationException();
        }

        public IVisitHelper ForNewCondModule(string moduleName, FIRRTL.IDefModule? moduleDef)
        {
            throw new InvalidOperationException();
        }

        public IVisitHelper ForNewModule(string moduleName, string instanceName, FIRRTL.IDefModule moduleDef)
        {
            return new VisitHelper(new Module(moduleName, instanceName, null, moduleDef), LowFirGraph, ModuleRoots, this, false, this);
        }

        public FIRRTL.IFirrtlNode GetDefNodeFromLowFirrtlGraph(string nodeName)
        {
            throw new InvalidOperationException();
        }

        public bool HasLowFirGraph()
        {
            throw new NotImplementedException();
        }

        public string GetUniqueName()
        {
            return $"~{UniqueNumber++}";
        }
    }
}
