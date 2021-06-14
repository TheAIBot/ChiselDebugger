using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TestGenerator
{
    public record TestInfo(string firPath, string loFirPath, string vcdPath, bool isVerilatorVCD);
    public record TestCategory(string testPath, string categoryName);

    class Program
    {
        static void Main(string[] args)
        {
            TestCategory[] categories = new TestCategory[]
            {
                new TestCategory("TestFolders", "OthersFIRRTL"),
                new TestCategory("../ChiselTestGen/test_run_dir", "SimpleFIRRTL"),
                new TestCategory("../ChiselTestGen/test_comp_dir", "SingleOp"),
                new TestCategory("../ChiselTestGen/test_multi_comp_dir", "MultiOp"),
                new TestCategory("../ChiselTestGen/test_when_dir", "When"),
                new TestCategory("../ChiselTestGen/test_con_order_dir", "ConnectOrder")
            };

            const string testFilesDir = "TestFiles";
            foreach (var category in categories)
            {
                MakeTestCategory(category, testFilesDir);
            }
        }

        private static void MakeTestCategory(TestCategory category, string baseTestFilesDir)
        {
            //Create folderto put test files into
            string testFilesDir = Path.Combine(baseTestFilesDir, category.categoryName);
            Directory.CreateDirectory(testFilesDir);

            //Create folder to put tests into
            string testFilesPath = Path.Combine(Directory.GetCurrentDirectory(), "TestTexts");
            Directory.CreateDirectory(testFilesPath);

            List<TestInfo> foundTests = FindTests(category.testPath);
            List<TestInfo> movedTests = CopyTestsIntoFolder(foundTests, testFilesDir);
            MakeTestCode(movedTests, category, testFilesDir, testFilesPath);
        }

        private static List<TestInfo> FindTests(string testDir)
        {
            List<TestInfo> foundTests = new List<TestInfo>();

            Queue<string> dirsToLookInto = new Queue<string>();
            dirsToLookInto.Enqueue(testDir);

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

        private static void MakeTestCode(List<TestInfo> tests, TestCategory category, string testDir, string testFilesPath)
        {
            StringBuilder loadGraphTests = new StringBuilder();
            StringBuilder inferTypeTests = new StringBuilder();
            StringBuilder computeGraphTests = new StringBuilder();

            string loadClassName = category.categoryName + "LoadTests";
            string inferClassName = category.categoryName + "InferTests";
            string computeClassName = category.categoryName + "CompTests";

            AddHeader(loadGraphTests, loadClassName, testDir);
            AddHeader(inferTypeTests, inferClassName, testDir);
            AddHeader(computeGraphTests, computeClassName, testDir);

            foreach (var test in tests)
            {
                string moduleName = Path.GetFileNameWithoutExtension(test.firPath);
                string isVerilatorVCD = test.isVerilatorVCD.ToString().ToLower();

                loadGraphTests.AppendLine($"\t\t[TestMethod] public void {moduleName}_fir() => TestTools.VerifyMakeGraph(\"{moduleName}\", \"fir\", TestDir);");
                loadGraphTests.AppendLine($"\t\t[TestMethod] public void {moduleName}_lo_fir() => TestTools.VerifyMakeGraph(\"{moduleName}\", \"lo.fir\", TestDir);");
                loadGraphTests.AppendLine();


                inferTypeTests.AppendLine($"\t\t[TestMethod] public void {moduleName}_fir() => TestTools.VerifyInferTypes(\"{moduleName}\", \"fir\", {isVerilatorVCD}, TestDir);");
                inferTypeTests.AppendLine($"\t\t[TestMethod] public void {moduleName}_lo_fir() => TestTools.VerifyInferTypes(\"{moduleName}\", \"lo.fir\", {isVerilatorVCD}, TestDir);");
                inferTypeTests.AppendLine();

                computeGraphTests.AppendLine($"\t\t[TestMethod] public void {moduleName}_fir() => TestTools.VerifyComputeGraph(\"{moduleName}\", \"fir\", {isVerilatorVCD}, TestDir);");
                computeGraphTests.AppendLine($"\t\t[TestMethod] public void {moduleName}_lo_fir() => TestTools.VerifyComputeGraph(\"{moduleName}\", \"lo.fir\", {isVerilatorVCD}, TestDir);");
                computeGraphTests.AppendLine();
            }

            loadGraphTests.AppendLine("\t}");
            loadGraphTests.AppendLine("}");

            inferTypeTests.AppendLine("\t}");
            inferTypeTests.AppendLine("}");

            computeGraphTests.AppendLine("\t}");
            computeGraphTests.AppendLine("}");

            File.WriteAllText(Path.Combine(testFilesPath, loadClassName + ".cs"), loadGraphTests.ToString());
            File.WriteAllText(Path.Combine(testFilesPath, inferClassName + ".cs"), inferTypeTests.ToString());
            File.WriteAllText(Path.Combine(testFilesPath, computeClassName + ".cs"), computeGraphTests.ToString());
        }

        private static void AddHeader(StringBuilder sBuilder, string className, string testDir)
        {
            sBuilder.AppendLine("using Microsoft.VisualStudio.TestTools.UnitTesting;");
            sBuilder.AppendLine();
            sBuilder.AppendLine("namespace ChiselDebugTests");
            sBuilder.AppendLine("{");
            sBuilder.AppendLine("\t[TestClass]");
            sBuilder.AppendLine($"\tpublic class {className}");
            sBuilder.AppendLine("\t{");
            sBuilder.AppendLine($"\t\tconst string TestDir = @\"{testDir}\";");
            sBuilder.AppendLine();
        }
    }
}
