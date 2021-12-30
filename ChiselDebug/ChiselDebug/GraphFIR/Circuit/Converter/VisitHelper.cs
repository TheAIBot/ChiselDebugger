using ChiselDebug.GraphFIR.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.Circuit.Converter
{
    internal class VisitHelper
    {
        public readonly Module Mod;
        private readonly CircuitGraph LowFirGraph;
        public readonly Dictionary<string, FIRRTL.DefModule> ModuleRoots;
        public readonly bool IsConditionalModule;
        private readonly VisitHelper ParentHelper;
        private readonly VisitHelper RootHelper;

        private readonly Stack<IO.Output> ScopeEnabledConditions = new Stack<IO.Output>();
        public IO.Output ScopeEnabledCond
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

        public VisitHelper(Module mod, CircuitGraph lowFirGraph) : this(mod, lowFirGraph, new Dictionary<string, FIRRTL.DefModule>(), null, false, null)
        { }

        private VisitHelper(Module mod, CircuitGraph lowFirGraph, Dictionary<string, FIRRTL.DefModule> roots, VisitHelper parentHelper, bool isConditional, VisitHelper rootHelper)
        {
            Mod = mod;
            LowFirGraph = lowFirGraph;
            ModuleRoots = roots;
            ParentHelper = parentHelper;
            IsConditionalModule = isConditional;
            RootHelper = rootHelper;
        }

        public VisitHelper ForNewModule(string moduleName, string instanceName, FIRRTL.DefModule moduleDef)
        {
            return new VisitHelper(new Module(moduleName, instanceName, Mod, moduleDef), LowFirGraph, ModuleRoots, this, false, RootHelper ?? this);
        }

        public VisitHelper ForNewCondModule(string moduleName, FIRRTL.DefModule moduleDef)
        {
            return new VisitHelper(new Module(moduleName, null, Mod, moduleDef), LowFirGraph, ModuleRoots, this, true, RootHelper ?? this);
        }

        public void AddNodeToModule(FIRRTLNode node)
        {
            Mod.AddNode(node);
        }

        public void EnterEnabledScope(IO.Output enableCond)
        {
            ScopeEnabledConditions.Push(enableCond);
        }

        public void ExitEnabledScope()
        {
            ScopeEnabledConditions.Pop();
        }

        private string GetActualModuleName()
        {
            VisitHelper helper = this;
            while (helper.IsConditionalModule)
            {
                helper = helper.ParentHelper;
            }

            return helper.Mod.Name;
        }

        private string[] GetPathToCurrentActualModule()
        {
            List<string> path = new List<string>();

            VisitHelper helper = this;
            while (helper.Mod != null)
            {
                if (!helper.IsConditionalModule)
                {
                    path.Add(helper.Mod.Name);
                }

                helper = helper.ParentHelper;
            }

            //Path is from node to root but want from root to node
            //which is why the path is reversed
            path.Reverse();

            return path.ToArray();
        }

        public bool HasLowFirGraph()
        {
            return LowFirGraph != null;
        }

        public bool IsHighFirGrapth()
        {
            return HasLowFirGraph();
        }

        public FIRRTL.FirrtlNode GetDefNodeFromLowFirrtlGraph(string nodeName)
        {
            Module lowFirMod = LowFirGraph.MainModule;

            string[] pathToModule = GetPathToCurrentActualModule();

            //Skip first module name as it's the name of the root node
            //that we start with
            foreach (var pathModName in pathToModule.Skip(1))
            {
                FIRRTLNode[] lowModNodes = lowFirMod.GetAllNodes();
                FIRRTLNode childLowModNode = lowModNodes.FirstOrDefault(x => x is Module mod && mod.Name == pathModName);
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

        private long UniqueNumber = 0;

        internal string GetUniqueName()
        {
            if (RootHelper != null)
            {
                return RootHelper.GetUniqueName();
            }
            else
            {
                return $"~{UniqueNumber++}";
            }
        }
    }
}
