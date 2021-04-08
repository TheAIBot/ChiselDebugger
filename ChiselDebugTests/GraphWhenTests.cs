using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebugTests
{
    [TestClass]
    public class GraphWhenTests
    {
        [TestMethod] public void When1xVectorAccess_fir() => TestTools.VerifyChiselTest("When1xVectorAccess", "fir", false);
        [TestMethod] public void When1xVectorAccess_lo_fir() => TestTools.VerifyChiselTest("When1xVectorAccess", "lo.fir", false);
        [TestMethod] public void When1xVectorAccess_treadle_lo_fir() => TestTools.VerifyChiselTest("When1xVectorAccess", "treadle.lo.fir", false);
        [TestMethod] public void When1xVectorAccess_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("When1xVectorAccess", "treadle.lo.fir", true);

        [TestMethod] public void When2xVectorAccess_fir() => TestTools.VerifyChiselTest("When2xVectorAccess", "fir", false);
        [TestMethod] public void When2xVectorAccess_lo_fir() => TestTools.VerifyChiselTest("When2xVectorAccess", "lo.fir", false);
        [TestMethod] public void When2xVectorAccess_treadle_lo_fir() => TestTools.VerifyChiselTest("When2xVectorAccess", "treadle.lo.fir", false);
        [TestMethod] public void When2xVectorAccess_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("When2xVectorAccess", "treadle.lo.fir", true);

        [TestMethod] public void When3xVectorAccess_fir() => TestTools.VerifyChiselTest("When3xVectorAccess", "fir", false);
        [TestMethod] public void When3xVectorAccess_lo_fir() => TestTools.VerifyChiselTest("When3xVectorAccess", "lo.fir", false);
        [TestMethod] public void When3xVectorAccess_treadle_lo_fir() => TestTools.VerifyChiselTest("When3xVectorAccess", "treadle.lo.fir", false);
        [TestMethod] public void When3xVectorAccess_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("When3xVectorAccess", "treadle.lo.fir", true);

        [TestMethod] public void When1xMemAccess_fir() => TestTools.VerifyChiselTest("When1xMemAccess", "fir", false);
        [TestMethod] public void When1xMemAccess_lo_fir() => TestTools.VerifyChiselTest("When1xMemAccess", "lo.fir", false);
        [TestMethod] public void When1xMemAccess_treadle_lo_fir() => TestTools.VerifyChiselTest("When1xMemAccess", "treadle.lo.fir", false);
        [TestMethod] public void When1xMemAccess_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("When1xMemAccess", "treadle.lo.fir", true);

        [TestMethod] public void When2xMemAccess_fir() => TestTools.VerifyChiselTest("When2xMemAccess", "fir", false);
        [TestMethod] public void When2xMemAccess_lo_fir() => TestTools.VerifyChiselTest("When2xMemAccess", "lo.fir", false);
        [TestMethod] public void When2xMemAccess_treadle_lo_fir() => TestTools.VerifyChiselTest("When2xMemAccess", "treadle.lo.fir", false);
        [TestMethod] public void When2xMemAccess_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("When2xMemAccess", "treadle.lo.fir", true);

        [TestMethod] public void When3xMemAccess_fir() => TestTools.VerifyChiselTest("When3xMemAccess", "fir", false);
        [TestMethod] public void When3xMemAccess_lo_fir() => TestTools.VerifyChiselTest("When3xMemAccess", "lo.fir", false);
        [TestMethod] public void When3xMemAccess_treadle_lo_fir() => TestTools.VerifyChiselTest("When3xMemAccess", "treadle.lo.fir", false);
        [TestMethod] public void When3xMemAccess_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("When3xMemAccess", "treadle.lo.fir", true);
    }
}
