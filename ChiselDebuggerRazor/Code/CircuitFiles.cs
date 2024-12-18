using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
#nullable enable

namespace ChiselDebuggerRazor.Code
{
    public sealed class CircuitFiles : IDisposable
    {
#pragma warning disable IDISP008 // Don't assign member with injected and created disposables
        private Stream HiFirrtlStream;
        private Stream? LoFirrtlStream;
        private Stream? VCDStream;
#pragma warning restore IDISP008 // Don't assign member with injected and created disposables

        public CircuitFiles(Stream hiFirrtlStream, Stream? loFirrtlStream, Stream? vcdStream)
        {
            HiFirrtlStream = hiFirrtlStream;
            LoFirrtlStream = loFirrtlStream;
            VCDStream = vcdStream;
        }

        public Stream GetHiFirrtlStream()
        {
            return HiFirrtlStream;
        }
        public Stream? GetLoFirrtlStream()
        {
            return LoFirrtlStream;
        }
        public Stream? GetVCDStream()
        {
            return VCDStream;
        }

        public static CircuitFiles FromPath(string hiFirrtlPath, string loFirrtlPath, string vcdPath)
        {
            Stream hiFirrtlStream;
            Stream? loFirrtlStream = null;
            Stream? vcdStream = null;

            if (File.Exists(hiFirrtlPath))
            {
                hiFirrtlStream = File.OpenRead(hiFirrtlPath);
            }
            else
            {
                throw new FileNotFoundException("The specified file does not exist.", hiFirrtlPath);
            }

            if (File.Exists(loFirrtlPath))
            {
                loFirrtlStream = File.OpenRead(loFirrtlPath);
            }
            if (File.Exists(vcdPath))
            {
                vcdStream = File.OpenRead(vcdPath);
            }

            return new CircuitFiles(hiFirrtlStream, loFirrtlStream, vcdStream);
        }

        public static async Task<CircuitFiles?> FromFilesAsync(IReadOnlyList<IBrowserFile> files)
        {
            var firFiles = files.Where(x => x.Name.EndsWith(".fir")).ToList();
            if (firFiles.Count == 0)
            {
                return null;
            }

            IBrowserFile? hiFirrtlFile = null;
            IBrowserFile? loFirrtlFile = null;
            IBrowserFile? vcdFile = null;
            if (firFiles.Count(x => !x.Name.Contains(".lo.")) == 1)
            {
                hiFirrtlFile = firFiles.Single(x => !x.Name.Contains(".lo.fir"));
            }
            else
            {
                hiFirrtlFile = firFiles.OrderByDescending(x => x.Name.Split('.').Length).Last();
            }

            if (firFiles.Count > 1)
            {
                loFirrtlFile = firFiles.Where(x => x != hiFirrtlFile).OrderByDescending(x => x.Name.Split('.').Length).First();
            }

            vcdFile = files.FirstOrDefault(x => x.Name.EndsWith(".vcd"));

            var hiFirrtlPath = CopyBrowserFileToMemoryAsync(hiFirrtlFile);
            var loFirrtlPath = CopyBrowserFileToMemoryAsync(loFirrtlFile);
            var vcdPath = CopyBrowserFileToMemoryAsync(vcdFile);
            await Task.WhenAll(hiFirrtlPath, loFirrtlPath, vcdPath);

            Stream? hiFirrtl = await hiFirrtlPath;
            if (hiFirrtl == null)
            {
                throw new FileNotFoundException("No firrtl file specified.");
            }


            return new CircuitFiles(hiFirrtl, await loFirrtlPath, await vcdPath);
        }

        public void Dispose()
        {
            HiFirrtlStream?.Dispose();
            LoFirrtlStream?.Dispose();
            VCDStream?.Dispose();
        }

        private static async Task<Stream?> CopyBrowserFileToMemoryAsync(IBrowserFile? file)
        {
            if (file == null)
            {
                return null;
            }

            MemoryStream memStream = new MemoryStream();
            using Stream fileStream = file.OpenReadStream(100_000_000_000L);
            await fileStream.CopyToAsync(memStream);
            memStream.Position = 0;

            return memStream;
        }
    }
}
