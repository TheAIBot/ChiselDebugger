using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR.Circuit.Converter
{
    internal interface IVisitHelper
    {
        Module Mod { get; }
        bool IsConditionalModule { get; }
        Source ScopeEnabledCond { get; }
        Dictionary<string, FIRRTL.IDefModule> ModuleRoots { get; }

        void AddNodeToModule(FIRRTLNode node);
        void EnterEnabledScope(Source enableCond);
        void ExitEnabledScope();
        IVisitHelper ForNewCondModule(string moduleName, FIRRTL.IDefModule? moduleDef);
        IVisitHelper ForNewModule(string moduleName, string instanceName, FIRRTL.IDefModule moduleDef);
        FIRRTL.IFirrtlNode GetDefNodeFromLowFirrtlGraph(string nodeName);
        bool HasLowFirGraph();
        string GetUniqueName();
    }
}
