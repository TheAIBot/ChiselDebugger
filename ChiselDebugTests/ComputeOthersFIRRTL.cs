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

        [TestMethod] public void AxonSystem_fir() => TestTools.VerifyComputeGraph("AxonSystem", "fir", false, TestDir);
        [TestMethod] public void AxonSystem_lo_fir() => TestTools.VerifyComputeGraph("AxonSystem", "lo.fir", false, TestDir);

        [TestMethod] public void BusArbiter_fir() => TestTools.VerifyComputeGraph("BusArbiter", "fir", false, TestDir);
        [TestMethod] public void BusArbiter_lo_fir() => TestTools.VerifyComputeGraph("BusArbiter", "lo.fir", false, TestDir);

        [TestMethod] public void BusInterface_fir() => TestTools.VerifyComputeGraph("BusInterface", "fir", false, TestDir);
        [TestMethod] public void BusInterface_lo_fir() => TestTools.VerifyComputeGraph("BusInterface", "lo.fir", false, TestDir);

        [TestMethod] public void InputCore_fir() => TestTools.VerifyComputeGraph("InputCore", "fir", false, TestDir);
        [TestMethod] public void InputCore_lo_fir() => TestTools.VerifyComputeGraph("InputCore", "lo.fir", false, TestDir);

        [TestMethod] public void NeuronEvaluator_fir() => TestTools.VerifyComputeGraph("NeuronEvaluator", "fir", false, TestDir);
        [TestMethod] public void NeuronEvaluator_lo_fir() => TestTools.VerifyComputeGraph("NeuronEvaluator", "lo.fir", false, TestDir);

        [TestMethod] public void OffChipCom_fir() => TestTools.VerifyComputeGraph("OffChipCom", "fir", false, TestDir);
        [TestMethod] public void OffChipCom_lo_fir() => TestTools.VerifyComputeGraph("OffChipCom", "lo.fir", false, TestDir);

        [TestMethod] public void PriorityMaskRstEncoder_fir() => TestTools.VerifyComputeGraph("PriorityMaskRstEncoder", "fir", false, TestDir);
        [TestMethod] public void PriorityMaskRstEncoder_lo_fir() => TestTools.VerifyComputeGraph("PriorityMaskRstEncoder", "lo.fir", false, TestDir);

        [TestMethod] public void UartEcho_fir() => TestTools.VerifyComputeGraph("UartEcho", "fir", false, TestDir);
        [TestMethod] public void UartEcho_lo_fir() => TestTools.VerifyComputeGraph("UartEcho", "lo.fir", false, TestDir);

        [TestMethod] public void ALUTester_fir() => TestTools.VerifyComputeGraph("ALUTester", "fir", true, TestDir);
        [TestMethod] public void ALUTester_lo_fir() => TestTools.VerifyComputeGraph("ALUTester", "lo.fir", true, TestDir);

        [TestMethod] public void ALUTester_0_fir() => TestTools.VerifyComputeGraph("ALUTester_0", "fir", true, TestDir);
        [TestMethod] public void ALUTester_0_lo_fir() => TestTools.VerifyComputeGraph("ALUTester_0", "lo.fir", true, TestDir);

        [TestMethod] public void BrCondTester_fir() => TestTools.VerifyComputeGraph("BrCondTester", "fir", true, TestDir);
        [TestMethod] public void BrCondTester_lo_fir() => TestTools.VerifyComputeGraph("BrCondTester", "lo.fir", true, TestDir);

        [TestMethod] public void BrCondTester_0_fir() => TestTools.VerifyComputeGraph("BrCondTester_0", "fir", true, TestDir);
        [TestMethod] public void BrCondTester_0_lo_fir() => TestTools.VerifyComputeGraph("BrCondTester_0", "lo.fir", true, TestDir);

        [TestMethod] public void CacheTester_fir() => TestTools.VerifyComputeGraph("CacheTester", "fir", true, TestDir);
        [TestMethod] public void CacheTester_lo_fir() => TestTools.VerifyComputeGraph("CacheTester", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_fir() => TestTools.VerifyComputeGraph("CoreTester", "fir", true, TestDir);
        [TestMethod] public void CoreTester_lo_fir() => TestTools.VerifyComputeGraph("CoreTester", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_0_fir() => TestTools.VerifyComputeGraph("CoreTester_0", "fir", true, TestDir);
        [TestMethod] public void CoreTester_0_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_0", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_1_fir() => TestTools.VerifyComputeGraph("CoreTester_1", "fir", true, TestDir);
        [TestMethod] public void CoreTester_1_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_1", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_2_fir() => TestTools.VerifyComputeGraph("CoreTester_2", "fir", true, TestDir);
        [TestMethod] public void CoreTester_2_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_2", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_3_fir() => TestTools.VerifyComputeGraph("CoreTester_3", "fir", true, TestDir);
        [TestMethod] public void CoreTester_3_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_3", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_4_fir() => TestTools.VerifyComputeGraph("CoreTester_4", "fir", true, TestDir);
        [TestMethod] public void CoreTester_4_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_4", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_5_fir() => TestTools.VerifyComputeGraph("CoreTester_5", "fir", true, TestDir);
        [TestMethod] public void CoreTester_5_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_5", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_6_fir() => TestTools.VerifyComputeGraph("CoreTester_6", "fir", true, TestDir);
        [TestMethod] public void CoreTester_6_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_6", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_7_fir() => TestTools.VerifyComputeGraph("CoreTester_7", "fir", true, TestDir);
        [TestMethod] public void CoreTester_7_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_7", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_8_fir() => TestTools.VerifyComputeGraph("CoreTester_8", "fir", true, TestDir);
        [TestMethod] public void CoreTester_8_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_8", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_9_fir() => TestTools.VerifyComputeGraph("CoreTester_9", "fir", true, TestDir);
        [TestMethod] public void CoreTester_9_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_9", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_10_fir() => TestTools.VerifyComputeGraph("CoreTester_10", "fir", true, TestDir);
        [TestMethod] public void CoreTester_10_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_10", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_11_fir() => TestTools.VerifyComputeGraph("CoreTester_11", "fir", true, TestDir);
        [TestMethod] public void CoreTester_11_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_11", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_12_fir() => TestTools.VerifyComputeGraph("CoreTester_12", "fir", true, TestDir);
        [TestMethod] public void CoreTester_12_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_12", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_13_fir() => TestTools.VerifyComputeGraph("CoreTester_13", "fir", true, TestDir);
        [TestMethod] public void CoreTester_13_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_13", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_14_fir() => TestTools.VerifyComputeGraph("CoreTester_14", "fir", true, TestDir);
        [TestMethod] public void CoreTester_14_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_14", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_15_fir() => TestTools.VerifyComputeGraph("CoreTester_15", "fir", true, TestDir);
        [TestMethod] public void CoreTester_15_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_15", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_16_fir() => TestTools.VerifyComputeGraph("CoreTester_16", "fir", true, TestDir);
        [TestMethod] public void CoreTester_16_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_16", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_17_fir() => TestTools.VerifyComputeGraph("CoreTester_17", "fir", true, TestDir);
        [TestMethod] public void CoreTester_17_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_17", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_18_fir() => TestTools.VerifyComputeGraph("CoreTester_18", "fir", true, TestDir);
        [TestMethod] public void CoreTester_18_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_18", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_19_fir() => TestTools.VerifyComputeGraph("CoreTester_19", "fir", true, TestDir);
        [TestMethod] public void CoreTester_19_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_19", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_20_fir() => TestTools.VerifyComputeGraph("CoreTester_20", "fir", true, TestDir);
        [TestMethod] public void CoreTester_20_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_20", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_21_fir() => TestTools.VerifyComputeGraph("CoreTester_21", "fir", true, TestDir);
        [TestMethod] public void CoreTester_21_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_21", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_22_fir() => TestTools.VerifyComputeGraph("CoreTester_22", "fir", true, TestDir);
        [TestMethod] public void CoreTester_22_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_22", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_23_fir() => TestTools.VerifyComputeGraph("CoreTester_23", "fir", true, TestDir);
        [TestMethod] public void CoreTester_23_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_23", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_24_fir() => TestTools.VerifyComputeGraph("CoreTester_24", "fir", true, TestDir);
        [TestMethod] public void CoreTester_24_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_24", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_25_fir() => TestTools.VerifyComputeGraph("CoreTester_25", "fir", true, TestDir);
        [TestMethod] public void CoreTester_25_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_25", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_26_fir() => TestTools.VerifyComputeGraph("CoreTester_26", "fir", true, TestDir);
        [TestMethod] public void CoreTester_26_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_26", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_27_fir() => TestTools.VerifyComputeGraph("CoreTester_27", "fir", true, TestDir);
        [TestMethod] public void CoreTester_27_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_27", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_28_fir() => TestTools.VerifyComputeGraph("CoreTester_28", "fir", true, TestDir);
        [TestMethod] public void CoreTester_28_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_28", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_29_fir() => TestTools.VerifyComputeGraph("CoreTester_29", "fir", true, TestDir);
        [TestMethod] public void CoreTester_29_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_29", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_30_fir() => TestTools.VerifyComputeGraph("CoreTester_30", "fir", true, TestDir);
        [TestMethod] public void CoreTester_30_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_30", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_31_fir() => TestTools.VerifyComputeGraph("CoreTester_31", "fir", true, TestDir);
        [TestMethod] public void CoreTester_31_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_31", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_32_fir() => TestTools.VerifyComputeGraph("CoreTester_32", "fir", true, TestDir);
        [TestMethod] public void CoreTester_32_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_32", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_33_fir() => TestTools.VerifyComputeGraph("CoreTester_33", "fir", true, TestDir);
        [TestMethod] public void CoreTester_33_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_33", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_34_fir() => TestTools.VerifyComputeGraph("CoreTester_34", "fir", true, TestDir);
        [TestMethod] public void CoreTester_34_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_34", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_35_fir() => TestTools.VerifyComputeGraph("CoreTester_35", "fir", true, TestDir);
        [TestMethod] public void CoreTester_35_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_35", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_36_fir() => TestTools.VerifyComputeGraph("CoreTester_36", "fir", true, TestDir);
        [TestMethod] public void CoreTester_36_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_36", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_37_fir() => TestTools.VerifyComputeGraph("CoreTester_37", "fir", true, TestDir);
        [TestMethod] public void CoreTester_37_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_37", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_38_fir() => TestTools.VerifyComputeGraph("CoreTester_38", "fir", true, TestDir);
        [TestMethod] public void CoreTester_38_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_38", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_39_fir() => TestTools.VerifyComputeGraph("CoreTester_39", "fir", true, TestDir);
        [TestMethod] public void CoreTester_39_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_39", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_40_fir() => TestTools.VerifyComputeGraph("CoreTester_40", "fir", true, TestDir);
        [TestMethod] public void CoreTester_40_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_40", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_41_fir() => TestTools.VerifyComputeGraph("CoreTester_41", "fir", true, TestDir);
        [TestMethod] public void CoreTester_41_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_41", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_42_fir() => TestTools.VerifyComputeGraph("CoreTester_42", "fir", true, TestDir);
        [TestMethod] public void CoreTester_42_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_42", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_43_fir() => TestTools.VerifyComputeGraph("CoreTester_43", "fir", true, TestDir);
        [TestMethod] public void CoreTester_43_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_43", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_44_fir() => TestTools.VerifyComputeGraph("CoreTester_44", "fir", true, TestDir);
        [TestMethod] public void CoreTester_44_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_44", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_45_fir() => TestTools.VerifyComputeGraph("CoreTester_45", "fir", true, TestDir);
        [TestMethod] public void CoreTester_45_lo_fir() => TestTools.VerifyComputeGraph("CoreTester_45", "lo.fir", true, TestDir);

        [TestMethod] public void CSRTester_fir() => TestTools.VerifyComputeGraph("CSRTester", "fir", true, TestDir);
        [TestMethod] public void CSRTester_lo_fir() => TestTools.VerifyComputeGraph("CSRTester", "lo.fir", true, TestDir);

        [TestMethod] public void DatapathTester_fir() => TestTools.VerifyComputeGraph("DatapathTester", "fir", true, TestDir);
        [TestMethod] public void DatapathTester_lo_fir() => TestTools.VerifyComputeGraph("DatapathTester", "lo.fir", true, TestDir);

        [TestMethod] public void DatapathTester_0_fir() => TestTools.VerifyComputeGraph("DatapathTester_0", "fir", true, TestDir);
        [TestMethod] public void DatapathTester_0_lo_fir() => TestTools.VerifyComputeGraph("DatapathTester_0", "lo.fir", true, TestDir);

        [TestMethod] public void ImmGenTester_fir() => TestTools.VerifyComputeGraph("ImmGenTester", "fir", true, TestDir);
        [TestMethod] public void ImmGenTester_lo_fir() => TestTools.VerifyComputeGraph("ImmGenTester", "lo.fir", true, TestDir);

        [TestMethod] public void ImmGenTester_0_fir() => TestTools.VerifyComputeGraph("ImmGenTester_0", "fir", true, TestDir);
        [TestMethod] public void ImmGenTester_0_lo_fir() => TestTools.VerifyComputeGraph("ImmGenTester_0", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_fir() => TestTools.VerifyComputeGraph("TileTester", "fir", true, TestDir);
        [TestMethod] public void TileTester_lo_fir() => TestTools.VerifyComputeGraph("TileTester", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_0_fir() => TestTools.VerifyComputeGraph("TileTester_0", "fir", true, TestDir);
        [TestMethod] public void TileTester_0_lo_fir() => TestTools.VerifyComputeGraph("TileTester_0", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_1_fir() => TestTools.VerifyComputeGraph("TileTester_1", "fir", true, TestDir);
        [TestMethod] public void TileTester_1_lo_fir() => TestTools.VerifyComputeGraph("TileTester_1", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_2_fir() => TestTools.VerifyComputeGraph("TileTester_2", "fir", true, TestDir);
        [TestMethod] public void TileTester_2_lo_fir() => TestTools.VerifyComputeGraph("TileTester_2", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_3_fir() => TestTools.VerifyComputeGraph("TileTester_3", "fir", true, TestDir);
        [TestMethod] public void TileTester_3_lo_fir() => TestTools.VerifyComputeGraph("TileTester_3", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_4_fir() => TestTools.VerifyComputeGraph("TileTester_4", "fir", true, TestDir);
        [TestMethod] public void TileTester_4_lo_fir() => TestTools.VerifyComputeGraph("TileTester_4", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_5_fir() => TestTools.VerifyComputeGraph("TileTester_5", "fir", true, TestDir);
        [TestMethod] public void TileTester_5_lo_fir() => TestTools.VerifyComputeGraph("TileTester_5", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_6_fir() => TestTools.VerifyComputeGraph("TileTester_6", "fir", true, TestDir);
        [TestMethod] public void TileTester_6_lo_fir() => TestTools.VerifyComputeGraph("TileTester_6", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_7_fir() => TestTools.VerifyComputeGraph("TileTester_7", "fir", true, TestDir);
        [TestMethod] public void TileTester_7_lo_fir() => TestTools.VerifyComputeGraph("TileTester_7", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_8_fir() => TestTools.VerifyComputeGraph("TileTester_8", "fir", true, TestDir);
        [TestMethod] public void TileTester_8_lo_fir() => TestTools.VerifyComputeGraph("TileTester_8", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_9_fir() => TestTools.VerifyComputeGraph("TileTester_9", "fir", true, TestDir);
        [TestMethod] public void TileTester_9_lo_fir() => TestTools.VerifyComputeGraph("TileTester_9", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_10_fir() => TestTools.VerifyComputeGraph("TileTester_10", "fir", true, TestDir);
        [TestMethod] public void TileTester_10_lo_fir() => TestTools.VerifyComputeGraph("TileTester_10", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_11_fir() => TestTools.VerifyComputeGraph("TileTester_11", "fir", true, TestDir);
        [TestMethod] public void TileTester_11_lo_fir() => TestTools.VerifyComputeGraph("TileTester_11", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_12_fir() => TestTools.VerifyComputeGraph("TileTester_12", "fir", true, TestDir);
        [TestMethod] public void TileTester_12_lo_fir() => TestTools.VerifyComputeGraph("TileTester_12", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_13_fir() => TestTools.VerifyComputeGraph("TileTester_13", "fir", true, TestDir);
        [TestMethod] public void TileTester_13_lo_fir() => TestTools.VerifyComputeGraph("TileTester_13", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_14_fir() => TestTools.VerifyComputeGraph("TileTester_14", "fir", true, TestDir);
        [TestMethod] public void TileTester_14_lo_fir() => TestTools.VerifyComputeGraph("TileTester_14", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_15_fir() => TestTools.VerifyComputeGraph("TileTester_15", "fir", true, TestDir);
        [TestMethod] public void TileTester_15_lo_fir() => TestTools.VerifyComputeGraph("TileTester_15", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_16_fir() => TestTools.VerifyComputeGraph("TileTester_16", "fir", true, TestDir);
        [TestMethod] public void TileTester_16_lo_fir() => TestTools.VerifyComputeGraph("TileTester_16", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_17_fir() => TestTools.VerifyComputeGraph("TileTester_17", "fir", true, TestDir);
        [TestMethod] public void TileTester_17_lo_fir() => TestTools.VerifyComputeGraph("TileTester_17", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_18_fir() => TestTools.VerifyComputeGraph("TileTester_18", "fir", true, TestDir);
        [TestMethod] public void TileTester_18_lo_fir() => TestTools.VerifyComputeGraph("TileTester_18", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_19_fir() => TestTools.VerifyComputeGraph("TileTester_19", "fir", true, TestDir);
        [TestMethod] public void TileTester_19_lo_fir() => TestTools.VerifyComputeGraph("TileTester_19", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_20_fir() => TestTools.VerifyComputeGraph("TileTester_20", "fir", true, TestDir);
        [TestMethod] public void TileTester_20_lo_fir() => TestTools.VerifyComputeGraph("TileTester_20", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_21_fir() => TestTools.VerifyComputeGraph("TileTester_21", "fir", true, TestDir);
        [TestMethod] public void TileTester_21_lo_fir() => TestTools.VerifyComputeGraph("TileTester_21", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_22_fir() => TestTools.VerifyComputeGraph("TileTester_22", "fir", true, TestDir);
        [TestMethod] public void TileTester_22_lo_fir() => TestTools.VerifyComputeGraph("TileTester_22", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_23_fir() => TestTools.VerifyComputeGraph("TileTester_23", "fir", true, TestDir);
        [TestMethod] public void TileTester_23_lo_fir() => TestTools.VerifyComputeGraph("TileTester_23", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_24_fir() => TestTools.VerifyComputeGraph("TileTester_24", "fir", true, TestDir);
        [TestMethod] public void TileTester_24_lo_fir() => TestTools.VerifyComputeGraph("TileTester_24", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_25_fir() => TestTools.VerifyComputeGraph("TileTester_25", "fir", true, TestDir);
        [TestMethod] public void TileTester_25_lo_fir() => TestTools.VerifyComputeGraph("TileTester_25", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_26_fir() => TestTools.VerifyComputeGraph("TileTester_26", "fir", true, TestDir);
        [TestMethod] public void TileTester_26_lo_fir() => TestTools.VerifyComputeGraph("TileTester_26", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_27_fir() => TestTools.VerifyComputeGraph("TileTester_27", "fir", true, TestDir);
        [TestMethod] public void TileTester_27_lo_fir() => TestTools.VerifyComputeGraph("TileTester_27", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_28_fir() => TestTools.VerifyComputeGraph("TileTester_28", "fir", true, TestDir);
        [TestMethod] public void TileTester_28_lo_fir() => TestTools.VerifyComputeGraph("TileTester_28", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_29_fir() => TestTools.VerifyComputeGraph("TileTester_29", "fir", true, TestDir);
        [TestMethod] public void TileTester_29_lo_fir() => TestTools.VerifyComputeGraph("TileTester_29", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_30_fir() => TestTools.VerifyComputeGraph("TileTester_30", "fir", true, TestDir);
        [TestMethod] public void TileTester_30_lo_fir() => TestTools.VerifyComputeGraph("TileTester_30", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_31_fir() => TestTools.VerifyComputeGraph("TileTester_31", "fir", true, TestDir);
        [TestMethod] public void TileTester_31_lo_fir() => TestTools.VerifyComputeGraph("TileTester_31", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_32_fir() => TestTools.VerifyComputeGraph("TileTester_32", "fir", true, TestDir);
        [TestMethod] public void TileTester_32_lo_fir() => TestTools.VerifyComputeGraph("TileTester_32", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_33_fir() => TestTools.VerifyComputeGraph("TileTester_33", "fir", true, TestDir);
        [TestMethod] public void TileTester_33_lo_fir() => TestTools.VerifyComputeGraph("TileTester_33", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_34_fir() => TestTools.VerifyComputeGraph("TileTester_34", "fir", true, TestDir);
        [TestMethod] public void TileTester_34_lo_fir() => TestTools.VerifyComputeGraph("TileTester_34", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_35_fir() => TestTools.VerifyComputeGraph("TileTester_35", "fir", true, TestDir);
        [TestMethod] public void TileTester_35_lo_fir() => TestTools.VerifyComputeGraph("TileTester_35", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_36_fir() => TestTools.VerifyComputeGraph("TileTester_36", "fir", true, TestDir);
        [TestMethod] public void TileTester_36_lo_fir() => TestTools.VerifyComputeGraph("TileTester_36", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_37_fir() => TestTools.VerifyComputeGraph("TileTester_37", "fir", true, TestDir);
        [TestMethod] public void TileTester_37_lo_fir() => TestTools.VerifyComputeGraph("TileTester_37", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_38_fir() => TestTools.VerifyComputeGraph("TileTester_38", "fir", true, TestDir);
        [TestMethod] public void TileTester_38_lo_fir() => TestTools.VerifyComputeGraph("TileTester_38", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_39_fir() => TestTools.VerifyComputeGraph("TileTester_39", "fir", true, TestDir);
        [TestMethod] public void TileTester_39_lo_fir() => TestTools.VerifyComputeGraph("TileTester_39", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_40_fir() => TestTools.VerifyComputeGraph("TileTester_40", "fir", true, TestDir);
        [TestMethod] public void TileTester_40_lo_fir() => TestTools.VerifyComputeGraph("TileTester_40", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_41_fir() => TestTools.VerifyComputeGraph("TileTester_41", "fir", true, TestDir);
        [TestMethod] public void TileTester_41_lo_fir() => TestTools.VerifyComputeGraph("TileTester_41", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_42_fir() => TestTools.VerifyComputeGraph("TileTester_42", "fir", true, TestDir);
        [TestMethod] public void TileTester_42_lo_fir() => TestTools.VerifyComputeGraph("TileTester_42", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_43_fir() => TestTools.VerifyComputeGraph("TileTester_43", "fir", true, TestDir);
        [TestMethod] public void TileTester_43_lo_fir() => TestTools.VerifyComputeGraph("TileTester_43", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_44_fir() => TestTools.VerifyComputeGraph("TileTester_44", "fir", true, TestDir);
        [TestMethod] public void TileTester_44_lo_fir() => TestTools.VerifyComputeGraph("TileTester_44", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_45_fir() => TestTools.VerifyComputeGraph("TileTester_45", "fir", true, TestDir);
        [TestMethod] public void TileTester_45_lo_fir() => TestTools.VerifyComputeGraph("TileTester_45", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_46_fir() => TestTools.VerifyComputeGraph("TileTester_46", "fir", true, TestDir);
        [TestMethod] public void TileTester_46_lo_fir() => TestTools.VerifyComputeGraph("TileTester_46", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_47_fir() => TestTools.VerifyComputeGraph("TileTester_47", "fir", true, TestDir);
        [TestMethod] public void TileTester_47_lo_fir() => TestTools.VerifyComputeGraph("TileTester_47", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_48_fir() => TestTools.VerifyComputeGraph("TileTester_48", "fir", true, TestDir);
        [TestMethod] public void TileTester_48_lo_fir() => TestTools.VerifyComputeGraph("TileTester_48", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_49_fir() => TestTools.VerifyComputeGraph("TileTester_49", "fir", true, TestDir);
        [TestMethod] public void TileTester_49_lo_fir() => TestTools.VerifyComputeGraph("TileTester_49", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_50_fir() => TestTools.VerifyComputeGraph("TileTester_50", "fir", true, TestDir);
        [TestMethod] public void TileTester_50_lo_fir() => TestTools.VerifyComputeGraph("TileTester_50", "lo.fir", true, TestDir);


    }
}
