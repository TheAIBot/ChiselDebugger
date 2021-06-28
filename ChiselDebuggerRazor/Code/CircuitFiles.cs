using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code
{
    public class CircuitFiles
    {
        private bool Ready = false;

        private string HiFirrtlPath;
        private string LoFirrtlPath;
        private string VCDPath;
        public bool IsVerilogVCD;

        public Stream GetHiFirrtlStream()
        { 
            if (HiFirrtlPath != null && File.Exists(HiFirrtlPath))
            {
                return File.OpenRead(HiFirrtlPath);
            }

            return null;
        }
        public Stream GetLoFirrtlStream()
        {
            if (LoFirrtlPath != null && File.Exists(LoFirrtlPath))
            {
                return File.OpenRead(LoFirrtlPath);
            }

            return null;
        }
        public Stream GetVCDStream()
        {
            if (VCDPath != null && File.Exists(VCDPath))
            {
                return File.OpenRead(VCDPath);
            }

            return null;
        }

        public bool IsReady => Ready;

        public void UpdateFromPath(string hiFirrtlPath, string loFirrtlPath, string vcdPath, bool isVerilogVCD)
        {
            Clear();

            HiFirrtlPath = hiFirrtlPath;
            LoFirrtlPath = loFirrtlPath;
            VCDPath = vcdPath;
            IsVerilogVCD = isVerilogVCD;

            Ready = true;
        }

        private async Task<string> TransferFileToTempFile(IBrowserFile file)
        {
            if (file == null)
            {
                return null;
            }

            string tmpFileName = Path.GetTempFileName();
            using (FileStream tmpFile = File.OpenWrite(tmpFileName))
            {
                await file.OpenReadStream(long.MaxValue).CopyToAsync(tmpFile);
            }

            return tmpFileName;
        }

        public async Task<bool> UpdateFromFiles(IReadOnlyList<IBrowserFile> files, bool isVerilogVCD)
        {
            Clear();

            var firFiles = files.Where(x => x.Name.EndsWith(".fir")).ToList();
            if (firFiles.Count == 0)
            {
                return false;
            }

            IBrowserFile hiFirrtlFile = null;
            IBrowserFile loFirrtlFile = null;
            IBrowserFile vcdFile = null;
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

            var hiFirrtlPath = TransferFileToTempFile(hiFirrtlFile);
            var loFirrtlPath = TransferFileToTempFile(loFirrtlFile);
            var vcdPath = TransferFileToTempFile(vcdFile);
            await Task.WhenAll(hiFirrtlPath, loFirrtlPath, vcdPath);

            UpdateFromPath(hiFirrtlPath.Result, loFirrtlPath.Result, vcdPath.Result, isVerilogVCD);
            return true;
        }

        private void Clear()
        {
            Ready = false;

            HiFirrtlPath = null;
            LoFirrtlPath = null;
            VCDPath = null;
        }
    }
}
