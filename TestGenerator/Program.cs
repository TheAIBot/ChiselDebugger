using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TestGenerator
{
    public record TestInfo(string firPath, string loFirPath, string vcdPath, bool isVerilatorVCD);

    class Program
    {
        static void Main(string[] args)
        {
            const string baseDir = @"...";
            string[] testDirs = new string[] 
            {
                Path.Combine(baseDir, @"neuromorphic-test_run_dir"),
                Path.Combine(baseDir, @"riscv-mini-test_run_dir")
            };

            List<TestInfo> foundTests = FindTests(testDirs);

            string newTestDir = Path.Combine(Directory.GetCurrentDirectory(), "OthersFIRRTL");
            Directory.CreateDirectory(newTestDir);

            List<TestInfo> movedTests = CopyTestsIntoFolder(foundTests, newTestDir);

            const string testFilesDir = "TestTexts";
            string testFilesPath = Path.Combine(Directory.GetCurrentDirectory(), testFilesDir);
            Directory.CreateDirectory(testFilesPath);

            MakeTestCode(movedTests, newTestDir, testFilesPath);
        }

        private static List<TestInfo> FindTests(string[] testDirs)
        {
            List<TestInfo> foundTests = new List<TestInfo>();

            Queue<string> dirsToLookInto = new Queue<string>();
            foreach (var testDir in testDirs)
            {
                dirsToLookInto.Enqueue(testDir);
            }

            while (dirsToLookInto.Count > 0)
            {
                string dir = dirsToLookInto.Dequeue();

                string[] files = Directory.EnumerateFiles(dir).ToArray();
                if (files.Length > 0)
                {
                    string firFile = files.Single(x => !x.EndsWith("treadle.lo.fir") && !x.EndsWith(".lo.fir") && !x.EndsWith(".hi.fir") && x.EndsWith(".fir"));
                    string loFirFile;
                    if (files.Any(x => x.EndsWith("treadle.lo.fir")))
                    {
                        loFirFile = files.Single(x => x.EndsWith("treadle.lo.fir"));
                    }
                    else
                    {
                        loFirFile = files.Single(x => x.EndsWith(".lo.fir") || x.EndsWith(".hi.fir"));
                    }
                    string vcdFile = files.Single(x => x.EndsWith(".vcd"));
                    bool isVerilatorVCD = files.Any(x => x.EndsWith(".v"));

                    foundTests.Add(new TestInfo(firFile, loFirFile, vcdFile, isVerilatorVCD));
                }
                else
                {
                    foreach (var childDir in Directory.EnumerateDirectories(dir))
                    {
                        dirsToLookInto.Enqueue(childDir);
                    }
                }
            }

            return foundTests;
        }

        private static List<TestInfo> CopyTestsIntoFolder(List<TestInfo> foundTests, string copyInto)
        {
            List<TestInfo> testsWithNewNames = new List<TestInfo>();
            HashSet<string> usedNames = new HashSet<string>();

            foreach (var testInfo in foundTests)
            {
                string firName = Path.GetFileNameWithoutExtension(testInfo.firPath);

                string testName = firName;
                int counter = 0;
                while (usedNames.Contains(testName))
                {
                    testName = firName + "_" + counter++;
                }
                usedNames.Add(testName);

                string newFirPath = Path.Combine(copyInto, testName + ".fir");
                string newLoFirPath = Path.Combine(copyInto, testName + ".lo.fir");
                string newVcdPath = Path.Combine(copyInto, testName + ".vcd");

                if (!File.Exists(newFirPath))
                {
                    File.Copy(testInfo.firPath, newFirPath);
                    File.Copy(testInfo.loFirPath, newLoFirPath);
                    File.Copy(testInfo.vcdPath, newVcdPath);   
                }

                testsWithNewNames.Add(new TestInfo(newFirPath, newLoFirPath, newVcdPath, testInfo.isVerilatorVCD));
            }

            return testsWithNewNames;
        }

        private static void MakeTestCode(List<TestInfo> tests, string testDir, string testFilesPath)
        {
            StringBuilder loadGraphTests = new StringBuilder();
            StringBuilder inferTypeTests = new StringBuilder();
            StringBuilder computeGraphTests = new StringBuilder();

            loadGraphTests.AppendLine($"const string TestDir = @\"{testDir}\";");
            loadGraphTests.AppendLine();

            inferTypeTests.AppendLine($"const string TestDir = @\"{testDir}\";");
            inferTypeTests.AppendLine();

            computeGraphTests.AppendLine($"const string TestDir = @\"{testDir}\";");
            computeGraphTests.AppendLine();

            foreach (var test in tests)
            {
                string moduleName = Path.GetFileNameWithoutExtension(test.firPath);
                string isVerilatorVCD = test.isVerilatorVCD.ToString().ToLower();

                loadGraphTests.AppendLine($"[TestMethod] public void {moduleName}_fir() => TestTools.VerifyMakeGraph(\"{moduleName}\", \"fir\", TestDir);");
                loadGraphTests.AppendLine($"[TestMethod] public void {moduleName}_lo_fir() => TestTools.VerifyMakeGraph(\"{moduleName}\", \"lo.fir\", TestDir);");
                loadGraphTests.AppendLine();


                inferTypeTests.AppendLine($"[TestMethod] public void {moduleName}_fir() => TestTools.VerifyInferTypes(\"{moduleName}\", \"fir\", {isVerilatorVCD}, TestDir);");
                inferTypeTests.AppendLine($"[TestMethod] public void {moduleName}_lo_fir() => TestTools.VerifyInferTypes(\"{moduleName}\", \"lo.fir\", {isVerilatorVCD}, TestDir);");
                inferTypeTests.AppendLine();

                computeGraphTests.AppendLine($"[TestMethod] public void {moduleName}_fir() => TestTools.VerifyComputeGraph(\"{moduleName}\", \"fir\", {isVerilatorVCD}, TestDir);");
                computeGraphTests.AppendLine($"[TestMethod] public void {moduleName}_lo_fir() => TestTools.VerifyComputeGraph(\"{moduleName}\", \"lo.fir\", {isVerilatorVCD}, TestDir);");
                computeGraphTests.AppendLine();
            }

            File.WriteAllText(Path.Combine(testFilesPath, "TestLoadGraph.txt"), loadGraphTests.ToString());
            File.WriteAllText(Path.Combine(testFilesPath, "TestTypeInferGraph.txt"), inferTypeTests.ToString());
            File.WriteAllText(Path.Combine(testFilesPath, "TestComputeGraph.txt"), computeGraphTests.ToString());
        }
    }
}
