using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using VCDReader;

namespace VCDReaderTests
{
    [TestClass]
    public class ParseFromFileTests
    {
        private const string VCDFolder = "VCDTestFiles";

        [TestMethod]
        public void Parsevcd1()
        {
            Parse.FromFile(Path.Combine(VCDFolder, "vcd1.vcd"));
        }
    }
}
