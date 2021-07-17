using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.CombGraph.CombGraphOptimizations
{
    internal static class ConstFolding
    {
        public static void Optimize(CombComputeOrder compOrder)
        {
            HashSet<Output> constOutput = new HashSet<Output>();
            ReadOnlySpan<Computable> oldOrder = compOrder.GetComputeOrder();
            List<Computable> newOrder = new List<Computable>();

            for (int i = 0; i < oldOrder.Length; i++)
            {
                ref readonly var comp = ref oldOrder[i];
                var firNode = comp.GetNode();

                if (firNode != null)
                {
                    if (firNode is ConstValue constNode)
                    {
                        constOutput.Add(constNode.Result);
                    }
                    else
                    {
                        bool allConstInputs = true;
                        foreach (var nodeInput in firNode.GetInputs())
                        {
                            foreach (var inputCon in nodeInput.GetConnections())
                            {
                                if (!constOutput.Contains(inputCon.From))
                                {
                                    allConstInputs = false;
                                    break;
                                }
                                if (inputCon.Condition != null && !constOutput.Contains(inputCon.Condition))
                                {
                                    allConstInputs = false;
                                    break;
                                }
                            }

                            if (!allConstInputs)
                            {
                                break;
                            }
                        }

                        if (allConstInputs)
                        {
                            foreach (var nodeOutput in firNode.GetOutputs())
                            {
                                constOutput.Add(nodeOutput);
                            }
                        }
                        else
                        {
                            newOrder.Add(comp);
                        }
                    }
                }
                else
                {
                    var connection = comp.GetConnection();
                    if (!constOutput.Contains(connection))
                    {
                        newOrder.Add(comp);
                    }
                }
            }

            compOrder.SetComputeOrder(newOrder.ToArray());
        }
    }
}
