using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.Transformations
{
    internal static class FlattenConditionalStructure
    {
        internal static void Transform(Module mod)
        {
            foreach (var nestedMod in mod.GetAllNodes().OfType<Module>())
            {
                Transform(nestedMod);
            }

            foreach (var conditional in mod.GetAllNodes().OfType<Conditional>())
            {
                while (true)
                {
                    Module candidate = conditional.CondMods.Last();
                    if (!CanExtractInternalConditional(candidate))
                    {
                        break;
                    }

                    /*
                     * ###
                     * TODO
                     * Still need to fix connections from DummyPassthrough source to anything that depends on it
                     * ###
                     */


                    // Remove dummy nodes and any conditional chaining
                    var allCandidateNodes = candidate.GetAllNodes();
                    //DummyPassthrough dummyPassthrough = allCandidateNodes.OfType<DummyPassthrough>().Single();
                    //DummySink dummySink = allCandidateNodes.OfType<DummySink>().Single();
                    //candidate.RemoveNode(dummyPassthrough);
                    //candidate.RemoveNode(dummySink);
                    //var dummyConnectedSource = dummyPassthrough.InIO.GetConnections().Single().From;
                    //if (dummyConnectedSource.Node is FIRAnd chainAnd && chainAnd.FirDefNode == null)
                    //{
                    //    chainAnd.ResideIn.RemoveNode(chainAnd);
                    //    if (chainAnd.B.GetConnections().Single().From.Node is FIRNot elseChainNot && elseChainNot.FirDefNode == null)
                    //    {
                    //        elseChainNot.ResideIn.RemoveNode(elseChainNot);
                    //        elseChainNot.Disconnect();
                    //    }

                    //    chainAnd.Disconnect();
                    //}
                    //else if (dummyConnectedSource.Node is FIRNot chainNot && chainNot.FirDefNode == null)
                    //{
                    //    chainNot.ResideIn.RemoveNode(chainNot);
                    //    chainNot.Disconnect();
                    //}
                    //dummyPassthrough.Disconnect();
                    //dummySink.Disconnect();


                    // Move all nodes except conditional to external module
                    foreach (var node in candidate.GetAllNodes().Where(x => x is not Conditional))
                    {
                        candidate.RemoveNode(node);
                        mod.AddNode(node);
                    }


                    conditional.RemoveConditionalModule(candidate);
                    Conditional innerConditional = allCandidateNodes.OfType<Conditional>().Single();
                    candidate.RemoveNode(innerConditional);
                    foreach (var innerCondMod in innerConditional.CondMods.ToArray())
                    {
                        innerConditional.RemoveConditionalModule(innerCondMod);
                        conditional.AddConditionalModule(innerCondMod);
                    }
                }
            }
        }

        private static bool CanExtractInternalConditional(Module mod)
        {
            return mod.GetAllNodes().Count(x => x is Conditional) == 1 &&
                   IsAllConnectionsGoingFurtherIn(mod);
        }

        private static bool IsAllConnectionsGoingFurtherIn(Module mod)
        {
            HashSet<Module> currentAndNestedMods = new HashSet<Module>(mod.GetAllNestedNodesOfType<Module>());
            List<FIRRTLNode> modNode = mod.GetAllNodes().Where(x => x is not Conditional).ToList();
            List<Source> allNodeOutputs = modNode.SelectMany(x => x.GetSources()).ToList();
            foreach (var output in allNodeOutputs)
            {
                foreach (var input in output.GetConnectedInputs())
                {
                    if (!currentAndNestedMods.Contains(input.GetModResideIn()))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
