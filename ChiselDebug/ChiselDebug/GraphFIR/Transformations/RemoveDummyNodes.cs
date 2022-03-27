using ChiselDebug.GraphFIR.Components;
using System;
using System.Diagnostics;
using System.Linq;

namespace ChiselDebug.GraphFIR.Transformations
{
    internal static class RemoveDummyNodes
    {
        internal static void Transform(Module mod)
        {
            foreach (var node in mod.GetAllNestedNodesOfType<Module>())
            {
                TransformModule(node);
            }
        }

        private static void TransformModule(Module mod)
        {
            if (!mod.IsConditional)
            {
                return;
            }

            Debug.Assert(mod.EnableCon != null);
            Debug.Assert(mod.EnableCon.Node is DummyPassthrough);
            Debug.Assert(mod.EnableCon.Node.GetSinks().Length == 1);
            DummyPassthrough dummyNode = (DummyPassthrough)mod.EnableCon.Node;
            DummySink dummySinkNode = mod.GetAllNodes().OfType<DummySink>().Single();
            Debug.Assert(dummyNode.InIO.GetConnections().Length == 1);

            var dummyNodesSourceSinks = mod.EnableCon.GetConnectedInputs().ToArray();
            var dummyNodeSinksSourceConnection = (mod.EnableCon.Node as DummyPassthrough).InIO.GetConnections().Single();


            foreach (var conditionalSinks in dummyNodesSourceSinks)
            {
                if (conditionalSinks.Node is DummySink)
                {
                    continue;
                }

                if (conditionalSinks.TryGetConnection(dummyNode.Result, null, out var connectionNoCond))
                {
                    conditionalSinks.ReplaceConnection(connectionNoCond.Value, dummyNodeSinksSourceConnection.From);
                }
                else if (conditionalSinks.TryGetConnection(dummyNode.Result, dummyNode.Result, out var connectionWithCond))
                {
                    conditionalSinks.ReplaceConnection(connectionWithCond.Value, dummyNodeSinksSourceConnection.From);
                }
                else
                {
                    throw new Exception("No connection found to the source.");
                }
            }

            //dummyNode.Disconnect();
            //dummySinkNode.Disconnect();
            //mod.RemoveNode(dummyNode);
            //mod.RemoveNode(dummySinkNode);
        }
    }
}
