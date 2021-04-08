using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChiselDebug;
using VCDReader;
using System.IO;
using ChiselDebug.Timeline;

namespace ChiselDebugTests
{
    [TestClass]
    public class FIRRTLGraphTests
    {
        [TestMethod] public void SimpleIO_fir() => TestTools.VerifyChiselTest("ModA", "fir", false);
        [TestMethod] public void SimpleIO_lo_fir() => TestTools.VerifyChiselTest("ModA", "lo.fir", false);
        [TestMethod] public void SimpleIO_treadle_lo_fir() => TestTools.VerifyChiselTest("ModA", "treadle.lo.fir", false);
        [TestMethod] public void SimpleIO_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("ModA", "treadle.lo.fir", true);

        [TestMethod] public void Nested1Module_fir() => TestTools.VerifyChiselTest("ModB", "fir", false);
        [TestMethod] public void Nested1Module_lo_fir() => TestTools.VerifyChiselTest("ModB", "lo.fir", false);
        [TestMethod] public void Nested1Module_treadle_lo_fir() => TestTools.VerifyChiselTest("ModB", "treadle.lo.fir", false);
        [TestMethod] public void Nested1Module_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("ModB", "treadle.lo.fir", true);

        [TestMethod] public void Nested1Module2x_fir() => TestTools.VerifyChiselTest("ModC", "fir", false);
        [TestMethod] public void Nested1Module2x_lo_fir() => TestTools.VerifyChiselTest("ModC", "lo.fir", false);
        [TestMethod] public void Nested1Module2x_treadle_lo_fir() => TestTools.VerifyChiselTest("ModC", "treadle.lo.fir", false);
        [TestMethod] public void Nested1Module2x_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("ModC", "treadle.lo.fir", true);

        [TestMethod] public void MuxOnBundles_fir() => TestTools.VerifyChiselTest("ModD", "fir", false);
        [TestMethod] public void MuxOnBundles_lo_fir() => TestTools.VerifyChiselTest("ModD", "lo.fir", false);
        [TestMethod] public void MuxOnBundles_treadle_lo_fir() => TestTools.VerifyChiselTest("ModD", "treadle.lo.fir", false);
        [TestMethod] public void MuxOnBundles_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("ModD", "treadle.lo.fir", true);

        [TestMethod] public void SimpleVector_fir() => TestTools.VerifyChiselTest("ModE", "fir", false);
        [TestMethod] public void SimpleVector_lo_fir() => TestTools.VerifyChiselTest("ModE", "lo.fir", false);
        [TestMethod] public void SimpleVector_treadle_lo_fir() => TestTools.VerifyChiselTest("ModE", "treadle.lo.fir", false);
        [TestMethod] public void SimpleVector_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("ModE", "treadle.lo.fir", true);

        [TestMethod] public void MuxOnBundlesWithVector_fir() => TestTools.VerifyChiselTest("ModF", "fir", false);
        [TestMethod] public void MuxOnBundlesWithVector_lo_fir() => TestTools.VerifyChiselTest("ModF", "lo.fir", false);
        [TestMethod] public void MuxOnBundlesWithVector_treadle_lo_fir() => TestTools.VerifyChiselTest("ModF", "treadle.lo.fir", false);
        [TestMethod] public void MuxOnBundlesWithVector_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("ModF", "treadle.lo.fir", true);

        [TestMethod] public void SimpleBundleWithVector_fir() => TestTools.VerifyChiselTest("ModG", "fir", false);
        [TestMethod] public void SimpleBundleWithVector_lo_fir() => TestTools.VerifyChiselTest("ModG", "lo.fir", false);
        [TestMethod] public void SimpleBundleWithVector_treadle_lo_fir() => TestTools.VerifyChiselTest("ModG", "treadle.lo.fir", false);
        [TestMethod] public void SimpleBundleWithVector_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("ModG", "treadle.lo.fir", true);

