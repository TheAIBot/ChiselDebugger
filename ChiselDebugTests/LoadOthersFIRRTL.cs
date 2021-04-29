using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebugTests
{
    [TestClass]
    public class LoadOthersFIRRTL
    {
        const string TestDir = "../../../../TestGenerator/OthersFIRRTL";

        [TestMethod] public void AxonSystem_fir() => TestTools.VerifyMakeGraph("AxonSystem", "fir", TestDir);
        [TestMethod] public void AxonSystem_lo_fir() => TestTools.VerifyMakeGraph("AxonSystem", "lo.fir", TestDir);

        [TestMethod] public void BusArbiter_fir() => TestTools.VerifyMakeGraph("BusArbiter", "fir", TestDir);
        [TestMethod] public void BusArbiter_lo_fir() => TestTools.VerifyMakeGraph("BusArbiter", "lo.fir", TestDir);

        [TestMethod] public void BusInterface_fir() => TestTools.VerifyMakeGraph("BusInterface", "fir", TestDir);
        [TestMethod] public void BusInterface_lo_fir() => TestTools.VerifyMakeGraph("BusInterface", "lo.fir", TestDir);

        [TestMethod] public void InputCore_fir() => TestTools.VerifyMakeGraph("InputCore", "fir", TestDir);
        [TestMethod] public void InputCore_lo_fir() => TestTools.VerifyMakeGraph("InputCore", "lo.fir", TestDir);

        [TestMethod] public void NeuronEvaluator_fir() => TestTools.VerifyMakeGraph("NeuronEvaluator", "fir", TestDir);
        [TestMethod] public void NeuronEvaluator_lo_fir() => TestTools.VerifyMakeGraph("NeuronEvaluator", "lo.fir", TestDir);

        [TestMethod] public void OffChipCom_fir() => TestTools.VerifyMakeGraph("OffChipCom", "fir", TestDir);
        [TestMethod] public void OffChipCom_lo_fir() => TestTools.VerifyMakeGraph("OffChipCom", "lo.fir", TestDir);

        [TestMethod] public void PriorityMaskRstEncoder_fir() => TestTools.VerifyMakeGraph("PriorityMaskRstEncoder", "fir", TestDir);
        [TestMethod] public void PriorityMaskRstEncoder_lo_fir() => TestTools.VerifyMakeGraph("PriorityMaskRstEncoder", "lo.fir", TestDir);

        [TestMethod] public void UartEcho_fir() => TestTools.VerifyMakeGraph("UartEcho", "fir", TestDir);
        [TestMethod] public void UartEcho_lo_fir() => TestTools.VerifyMakeGraph("UartEcho", "lo.fir", TestDir);

        [TestMethod] public void ALUTester_fir() => TestTools.VerifyMakeGraph("ALUTester", "fir", TestDir);
        [TestMethod] public void ALUTester_lo_fir() => TestTools.VerifyMakeGraph("ALUTester", "lo.fir", TestDir);

        [TestMethod] public void ALUTester_0_fir() => TestTools.VerifyMakeGraph("ALUTester_0", "fir", TestDir);
        [TestMethod] public void ALUTester_0_lo_fir() => TestTools.VerifyMakeGraph("ALUTester_0", "lo.fir", TestDir);

        [TestMethod] public void BrCondTester_fir() => TestTools.VerifyMakeGraph("BrCondTester", "fir", TestDir);
        [TestMethod] public void BrCondTester_lo_fir() => TestTools.VerifyMakeGraph("BrCondTester", "lo.fir", TestDir);

        [TestMethod] public void BrCondTester_0_fir() => TestTools.VerifyMakeGraph("BrCondTester_0", "fir", TestDir);
        [TestMethod] public void BrCondTester_0_lo_fir() => TestTools.VerifyMakeGraph("BrCondTester_0", "lo.fir", TestDir);

        [TestMethod] public void CacheTester_fir() => TestTools.VerifyMakeGraph("CacheTester", "fir", TestDir);
        [TestMethod] public void CacheTester_lo_fir() => TestTools.VerifyMakeGraph("CacheTester", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_fir() => TestTools.VerifyMakeGraph("CoreTester", "fir", TestDir);
        [TestMethod] public void CoreTester_lo_fir() => TestTools.VerifyMakeGraph("CoreTester", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_0_fir() => TestTools.VerifyMakeGraph("CoreTester_0", "fir", TestDir);
        [TestMethod] public void CoreTester_0_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_0", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_1_fir() => TestTools.VerifyMakeGraph("CoreTester_1", "fir", TestDir);
        [TestMethod] public void CoreTester_1_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_1", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_2_fir() => TestTools.VerifyMakeGraph("CoreTester_2", "fir", TestDir);
        [TestMethod] public void CoreTester_2_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_2", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_3_fir() => TestTools.VerifyMakeGraph("CoreTester_3", "fir", TestDir);
        [TestMethod] public void CoreTester_3_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_3", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_4_fir() => TestTools.VerifyMakeGraph("CoreTester_4", "fir", TestDir);
        [TestMethod] public void CoreTester_4_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_4", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_5_fir() => TestTools.VerifyMakeGraph("CoreTester_5", "fir", TestDir);
        [TestMethod] public void CoreTester_5_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_5", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_6_fir() => TestTools.VerifyMakeGraph("CoreTester_6", "fir", TestDir);
        [TestMethod] public void CoreTester_6_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_6", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_7_fir() => TestTools.VerifyMakeGraph("CoreTester_7", "fir", TestDir);
        [TestMethod] public void CoreTester_7_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_7", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_8_fir() => TestTools.VerifyMakeGraph("CoreTester_8", "fir", TestDir);
        [TestMethod] public void CoreTester_8_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_8", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_9_fir() => TestTools.VerifyMakeGraph("CoreTester_9", "fir", TestDir);
        [TestMethod] public void CoreTester_9_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_9", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_10_fir() => TestTools.VerifyMakeGraph("CoreTester_10", "fir", TestDir);
        [TestMethod] public void CoreTester_10_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_10", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_11_fir() => TestTools.VerifyMakeGraph("CoreTester_11", "fir", TestDir);
        [TestMethod] public void CoreTester_11_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_11", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_12_fir() => TestTools.VerifyMakeGraph("CoreTester_12", "fir", TestDir);
        [TestMethod] public void CoreTester_12_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_12", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_13_fir() => TestTools.VerifyMakeGraph("CoreTester_13", "fir", TestDir);
        [TestMethod] public void CoreTester_13_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_13", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_14_fir() => TestTools.VerifyMakeGraph("CoreTester_14", "fir", TestDir);
        [TestMethod] public void CoreTester_14_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_14", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_15_fir() => TestTools.VerifyMakeGraph("CoreTester_15", "fir", TestDir);
        [TestMethod] public void CoreTester_15_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_15", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_16_fir() => TestTools.VerifyMakeGraph("CoreTester_16", "fir", TestDir);
        [TestMethod] public void CoreTester_16_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_16", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_17_fir() => TestTools.VerifyMakeGraph("CoreTester_17", "fir", TestDir);
        [TestMethod] public void CoreTester_17_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_17", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_18_fir() => TestTools.VerifyMakeGraph("CoreTester_18", "fir", TestDir);
        [TestMethod] public void CoreTester_18_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_18", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_19_fir() => TestTools.VerifyMakeGraph("CoreTester_19", "fir", TestDir);
        [TestMethod] public void CoreTester_19_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_19", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_20_fir() => TestTools.VerifyMakeGraph("CoreTester_20", "fir", TestDir);
        [TestMethod] public void CoreTester_20_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_20", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_21_fir() => TestTools.VerifyMakeGraph("CoreTester_21", "fir", TestDir);
        [TestMethod] public void CoreTester_21_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_21", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_22_fir() => TestTools.VerifyMakeGraph("CoreTester_22", "fir", TestDir);
        [TestMethod] public void CoreTester_22_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_22", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_23_fir() => TestTools.VerifyMakeGraph("CoreTester_23", "fir", TestDir);
        [TestMethod] public void CoreTester_23_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_23", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_24_fir() => TestTools.VerifyMakeGraph("CoreTester_24", "fir", TestDir);
        [TestMethod] public void CoreTester_24_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_24", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_25_fir() => TestTools.VerifyMakeGraph("CoreTester_25", "fir", TestDir);
        [TestMethod] public void CoreTester_25_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_25", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_26_fir() => TestTools.VerifyMakeGraph("CoreTester_26", "fir", TestDir);
        [TestMethod] public void CoreTester_26_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_26", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_27_fir() => TestTools.VerifyMakeGraph("CoreTester_27", "fir", TestDir);
        [TestMethod] public void CoreTester_27_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_27", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_28_fir() => TestTools.VerifyMakeGraph("CoreTester_28", "fir", TestDir);
        [TestMethod] public void CoreTester_28_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_28", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_29_fir() => TestTools.VerifyMakeGraph("CoreTester_29", "fir", TestDir);
        [TestMethod] public void CoreTester_29_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_29", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_30_fir() => TestTools.VerifyMakeGraph("CoreTester_30", "fir", TestDir);
        [TestMethod] public void CoreTester_30_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_30", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_31_fir() => TestTools.VerifyMakeGraph("CoreTester_31", "fir", TestDir);
        [TestMethod] public void CoreTester_31_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_31", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_32_fir() => TestTools.VerifyMakeGraph("CoreTester_32", "fir", TestDir);
        [TestMethod] public void CoreTester_32_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_32", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_33_fir() => TestTools.VerifyMakeGraph("CoreTester_33", "fir", TestDir);
        [TestMethod] public void CoreTester_33_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_33", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_34_fir() => TestTools.VerifyMakeGraph("CoreTester_34", "fir", TestDir);
        [TestMethod] public void CoreTester_34_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_34", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_35_fir() => TestTools.VerifyMakeGraph("CoreTester_35", "fir", TestDir);
        [TestMethod] public void CoreTester_35_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_35", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_36_fir() => TestTools.VerifyMakeGraph("CoreTester_36", "fir", TestDir);
        [TestMethod] public void CoreTester_36_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_36", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_37_fir() => TestTools.VerifyMakeGraph("CoreTester_37", "fir", TestDir);
        [TestMethod] public void CoreTester_37_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_37", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_38_fir() => TestTools.VerifyMakeGraph("CoreTester_38", "fir", TestDir);
        [TestMethod] public void CoreTester_38_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_38", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_39_fir() => TestTools.VerifyMakeGraph("CoreTester_39", "fir", TestDir);
        [TestMethod] public void CoreTester_39_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_39", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_40_fir() => TestTools.VerifyMakeGraph("CoreTester_40", "fir", TestDir);
        [TestMethod] public void CoreTester_40_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_40", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_41_fir() => TestTools.VerifyMakeGraph("CoreTester_41", "fir", TestDir);
        [TestMethod] public void CoreTester_41_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_41", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_42_fir() => TestTools.VerifyMakeGraph("CoreTester_42", "fir", TestDir);
        [TestMethod] public void CoreTester_42_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_42", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_43_fir() => TestTools.VerifyMakeGraph("CoreTester_43", "fir", TestDir);
        [TestMethod] public void CoreTester_43_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_43", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_44_fir() => TestTools.VerifyMakeGraph("CoreTester_44", "fir", TestDir);
        [TestMethod] public void CoreTester_44_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_44", "lo.fir", TestDir);

        [TestMethod] public void CoreTester_45_fir() => TestTools.VerifyMakeGraph("CoreTester_45", "fir", TestDir);
        [TestMethod] public void CoreTester_45_lo_fir() => TestTools.VerifyMakeGraph("CoreTester_45", "lo.fir", TestDir);

        [TestMethod] public void CSRTester_fir() => TestTools.VerifyMakeGraph("CSRTester", "fir", TestDir);
        [TestMethod] public void CSRTester_lo_fir() => TestTools.VerifyMakeGraph("CSRTester", "lo.fir", TestDir);

        [TestMethod] public void DatapathTester_fir() => TestTools.VerifyMakeGraph("DatapathTester", "fir", TestDir);
        [TestMethod] public void DatapathTester_lo_fir() => TestTools.VerifyMakeGraph("DatapathTester", "lo.fir", TestDir);

        [TestMethod] public void DatapathTester_0_fir() => TestTools.VerifyMakeGraph("DatapathTester_0", "fir", TestDir);
        [TestMethod] public void DatapathTester_0_lo_fir() => TestTools.VerifyMakeGraph("DatapathTester_0", "lo.fir", TestDir);

        [TestMethod] public void ImmGenTester_fir() => TestTools.VerifyMakeGraph("ImmGenTester", "fir", TestDir);
        [TestMethod] public void ImmGenTester_lo_fir() => TestTools.VerifyMakeGraph("ImmGenTester", "lo.fir", TestDir);

        [TestMethod] public void ImmGenTester_0_fir() => TestTools.VerifyMakeGraph("ImmGenTester_0", "fir", TestDir);
        [TestMethod] public void ImmGenTester_0_lo_fir() => TestTools.VerifyMakeGraph("ImmGenTester_0", "lo.fir", TestDir);

        [TestMethod] public void TileTester_fir() => TestTools.VerifyMakeGraph("TileTester", "fir", TestDir);
        [TestMethod] public void TileTester_lo_fir() => TestTools.VerifyMakeGraph("TileTester", "lo.fir", TestDir);

        [TestMethod] public void TileTester_0_fir() => TestTools.VerifyMakeGraph("TileTester_0", "fir", TestDir);
        [TestMethod] public void TileTester_0_lo_fir() => TestTools.VerifyMakeGraph("TileTester_0", "lo.fir", TestDir);

        [TestMethod] public void TileTester_1_fir() => TestTools.VerifyMakeGraph("TileTester_1", "fir", TestDir);
        [TestMethod] public void TileTester_1_lo_fir() => TestTools.VerifyMakeGraph("TileTester_1", "lo.fir", TestDir);

        [TestMethod] public void TileTester_2_fir() => TestTools.VerifyMakeGraph("TileTester_2", "fir", TestDir);
        [TestMethod] public void TileTester_2_lo_fir() => TestTools.VerifyMakeGraph("TileTester_2", "lo.fir", TestDir);

        [TestMethod] public void TileTester_3_fir() => TestTools.VerifyMakeGraph("TileTester_3", "fir", TestDir);
        [TestMethod] public void TileTester_3_lo_fir() => TestTools.VerifyMakeGraph("TileTester_3", "lo.fir", TestDir);

        [TestMethod] public void TileTester_4_fir() => TestTools.VerifyMakeGraph("TileTester_4", "fir", TestDir);
        [TestMethod] public void TileTester_4_lo_fir() => TestTools.VerifyMakeGraph("TileTester_4", "lo.fir", TestDir);

        [TestMethod] public void TileTester_5_fir() => TestTools.VerifyMakeGraph("TileTester_5", "fir", TestDir);
        [TestMethod] public void TileTester_5_lo_fir() => TestTools.VerifyMakeGraph("TileTester_5", "lo.fir", TestDir);

        [TestMethod] public void TileTester_6_fir() => TestTools.VerifyMakeGraph("TileTester_6", "fir", TestDir);
        [TestMethod] public void TileTester_6_lo_fir() => TestTools.VerifyMakeGraph("TileTester_6", "lo.fir", TestDir);

        [TestMethod] public void TileTester_7_fir() => TestTools.VerifyMakeGraph("TileTester_7", "fir", TestDir);
        [TestMethod] public void TileTester_7_lo_fir() => TestTools.VerifyMakeGraph("TileTester_7", "lo.fir", TestDir);

        [TestMethod] public void TileTester_8_fir() => TestTools.VerifyMakeGraph("TileTester_8", "fir", TestDir);
        [TestMethod] public void TileTester_8_lo_fir() => TestTools.VerifyMakeGraph("TileTester_8", "lo.fir", TestDir);

        [TestMethod] public void TileTester_9_fir() => TestTools.VerifyMakeGraph("TileTester_9", "fir", TestDir);
        [TestMethod] public void TileTester_9_lo_fir() => TestTools.VerifyMakeGraph("TileTester_9", "lo.fir", TestDir);

        [TestMethod] public void TileTester_10_fir() => TestTools.VerifyMakeGraph("TileTester_10", "fir", TestDir);
        [TestMethod] public void TileTester_10_lo_fir() => TestTools.VerifyMakeGraph("TileTester_10", "lo.fir", TestDir);

        [TestMethod] public void TileTester_11_fir() => TestTools.VerifyMakeGraph("TileTester_11", "fir", TestDir);
        [TestMethod] public void TileTester_11_lo_fir() => TestTools.VerifyMakeGraph("TileTester_11", "lo.fir", TestDir);

        [TestMethod] public void TileTester_12_fir() => TestTools.VerifyMakeGraph("TileTester_12", "fir", TestDir);
        [TestMethod] public void TileTester_12_lo_fir() => TestTools.VerifyMakeGraph("TileTester_12", "lo.fir", TestDir);

        [TestMethod] public void TileTester_13_fir() => TestTools.VerifyMakeGraph("TileTester_13", "fir", TestDir);
        [TestMethod] public void TileTester_13_lo_fir() => TestTools.VerifyMakeGraph("TileTester_13", "lo.fir", TestDir);

        [TestMethod] public void TileTester_14_fir() => TestTools.VerifyMakeGraph("TileTester_14", "fir", TestDir);
        [TestMethod] public void TileTester_14_lo_fir() => TestTools.VerifyMakeGraph("TileTester_14", "lo.fir", TestDir);

        [TestMethod] public void TileTester_15_fir() => TestTools.VerifyMakeGraph("TileTester_15", "fir", TestDir);
        [TestMethod] public void TileTester_15_lo_fir() => TestTools.VerifyMakeGraph("TileTester_15", "lo.fir", TestDir);

        [TestMethod] public void TileTester_16_fir() => TestTools.VerifyMakeGraph("TileTester_16", "fir", TestDir);
        [TestMethod] public void TileTester_16_lo_fir() => TestTools.VerifyMakeGraph("TileTester_16", "lo.fir", TestDir);

        [TestMethod] public void TileTester_17_fir() => TestTools.VerifyMakeGraph("TileTester_17", "fir", TestDir);
        [TestMethod] public void TileTester_17_lo_fir() => TestTools.VerifyMakeGraph("TileTester_17", "lo.fir", TestDir);

        [TestMethod] public void TileTester_18_fir() => TestTools.VerifyMakeGraph("TileTester_18", "fir", TestDir);
        [TestMethod] public void TileTester_18_lo_fir() => TestTools.VerifyMakeGraph("TileTester_18", "lo.fir", TestDir);

        [TestMethod] public void TileTester_19_fir() => TestTools.VerifyMakeGraph("TileTester_19", "fir", TestDir);
        [TestMethod] public void TileTester_19_lo_fir() => TestTools.VerifyMakeGraph("TileTester_19", "lo.fir", TestDir);

        [TestMethod] public void TileTester_20_fir() => TestTools.VerifyMakeGraph("TileTester_20", "fir", TestDir);
        [TestMethod] public void TileTester_20_lo_fir() => TestTools.VerifyMakeGraph("TileTester_20", "lo.fir", TestDir);

        [TestMethod] public void TileTester_21_fir() => TestTools.VerifyMakeGraph("TileTester_21", "fir", TestDir);
        [TestMethod] public void TileTester_21_lo_fir() => TestTools.VerifyMakeGraph("TileTester_21", "lo.fir", TestDir);

        [TestMethod] public void TileTester_22_fir() => TestTools.VerifyMakeGraph("TileTester_22", "fir", TestDir);
        [TestMethod] public void TileTester_22_lo_fir() => TestTools.VerifyMakeGraph("TileTester_22", "lo.fir", TestDir);

        [TestMethod] public void TileTester_23_fir() => TestTools.VerifyMakeGraph("TileTester_23", "fir", TestDir);
        [TestMethod] public void TileTester_23_lo_fir() => TestTools.VerifyMakeGraph("TileTester_23", "lo.fir", TestDir);

        [TestMethod] public void TileTester_24_fir() => TestTools.VerifyMakeGraph("TileTester_24", "fir", TestDir);
        [TestMethod] public void TileTester_24_lo_fir() => TestTools.VerifyMakeGraph("TileTester_24", "lo.fir", TestDir);

        [TestMethod] public void TileTester_25_fir() => TestTools.VerifyMakeGraph("TileTester_25", "fir", TestDir);
        [TestMethod] public void TileTester_25_lo_fir() => TestTools.VerifyMakeGraph("TileTester_25", "lo.fir", TestDir);

        [TestMethod] public void TileTester_26_fir() => TestTools.VerifyMakeGraph("TileTester_26", "fir", TestDir);
        [TestMethod] public void TileTester_26_lo_fir() => TestTools.VerifyMakeGraph("TileTester_26", "lo.fir", TestDir);

        [TestMethod] public void TileTester_27_fir() => TestTools.VerifyMakeGraph("TileTester_27", "fir", TestDir);
        [TestMethod] public void TileTester_27_lo_fir() => TestTools.VerifyMakeGraph("TileTester_27", "lo.fir", TestDir);

        [TestMethod] public void TileTester_28_fir() => TestTools.VerifyMakeGraph("TileTester_28", "fir", TestDir);
        [TestMethod] public void TileTester_28_lo_fir() => TestTools.VerifyMakeGraph("TileTester_28", "lo.fir", TestDir);

        [TestMethod] public void TileTester_29_fir() => TestTools.VerifyMakeGraph("TileTester_29", "fir", TestDir);
        [TestMethod] public void TileTester_29_lo_fir() => TestTools.VerifyMakeGraph("TileTester_29", "lo.fir", TestDir);

        [TestMethod] public void TileTester_30_fir() => TestTools.VerifyMakeGraph("TileTester_30", "fir", TestDir);
        [TestMethod] public void TileTester_30_lo_fir() => TestTools.VerifyMakeGraph("TileTester_30", "lo.fir", TestDir);

        [TestMethod] public void TileTester_31_fir() => TestTools.VerifyMakeGraph("TileTester_31", "fir", TestDir);
        [TestMethod] public void TileTester_31_lo_fir() => TestTools.VerifyMakeGraph("TileTester_31", "lo.fir", TestDir);

        [TestMethod] public void TileTester_32_fir() => TestTools.VerifyMakeGraph("TileTester_32", "fir", TestDir);
        [TestMethod] public void TileTester_32_lo_fir() => TestTools.VerifyMakeGraph("TileTester_32", "lo.fir", TestDir);

        [TestMethod] public void TileTester_33_fir() => TestTools.VerifyMakeGraph("TileTester_33", "fir", TestDir);
        [TestMethod] public void TileTester_33_lo_fir() => TestTools.VerifyMakeGraph("TileTester_33", "lo.fir", TestDir);

        [TestMethod] public void TileTester_34_fir() => TestTools.VerifyMakeGraph("TileTester_34", "fir", TestDir);
        [TestMethod] public void TileTester_34_lo_fir() => TestTools.VerifyMakeGraph("TileTester_34", "lo.fir", TestDir);

        [TestMethod] public void TileTester_35_fir() => TestTools.VerifyMakeGraph("TileTester_35", "fir", TestDir);
        [TestMethod] public void TileTester_35_lo_fir() => TestTools.VerifyMakeGraph("TileTester_35", "lo.fir", TestDir);

        [TestMethod] public void TileTester_36_fir() => TestTools.VerifyMakeGraph("TileTester_36", "fir", TestDir);
        [TestMethod] public void TileTester_36_lo_fir() => TestTools.VerifyMakeGraph("TileTester_36", "lo.fir", TestDir);

        [TestMethod] public void TileTester_37_fir() => TestTools.VerifyMakeGraph("TileTester_37", "fir", TestDir);
        [TestMethod] public void TileTester_37_lo_fir() => TestTools.VerifyMakeGraph("TileTester_37", "lo.fir", TestDir);

        [TestMethod] public void TileTester_38_fir() => TestTools.VerifyMakeGraph("TileTester_38", "fir", TestDir);
        [TestMethod] public void TileTester_38_lo_fir() => TestTools.VerifyMakeGraph("TileTester_38", "lo.fir", TestDir);

        [TestMethod] public void TileTester_39_fir() => TestTools.VerifyMakeGraph("TileTester_39", "fir", TestDir);
        [TestMethod] public void TileTester_39_lo_fir() => TestTools.VerifyMakeGraph("TileTester_39", "lo.fir", TestDir);

        [TestMethod] public void TileTester_40_fir() => TestTools.VerifyMakeGraph("TileTester_40", "fir", TestDir);
        [TestMethod] public void TileTester_40_lo_fir() => TestTools.VerifyMakeGraph("TileTester_40", "lo.fir", TestDir);

        [TestMethod] public void TileTester_41_fir() => TestTools.VerifyMakeGraph("TileTester_41", "fir", TestDir);
        [TestMethod] public void TileTester_41_lo_fir() => TestTools.VerifyMakeGraph("TileTester_41", "lo.fir", TestDir);

        [TestMethod] public void TileTester_42_fir() => TestTools.VerifyMakeGraph("TileTester_42", "fir", TestDir);
        [TestMethod] public void TileTester_42_lo_fir() => TestTools.VerifyMakeGraph("TileTester_42", "lo.fir", TestDir);

        [TestMethod] public void TileTester_43_fir() => TestTools.VerifyMakeGraph("TileTester_43", "fir", TestDir);
        [TestMethod] public void TileTester_43_lo_fir() => TestTools.VerifyMakeGraph("TileTester_43", "lo.fir", TestDir);

        [TestMethod] public void TileTester_44_fir() => TestTools.VerifyMakeGraph("TileTester_44", "fir", TestDir);
        [TestMethod] public void TileTester_44_lo_fir() => TestTools.VerifyMakeGraph("TileTester_44", "lo.fir", TestDir);

        [TestMethod] public void TileTester_45_fir() => TestTools.VerifyMakeGraph("TileTester_45", "fir", TestDir);
        [TestMethod] public void TileTester_45_lo_fir() => TestTools.VerifyMakeGraph("TileTester_45", "lo.fir", TestDir);

        [TestMethod] public void TileTester_46_fir() => TestTools.VerifyMakeGraph("TileTester_46", "fir", TestDir);
        [TestMethod] public void TileTester_46_lo_fir() => TestTools.VerifyMakeGraph("TileTester_46", "lo.fir", TestDir);

        [TestMethod] public void TileTester_47_fir() => TestTools.VerifyMakeGraph("TileTester_47", "fir", TestDir);
        [TestMethod] public void TileTester_47_lo_fir() => TestTools.VerifyMakeGraph("TileTester_47", "lo.fir", TestDir);

        [TestMethod] public void TileTester_48_fir() => TestTools.VerifyMakeGraph("TileTester_48", "fir", TestDir);
        [TestMethod] public void TileTester_48_lo_fir() => TestTools.VerifyMakeGraph("TileTester_48", "lo.fir", TestDir);

        [TestMethod] public void TileTester_49_fir() => TestTools.VerifyMakeGraph("TileTester_49", "fir", TestDir);
        [TestMethod] public void TileTester_49_lo_fir() => TestTools.VerifyMakeGraph("TileTester_49", "lo.fir", TestDir);

        [TestMethod] public void TileTester_50_fir() => TestTools.VerifyMakeGraph("TileTester_50", "fir", TestDir);
        [TestMethod] public void TileTester_50_lo_fir() => TestTools.VerifyMakeGraph("TileTester_50", "lo.fir", TestDir);


    }
}
