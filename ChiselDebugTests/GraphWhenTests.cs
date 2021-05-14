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
        [TestMethod] public void When1xVectorAccess_fir() => TestTools.VerifyChiselTest("When1xVectorAccess", "fir", true);
        [TestMethod] public void When1xVectorAccess_lo_fir() => TestTools.VerifyChiselTest("When1xVectorAccess", "lo.fir", true);
        [TestMethod] public void When1xVectorAccess_treadle_lo_fir() => TestTools.VerifyChiselTest("When1xVectorAccess", "treadle.lo.fir", true);
        [TestMethod] public void When1xVectorAccess_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("When1xVectorAccess", "treadle.lo.fir", true);

        [TestMethod] public void When2xVectorAccess_fir() => TestTools.VerifyChiselTest("When2xVectorAccess", "fir", true);
        [TestMethod] public void When2xVectorAccess_lo_fir() => TestTools.VerifyChiselTest("When2xVectorAccess", "lo.fir", true);
        [TestMethod] public void When2xVectorAccess_treadle_lo_fir() => TestTools.VerifyChiselTest("When2xVectorAccess", "treadle.lo.fir", true);
        [TestMethod] public void When2xVectorAccess_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("When2xVectorAccess", "treadle.lo.fir", true);

        [TestMethod] public void When3xVectorAccess_fir() => TestTools.VerifyChiselTest("When3xVectorAccess", "fir", true);
        [TestMethod] public void When3xVectorAccess_lo_fir() => TestTools.VerifyChiselTest("When3xVectorAccess", "lo.fir", true);
        [TestMethod] public void When3xVectorAccess_treadle_lo_fir() => TestTools.VerifyChiselTest("When3xVectorAccess", "treadle.lo.fir", true);
        [TestMethod] public void When3xVectorAccess_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("When3xVectorAccess", "treadle.lo.fir", true);

        [TestMethod] public void When1xMemAccess_fir() => TestTools.VerifyChiselTest("When1xMemAccess", "fir", true);
        [TestMethod] public void When1xMemAccess_lo_fir() => TestTools.VerifyChiselTest("When1xMemAccess", "lo.fir", true);
        [TestMethod] public void When1xMemAccess_treadle_lo_fir() => TestTools.VerifyChiselTest("When1xMemAccess", "treadle.lo.fir", true);
        [TestMethod] public void When1xMemAccess_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("When1xMemAccess", "treadle.lo.fir", true);

        [TestMethod] public void When2xMemAccess_fir() => TestTools.VerifyChiselTest("When2xMemAccess", "fir", true);
        [TestMethod] public void When2xMemAccess_lo_fir() => TestTools.VerifyChiselTest("When2xMemAccess", "lo.fir", true);
        [TestMethod] public void When2xMemAccess_treadle_lo_fir() => TestTools.VerifyChiselTest("When2xMemAccess", "treadle.lo.fir", true);
        [TestMethod] public void When2xMemAccess_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("When2xMemAccess", "treadle.lo.fir", true);

        [TestMethod] public void When3xMemAccess_fir() => TestTools.VerifyChiselTest("When3xMemAccess", "fir", true);
        [TestMethod] public void When3xMemAccess_lo_fir() => TestTools.VerifyChiselTest("When3xMemAccess", "lo.fir", true);
        [TestMethod] public void When3xMemAccess_treadle_lo_fir() => TestTools.VerifyChiselTest("When3xMemAccess", "treadle.lo.fir", true);
        [TestMethod] public void When3xMemAccess_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("When3xMemAccess", "treadle.lo.fir", true);

        [TestMethod] public void When1xDuplexInput_fir() => TestTools.VerifyChiselTest("When1xDuplexInput", "fir", true);
        [TestMethod] public void When1xDuplexInput_lo_fir() => TestTools.VerifyChiselTest("When1xDuplexInput", "lo.fir", true);
        [TestMethod] public void When1xDuplexInput_treadle_lo_fir() => TestTools.VerifyChiselTest("When1xDuplexInput", "treadle.lo.fir", true);
        [TestMethod] public void When1xDuplexInput_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("When1xDuplexInput", "treadle.lo.fir", true);

        [TestMethod] public void WhenWireCondInput_fir() => TestTools.VerifyChiselTest("WhenWireCondInput", "fir", true);
        [TestMethod] public void WhenWireCondInput_lo_fir() => TestTools.VerifyChiselTest("WhenWireCondInput", "lo.fir", true);
        [TestMethod] public void WhenWireCondInput_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenWireCondInput", "treadle.lo.fir", true);
        [TestMethod] public void WhenWireCondInput_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenWireCondInput", "treadle.lo.fir", true);

        [TestMethod] public void WhenConstCondInput_fir() => TestTools.VerifyChiselTest("WhenConstCondInput", "fir", true);
        [TestMethod] public void WhenConstCondInput_lo_fir() => TestTools.VerifyChiselTest("WhenConstCondInput", "lo.fir", true);
        [TestMethod] public void WhenConstCondInput_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenConstCondInput", "treadle.lo.fir", true);
        [TestMethod] public void WhenConstCondInput_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenConstCondInput", "treadle.lo.fir", true);
    }
}