        [TestMethod] public void Nested1ModuleBundleWithVector_fir() => TestTools.VerifyChiselTest("ModH", "fir", false);
        [TestMethod] public void Nested1ModuleBundleWithVector_lo_fir() => TestTools.VerifyChiselTest("ModH", "lo.fir", false);
        [TestMethod] public void Nested1ModuleBundleWithVector_treadle_lo_fir() => TestTools.VerifyChiselTest("ModH", "treadle.lo.fir", false);
        [TestMethod] public void Nested1ModuleBundleWithVector_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("ModH", "treadle.lo.fir", true);

        [TestMethod] public void InOutWireVecBundle_fir() => TestTools.VerifyChiselTest("ModI", "fir", false);
        [TestMethod] public void InOutWireVecBundle_lo_fir() => TestTools.VerifyChiselTest("ModI", "lo.fir", false);
        [TestMethod] public void InOutWireVecBundle_treadle_lo_fir() => TestTools.VerifyChiselTest("ModI", "treadle.lo.fir", false);
        [TestMethod] public void InOutWireVecBundle_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("ModI", "treadle.lo.fir", true);

        [TestMethod] public void WireConnectInBeforeOut_fir() => TestTools.VerifyChiselTest("ModJ", "fir", false);
        [TestMethod] public void WireConnectInBeforeOut_lo_fir() => TestTools.VerifyChiselTest("ModJ", "lo.fir", false);
        [TestMethod] public void WireConnectInBeforeOut_treadle_lo_fir() => TestTools.VerifyChiselTest("ModJ", "treadle.lo.fir", false);
        [TestMethod] public void WireConnectInBeforeOut_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("ModJ", "treadle.lo.fir", true);

        [TestMethod] public void WireConnectOutBeforeIn_fir() => TestTools.VerifyChiselTest("ModK", "fir", false);
        [TestMethod] public void WireConnectOutBeforeIn_lo_fir() => TestTools.VerifyChiselTest("ModK", "lo.fir", false);
        [TestMethod] public void WireConnectOutBeforeIn_treadle_lo_fir() => TestTools.VerifyChiselTest("ModK", "treadle.lo.fir", false);
        [TestMethod] public void WireConnectOutBeforeIn_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("ModK", "treadle.lo.fir", true);

        [TestMethod] public void WireConnectConditionalOrder_fir() => TestTools.VerifyChiselTest("ModL", "fir", false);
        [TestMethod] public void WireConnectConditionalOrder_lo_fir() => TestTools.VerifyChiselTest("ModL", "lo.fir", false);
        [TestMethod] public void WireConnectConditionalOrder_treadle_lo_fir() => TestTools.VerifyChiselTest("ModL", "treadle.lo.fir", false);
        [TestMethod] public void WireConnectConditionalOrder_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("ModL", "treadle.lo.fir", true);

        [TestMethod] public void RegConnectVecBundleVec_fir() => TestTools.VerifyChiselTest("ModM", "fir", false);
        [TestMethod] public void RegConnectVecBundleVec_lo_fir() => TestTools.VerifyChiselTest("ModM", "lo.fir", false);
        [TestMethod] public void RegConnectVecBundleVec_treadle_lo_fir() => TestTools.VerifyChiselTest("ModM", "treadle.lo.fir", false);
        [TestMethod] public void RegConnectVecBundleVec_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("ModM", "treadle.lo.fir", true);

        [TestMethod] public void RegConnectBundleVec_fir() => TestTools.VerifyChiselTest("ModN", "fir", false);
        [TestMethod] public void RegConnectBundleVec_lo_fir() => TestTools.VerifyChiselTest("ModN", "lo.fir", false);
        [TestMethod] public void RegConnectBundleVec_treadle_lo_fir() => TestTools.VerifyChiselTest("ModN", "treadle.lo.fir", false);
        [TestMethod] public void RegConnectBundleVec_vcd_treadle_lo_fir() => TestTools.VerifyChiselTest("ModN", "treadle.lo.fir", true);
    }
}
