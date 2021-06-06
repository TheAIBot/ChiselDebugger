using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebugTests
{
    [TestClass]
    public class ConnectOrderTests
    {
        [TestMethod] public void WhenWireConnectOrderUncondFirst_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrderUncondFirst", "fir");
        [TestMethod] public void WhenWireConnectOrderUncondFirst_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrderUncondFirst", "lo.fir");
        [TestMethod] public void WhenWireConnectOrderUncondFirst_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrderUncondFirst", "treadle.lo.fir");

        [TestMethod] public void WhenWireConnectOrderUncondLast_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrderUncondLast", "fir");
        [TestMethod] public void WhenWireConnectOrderUncondLast_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrderUncondLast", "lo.fir");
        [TestMethod] public void WhenWireConnectOrderUncondLast_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrderUncondLast", "treadle.lo.fir");

        [TestMethod] public void WhenWireConnectOrder1To1Mix1_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder1To1Mix1", "fir");
        [TestMethod] public void WhenWireConnectOrder1To1Mix1_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder1To1Mix1", "lo.fir");
        [TestMethod] public void WhenWireConnectOrder1To1Mix1_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder1To1Mix1", "treadle.lo.fir");

        [TestMethod] public void WhenWireConnectOrder1To1Mix2_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder1To1Mix2", "fir");
        [TestMethod] public void WhenWireConnectOrder1To1Mix2_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder1To1Mix2", "lo.fir");
        [TestMethod] public void WhenWireConnectOrder1To1Mix2_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder1To1Mix2", "treadle.lo.fir");

        [TestMethod] public void WhenWireConnectOrder1To1Mix3_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder1To1Mix3", "fir");
        [TestMethod] public void WhenWireConnectOrder1To1Mix3_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder1To1Mix3", "lo.fir");
        [TestMethod] public void WhenWireConnectOrder1To1Mix3_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder1To1Mix3", "treadle.lo.fir");

        [TestMethod] public void WhenWireConnectOrder1To1Mix4_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder1To1Mix4", "fir");
        [TestMethod] public void WhenWireConnectOrder1To1Mix4_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder1To1Mix4", "lo.fir");
        [TestMethod] public void WhenWireConnectOrder1To1Mix4_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder1To1Mix4", "treadle.lo.fir");

        [TestMethod] public void WhenWireConnectOrder1To1Mix5_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder1To1Mix5", "fir");
        [TestMethod] public void WhenWireConnectOrder1To1Mix5_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder1To1Mix5", "lo.fir");
        [TestMethod] public void WhenWireConnectOrder1To1Mix5_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder1To1Mix5", "treadle.lo.fir");

        [TestMethod] public void WhenWireConnectOrder2To1Mix1_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix1", "fir");
        [TestMethod] public void WhenWireConnectOrder2To1Mix1_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix1", "lo.fir");
        [TestMethod] public void WhenWireConnectOrder2To1Mix1_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix1", "treadle.lo.fir");

        [TestMethod] public void WhenWireConnectOrder2To1Mix2_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix2", "fir");
        [TestMethod] public void WhenWireConnectOrder2To1Mix2_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix2", "lo.fir");
        [TestMethod] public void WhenWireConnectOrder2To1Mix2_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix2", "treadle.lo.fir");

        [TestMethod] public void WhenWireConnectOrder2To1Mix3_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix3", "fir");
        [TestMethod] public void WhenWireConnectOrder2To1Mix3_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix3", "lo.fir");
        [TestMethod] public void WhenWireConnectOrder2To1Mix3_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix3", "treadle.lo.fir");

        [TestMethod] public void WhenWireConnectOrder2To1Mix4_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix4", "fir");
        [TestMethod] public void WhenWireConnectOrder2To1Mix4_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix4", "lo.fir");
        [TestMethod] public void WhenWireConnectOrder2To1Mix4_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix4", "treadle.lo.fir");

        [TestMethod] public void WhenWireConnectOrder2To1Mix5_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix5", "fir");
        [TestMethod] public void WhenWireConnectOrder2To1Mix5_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix5", "lo.fir");
        [TestMethod] public void WhenWireConnectOrder2To1Mix5_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix5", "treadle.lo.fir");

        [TestMethod] public void WhenWireConnectOrder2To1Mix6_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix6", "fir");
        [TestMethod] public void WhenWireConnectOrder2To1Mix6_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix6", "lo.fir");
        [TestMethod] public void WhenWireConnectOrder2To1Mix6_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix6", "treadle.lo.fir");

        [TestMethod] public void WhenWireConnectOrder2To1Mix7_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix7", "fir");
        [TestMethod] public void WhenWireConnectOrder2To1Mix7_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix7", "lo.fir");
        [TestMethod] public void WhenWireConnectOrder2To1Mix7_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectOrder2To1Mix7", "treadle.lo.fir");

        [TestMethod] public void WhenWireConnectMultiSameSource_fir() => TestTools.VerifyChiselTest("WhenWireConnectMultiSameSource", "fir");
        [TestMethod] public void WhenWireConnectMultiSameSource_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectMultiSameSource", "lo.fir");
        [TestMethod] public void WhenWireConnectMultiSameSource_treadle_lo_fir() => TestTools.VerifyChiselTest("WhenWireConnectMultiSameSource", "treadle.lo.fir");
    }
}
