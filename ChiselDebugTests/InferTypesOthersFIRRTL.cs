using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebugTests
{
    [TestClass]
    public class InferTypesOthersFIRRTL
    {
        const string TestDir = @"...";

        [TestMethod] public void AxonSystem_fir() => TestTools.VerifyInferTypes("AxonSystem", "fir", false, TestDir);
        [TestMethod] public void AxonSystem_lo_fir() => TestTools.VerifyInferTypes("AxonSystem", "lo.fir", false, TestDir);

        [TestMethod] public void BusArbiter_fir() => TestTools.VerifyInferTypes("BusArbiter", "fir", false, TestDir);
        [TestMethod] public void BusArbiter_lo_fir() => TestTools.VerifyInferTypes("BusArbiter", "lo.fir", false, TestDir);

        [TestMethod] public void BusInterface_fir() => TestTools.VerifyInferTypes("BusInterface", "fir", false, TestDir);
        [TestMethod] public void BusInterface_lo_fir() => TestTools.VerifyInferTypes("BusInterface", "lo.fir", false, TestDir);

        [TestMethod] public void InputCore_fir() => TestTools.VerifyInferTypes("InputCore", "fir", false, TestDir);
        [TestMethod] public void InputCore_lo_fir() => TestTools.VerifyInferTypes("InputCore", "lo.fir", false, TestDir);

        [TestMethod] public void NeuronEvaluator_fir() => TestTools.VerifyInferTypes("NeuronEvaluator", "fir", false, TestDir);
        [TestMethod] public void NeuronEvaluator_lo_fir() => TestTools.VerifyInferTypes("NeuronEvaluator", "lo.fir", false, TestDir);

        [TestMethod] public void OffChipCom_fir() => TestTools.VerifyInferTypes("OffChipCom", "fir", false, TestDir);
        [TestMethod] public void OffChipCom_lo_fir() => TestTools.VerifyInferTypes("OffChipCom", "lo.fir", false, TestDir);

        [TestMethod] public void PriorityMaskRstEncoder_fir() => TestTools.VerifyInferTypes("PriorityMaskRstEncoder", "fir", false, TestDir);
        [TestMethod] public void PriorityMaskRstEncoder_lo_fir() => TestTools.VerifyInferTypes("PriorityMaskRstEncoder", "lo.fir", false, TestDir);

        [TestMethod] public void UartEcho_fir() => TestTools.VerifyInferTypes("UartEcho", "fir", false, TestDir);
        [TestMethod] public void UartEcho_lo_fir() => TestTools.VerifyInferTypes("UartEcho", "lo.fir", false, TestDir);

        [TestMethod] public void ALUTester_fir() => TestTools.VerifyInferTypes("ALUTester", "fir", true, TestDir);
        [TestMethod] public void ALUTester_lo_fir() => TestTools.VerifyInferTypes("ALUTester", "lo.fir", true, TestDir);

        [TestMethod] public void ALUTester_0_fir() => TestTools.VerifyInferTypes("ALUTester_0", "fir", true, TestDir);
        [TestMethod] public void ALUTester_0_lo_fir() => TestTools.VerifyInferTypes("ALUTester_0", "lo.fir", true, TestDir);

        [TestMethod] public void BrCondTester_fir() => TestTools.VerifyInferTypes("BrCondTester", "fir", true, TestDir);
        [TestMethod] public void BrCondTester_lo_fir() => TestTools.VerifyInferTypes("BrCondTester", "lo.fir", true, TestDir);

        [TestMethod] public void BrCondTester_0_fir() => TestTools.VerifyInferTypes("BrCondTester_0", "fir", true, TestDir);
        [TestMethod] public void BrCondTester_0_lo_fir() => TestTools.VerifyInferTypes("BrCondTester_0", "lo.fir", true, TestDir);

        [TestMethod] public void CacheTester_fir() => TestTools.VerifyInferTypes("CacheTester", "fir", true, TestDir);
        [TestMethod] public void CacheTester_lo_fir() => TestTools.VerifyInferTypes("CacheTester", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_fir() => TestTools.VerifyInferTypes("CoreTester", "fir", true, TestDir);
        [TestMethod] public void CoreTester_lo_fir() => TestTools.VerifyInferTypes("CoreTester", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_0_fir() => TestTools.VerifyInferTypes("CoreTester_0", "fir", true, TestDir);
        [TestMethod] public void CoreTester_0_lo_fir() => TestTools.VerifyInferTypes("CoreTester_0", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_1_fir() => TestTools.VerifyInferTypes("CoreTester_1", "fir", true, TestDir);
        [TestMethod] public void CoreTester_1_lo_fir() => TestTools.VerifyInferTypes("CoreTester_1", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_2_fir() => TestTools.VerifyInferTypes("CoreTester_2", "fir", true, TestDir);
        [TestMethod] public void CoreTester_2_lo_fir() => TestTools.VerifyInferTypes("CoreTester_2", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_3_fir() => TestTools.VerifyInferTypes("CoreTester_3", "fir", true, TestDir);
        [TestMethod] public void CoreTester_3_lo_fir() => TestTools.VerifyInferTypes("CoreTester_3", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_4_fir() => TestTools.VerifyInferTypes("CoreTester_4", "fir", true, TestDir);
        [TestMethod] public void CoreTester_4_lo_fir() => TestTools.VerifyInferTypes("CoreTester_4", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_5_fir() => TestTools.VerifyInferTypes("CoreTester_5", "fir", true, TestDir);
        [TestMethod] public void CoreTester_5_lo_fir() => TestTools.VerifyInferTypes("CoreTester_5", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_6_fir() => TestTools.VerifyInferTypes("CoreTester_6", "fir", true, TestDir);
        [TestMethod] public void CoreTester_6_lo_fir() => TestTools.VerifyInferTypes("CoreTester_6", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_7_fir() => TestTools.VerifyInferTypes("CoreTester_7", "fir", true, TestDir);
        [TestMethod] public void CoreTester_7_lo_fir() => TestTools.VerifyInferTypes("CoreTester_7", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_8_fir() => TestTools.VerifyInferTypes("CoreTester_8", "fir", true, TestDir);
        [TestMethod] public void CoreTester_8_lo_fir() => TestTools.VerifyInferTypes("CoreTester_8", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_9_fir() => TestTools.VerifyInferTypes("CoreTester_9", "fir", true, TestDir);
        [TestMethod] public void CoreTester_9_lo_fir() => TestTools.VerifyInferTypes("CoreTester_9", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_10_fir() => TestTools.VerifyInferTypes("CoreTester_10", "fir", true, TestDir);
        [TestMethod] public void CoreTester_10_lo_fir() => TestTools.VerifyInferTypes("CoreTester_10", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_11_fir() => TestTools.VerifyInferTypes("CoreTester_11", "fir", true, TestDir);
        [TestMethod] public void CoreTester_11_lo_fir() => TestTools.VerifyInferTypes("CoreTester_11", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_12_fir() => TestTools.VerifyInferTypes("CoreTester_12", "fir", true, TestDir);
        [TestMethod] public void CoreTester_12_lo_fir() => TestTools.VerifyInferTypes("CoreTester_12", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_13_fir() => TestTools.VerifyInferTypes("CoreTester_13", "fir", true, TestDir);
        [TestMethod] public void CoreTester_13_lo_fir() => TestTools.VerifyInferTypes("CoreTester_13", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_14_fir() => TestTools.VerifyInferTypes("CoreTester_14", "fir", true, TestDir);
        [TestMethod] public void CoreTester_14_lo_fir() => TestTools.VerifyInferTypes("CoreTester_14", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_15_fir() => TestTools.VerifyInferTypes("CoreTester_15", "fir", true, TestDir);
        [TestMethod] public void CoreTester_15_lo_fir() => TestTools.VerifyInferTypes("CoreTester_15", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_16_fir() => TestTools.VerifyInferTypes("CoreTester_16", "fir", true, TestDir);
        [TestMethod] public void CoreTester_16_lo_fir() => TestTools.VerifyInferTypes("CoreTester_16", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_17_fir() => TestTools.VerifyInferTypes("CoreTester_17", "fir", true, TestDir);
        [TestMethod] public void CoreTester_17_lo_fir() => TestTools.VerifyInferTypes("CoreTester_17", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_18_fir() => TestTools.VerifyInferTypes("CoreTester_18", "fir", true, TestDir);
        [TestMethod] public void CoreTester_18_lo_fir() => TestTools.VerifyInferTypes("CoreTester_18", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_19_fir() => TestTools.VerifyInferTypes("CoreTester_19", "fir", true, TestDir);
        [TestMethod] public void CoreTester_19_lo_fir() => TestTools.VerifyInferTypes("CoreTester_19", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_20_fir() => TestTools.VerifyInferTypes("CoreTester_20", "fir", true, TestDir);
        [TestMethod] public void CoreTester_20_lo_fir() => TestTools.VerifyInferTypes("CoreTester_20", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_21_fir() => TestTools.VerifyInferTypes("CoreTester_21", "fir", true, TestDir);
        [TestMethod] public void CoreTester_21_lo_fir() => TestTools.VerifyInferTypes("CoreTester_21", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_22_fir() => TestTools.VerifyInferTypes("CoreTester_22", "fir", true, TestDir);
        [TestMethod] public void CoreTester_22_lo_fir() => TestTools.VerifyInferTypes("CoreTester_22", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_23_fir() => TestTools.VerifyInferTypes("CoreTester_23", "fir", true, TestDir);
        [TestMethod] public void CoreTester_23_lo_fir() => TestTools.VerifyInferTypes("CoreTester_23", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_24_fir() => TestTools.VerifyInferTypes("CoreTester_24", "fir", true, TestDir);
        [TestMethod] public void CoreTester_24_lo_fir() => TestTools.VerifyInferTypes("CoreTester_24", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_25_fir() => TestTools.VerifyInferTypes("CoreTester_25", "fir", true, TestDir);
        [TestMethod] public void CoreTester_25_lo_fir() => TestTools.VerifyInferTypes("CoreTester_25", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_26_fir() => TestTools.VerifyInferTypes("CoreTester_26", "fir", true, TestDir);
        [TestMethod] public void CoreTester_26_lo_fir() => TestTools.VerifyInferTypes("CoreTester_26", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_27_fir() => TestTools.VerifyInferTypes("CoreTester_27", "fir", true, TestDir);
        [TestMethod] public void CoreTester_27_lo_fir() => TestTools.VerifyInferTypes("CoreTester_27", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_28_fir() => TestTools.VerifyInferTypes("CoreTester_28", "fir", true, TestDir);
        [TestMethod] public void CoreTester_28_lo_fir() => TestTools.VerifyInferTypes("CoreTester_28", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_29_fir() => TestTools.VerifyInferTypes("CoreTester_29", "fir", true, TestDir);
        [TestMethod] public void CoreTester_29_lo_fir() => TestTools.VerifyInferTypes("CoreTester_29", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_30_fir() => TestTools.VerifyInferTypes("CoreTester_30", "fir", true, TestDir);
        [TestMethod] public void CoreTester_30_lo_fir() => TestTools.VerifyInferTypes("CoreTester_30", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_31_fir() => TestTools.VerifyInferTypes("CoreTester_31", "fir", true, TestDir);
        [TestMethod] public void CoreTester_31_lo_fir() => TestTools.VerifyInferTypes("CoreTester_31", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_32_fir() => TestTools.VerifyInferTypes("CoreTester_32", "fir", true, TestDir);
        [TestMethod] public void CoreTester_32_lo_fir() => TestTools.VerifyInferTypes("CoreTester_32", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_33_fir() => TestTools.VerifyInferTypes("CoreTester_33", "fir", true, TestDir);
        [TestMethod] public void CoreTester_33_lo_fir() => TestTools.VerifyInferTypes("CoreTester_33", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_34_fir() => TestTools.VerifyInferTypes("CoreTester_34", "fir", true, TestDir);
        [TestMethod] public void CoreTester_34_lo_fir() => TestTools.VerifyInferTypes("CoreTester_34", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_35_fir() => TestTools.VerifyInferTypes("CoreTester_35", "fir", true, TestDir);
        [TestMethod] public void CoreTester_35_lo_fir() => TestTools.VerifyInferTypes("CoreTester_35", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_36_fir() => TestTools.VerifyInferTypes("CoreTester_36", "fir", true, TestDir);
        [TestMethod] public void CoreTester_36_lo_fir() => TestTools.VerifyInferTypes("CoreTester_36", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_37_fir() => TestTools.VerifyInferTypes("CoreTester_37", "fir", true, TestDir);
        [TestMethod] public void CoreTester_37_lo_fir() => TestTools.VerifyInferTypes("CoreTester_37", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_38_fir() => TestTools.VerifyInferTypes("CoreTester_38", "fir", true, TestDir);
        [TestMethod] public void CoreTester_38_lo_fir() => TestTools.VerifyInferTypes("CoreTester_38", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_39_fir() => TestTools.VerifyInferTypes("CoreTester_39", "fir", true, TestDir);
        [TestMethod] public void CoreTester_39_lo_fir() => TestTools.VerifyInferTypes("CoreTester_39", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_40_fir() => TestTools.VerifyInferTypes("CoreTester_40", "fir", true, TestDir);
        [TestMethod] public void CoreTester_40_lo_fir() => TestTools.VerifyInferTypes("CoreTester_40", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_41_fir() => TestTools.VerifyInferTypes("CoreTester_41", "fir", true, TestDir);
        [TestMethod] public void CoreTester_41_lo_fir() => TestTools.VerifyInferTypes("CoreTester_41", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_42_fir() => TestTools.VerifyInferTypes("CoreTester_42", "fir", true, TestDir);
        [TestMethod] public void CoreTester_42_lo_fir() => TestTools.VerifyInferTypes("CoreTester_42", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_43_fir() => TestTools.VerifyInferTypes("CoreTester_43", "fir", true, TestDir);
        [TestMethod] public void CoreTester_43_lo_fir() => TestTools.VerifyInferTypes("CoreTester_43", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_44_fir() => TestTools.VerifyInferTypes("CoreTester_44", "fir", true, TestDir);
        [TestMethod] public void CoreTester_44_lo_fir() => TestTools.VerifyInferTypes("CoreTester_44", "lo.fir", true, TestDir);

        [TestMethod] public void CoreTester_45_fir() => TestTools.VerifyInferTypes("CoreTester_45", "fir", true, TestDir);
        [TestMethod] public void CoreTester_45_lo_fir() => TestTools.VerifyInferTypes("CoreTester_45", "lo.fir", true, TestDir);

        [TestMethod] public void CSRTester_fir() => TestTools.VerifyInferTypes("CSRTester", "fir", true, TestDir);
        [TestMethod] public void CSRTester_lo_fir() => TestTools.VerifyInferTypes("CSRTester", "lo.fir", true, TestDir);

        [TestMethod] public void DatapathTester_fir() => TestTools.VerifyInferTypes("DatapathTester", "fir", true, TestDir);
        [TestMethod] public void DatapathTester_lo_fir() => TestTools.VerifyInferTypes("DatapathTester", "lo.fir", true, TestDir);

        [TestMethod] public void DatapathTester_0_fir() => TestTools.VerifyInferTypes("DatapathTester_0", "fir", true, TestDir);
        [TestMethod] public void DatapathTester_0_lo_fir() => TestTools.VerifyInferTypes("DatapathTester_0", "lo.fir", true, TestDir);

        [TestMethod] public void ImmGenTester_fir() => TestTools.VerifyInferTypes("ImmGenTester", "fir", true, TestDir);
        [TestMethod] public void ImmGenTester_lo_fir() => TestTools.VerifyInferTypes("ImmGenTester", "lo.fir", true, TestDir);

        [TestMethod] public void ImmGenTester_0_fir() => TestTools.VerifyInferTypes("ImmGenTester_0", "fir", true, TestDir);
        [TestMethod] public void ImmGenTester_0_lo_fir() => TestTools.VerifyInferTypes("ImmGenTester_0", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_fir() => TestTools.VerifyInferTypes("TileTester", "fir", true, TestDir);
        [TestMethod] public void TileTester_lo_fir() => TestTools.VerifyInferTypes("TileTester", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_0_fir() => TestTools.VerifyInferTypes("TileTester_0", "fir", true, TestDir);
        [TestMethod] public void TileTester_0_lo_fir() => TestTools.VerifyInferTypes("TileTester_0", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_1_fir() => TestTools.VerifyInferTypes("TileTester_1", "fir", true, TestDir);
        [TestMethod] public void TileTester_1_lo_fir() => TestTools.VerifyInferTypes("TileTester_1", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_2_fir() => TestTools.VerifyInferTypes("TileTester_2", "fir", true, TestDir);
        [TestMethod] public void TileTester_2_lo_fir() => TestTools.VerifyInferTypes("TileTester_2", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_3_fir() => TestTools.VerifyInferTypes("TileTester_3", "fir", true, TestDir);
        [TestMethod] public void TileTester_3_lo_fir() => TestTools.VerifyInferTypes("TileTester_3", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_4_fir() => TestTools.VerifyInferTypes("TileTester_4", "fir", true, TestDir);
        [TestMethod] public void TileTester_4_lo_fir() => TestTools.VerifyInferTypes("TileTester_4", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_5_fir() => TestTools.VerifyInferTypes("TileTester_5", "fir", true, TestDir);
        [TestMethod] public void TileTester_5_lo_fir() => TestTools.VerifyInferTypes("TileTester_5", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_6_fir() => TestTools.VerifyInferTypes("TileTester_6", "fir", true, TestDir);
        [TestMethod] public void TileTester_6_lo_fir() => TestTools.VerifyInferTypes("TileTester_6", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_7_fir() => TestTools.VerifyInferTypes("TileTester_7", "fir", true, TestDir);
        [TestMethod] public void TileTester_7_lo_fir() => TestTools.VerifyInferTypes("TileTester_7", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_8_fir() => TestTools.VerifyInferTypes("TileTester_8", "fir", true, TestDir);
        [TestMethod] public void TileTester_8_lo_fir() => TestTools.VerifyInferTypes("TileTester_8", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_9_fir() => TestTools.VerifyInferTypes("TileTester_9", "fir", true, TestDir);
        [TestMethod] public void TileTester_9_lo_fir() => TestTools.VerifyInferTypes("TileTester_9", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_10_fir() => TestTools.VerifyInferTypes("TileTester_10", "fir", true, TestDir);
        [TestMethod] public void TileTester_10_lo_fir() => TestTools.VerifyInferTypes("TileTester_10", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_11_fir() => TestTools.VerifyInferTypes("TileTester_11", "fir", true, TestDir);
        [TestMethod] public void TileTester_11_lo_fir() => TestTools.VerifyInferTypes("TileTester_11", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_12_fir() => TestTools.VerifyInferTypes("TileTester_12", "fir", true, TestDir);
        [TestMethod] public void TileTester_12_lo_fir() => TestTools.VerifyInferTypes("TileTester_12", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_13_fir() => TestTools.VerifyInferTypes("TileTester_13", "fir", true, TestDir);
        [TestMethod] public void TileTester_13_lo_fir() => TestTools.VerifyInferTypes("TileTester_13", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_14_fir() => TestTools.VerifyInferTypes("TileTester_14", "fir", true, TestDir);
        [TestMethod] public void TileTester_14_lo_fir() => TestTools.VerifyInferTypes("TileTester_14", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_15_fir() => TestTools.VerifyInferTypes("TileTester_15", "fir", true, TestDir);
        [TestMethod] public void TileTester_15_lo_fir() => TestTools.VerifyInferTypes("TileTester_15", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_16_fir() => TestTools.VerifyInferTypes("TileTester_16", "fir", true, TestDir);
        [TestMethod] public void TileTester_16_lo_fir() => TestTools.VerifyInferTypes("TileTester_16", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_17_fir() => TestTools.VerifyInferTypes("TileTester_17", "fir", true, TestDir);
        [TestMethod] public void TileTester_17_lo_fir() => TestTools.VerifyInferTypes("TileTester_17", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_18_fir() => TestTools.VerifyInferTypes("TileTester_18", "fir", true, TestDir);
        [TestMethod] public void TileTester_18_lo_fir() => TestTools.VerifyInferTypes("TileTester_18", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_19_fir() => TestTools.VerifyInferTypes("TileTester_19", "fir", true, TestDir);
        [TestMethod] public void TileTester_19_lo_fir() => TestTools.VerifyInferTypes("TileTester_19", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_20_fir() => TestTools.VerifyInferTypes("TileTester_20", "fir", true, TestDir);
        [TestMethod] public void TileTester_20_lo_fir() => TestTools.VerifyInferTypes("TileTester_20", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_21_fir() => TestTools.VerifyInferTypes("TileTester_21", "fir", true, TestDir);
        [TestMethod] public void TileTester_21_lo_fir() => TestTools.VerifyInferTypes("TileTester_21", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_22_fir() => TestTools.VerifyInferTypes("TileTester_22", "fir", true, TestDir);
        [TestMethod] public void TileTester_22_lo_fir() => TestTools.VerifyInferTypes("TileTester_22", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_23_fir() => TestTools.VerifyInferTypes("TileTester_23", "fir", true, TestDir);
        [TestMethod] public void TileTester_23_lo_fir() => TestTools.VerifyInferTypes("TileTester_23", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_24_fir() => TestTools.VerifyInferTypes("TileTester_24", "fir", true, TestDir);
        [TestMethod] public void TileTester_24_lo_fir() => TestTools.VerifyInferTypes("TileTester_24", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_25_fir() => TestTools.VerifyInferTypes("TileTester_25", "fir", true, TestDir);
        [TestMethod] public void TileTester_25_lo_fir() => TestTools.VerifyInferTypes("TileTester_25", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_26_fir() => TestTools.VerifyInferTypes("TileTester_26", "fir", true, TestDir);
        [TestMethod] public void TileTester_26_lo_fir() => TestTools.VerifyInferTypes("TileTester_26", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_27_fir() => TestTools.VerifyInferTypes("TileTester_27", "fir", true, TestDir);
        [TestMethod] public void TileTester_27_lo_fir() => TestTools.VerifyInferTypes("TileTester_27", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_28_fir() => TestTools.VerifyInferTypes("TileTester_28", "fir", true, TestDir);
        [TestMethod] public void TileTester_28_lo_fir() => TestTools.VerifyInferTypes("TileTester_28", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_29_fir() => TestTools.VerifyInferTypes("TileTester_29", "fir", true, TestDir);
        [TestMethod] public void TileTester_29_lo_fir() => TestTools.VerifyInferTypes("TileTester_29", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_30_fir() => TestTools.VerifyInferTypes("TileTester_30", "fir", true, TestDir);
        [TestMethod] public void TileTester_30_lo_fir() => TestTools.VerifyInferTypes("TileTester_30", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_31_fir() => TestTools.VerifyInferTypes("TileTester_31", "fir", true, TestDir);
        [TestMethod] public void TileTester_31_lo_fir() => TestTools.VerifyInferTypes("TileTester_31", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_32_fir() => TestTools.VerifyInferTypes("TileTester_32", "fir", true, TestDir);
        [TestMethod] public void TileTester_32_lo_fir() => TestTools.VerifyInferTypes("TileTester_32", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_33_fir() => TestTools.VerifyInferTypes("TileTester_33", "fir", true, TestDir);
        [TestMethod] public void TileTester_33_lo_fir() => TestTools.VerifyInferTypes("TileTester_33", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_34_fir() => TestTools.VerifyInferTypes("TileTester_34", "fir", true, TestDir);
        [TestMethod] public void TileTester_34_lo_fir() => TestTools.VerifyInferTypes("TileTester_34", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_35_fir() => TestTools.VerifyInferTypes("TileTester_35", "fir", true, TestDir);
        [TestMethod] public void TileTester_35_lo_fir() => TestTools.VerifyInferTypes("TileTester_35", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_36_fir() => TestTools.VerifyInferTypes("TileTester_36", "fir", true, TestDir);
        [TestMethod] public void TileTester_36_lo_fir() => TestTools.VerifyInferTypes("TileTester_36", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_37_fir() => TestTools.VerifyInferTypes("TileTester_37", "fir", true, TestDir);
        [TestMethod] public void TileTester_37_lo_fir() => TestTools.VerifyInferTypes("TileTester_37", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_38_fir() => TestTools.VerifyInferTypes("TileTester_38", "fir", true, TestDir);
        [TestMethod] public void TileTester_38_lo_fir() => TestTools.VerifyInferTypes("TileTester_38", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_39_fir() => TestTools.VerifyInferTypes("TileTester_39", "fir", true, TestDir);
        [TestMethod] public void TileTester_39_lo_fir() => TestTools.VerifyInferTypes("TileTester_39", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_40_fir() => TestTools.VerifyInferTypes("TileTester_40", "fir", true, TestDir);
        [TestMethod] public void TileTester_40_lo_fir() => TestTools.VerifyInferTypes("TileTester_40", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_41_fir() => TestTools.VerifyInferTypes("TileTester_41", "fir", true, TestDir);
        [TestMethod] public void TileTester_41_lo_fir() => TestTools.VerifyInferTypes("TileTester_41", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_42_fir() => TestTools.VerifyInferTypes("TileTester_42", "fir", true, TestDir);
        [TestMethod] public void TileTester_42_lo_fir() => TestTools.VerifyInferTypes("TileTester_42", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_43_fir() => TestTools.VerifyInferTypes("TileTester_43", "fir", true, TestDir);
        [TestMethod] public void TileTester_43_lo_fir() => TestTools.VerifyInferTypes("TileTester_43", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_44_fir() => TestTools.VerifyInferTypes("TileTester_44", "fir", true, TestDir);
        [TestMethod] public void TileTester_44_lo_fir() => TestTools.VerifyInferTypes("TileTester_44", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_45_fir() => TestTools.VerifyInferTypes("TileTester_45", "fir", true, TestDir);
        [TestMethod] public void TileTester_45_lo_fir() => TestTools.VerifyInferTypes("TileTester_45", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_46_fir() => TestTools.VerifyInferTypes("TileTester_46", "fir", true, TestDir);
        [TestMethod] public void TileTester_46_lo_fir() => TestTools.VerifyInferTypes("TileTester_46", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_47_fir() => TestTools.VerifyInferTypes("TileTester_47", "fir", true, TestDir);
        [TestMethod] public void TileTester_47_lo_fir() => TestTools.VerifyInferTypes("TileTester_47", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_48_fir() => TestTools.VerifyInferTypes("TileTester_48", "fir", true, TestDir);
        [TestMethod] public void TileTester_48_lo_fir() => TestTools.VerifyInferTypes("TileTester_48", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_49_fir() => TestTools.VerifyInferTypes("TileTester_49", "fir", true, TestDir);
        [TestMethod] public void TileTester_49_lo_fir() => TestTools.VerifyInferTypes("TileTester_49", "lo.fir", true, TestDir);

        [TestMethod] public void TileTester_50_fir() => TestTools.VerifyInferTypes("TileTester_50", "fir", true, TestDir);
        [TestMethod] public void TileTester_50_lo_fir() => TestTools.VerifyInferTypes("TileTester_50", "lo.fir", true, TestDir);


    }
}
