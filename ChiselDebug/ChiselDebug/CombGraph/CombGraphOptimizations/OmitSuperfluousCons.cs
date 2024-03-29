﻿using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;

namespace ChiselDebug.CombGraph.CombGraphOptimizations
{
    internal static class OmitSuperfluousCons
    {
        public static void Optimize(CombComputeOrder<Computable> compOrder)
        {
            ReadOnlySpan<Computable> oldOrder = compOrder.GetComputeOrder();
            List<Computable> newOrder = new List<Computable>();

            for (int i = 0; i < oldOrder.Length; i++)
            {
                ref readonly var comp = ref oldOrder[i];

                Source? con = comp.GetConnection();
                if (con != null)
                {
                    if (!comp.IsBorderIO)
                    {
                        continue;
                    }

                    Sink paired = con.GetPairedThrowIfNull();

                    if (!paired.IsConnectedToAnything())
                    {
                        continue;
                    }
                }

                newOrder.Add(comp);
            }

            compOrder.SetComputeOrder(newOrder.ToArray());
        }
    }
}
