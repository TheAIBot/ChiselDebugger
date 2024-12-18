using ChiselDebuggerRazor.Code;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
#nullable enable

namespace ChiselDebuggerWebAsmUI.Code
{
    internal sealed class ExampleCircuits : IExampleCircuits
    {
        private readonly HttpClient HttpClient;
        private const string ExamplesFolder = "examples";

        public ExampleCircuits(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public string[] GetExamples()
        {
            return ["MovingAveragePow2.fir"];
        }

        public async Task<CircuitFiles> GetExampleAsync(string exampleName)
        {
            string highFirrtlName = exampleName;
            string? lowFirrtlName = highFirrtlName.EndsWith(".lo.fir") ? null : highFirrtlName.Replace(".fir", ".lo.fir");
            string vcdName = $"{Path.GetFileNameWithoutExtension(exampleName)}.vcd";

            byte[]? highFirrtlFileContent = await TryGetExampleFileContentAsync(highFirrtlName);
            byte[]? lowFirrtlFileContent = await TryGetExampleFileContentAsync(lowFirrtlName);
            byte[]? vcdFileContent = await TryGetExampleFileContentAsync(vcdName);

            if (highFirrtlFileContent == null)
            {
                throw new InvalidOperationException($"Example file does not exist. Example name: {exampleName}");
            }

            var highFirrtlData = new MemoryStream(highFirrtlFileContent);
            var lowFirrtlData = lowFirrtlFileContent == null ? null : new MemoryStream(lowFirrtlFileContent);
            var vcdData = vcdFileContent == null ? null : new MemoryStream(vcdFileContent);

            return new CircuitFiles(highFirrtlData, lowFirrtlData, vcdData);
        }

        private async Task<byte[]?> TryGetExampleFileContentAsync(string? fileName)
        {
            if (fileName == null)
            {
                return null;
            }

            using HttpResponseMessage response = await HttpClient.GetAsync($"{ExamplesFolder}/{fileName}");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
