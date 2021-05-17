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
        [TestMethod] public void SimpleIO_fir() => TestTools.VerifyChiselTest("ModA", "fir");
        [TestMethod] public void SimpleIO_lo_fir() => TestTools.VerifyChiselTest("ModA", "lo.fir");
        [TestMethod] public void SimpleIO_treadle_lo_fir() => TestTools.VerifyChiselTest("ModA", "treadle.lo.fir");

        [TestMethod] public void Nested1Module_fir() => TestTools.VerifyChiselTest("ModB", "fir");
        [TestMethod] public void Nested1Module_lo_fir() => TestTools.VerifyChiselTest("ModB", "lo.fir");
        [TestMethod] public void Nested1Module_treadle_lo_fir() => TestTools.VerifyChiselTest("ModB", "treadle.lo.fir");

        [TestMethod] public void Nested1Module2x_fir() => TestTools.VerifyChiselTest("ModC", "fir");
        [TestMethod] public void Nested1Module2x_lo_fir() => TestTools.VerifyChiselTest("ModC", "lo.fir");
        [TestMethod] public void Nested1Module2x_treadle_lo_fir() => TestTools.VerifyChiselTest("ModC", "treadle.lo.fir");

        [TestMethod] public void MuxOnBundles_fir() => TestTools.VerifyChiselTest("ModD", "fir");
        [TestMethod] public void MuxOnBundles_lo_fir() => TestTools.VerifyChiselTest("ModD", "lo.fir");
        [TestMethod] public void MuxOnBundles_treadle_lo_fir() => TestTools.VerifyChiselTest("ModD", "treadle.lo.fir");

        [TestMethod] public void SimpleVector_fir() => TestTools.VerifyChiselTest("ModE", "fir");
        [TestMethod] public void SimpleVector_lo_fir() => TestTools.VerifyChiselTest("ModE", "lo.fir");
        [TestMethod] public void SimpleVector_treadle_lo_fir() => TestTools.VerifyChiselTest("ModE", "treadle.lo.fir");

        [TestMethod] public void MuxOnBundlesWithVector_fir() => TestTools.VerifyChiselTest("ModF", "fir");
        [TestMethod] public void MuxOnBundlesWithVector_lo_fir() => TestTools.VerifyChiselTest("ModF", "lo.fir");
        [TestMethod] public void MuxOnBundlesWithVector_treadle_lo_fir() => TestTools.VerifyChiselTest("ModF", "treadle.lo.fir");

        [TestMethod] public void SimpleBundleWithVector_fir() => TestTools.VerifyChiselTest("ModG", "fir");
        [TestMethod] public void SimpleBundleWithVector_lo_fir() => TestTools.VerifyChiselTest("ModG", "lo.fir");
        [TestMethod] public void SimpleBundleWithVector_treadle_lo_fir() => TestTools.VerifyChiselTest("ModG", "treadle.lo.fir");

        [TestMethod] public void Nested1ModuleBundleWithVector_fir() => TestTools.VerifyChiselTest("ModH", "fir");
        [TestMethod] public void Nested1ModuleBundleWithVector_lo_fir() => TestTools.VerifyChiselTest("ModH", "lo.fir");
        [TestMethod] public void Nested1ModuleBundleWithVector_treadle_lo_fir() => TestTools.VerifyChiselTest("ModH", "treadle.lo.fir");

        [TestMethod] public void InOutWireVecBundle_fir() => TestTools.VerifyChiselTest("ModI", "fir");
        [TestMethod] public void InOutWireVecBundle_lo_fir() => TestTools.VerifyChiselTest("ModI", "lo.fir");
        [TestMethod] public void InOutWireVecBundle_treadle_lo_fir() => TestTools.VerifyChiselTest("ModI", "treadle.lo.fir");

        [TestMethod] public void WireConnectInBeforeOut_fir() => TestTools.VerifyChiselTest("ModJ", "fir");
        [TestMethod] public void WireConnectInBeforeOut_lo_fir() => TestTools.VerifyChiselTest("ModJ", "lo.fir");
        [TestMethod] public void WireConnectInBeforeOut_treadle_lo_fir() => TestTools.VerifyChiselTest("ModJ", "treadle.lo.fir");

        [TestMethod] public void WireConnectOutBeforeIn_fir() => TestTools.VerifyChiselTest("ModK", "fir");
        [TestMethod] public void WireConnectOutBeforeIn_lo_fir() => TestTools.VerifyChiselTest("ModK", "lo.fir");
        [TestMethod] public void WireConnectOutBeforeIn_treadle_lo_fir() => TestTools.VerifyChiselTest("ModK", "treadle.lo.fir");

        [TestMethod] public void WireConnectConditionalOrder_fir() => TestTools.VerifyChiselTest("ModL", "fir");
        [TestMethod] public void WireConnectConditionalOrder_lo_fir() => TestTools.VerifyChiselTest("ModL", "lo.fir");
        [TestMethod] public void WireConnectConditionalOrder_treadle_lo_fir() => TestTools.VerifyChiselTest("ModL", "treadle.lo.fir");

        [TestMethod] public void RegConnectVecBundleVec_fir() => TestTools.VerifyChiselTest("ModM", "fir");
        [TestMethod] public void RegConnectVecBundleVec_lo_fir() => TestTools.VerifyChiselTest("ModM", "lo.fir");
        [TestMethod] public void RegConnectVecBundleVec_treadle_lo_fir() => TestTools.VerifyChiselTest("ModM", "treadle.lo.fir");

        [TestMethod] public void RegConnectBundleVec_fir() => TestTools.VerifyChiselTest("ModN", "fir");
        [TestMethod] public void RegConnectBundleVec_lo_fir() => TestTools.VerifyChiselTest("ModN", "lo.fir");
        [TestMethod] public void RegConnectBundleVec_treadle_lo_fir() => TestTools.VerifyChiselTest("ModN", "treadle.lo.fir");
    }
}
