using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.CombGraph.CombGraphOptimizations
{
    public static class RemoveBorderPassCopies
    {
        public static void Optimize(CombComputeOrder<Computable> compOrder)
        {
            ReadOnlySpan<Computable> oldOrder = compOrder.GetComputeOrder();
            List<Computable> newOrder = new List<Computable>();

            for (int i = 0; i < oldOrder.Length; i++)
            {
                Computable comp = oldOrder[i];

                if (comp.GetNode() != null)
                {
                    newOrder.Add(comp);
                    continue;
                }

                Output con = comp.GetConnection();
                Input paired = con.GetPaired();
                Connection[] pairedCons = paired.GetConnections();
                if (pairedCons.Length != 1)
                {
                    newOrder.Add(comp);
                    continue;
                }

                Connection pairedCon = pairedCons[0];
                if (pairedCon.Condition != null)
                {
                    newOrder.Add(comp);
                    continue;
                }

                Output pairedSource = pairedCon.From;
                if (pairedSource.Type.Width != con.Type.Width)
                {
                    newOrder.Add(comp);
                    continue;
                }

                paired.Value = pairedSource.Value;
                con.Value = pairedSource.Value;
            }


            compOrder.SetComputeOrder(newOrder.ToArray());
        }
    }
}
