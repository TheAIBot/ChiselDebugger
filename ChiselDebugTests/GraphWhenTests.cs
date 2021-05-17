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
        [TestMethod] public void When1xVectorAccess_fir() => TestTools.VerifyChiselTest("When1xVectorAccess", "fir");
        [TestMethod] public void When1xVectorAccess_lo_fir() => TestTools.VerifyChiselTest("When1xVectorAccess", "lo.fir");
        [TestMethod] public void When1xVectorAccess_treadle_lo_fir() => TestTools.VerifyChiselTest("When1xVectorAccess", "treadle.lo.fir");

        [TestMethod] public void When2xVectorAccess_fir() => TestTools.VerifyChiselTest("When2xVectorAccess", "fir");
        [TestMethod] public void When2xVectorAccess_lo_fir() => TestTools.VerifyChiselTest("When2xVectorAccess", "lo.fir");
        [TestMethod] public void When2xVectorAccess_treadle_lo_fir() => TestTools.VerifyChiselTest("When2xVectorAccess", "treadle.lo.fir");

        [TestMethod] public void When3xVectorAccess_fir() => TestTools.VerifyChiselTest("When3xVectorAccess", "fir");
        [TestMethod] public void When3xVectorAccess_lo_fir() => TestTools.VerifyChiselTest("When3xVectorAccess", "lo.fir");
        [TestMethod] public void When3xVectorAccess_treadle_lo_fir() => TestTools.VerifyChiselTest("When3xVectorAccess", "treadle.lo.fir");

        [TestMethod] public void When1xMemAccess_fir() => TestTools.VerifyChiselTest("When1xMemAccess", "fir");
        [TestMethod] public void When1xMemAccess_lo_fir() => TestTools.VerifyChiselTest("When1xMemAccess", "lo.fir");
        [TestMethod] public void When1xMemAccess_treadle_lo_fir() => TestTools.VerifyChiselTest("When1xMemAccess", "treadle.lo.fir");

        [TestMethod] public void When2xMemAccess_fir() => TestTools.VerifyChiselTest("When2xMemAccess", "fir");
        [TestMethod] public void When2xMemAccess_lo_fir() => TestTools.VerifyChiselTest("When2xMemAccess", "lo.fir");
        [TestMethod] public void When2xMemAccess_treadle_lo_fir() => TestTools.VerifyChiselTest("When2xMemAccess", "treadle.lo.fir");

        [TestMethod] public void When3xMemAccess_fir() => TestTools.VerifyChiselTest("When3xMemAccess", "fir");
        [TestMethod] public void When3xMemAccess_lo_fir() => TestTools.VerifyChiselTest("When3xMemAccess", "lo.fir");
        [TestMethod] public void When3xMemAccess_treadle_lo_fir() => TestTools.VerifyChiselTest("When3xMemAccess", "treadle.lo.fir");

        [TestMethod] public void When1xDuplexInput_fir() => TestTools.VerifyChiselTest("When1xDuplexInput", "fir");
        [TestMethod] public void When1xDuplexInput_lo_fir() => TestTools.VerifyChiselTest("When1xDuplexInput", "lo.fir");
        [TestMethod] public void When1xDuplexInput_treadle_lo_fir() => TestTools.VerifyChiselTest("When1xDuplexInput", "treadle.lo.fir");

        [TestMethod] public void WhenWireCondInput_fir() => TestTools.VerifyChiselTest("WhenWireCondInput", "fir");
        [TestMethod] public void WhenWireCondInput_lo_fir() => TestTools.VerifyChiselTest("WhenWireCondInput", "lo.fir");
        [TestMethod] public void WhenWireCondInput_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenWireCondInput", "treadle.lo.fir");

        [TestMethod] public void WhenConstCondInput_fir() => TestTools.VerifyChiselTest("WhenConstCondInput", "fir");
        [TestMethod] public void WhenConstCondInput_lo_fir() => TestTools.VerifyChiselTest("WhenConstCondInput", "lo.fir");
        [TestMethod] public void WhenConstCondInput_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenConstCondInput", "treadle.lo.fir");
    }
}
