using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ChiselDebug.CombGraph.CombGraphOptimizations
{
    public static class MergeStates
    {
        public static void Optimize(CombComputeOrder<Computable> compOrder)
        {
            ReadOnlySpan<Computable> oldOrder = compOrder.GetComputeOrder();
            List<Computable> newOrder = new List<Computable>();

            for (int i = 0; i < oldOrder.Length; i++)
            {
                Computable comp = oldOrder[i];

                if (comp.TryGetNode(out FIRRTLNode? node))
                {
                    MergeNodeSinks(node);
                    newOrder.Add(comp);
                }
                else if (!TryMergeBorderPass(comp.GetConnection()!))
                {
                    newOrder.Add(comp);
                }
            }

            compOrder.SetComputeOrder(newOrder.ToArray());
        }

        private static void MergeNodeSinks(FIRRTLNode node)
        {
            foreach (var input in node.GetSinks())
            {
                if (CanMergeSourceAndSink(input, out Source? source))
                {
                    input.Value = source.Value;
                }
            }
        }

        private static bool TryMergeBorderPass(Source passSource)
        {
            Sink paired = passSource.GetPairedThrowIfNull();
            if (CanMergeSourceAndSink(paired, out Source? source))
            {
                paired.Value = source.Value;
                passSource.Value = source.Value;
                return true;
            }

            return false;
        }

        private static bool CanMergeSourceAndSink(Sink input, [NotNullWhen(true)] out Source? source)
        {
            Connection[] cons = input.GetConnections();
            if (cons.Length != 1)
            {
                source = null;
                return false;
            }

            Connection con = cons[0];
            if (con.Condition != null)
            {
                source = null;
                return false;
            }

            Source conSource = con.From;
            if (conSource.Type.Width != input.Type.Width)
            {
                source = null;
                return false;
            }

            source = conSource;
            return true;
        }
    }
}
