﻿using Microsoft.AspNetCore.Components.Forms;
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

        private Stream HiFirrtlStream;
        private Stream LoFirrtlStream;
        private Stream VCDStream;
        public bool IsVerilogVCD;
        public bool IsReady => Ready;

        public delegate void CircuitData();
        public event CircuitData OnViewCircuit;

        public Stream GetHiFirrtlStream()
        { 
            return HiFirrtlStream;
        }
        public Stream GetLoFirrtlStream()
        {
            return LoFirrtlStream;
        }
        public Stream GetVCDStream()
        {
            return VCDStream;
        }

        public void UpdateFromPath(string hiFirrtlPath, string loFirrtlPath, string vcdPath, bool isVerilogVCD)
        {
            Clear();

            if (File.Exists(hiFirrtlPath))
            {
                HiFirrtlStream = File.OpenRead(hiFirrtlPath);
            }
            if (File.Exists(loFirrtlPath))
            {
                LoFirrtlStream = File.OpenRead(loFirrtlPath);
            }
            if (File.Exists(vcdPath))
            {
                VCDStream = File.OpenRead(vcdPath);
            }

            IsVerilogVCD = isVerilogVCD;
            Ready = true;
            OnViewCircuit?.Invoke();
        }

        private async Task<Stream> CopyBrowserFileToMemory(IBrowserFile file)
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

            var hiFirrtlPath = CopyBrowserFileToMemory(hiFirrtlFile);
            var loFirrtlPath = CopyBrowserFileToMemory(loFirrtlFile);
            var vcdPath = CopyBrowserFileToMemory(vcdFile);
            await Task.WhenAll(hiFirrtlPath, loFirrtlPath, vcdPath);

            HiFirrtlStream = hiFirrtlPath.Result;
            LoFirrtlStream = loFirrtlPath.Result;
            VCDStream = vcdPath.Result;

            IsVerilogVCD = isVerilogVCD;
            Ready = true;
            OnViewCircuit?.Invoke();
            return true;
        }

        private void Clear()
        {
            Ready = false;

            HiFirrtlStream?.Dispose();
            LoFirrtlStream?.Dispose();
            VCDStream?.Dispose();

            HiFirrtlStream = null;
            LoFirrtlStream = null;
            VCDStream = null;
        }
    }
}