using ChiselDebug.GraphFIR.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ChiselDebug.GraphFIR.Circuit.Converter
{
    internal sealed class VisitHelper : IVisitHelper
    {
        private readonly CircuitGraph? LowFirGraph;
        private readonly IVisitHelper ParentHelper;
        private readonly RootVisitHelper RootHelper;

        private readonly Stack<IO.Source> ScopeEnabledConditions = new Stack<IO.Source>();

        public Module Mod { get; }
        public bool IsConditionalModule { get; }
        public IO.Source ScopeEnabledCond
        {
            get
            {
                if (ScopeEnabledConditions.Count == 0)
                {
                    ConstValue constEnabled = new ConstValue(new FIRRTL.UIntLiteral(1, 1));
                    AddNodeToModule(constEnabled);

                    ScopeEnabledConditions.Push(constEnabled.Result);
                }

                return ScopeEnabledConditions.Peek();
            }
        }
        public Dictionary<string, FIRRTL.IDefModule> ModuleRoots { get; }

        public VisitHelper(Module mod, CircuitGraph? lowFirGraph, Dictionary<string, FIRRTL.IDefModule> roots, IVisitHelper parentHelper, bool isConditional, RootVisitHelper rootHelper)
        {
            Mod = mod;
            LowFirGraph = lowFirGraph;
            ModuleRoots = roots;
            ParentHelper = parentHelper;
            IsConditionalModule = isConditional;
            RootHelper = rootHelper;
        }

        public IVisitHelper ForNewModule(string moduleName, string instanceName, FIRRTL.IDefModule moduleDef)
        {
            return new VisitHelper(new Module(moduleName, instanceName, Mod, moduleDef), LowFirGraph, ModuleRoots, this, false, RootHelper);
        }

        public IVisitHelper ForNewCondModule(string moduleName, FIRRTL.IDefModule? moduleDef)
        {
            return new VisitHelper(new Module(moduleName, null, Mod, moduleDef), LowFirGraph, ModuleRoots, this, true, RootHelper);
        }

        public void AddNodeToModule(FIRRTLNode node)
        {
            Mod.AddNode(node);
        }

        public void EnterEnabledScope(IO.Source enableCond)
        {
            ScopeEnabledConditions.Push(enableCond);
        }

        public void ExitEnabledScope()
        {
            ScopeEnabledConditions.Pop();
        }

        private string[] GetPathToCurrentActualModule()
        {
            List<string> path = new List<string>();

            IVisitHelper helper = this;
            while (helper is VisitHelper visitHelper)
            {
                if (!visitHelper.IsConditionalModule)
                {
                    path.Add(visitHelper.Mod.Name);
                }

                helper = visitHelper.ParentHelper;
            }

            //Path is from node to root but want from root to node
            //which is why the path is reversed
            path.Reverse();

            return path.ToArray();
        }

        [MemberNotNullWhen(true, nameof(LowFirGraph))]
        public bool HasLowFirGraph()
        {
            return LowFirGraph != null;
        }

        public FIRRTL.IFirrtlNode GetDefNodeFromLowFirrtlGraph(string nodeName)
        {
            Module lowFirMod = LowFirGraph.MainModule;

            string[] pathToModule = GetPathToCurrentActualModule();

            //Skip first module name as it's the name of the root node
            //that we start with
            foreach (var pathModName in pathToModule.Skip(1))
            {
                FIRRTLNode[] lowModNodes = lowFirMod.GetAllNodes();
                FIRRTLNode? childLowModNode = lowModNodes.FirstOrDefault(x => x is Module mod && mod.Name == pathModName);
                if (childLowModNode == null)
                {
                    throw new Exception("High level firrtl module path didn't match low level firrtl module path.");
                }

                lowFirMod = (Module)childLowModNode;
            }

            //This is a meh way of going about getting the correct node.
            //Nodes by themselves don't have a name so it works on the assumption
            //that there exists an io with the name which points to the correct node.
            IO.FIRIO nodeIO = (IO.FIRIO)lowFirMod.GetIO(nodeName);
            return nodeIO.Flatten().First().Node.FirDefNode;
        }

        public string GetUniqueName()
        {
            return RootHelper.GetUniqueName();
        }
    }
}
