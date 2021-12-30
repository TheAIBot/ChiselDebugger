using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                FIRRTLNode node = comp.GetNode();
                if (node != null)
                {
                    MergeNodeSinks(node);
                    newOrder.Add(comp);
                }
                else if (!TryMergeBorderPass(comp.GetConnection()))
                {
                    newOrder.Add(comp);
                }
            }

            compOrder.SetComputeOrder(newOrder.ToArray());
        }

        private static void MergeNodeSinks(FIRRTLNode node)
        {
            foreach (var input in node.GetInputs())
            {
                if (CanMergeSourceAndSink(input, out Output source))
                {
                    input.Value = source.Value;
                }
            }
        }

        private static bool TryMergeBorderPass(Output passSource)
        {
            Input paired = passSource.GetPaired();
            if (CanMergeSourceAndSink(paired, out Output source))
            {
                paired.Value = source.Value;
                passSource.Value = source.Value;
                return true;
            }

            return false;
        }

        private static bool CanMergeSourceAndSink(Input input, out Output source)
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

            Output conSource = con.From;
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
