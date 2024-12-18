using ChiselDebuggerRazor.Code;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Code
{
    internal sealed class ExampleCircuits : IExampleCircuits
    {
        private readonly Lazy<string[]> Examples = new Lazy<string[]>(GetExamplesFromFiles, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        private const string ExamplesFolder = "Examples";

        public string[] GetExamples()
        {
            return Examples.Value;
        }

        public Task<CircuitFiles> GetExampleAsync(string exampleName)
        {
            string loFirPath = Path.Combine(ExamplesFolder, exampleName.Replace(".fir", ".lo.fir"));
            string hiFirPath = Path.Combine(ExamplesFolder, exampleName);

            string filenameNoExtension = exampleName.Split('.').First();
            string vcdPath = Path.Combine(ExamplesFolder, $"{filenameNoExtension}.vcd");

            return Task.FromResult(CircuitFiles.FromPath(hiFirPath, loFirPath, vcdPath));
        }

        private static string[] GetExamplesFromFiles()
        {
            return Directory.GetFiles(ExamplesFolder)
                            .Select(Path.GetFileName)
                            .Where(x => x.EndsWith(".fir"))
                            .ToArray();
        }
    }
}
