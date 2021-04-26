using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebugTests
{
    [TestClass]
    public class ComputeOthersFIRRTL
    {
        const string TestDir = @"...";

        [TestMethod] public void AxonSystem_fir() => TestTools.VerifyChiselTest("AxonSystem", "fir", true, TestDir);
        [TestMethod] public void AxonSystem_lo_fir() => TestTools.VerifyChiselTest("AxonSystem", "lo.fir", true, TestDir);

        [TestMethod] public void BusArbiter_fir() => TestTools.VerifyChiselTest("BusArbiter", "fir", true, TestDir);
        [TestMethod] public void BusArbiter_lo_fir() => TestTools.VerifyChiselTest("BusArbiter", "lo.fir", true, TestDir);

        [TestMethod] public void BusInterface_fir() => TestTools.VerifyChiselTest("BusInterface", "fir", true, TestDir);
        [TestMethod] public void BusInterface_lo_fir() => TestTools.VerifyChiselTest("BusInterface", "lo.fir", true, TestDir);

        [TestMethod] public void InputCore_fir() => TestTools.VerifyChiselTest("InputCore", "fir", true, TestDir);
        [TestMethod] public void InputCore_lo_fir() => TestTools.VerifyChiselTest("InputCore", "lo.fir", true, TestDir);

        [TestMethod] public void NeuromorphicProcessor_fir() => TestTools.VerifyChiselTest("NeuromorphicProcessor", "fir", true, TestDir);
        [TestMethod] public void NeuromorphicProcessor_lo_fir() => TestTools.VerifyChiselTest("NeuromorphicProcessor", "lo.fir", true, TestDir);

        [TestMethod] public void NeuromorphicProcessor_0_fir() => TestTools.VerifyChiselTest("NeuromorphicProcessor_0", "fir", true, TestDir);
        [TestMethod] public void NeuromorphicProcessor_0_lo_fir() => TestTools.VerifyChiselTest("NeuromorphicProcessor_0", "lo.fir", true, TestDir);

        [TestMethod] public void NeuronEvaluator_fir() => TestTools.VerifyChiselTest("NeuronEvaluator", "fir", true, TestDir);
        [TestMethod] public void NeuronEvaluator_lo_fir() => TestTools.VerifyChiselTest("NeuronEvaluator", "lo.fir", true, TestDir);

        [TestMethod] public void OffChipCom_fir() => TestTools.VerifyChiselTest("OffChipCom", "fir", true, TestDir);
        [TestMethod] public void OffChipCom_lo_fir() => TestTools.VerifyChiselTest("OffChipCom", "lo.fir", true, TestDir);

        [TestMethod] public void PriorityMaskRstEncoder_fir() => TestTools.VerifyChiselTest("PriorityMaskRstEncoder", "fir", true, TestDir);
        [TestMethod] public void PriorityMaskRstEncoder_lo_fir() => TestTools.VerifyChiselTest("PriorityMaskRstEncoder", "lo.fir", true, TestDir);

        [TestMethod] public void UartEcho_fir() => TestTools.VerifyChiselTest("UartEcho", "fir", true, TestDir);
        [TestMethod] public void UartEcho_lo_fir() => TestTools.VerifyChiselTest("UartEcho", "lo.fir", true, TestDir);

        [TestMethod] public void ALUTester_fir() => TestTools.VerifyChiselTest("ALUTester", "fir", true, TestDir);
        [TestMethod] public void ALUTester_lo_fir() => TestTools.VerifyChiselTest("ALUTester", "lo.fir", true, TestDir);

        [TestMethod] public void ALUTester_0_fir() => TestTools.VerifyChiselTest("ALUTester_0", "fir", true, TestDir);
        [TestMethod] public void ALUTester_0_lo_fir() => TestTools.VerifyChiselTest("ALUTester_0", "lo.fir", true, TestDir);

        [TestMethod] public void BrCondTester_fir() => TestTools.VerifyChiselTest("BrCondTester", "fir", true, TestDir);
        [TestMethod] public void BrCondTester_lo_fir() => TestTools.VerifyChiselTest("BrCondTester", "lo.fir", true, TestDir);

        [TestMethod] public void BrCondTester_0_fir() => TestTools.VerifyChiselTest("BrCondTester_0", "fir", true, TestDir);
        [TestMethod] public void BrCondTester_0_lo_fir() => TestTools.VerifyChiselTest("BrCondTester_0", "lo.fir", true, TestDir);

        [TestMethod] public void CacheTester_fir() => TestTools.VerifyChiselTest("CacheTester", "fir", true, TestDir);
        [TestMethod] public void CacheTester_lo_fir() => TestTools.VerifyChiselTest("CacheTester", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_fir() => TestTools.VerifyChiselTest("CoreTester", "fir", true, TestDir);
        [TestMethod] public void CoreTester_lo_fir() => TestTools.VerifyChiselTest("CoreTester", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_0_fir() => TestTools.VerifyChiselTest("CoreTester_0", "fir", true, TestDir);
        [TestMethod] public void CoreTester_0_lo_fir() => TestTools.VerifyChiselTest("CoreTester_0", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_1_fir() => TestTools.VerifyChiselTest("CoreTester_1", "fir", true, TestDir);
        [TestMethod] public void CoreTester_1_lo_fir() => TestTools.VerifyChiselTest("CoreTester_1", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_2_fir() => TestTools.VerifyChiselTest("CoreTester_2", "fir", true, TestDir);
        [TestMethod] public void CoreTester_2_lo_fir() => TestTools.VerifyChiselTest("CoreTester_2", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_3_fir() => TestTools.VerifyChiselTest("CoreTester_3", "fir", true, TestDir);
        [TestMethod] public void CoreTester_3_lo_fir() => TestTools.VerifyChiselTest("CoreTester_3", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_4_fir() => TestTools.VerifyChiselTest("CoreTester_4", "fir", true, TestDir);
        [TestMethod] public void CoreTester_4_lo_fir() => TestTools.VerifyChiselTest("CoreTester_4", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_5_fir() => TestTools.VerifyChiselTest("CoreTester_5", "fir", true, TestDir);
        [TestMethod] public void CoreTester_5_lo_fir() => TestTools.VerifyChiselTest("CoreTester_5", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_6_fir() => TestTools.VerifyChiselTest("CoreTester_6", "fir", true, TestDir);
        [TestMethod] public void CoreTester_6_lo_fir() => TestTools.VerifyChiselTest("CoreTester_6", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_7_fir() => TestTools.VerifyChiselTest("CoreTester_7", "fir", true, TestDir);
        [TestMethod] public void CoreTester_7_lo_fir() => TestTools.VerifyChiselTest("CoreTester_7", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_8_fir() => TestTools.VerifyChiselTest("CoreTester_8", "fir", true, TestDir);
        [TestMethod] public void CoreTester_8_lo_fir() => TestTools.VerifyChiselTest("CoreTester_8", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_9_fir() => TestTools.VerifyChiselTest("CoreTester_9", "fir", true, TestDir);
        [TestMethod] public void CoreTester_9_lo_fir() => TestTools.VerifyChiselTest("CoreTester_9", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_10_fir() => TestTools.VerifyChiselTest("CoreTester_10", "fir", true, TestDir);
        [TestMethod] public void CoreTester_10_lo_fir() => TestTools.VerifyChiselTest("CoreTester_10", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_11_fir() => TestTools.VerifyChiselTest("CoreTester_11", "fir", true, TestDir);
        [TestMethod] public void CoreTester_11_lo_fir() => TestTools.VerifyChiselTest("CoreTester_11", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_12_fir() => TestTools.VerifyChiselTest("CoreTester_12", "fir", true, TestDir);
        [TestMethod] public void CoreTester_12_lo_fir() => TestTools.VerifyChiselTest("CoreTester_12", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_13_fir() => TestTools.VerifyChiselTest("CoreTester_13", "fir", true, TestDir);
        [TestMethod] public void CoreTester_13_lo_fir() => TestTools.VerifyChiselTest("CoreTester_13", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_14_fir() => TestTools.VerifyChiselTest("CoreTester_14", "fir", true, TestDir);
        [TestMethod] public void CoreTester_14_lo_fir() => TestTools.VerifyChiselTest("CoreTester_14", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_15_fir() => TestTools.VerifyChiselTest("CoreTester_15", "fir", true, TestDir);
        [TestMethod] public void CoreTester_15_lo_fir() => TestTools.VerifyChiselTest("CoreTester_15", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_16_fir() => TestTools.VerifyChiselTest("CoreTester_16", "fir", true, TestDir);
        [TestMethod] public void CoreTester_16_lo_fir() => TestTools.VerifyChiselTest("CoreTester_16", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_17_fir() => TestTools.VerifyChiselTest("CoreTester_17", "fir", true, TestDir);
        [TestMethod] public void CoreTester_17_lo_fir() => TestTools.VerifyChiselTest("CoreTester_17", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_18_fir() => TestTools.VerifyChiselTest("CoreTester_18", "fir", true, TestDir);
        [TestMethod] public void CoreTester_18_lo_fir() => TestTools.VerifyChiselTest("CoreTester_18", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_19_fir() => TestTools.VerifyChiselTest("CoreTester_19", "fir", true, TestDir);
        [TestMethod] public void CoreTester_19_lo_fir() => TestTools.VerifyChiselTest("CoreTester_19", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_20_fir() => TestTools.VerifyChiselTest("CoreTester_20", "fir", true, TestDir);
        [TestMethod] public void CoreTester_20_lo_fir() => TestTools.VerifyChiselTest("CoreTester_20", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_21_fir() => TestTools.VerifyChiselTest("CoreTester_21", "fir", true, TestDir);
        [TestMethod] public void CoreTester_21_lo_fir() => TestTools.VerifyChiselTest("CoreTester_21", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_22_fir() => TestTools.VerifyChiselTest("CoreTester_22", "fir", true, TestDir);
        [TestMethod] public void CoreTester_22_lo_fir() => TestTools.VerifyChiselTest("CoreTester_22", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_23_fir() => TestTools.VerifyChiselTest("CoreTester_23", "fir", true, TestDir);
        [TestMethod] public void CoreTester_23_lo_fir() => TestTools.VerifyChiselTest("CoreTester_23", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_24_fir() => TestTools.VerifyChiselTest("CoreTester_24", "fir", true, TestDir);
        [TestMethod] public void CoreTester_24_lo_fir() => TestTools.VerifyChiselTest("CoreTester_24", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_25_fir() => TestTools.VerifyChiselTest("CoreTester_25", "fir", true, TestDir);
        [TestMethod] public void CoreTester_25_lo_fir() => TestTools.VerifyChiselTest("CoreTester_25", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_26_fir() => TestTools.VerifyChiselTest("CoreTester_26", "fir", true, TestDir);
        [TestMethod] public void CoreTester_26_lo_fir() => TestTools.VerifyChiselTest("CoreTester_26", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_27_fir() => TestTools.VerifyChiselTest("CoreTester_27", "fir", true, TestDir);
        [TestMethod] public void CoreTester_27_lo_fir() => TestTools.VerifyChiselTest("CoreTester_27", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_28_fir() => TestTools.VerifyChiselTest("CoreTester_28", "fir", true, TestDir);
        [TestMethod] public void CoreTester_28_lo_fir() => TestTools.VerifyChiselTest("CoreTester_28", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_29_fir() => TestTools.VerifyChiselTest("CoreTester_29", "fir", true, TestDir);
        [TestMethod] public void CoreTester_29_lo_fir() => TestTools.VerifyChiselTest("CoreTester_29", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_30_fir() => TestTools.VerifyChiselTest("CoreTester_30", "fir", true, TestDir);
        [TestMethod] public void CoreTester_30_lo_fir() => TestTools.VerifyChiselTest("CoreTester_30", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_31_fir() => TestTools.VerifyChiselTest("CoreTester_31", "fir", true, TestDir);
        [TestMethod] public void CoreTester_31_lo_fir() => TestTools.VerifyChiselTest("CoreTester_31", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_32_fir() => TestTools.VerifyChiselTest("CoreTester_32", "fir", true, TestDir);
        [TestMethod] public void CoreTester_32_lo_fir() => TestTools.VerifyChiselTest("CoreTester_32", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_33_fir() => TestTools.VerifyChiselTest("CoreTester_33", "fir", true, TestDir);
        [TestMethod] public void CoreTester_33_lo_fir() => TestTools.VerifyChiselTest("CoreTester_33", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_34_fir() => TestTools.VerifyChiselTest("CoreTester_34", "fir", true, TestDir);
        [TestMethod] public void CoreTester_34_lo_fir() => TestTools.VerifyChiselTest("CoreTester_34", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_35_fir() => TestTools.VerifyChiselTest("CoreTester_35", "fir", true, TestDir);
        [TestMethod] public void CoreTester_35_lo_fir() => TestTools.VerifyChiselTest("CoreTester_35", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_36_fir() => TestTools.VerifyChiselTest("CoreTester_36", "fir", true, TestDir);
        [TestMethod] public void CoreTester_36_lo_fir() => TestTools.VerifyChiselTest("CoreTester_36", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_37_fir() => TestTools.VerifyChiselTest("CoreTester_37", "fir", true, TestDir);
        [TestMethod] public void CoreTester_37_lo_fir() => TestTools.VerifyChiselTest("CoreTester_37", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_38_fir() => TestTools.VerifyChiselTest("CoreTester_38", "fir", true, TestDir);
        [TestMethod] public void CoreTester_38_lo_fir() => TestTools.VerifyChiselTest("CoreTester_38", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_39_fir() => TestTools.VerifyChiselTest("CoreTester_39", "fir", true, TestDir);
        [TestMethod] public void CoreTester_39_lo_fir() => TestTools.VerifyChiselTest("CoreTester_39", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_40_fir() => TestTools.VerifyChiselTest("CoreTester_40", "fir", true, TestDir);
        [TestMethod] public void CoreTester_40_lo_fir() => TestTools.VerifyChiselTest("CoreTester_40", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_41_fir() => TestTools.VerifyChiselTest("CoreTester_41", "fir", true, TestDir);
        [TestMethod] public void CoreTester_41_lo_fir() => TestTools.VerifyChiselTest("CoreTester_41", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_42_fir() => TestTools.VerifyChiselTest("CoreTester_42", "fir", true, TestDir);
        [TestMethod] public void CoreTester_42_lo_fir() => TestTools.VerifyChiselTest("CoreTester_42", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_43_fir() => TestTools.VerifyChiselTest("CoreTester_43", "fir", true, TestDir);
        [TestMethod] public void CoreTester_43_lo_fir() => TestTools.VerifyChiselTest("CoreTester_43", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_44_fir() => TestTools.VerifyChiselTest("CoreTester_44", "fir", true, TestDir);
        [TestMethod] public void CoreTester_44_lo_fir() => TestTools.VerifyChiselTest("CoreTester_44", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_45_fir() => TestTools.VerifyChiselTest("CoreTester_45", "fir", true, TestDir);
        [TestMethod] public void CoreTester_45_lo_fir() => TestTools.VerifyChiselTest("CoreTester_45", "lo.fir", true, TestDir);

        [TestMethod] public void CSRTester_fir() => TestTools.VerifyChiselTest("CSRTester", "fir", true, TestDir);
        [TestMethod] public void CSRTester_lo_fir() => TestTools.VerifyChiselTest("CSRTester", "lo.fir", true, TestDir);

        [TestMethod] public void DatapathTester_fir() => TestTools.VerifyChiselTest("DatapathTester", "fir", true, TestDir);
        [TestMethod] public void DatapathTester_lo_fir() => TestTools.VerifyChiselTest("DatapathTester", "lo.fir", true, TestDir);

        [TestMethod] public void DatapathTester_0_fir() => TestTools.VerifyChiselTest("DatapathTester_0", "fir", true, TestDir);
        [TestMethod] public void DatapathTester_0_lo_fir() => TestTools.VerifyChiselTest("DatapathTester_0", "lo.fir", true, TestDir);

        [TestMethod] public void ImmGenTester_fir() => TestTools.VerifyChiselTest("ImmGenTester", "fir", true, TestDir);
        [TestMethod] public void ImmGenTester_lo_fir() => TestTools.VerifyChiselTest("ImmGenTester", "lo.fir", true, TestDir);

        [TestMethod] public void ImmGenTester_0_fir() => TestTools.VerifyChiselTest("ImmGenTester_0", "fir", true, TestDir);
        [TestMethod] public void ImmGenTester_0_lo_fir() => TestTools.VerifyChiselTest("ImmGenTester_0", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_fir() => TestTools.VerifyChiselTest("TileTester", "fir", true, TestDir);
        [TestMethod] public void TileTester_lo_fir() => TestTools.VerifyChiselTest("TileTester", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_0_fir() => TestTools.VerifyChiselTest("TileTester_0", "fir", true, TestDir);
        [TestMethod] public void TileTester_0_lo_fir() => TestTools.VerifyChiselTest("TileTester_0", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_1_fir() => TestTools.VerifyChiselTest("TileTester_1", "fir", true, TestDir);
        [TestMethod] public void TileTester_1_lo_fir() => TestTools.VerifyChiselTest("TileTester_1", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_2_fir() => TestTools.VerifyChiselTest("TileTester_2", "fir", true, TestDir);
        [TestMethod] public void TileTester_2_lo_fir() => TestTools.VerifyChiselTest("TileTester_2", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_3_fir() => TestTools.VerifyChiselTest("TileTester_3", "fir", true, TestDir);
        [TestMethod] public void TileTester_3_lo_fir() => TestTools.VerifyChiselTest("TileTester_3", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_4_fir() => TestTools.VerifyChiselTest("TileTester_4", "fir", true, TestDir);
        [TestMethod] public void TileTester_4_lo_fir() => TestTools.VerifyChiselTest("TileTester_4", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_5_fir() => TestTools.VerifyChiselTest("TileTester_5", "fir", true, TestDir);
        [TestMethod] public void TileTester_5_lo_fir() => TestTools.VerifyChiselTest("TileTester_5", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_6_fir() => TestTools.VerifyChiselTest("TileTester_6", "fir", true, TestDir);
        [TestMethod] public void TileTester_6_lo_fir() => TestTools.VerifyChiselTest("TileTester_6", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_7_fir() => TestTools.VerifyChiselTest("TileTester_7", "fir", true, TestDir);
        [TestMethod] public void TileTester_7_lo_fir() => TestTools.VerifyChiselTest("TileTester_7", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_8_fir() => TestTools.VerifyChiselTest("TileTester_8", "fir", true, TestDir);
        [TestMethod] public void TileTester_8_lo_fir() => TestTools.VerifyChiselTest("TileTester_8", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_9_fir() => TestTools.VerifyChiselTest("TileTester_9", "fir", true, TestDir);
        [TestMethod] public void TileTester_9_lo_fir() => TestTools.VerifyChiselTest("TileTester_9", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_10_fir() => TestTools.VerifyChiselTest("TileTester_10", "fir", true, TestDir);
        [TestMethod] public void TileTester_10_lo_fir() => TestTools.VerifyChiselTest("TileTester_10", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_11_fir() => TestTools.VerifyChiselTest("TileTester_11", "fir", true, TestDir);
        [TestMethod] public void TileTester_11_lo_fir() => TestTools.VerifyChiselTest("TileTester_11", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_12_fir() => TestTools.VerifyChiselTest("TileTester_12", "fir", true, TestDir);
        [TestMethod] public void TileTester_12_lo_fir() => TestTools.VerifyChiselTest("TileTester_12", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_13_fir() => TestTools.VerifyChiselTest("TileTester_13", "fir", true, TestDir);
        [TestMethod] public void TileTester_13_lo_fir() => TestTools.VerifyChiselTest("TileTester_13", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_14_fir() => TestTools.VerifyChiselTest("TileTester_14", "fir", true, TestDir);
        [TestMethod] public void TileTester_14_lo_fir() => TestTools.VerifyChiselTest("TileTester_14", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_15_fir() => TestTools.VerifyChiselTest("TileTester_15", "fir", true, TestDir);
        [TestMethod] public void TileTester_15_lo_fir() => TestTools.VerifyChiselTest("TileTester_15", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_16_fir() => TestTools.VerifyChiselTest("TileTester_16", "fir", true, TestDir);
        [TestMethod] public void TileTester_16_lo_fir() => TestTools.VerifyChiselTest("TileTester_16", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_17_fir() => TestTools.VerifyChiselTest("TileTester_17", "fir", true, TestDir);
        [TestMethod] public void TileTester_17_lo_fir() => TestTools.VerifyChiselTest("TileTester_17", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_18_fir() => TestTools.VerifyChiselTest("TileTester_18", "fir", true, TestDir);
        [TestMethod] public void TileTester_18_lo_fir() => TestTools.VerifyChiselTest("TileTester_18", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_19_fir() => TestTools.VerifyChiselTest("TileTester_19", "fir", true, TestDir);
        [TestMethod] public void TileTester_19_lo_fir() => TestTools.VerifyChiselTest("TileTester_19", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_20_fir() => TestTools.VerifyChiselTest("TileTester_20", "fir", true, TestDir);
        [TestMethod] public void TileTester_20_lo_fir() => TestTools.VerifyChiselTest("TileTester_20", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_21_fir() => TestTools.VerifyChiselTest("TileTester_21", "fir", true, TestDir);
        [TestMethod] public void TileTester_21_lo_fir() => TestTools.VerifyChiselTest("TileTester_21", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_22_fir() => TestTools.VerifyChiselTest("TileTester_22", "fir", true, TestDir);
        [TestMethod] public void TileTester_22_lo_fir() => TestTools.VerifyChiselTest("TileTester_22", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_23_fir() => TestTools.VerifyChiselTest("TileTester_23", "fir", true, TestDir);
        [TestMethod] public void TileTester_23_lo_fir() => TestTools.VerifyChiselTest("TileTester_23", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_24_fir() => TestTools.VerifyChiselTest("TileTester_24", "fir", true, TestDir);
        [TestMethod] public void TileTester_24_lo_fir() => TestTools.VerifyChiselTest("TileTester_24", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_25_fir() => TestTools.VerifyChiselTest("TileTester_25", "fir", true, TestDir);
        [TestMethod] public void TileTester_25_lo_fir() => TestTools.VerifyChiselTest("TileTester_25", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_26_fir() => TestTools.VerifyChiselTest("TileTester_26", "fir", true, TestDir);
        [TestMethod] public void TileTester_26_lo_fir() => TestTools.VerifyChiselTest("TileTester_26", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_27_fir() => TestTools.VerifyChiselTest("TileTester_27", "fir", true, TestDir);
        [TestMethod] public void TileTester_27_lo_fir() => TestTools.VerifyChiselTest("TileTester_27", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_28_fir() => TestTools.VerifyChiselTest("TileTester_28", "fir", true, TestDir);
        [TestMethod] public void TileTester_28_lo_fir() => TestTools.VerifyChiselTest("TileTester_28", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_29_fir() => TestTools.VerifyChiselTest("TileTester_29", "fir", true, TestDir);
        [TestMethod] public void TileTester_29_lo_fir() => TestTools.VerifyChiselTest("TileTester_29", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_30_fir() => TestTools.VerifyChiselTest("TileTester_30", "fir", true, TestDir);
        [TestMethod] public void TileTester_30_lo_fir() => TestTools.VerifyChiselTest("TileTester_30", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_31_fir() => TestTools.VerifyChiselTest("TileTester_31", "fir", true, TestDir);
        [TestMethod] public void TileTester_31_lo_fir() => TestTools.VerifyChiselTest("TileTester_31", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_32_fir() => TestTools.VerifyChiselTest("TileTester_32", "fir", true, TestDir);
        [TestMethod] public void TileTester_32_lo_fir() => TestTools.VerifyChiselTest("TileTester_32", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_33_fir() => TestTools.VerifyChiselTest("TileTester_33", "fir", true, TestDir);
        [TestMethod] public void TileTester_33_lo_fir() => TestTools.VerifyChiselTest("TileTester_33", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_34_fir() => TestTools.VerifyChiselTest("TileTester_34", "fir", true, TestDir);
        [TestMethod] public void TileTester_34_lo_fir() => TestTools.VerifyChiselTest("TileTester_34", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_35_fir() => TestTools.VerifyChiselTest("TileTester_35", "fir", true, TestDir);
        [TestMethod] public void TileTester_35_lo_fir() => TestTools.VerifyChiselTest("TileTester_35", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_36_fir() => TestTools.VerifyChiselTest("TileTester_36", "fir", true, TestDir);
        [TestMethod] public void TileTester_36_lo_fir() => TestTools.VerifyChiselTest("TileTester_36", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_37_fir() => TestTools.VerifyChiselTest("TileTester_37", "fir", true, TestDir);
        [TestMethod] public void TileTester_37_lo_fir() => TestTools.VerifyChiselTest("TileTester_37", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_38_fir() => TestTools.VerifyChiselTest("TileTester_38", "fir", true, TestDir);
        [TestMethod] public void TileTester_38_lo_fir() => TestTools.VerifyChiselTest("TileTester_38", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_39_fir() => TestTools.VerifyChiselTest("TileTester_39", "fir", true, TestDir);
        [TestMethod] public void TileTester_39_lo_fir() => TestTools.VerifyChiselTest("TileTester_39", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_40_fir() => TestTools.VerifyChiselTest("TileTester_40", "fir", true, TestDir);
        [TestMethod] public void TileTester_40_lo_fir() => TestTools.VerifyChiselTest("TileTester_40", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_41_fir() => TestTools.VerifyChiselTest("TileTester_41", "fir", true, TestDir);
        [TestMethod] public void TileTester_41_lo_fir() => TestTools.VerifyChiselTest("TileTester_41", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_42_fir() => TestTools.VerifyChiselTest("TileTester_42", "fir", true, TestDir);
        [TestMethod] public void TileTester_42_lo_fir() => TestTools.VerifyChiselTest("TileTester_42", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_43_fir() => TestTools.VerifyChiselTest("TileTester_43", "fir", true, TestDir);
        [TestMethod] public void TileTester_43_lo_fir() => TestTools.VerifyChiselTest("TileTester_43", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_44_fir() => TestTools.VerifyChiselTest("TileTester_44", "fir", true, TestDir);
        [TestMethod] public void TileTester_44_lo_fir() => TestTools.VerifyChiselTest("TileTester_44", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_45_fir() => TestTools.VerifyChiselTest("TileTester_45", "fir", true, TestDir);
        [TestMethod] public void TileTester_45_lo_fir() => TestTools.VerifyChiselTest("TileTester_45", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_46_fir() => TestTools.VerifyChiselTest("TileTester_46", "fir", true, TestDir);
        [TestMethod] public void TileTester_46_lo_fir() => TestTools.VerifyChiselTest("TileTester_46", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_47_fir() => TestTools.VerifyChiselTest("TileTester_47", "fir", true, TestDir);
        [TestMethod] public void TileTester_47_lo_fir() => TestTools.VerifyChiselTest("TileTester_47", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_48_fir() => TestTools.VerifyChiselTest("TileTester_48", "fir", true, TestDir);
        [TestMethod] public void TileTester_48_lo_fir() => TestTools.VerifyChiselTest("TileTester_48", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_49_fir() => TestTools.VerifyChiselTest("TileTester_49", "fir", true, TestDir);
        [TestMethod] public void TileTester_49_lo_fir() => TestTools.VerifyChiselTest("TileTester_49", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_50_fir() => TestTools.VerifyChiselTest("TileTester_50", "fir", true, TestDir);
        [TestMethod] public void TileTester_50_lo_fir() => TestTools.VerifyChiselTest("TileTester_50", "lo.fir", true, TestDir);


    }
}
