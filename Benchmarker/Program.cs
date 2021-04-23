using ChiselDebug;
using FIRRTL;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace Benchmarker
{
    class Program
    {
        static void Main(string[] args)
        {
            Circuit circuddit = FIRRTL.Parse.FromFile(Path.Combine(dir, "TileTester_4.lo.fir"));
            //ConcurrentBag<CircuitGraph> graphs = new ConcurrentBag<CircuitGraph>();
            //Parallel.For(0, 8, x =>
            //    {
            //        graphs.Add(CircuitToGraph.GetAsGraph(circuddit));
            //    });
            //foreach (var graph in graphs)
            //{
            //    Console.WriteLine(graph.Name);
            //}

            CircuitGraph lol;
            for (int i = 0; i < 10; i++)
            {
                lol = CircuitToGraph.GetAsGraph(circuddit);
            }


        }
    }
}
