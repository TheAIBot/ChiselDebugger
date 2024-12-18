using ChiselDebug.GraphFIR.Circuit;
using ChiselDebug.GraphFIR.Circuit.Converter;
using ChiselDebug.Timeline;
using FIRRTL;
using System;
using System.Diagnostics;
using System.IO;
using VCDReader;

namespace Benchmarker
{
    class Program
    {
        static void Main(string[] args)
        {
            const string TestDir = @"C:\Users\Andreas\Documents\GitHub\ChiselDebugger\TestGenerator\TestFolders\riscv-mini-test_run_dir\TileTester\202105061708406469251142612812063";
            Stopwatch timer = new Stopwatch();

            if (false)
            {
                timer.Start();
                VerifyComputeGraph("TileTester_4", "lo.fir", true, TestDir);
                timer.Stop();
            }
            else
            {
                //using VCD vcd = VCDReader.Parse.FromFile(Path.Combine(TestDir, "TileTester_1.vcd"));
                using VCD vcd = VCDReader.Parse.FromFile(Path.Combine(TestDir, "dump.vcd"));
                //using VCD vcd = VCDReader.Parse.FromFile(Path.Combine(TestDir, "TileTester_4.vcd"));
                //using VCD vcd = VCDReader.Parse.FromFile(Path.Combine(TestDir, "TileTester_5.vcd"));

                timer.Start();
                new VCDTimeline(vcd);
                timer.Stop();
            }

            Console.WriteLine((timer.ElapsedMilliseconds / 1000.0f).ToString("N2"));
        }

        internal static void VerifyComputeGraph(string moduleName, string extension, bool isVerilogVCD, string modulePath)
        {
            CircuitGraph graph = VerifyMakeGraph(moduleName, extension, modulePath);
            VCDTimeline timeline = MakeTimeline(moduleName, modulePath);

            VerifyCircuitState(graph, timeline, isVerilogVCD);
        }

        private static VCDTimeline MakeTimeline(string moduleName, string modulePath)
        {
            using VCD vcd = LoadVCD(moduleName, modulePath);
            return new VCDTimeline(vcd);
        }

        private static VCD LoadVCD(string moduleName, string modulePath)
        {
            return VCDReader.Parse.FromFile($"{modulePath}/{moduleName}.vcd");
        }

        internal static CircuitGraph VerifyMakeGraph(string moduleName, string extension, string modulePath)
        {
            CircuitGraph lowFirGraph = null;
            if (extension == "fir")
            {
                string loFirPath = Path.Combine(modulePath, $"{moduleName}.lo.fir");
                lowFirGraph = VerifyCanCreateGraphFromFile(loFirPath);
            }

            string firPath = Path.Combine(modulePath, $"{moduleName}.{extension}");
            return VerifyCanCreateGraphFromFile(firPath, lowFirGraph);
        }

        private static CircuitGraph VerifyCanCreateGraphFromFile(string firrtlPath, CircuitGraph lowFirGraph = null)
        {
            using FileStream fileStream = new FileStream(firrtlPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            Circuit circuit = FIRRTL.Parse.FromStream(fileStream);
            CircuitGraph graph = CircuitToGraph.GetAsGraph(circuit, lowFirGraph);

            return graph;
        }

        internal static void VerifyCircuitState(CircuitGraph graph, VCDTimeline timeline, bool isVerilogVCD)
        {
            graph.SetState(timeline.GetStateAtTime(timeline.TimeInterval.StartInclusive));

            Stopwatch timer = new Stopwatch();
            timer.Start();
            int i = 0;
            foreach (var state in timeline.GetAllDistinctStates())
            {
                if (state.Time == timeline.TimeInterval.InclusiveEnd())
                {
                    break;
                }
                graph.SetState(state);
                graph.ComputeRemainingGraphFast();


                const int reportEveryXTimeStates = 20_000;
                if (i++ % reportEveryXTimeStates == 0)
                {
                    timer.Stop();
                    float msPerState = (float)timer.ElapsedMilliseconds / reportEveryXTimeStates;
                    float statesPerMs = 1.0f / msPerState;
                    float statesPerSecs = statesPerMs * 1000.0f;
                    float totalRemainingMs = msPerState * (timeline.StateCount - i);

                    const int msPerSec = 1_000;
                    float remainingSeconds = totalRemainingMs / msPerSec;
                    Console.WriteLine($"{i}/{timeline.StateCount} Remaining: {remainingSeconds:N0} States/sec: {statesPerSecs:N0}");
                    timer.Restart();
                }
            }
            timer.Stop();
        }
    }
}