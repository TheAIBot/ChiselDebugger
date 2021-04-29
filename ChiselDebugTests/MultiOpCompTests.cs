using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebugTests
{
    [TestClass]
    public class MultiOpCompTests
    {
        const string TestDir = "../../../../TestGenerator/OthersFIRRTL";

        [TestMethod] public void ComputeSeq_fir() => TestTools.VerifyComputeGraph("ComputeSeq", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_0_fir() => TestTools.VerifyComputeGraph("ComputeSeq_0", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_0_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_0", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_1_fir() => TestTools.VerifyComputeGraph("ComputeSeq_1", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_1_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_1", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_2_fir() => TestTools.VerifyComputeGraph("ComputeSeq_2", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_2_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_2", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_3_fir() => TestTools.VerifyComputeGraph("ComputeSeq_3", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_3_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_3", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_4_fir() => TestTools.VerifyComputeGraph("ComputeSeq_4", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_4_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_4", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_5_fir() => TestTools.VerifyComputeGraph("ComputeSeq_5", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_5_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_5", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_6_fir() => TestTools.VerifyComputeGraph("ComputeSeq_6", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_6_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_6", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_7_fir() => TestTools.VerifyComputeGraph("ComputeSeq_7", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_7_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_7", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_8_fir() => TestTools.VerifyComputeGraph("ComputeSeq_8", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_8_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_8", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_9_fir() => TestTools.VerifyComputeGraph("ComputeSeq_9", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_9_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_9", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_10_fir() => TestTools.VerifyComputeGraph("ComputeSeq_10", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_10_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_10", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_11_fir() => TestTools.VerifyComputeGraph("ComputeSeq_11", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_11_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_11", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_12_fir() => TestTools.VerifyComputeGraph("ComputeSeq_12", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_12_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_12", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_13_fir() => TestTools.VerifyComputeGraph("ComputeSeq_13", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_13_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_13", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_14_fir() => TestTools.VerifyComputeGraph("ComputeSeq_14", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_14_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_14", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_15_fir() => TestTools.VerifyComputeGraph("ComputeSeq_15", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_15_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_15", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_16_fir() => TestTools.VerifyComputeGraph("ComputeSeq_16", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_16_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_16", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_17_fir() => TestTools.VerifyComputeGraph("ComputeSeq_17", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_17_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_17", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_18_fir() => TestTools.VerifyComputeGraph("ComputeSeq_18", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_18_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_18", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_19_fir() => TestTools.VerifyComputeGraph("ComputeSeq_19", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_19_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_19", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_20_fir() => TestTools.VerifyComputeGraph("ComputeSeq_20", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_20_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_20", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_21_fir() => TestTools.VerifyComputeGraph("ComputeSeq_21", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_21_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_21", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_22_fir() => TestTools.VerifyComputeGraph("ComputeSeq_22", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_22_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_22", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_23_fir() => TestTools.VerifyComputeGraph("ComputeSeq_23", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_23_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_23", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_24_fir() => TestTools.VerifyComputeGraph("ComputeSeq_24", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_24_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_24", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_25_fir() => TestTools.VerifyComputeGraph("ComputeSeq_25", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_25_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_25", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_26_fir() => TestTools.VerifyComputeGraph("ComputeSeq_26", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_26_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_26", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_27_fir() => TestTools.VerifyComputeGraph("ComputeSeq_27", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_27_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_27", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_28_fir() => TestTools.VerifyComputeGraph("ComputeSeq_28", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_28_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_28", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_29_fir() => TestTools.VerifyComputeGraph("ComputeSeq_29", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_29_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_29", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_30_fir() => TestTools.VerifyComputeGraph("ComputeSeq_30", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_30_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_30", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_31_fir() => TestTools.VerifyComputeGraph("ComputeSeq_31", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_31_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_31", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_32_fir() => TestTools.VerifyComputeGraph("ComputeSeq_32", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_32_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_32", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_33_fir() => TestTools.VerifyComputeGraph("ComputeSeq_33", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_33_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_33", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_34_fir() => TestTools.VerifyComputeGraph("ComputeSeq_34", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_34_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_34", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_35_fir() => TestTools.VerifyComputeGraph("ComputeSeq_35", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_35_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_35", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_36_fir() => TestTools.VerifyComputeGraph("ComputeSeq_36", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_36_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_36", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_37_fir() => TestTools.VerifyComputeGraph("ComputeSeq_37", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_37_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_37", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_38_fir() => TestTools.VerifyComputeGraph("ComputeSeq_38", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_38_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_38", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_39_fir() => TestTools.VerifyComputeGraph("ComputeSeq_39", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_39_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_39", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_40_fir() => TestTools.VerifyComputeGraph("ComputeSeq_40", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_40_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_40", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_41_fir() => TestTools.VerifyComputeGraph("ComputeSeq_41", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_41_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_41", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_42_fir() => TestTools.VerifyComputeGraph("ComputeSeq_42", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_42_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_42", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_43_fir() => TestTools.VerifyComputeGraph("ComputeSeq_43", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_43_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_43", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_44_fir() => TestTools.VerifyComputeGraph("ComputeSeq_44", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_44_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_44", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_45_fir() => TestTools.VerifyComputeGraph("ComputeSeq_45", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_45_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_45", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_46_fir() => TestTools.VerifyComputeGraph("ComputeSeq_46", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_46_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_46", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_47_fir() => TestTools.VerifyComputeGraph("ComputeSeq_47", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_47_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_47", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_48_fir() => TestTools.VerifyComputeGraph("ComputeSeq_48", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_48_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_48", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_49_fir() => TestTools.VerifyComputeGraph("ComputeSeq_49", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_49_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_49", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_50_fir() => TestTools.VerifyComputeGraph("ComputeSeq_50", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_50_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_50", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_51_fir() => TestTools.VerifyComputeGraph("ComputeSeq_51", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_51_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_51", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_52_fir() => TestTools.VerifyComputeGraph("ComputeSeq_52", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_52_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_52", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_53_fir() => TestTools.VerifyComputeGraph("ComputeSeq_53", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_53_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_53", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_54_fir() => TestTools.VerifyComputeGraph("ComputeSeq_54", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_54_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_54", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_55_fir() => TestTools.VerifyComputeGraph("ComputeSeq_55", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_55_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_55", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_56_fir() => TestTools.VerifyComputeGraph("ComputeSeq_56", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_56_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_56", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_57_fir() => TestTools.VerifyComputeGraph("ComputeSeq_57", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_57_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_57", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_58_fir() => TestTools.VerifyComputeGraph("ComputeSeq_58", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_58_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_58", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_59_fir() => TestTools.VerifyComputeGraph("ComputeSeq_59", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_59_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_59", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_60_fir() => TestTools.VerifyComputeGraph("ComputeSeq_60", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_60_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_60", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_61_fir() => TestTools.VerifyComputeGraph("ComputeSeq_61", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_61_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_61", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_62_fir() => TestTools.VerifyComputeGraph("ComputeSeq_62", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_62_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_62", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_63_fir() => TestTools.VerifyComputeGraph("ComputeSeq_63", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_63_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_63", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_64_fir() => TestTools.VerifyComputeGraph("ComputeSeq_64", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_64_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_64", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_65_fir() => TestTools.VerifyComputeGraph("ComputeSeq_65", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_65_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_65", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_66_fir() => TestTools.VerifyComputeGraph("ComputeSeq_66", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_66_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_66", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_67_fir() => TestTools.VerifyComputeGraph("ComputeSeq_67", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_67_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_67", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_68_fir() => TestTools.VerifyComputeGraph("ComputeSeq_68", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_68_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_68", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_69_fir() => TestTools.VerifyComputeGraph("ComputeSeq_69", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_69_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_69", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_70_fir() => TestTools.VerifyComputeGraph("ComputeSeq_70", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_70_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_70", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_71_fir() => TestTools.VerifyComputeGraph("ComputeSeq_71", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_71_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_71", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_72_fir() => TestTools.VerifyComputeGraph("ComputeSeq_72", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_72_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_72", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_73_fir() => TestTools.VerifyComputeGraph("ComputeSeq_73", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_73_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_73", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_74_fir() => TestTools.VerifyComputeGraph("ComputeSeq_74", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_74_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_74", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_75_fir() => TestTools.VerifyComputeGraph("ComputeSeq_75", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_75_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_75", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_76_fir() => TestTools.VerifyComputeGraph("ComputeSeq_76", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_76_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_76", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_77_fir() => TestTools.VerifyComputeGraph("ComputeSeq_77", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_77_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_77", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_78_fir() => TestTools.VerifyComputeGraph("ComputeSeq_78", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_78_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_78", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_79_fir() => TestTools.VerifyComputeGraph("ComputeSeq_79", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_79_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_79", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_80_fir() => TestTools.VerifyComputeGraph("ComputeSeq_80", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_80_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_80", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_81_fir() => TestTools.VerifyComputeGraph("ComputeSeq_81", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_81_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_81", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_82_fir() => TestTools.VerifyComputeGraph("ComputeSeq_82", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_82_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_82", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_83_fir() => TestTools.VerifyComputeGraph("ComputeSeq_83", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_83_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_83", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_84_fir() => TestTools.VerifyComputeGraph("ComputeSeq_84", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_84_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_84", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_85_fir() => TestTools.VerifyComputeGraph("ComputeSeq_85", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_85_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_85", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_86_fir() => TestTools.VerifyComputeGraph("ComputeSeq_86", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_86_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_86", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_87_fir() => TestTools.VerifyComputeGraph("ComputeSeq_87", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_87_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_87", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_88_fir() => TestTools.VerifyComputeGraph("ComputeSeq_88", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_88_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_88", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_89_fir() => TestTools.VerifyComputeGraph("ComputeSeq_89", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_89_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_89", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_90_fir() => TestTools.VerifyComputeGraph("ComputeSeq_90", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_90_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_90", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_91_fir() => TestTools.VerifyComputeGraph("ComputeSeq_91", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_91_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_91", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_92_fir() => TestTools.VerifyComputeGraph("ComputeSeq_92", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_92_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_92", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_93_fir() => TestTools.VerifyComputeGraph("ComputeSeq_93", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_93_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_93", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_94_fir() => TestTools.VerifyComputeGraph("ComputeSeq_94", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_94_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_94", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_95_fir() => TestTools.VerifyComputeGraph("ComputeSeq_95", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_95_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_95", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_96_fir() => TestTools.VerifyComputeGraph("ComputeSeq_96", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_96_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_96", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_97_fir() => TestTools.VerifyComputeGraph("ComputeSeq_97", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_97_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_97", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_98_fir() => TestTools.VerifyComputeGraph("ComputeSeq_98", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_98_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_98", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_99_fir() => TestTools.VerifyComputeGraph("ComputeSeq_99", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_99_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_99", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_100_fir() => TestTools.VerifyComputeGraph("ComputeSeq_100", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_100_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_100", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_101_fir() => TestTools.VerifyComputeGraph("ComputeSeq_101", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_101_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_101", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_102_fir() => TestTools.VerifyComputeGraph("ComputeSeq_102", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_102_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_102", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_103_fir() => TestTools.VerifyComputeGraph("ComputeSeq_103", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_103_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_103", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_104_fir() => TestTools.VerifyComputeGraph("ComputeSeq_104", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_104_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_104", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_105_fir() => TestTools.VerifyComputeGraph("ComputeSeq_105", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_105_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_105", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_106_fir() => TestTools.VerifyComputeGraph("ComputeSeq_106", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_106_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_106", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_107_fir() => TestTools.VerifyComputeGraph("ComputeSeq_107", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_107_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_107", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_108_fir() => TestTools.VerifyComputeGraph("ComputeSeq_108", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_108_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_108", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_109_fir() => TestTools.VerifyComputeGraph("ComputeSeq_109", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_109_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_109", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_110_fir() => TestTools.VerifyComputeGraph("ComputeSeq_110", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_110_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_110", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_111_fir() => TestTools.VerifyComputeGraph("ComputeSeq_111", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_111_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_111", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_112_fir() => TestTools.VerifyComputeGraph("ComputeSeq_112", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_112_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_112", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_113_fir() => TestTools.VerifyComputeGraph("ComputeSeq_113", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_113_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_113", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_114_fir() => TestTools.VerifyComputeGraph("ComputeSeq_114", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_114_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_114", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_115_fir() => TestTools.VerifyComputeGraph("ComputeSeq_115", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_115_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_115", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_116_fir() => TestTools.VerifyComputeGraph("ComputeSeq_116", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_116_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_116", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_117_fir() => TestTools.VerifyComputeGraph("ComputeSeq_117", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_117_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_117", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_118_fir() => TestTools.VerifyComputeGraph("ComputeSeq_118", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_118_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_118", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_119_fir() => TestTools.VerifyComputeGraph("ComputeSeq_119", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_119_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_119", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_120_fir() => TestTools.VerifyComputeGraph("ComputeSeq_120", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_120_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_120", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_121_fir() => TestTools.VerifyComputeGraph("ComputeSeq_121", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_121_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_121", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_122_fir() => TestTools.VerifyComputeGraph("ComputeSeq_122", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_122_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_122", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_123_fir() => TestTools.VerifyComputeGraph("ComputeSeq_123", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_123_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_123", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_124_fir() => TestTools.VerifyComputeGraph("ComputeSeq_124", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_124_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_124", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_125_fir() => TestTools.VerifyComputeGraph("ComputeSeq_125", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_125_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_125", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_126_fir() => TestTools.VerifyComputeGraph("ComputeSeq_126", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_126_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_126", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_127_fir() => TestTools.VerifyComputeGraph("ComputeSeq_127", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_127_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_127", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_128_fir() => TestTools.VerifyComputeGraph("ComputeSeq_128", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_128_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_128", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_129_fir() => TestTools.VerifyComputeGraph("ComputeSeq_129", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_129_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_129", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_130_fir() => TestTools.VerifyComputeGraph("ComputeSeq_130", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_130_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_130", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_131_fir() => TestTools.VerifyComputeGraph("ComputeSeq_131", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_131_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_131", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_132_fir() => TestTools.VerifyComputeGraph("ComputeSeq_132", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_132_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_132", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_133_fir() => TestTools.VerifyComputeGraph("ComputeSeq_133", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_133_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_133", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_134_fir() => TestTools.VerifyComputeGraph("ComputeSeq_134", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_134_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_134", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_135_fir() => TestTools.VerifyComputeGraph("ComputeSeq_135", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_135_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_135", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_136_fir() => TestTools.VerifyComputeGraph("ComputeSeq_136", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_136_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_136", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_137_fir() => TestTools.VerifyComputeGraph("ComputeSeq_137", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_137_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_137", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_138_fir() => TestTools.VerifyComputeGraph("ComputeSeq_138", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_138_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_138", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_139_fir() => TestTools.VerifyComputeGraph("ComputeSeq_139", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_139_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_139", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_140_fir() => TestTools.VerifyComputeGraph("ComputeSeq_140", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_140_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_140", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_141_fir() => TestTools.VerifyComputeGraph("ComputeSeq_141", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_141_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_141", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_142_fir() => TestTools.VerifyComputeGraph("ComputeSeq_142", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_142_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_142", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_143_fir() => TestTools.VerifyComputeGraph("ComputeSeq_143", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_143_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_143", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_144_fir() => TestTools.VerifyComputeGraph("ComputeSeq_144", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_144_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_144", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_145_fir() => TestTools.VerifyComputeGraph("ComputeSeq_145", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_145_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_145", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_146_fir() => TestTools.VerifyComputeGraph("ComputeSeq_146", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_146_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_146", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_147_fir() => TestTools.VerifyComputeGraph("ComputeSeq_147", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_147_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_147", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_148_fir() => TestTools.VerifyComputeGraph("ComputeSeq_148", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_148_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_148", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_149_fir() => TestTools.VerifyComputeGraph("ComputeSeq_149", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_149_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_149", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_150_fir() => TestTools.VerifyComputeGraph("ComputeSeq_150", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_150_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_150", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_151_fir() => TestTools.VerifyComputeGraph("ComputeSeq_151", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_151_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_151", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_152_fir() => TestTools.VerifyComputeGraph("ComputeSeq_152", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_152_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_152", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_153_fir() => TestTools.VerifyComputeGraph("ComputeSeq_153", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_153_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_153", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_154_fir() => TestTools.VerifyComputeGraph("ComputeSeq_154", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_154_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_154", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_155_fir() => TestTools.VerifyComputeGraph("ComputeSeq_155", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_155_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_155", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_156_fir() => TestTools.VerifyComputeGraph("ComputeSeq_156", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_156_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_156", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_157_fir() => TestTools.VerifyComputeGraph("ComputeSeq_157", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_157_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_157", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_158_fir() => TestTools.VerifyComputeGraph("ComputeSeq_158", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_158_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_158", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_159_fir() => TestTools.VerifyComputeGraph("ComputeSeq_159", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_159_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_159", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_160_fir() => TestTools.VerifyComputeGraph("ComputeSeq_160", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_160_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_160", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_161_fir() => TestTools.VerifyComputeGraph("ComputeSeq_161", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_161_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_161", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_162_fir() => TestTools.VerifyComputeGraph("ComputeSeq_162", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_162_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_162", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_163_fir() => TestTools.VerifyComputeGraph("ComputeSeq_163", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_163_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_163", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_164_fir() => TestTools.VerifyComputeGraph("ComputeSeq_164", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_164_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_164", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_165_fir() => TestTools.VerifyComputeGraph("ComputeSeq_165", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_165_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_165", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_166_fir() => TestTools.VerifyComputeGraph("ComputeSeq_166", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_166_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_166", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_167_fir() => TestTools.VerifyComputeGraph("ComputeSeq_167", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_167_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_167", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_168_fir() => TestTools.VerifyComputeGraph("ComputeSeq_168", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_168_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_168", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_169_fir() => TestTools.VerifyComputeGraph("ComputeSeq_169", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_169_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_169", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_170_fir() => TestTools.VerifyComputeGraph("ComputeSeq_170", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_170_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_170", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_171_fir() => TestTools.VerifyComputeGraph("ComputeSeq_171", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_171_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_171", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_172_fir() => TestTools.VerifyComputeGraph("ComputeSeq_172", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_172_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_172", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_173_fir() => TestTools.VerifyComputeGraph("ComputeSeq_173", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_173_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_173", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_174_fir() => TestTools.VerifyComputeGraph("ComputeSeq_174", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_174_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_174", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_175_fir() => TestTools.VerifyComputeGraph("ComputeSeq_175", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_175_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_175", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_176_fir() => TestTools.VerifyComputeGraph("ComputeSeq_176", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_176_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_176", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_177_fir() => TestTools.VerifyComputeGraph("ComputeSeq_177", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_177_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_177", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_178_fir() => TestTools.VerifyComputeGraph("ComputeSeq_178", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_178_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_178", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_179_fir() => TestTools.VerifyComputeGraph("ComputeSeq_179", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_179_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_179", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_180_fir() => TestTools.VerifyComputeGraph("ComputeSeq_180", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_180_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_180", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_181_fir() => TestTools.VerifyComputeGraph("ComputeSeq_181", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_181_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_181", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_182_fir() => TestTools.VerifyComputeGraph("ComputeSeq_182", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_182_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_182", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_183_fir() => TestTools.VerifyComputeGraph("ComputeSeq_183", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_183_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_183", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_184_fir() => TestTools.VerifyComputeGraph("ComputeSeq_184", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_184_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_184", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_185_fir() => TestTools.VerifyComputeGraph("ComputeSeq_185", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_185_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_185", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_186_fir() => TestTools.VerifyComputeGraph("ComputeSeq_186", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_186_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_186", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_187_fir() => TestTools.VerifyComputeGraph("ComputeSeq_187", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_187_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_187", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_188_fir() => TestTools.VerifyComputeGraph("ComputeSeq_188", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_188_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_188", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_189_fir() => TestTools.VerifyComputeGraph("ComputeSeq_189", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_189_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_189", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_190_fir() => TestTools.VerifyComputeGraph("ComputeSeq_190", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_190_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_190", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_191_fir() => TestTools.VerifyComputeGraph("ComputeSeq_191", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_191_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_191", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_192_fir() => TestTools.VerifyComputeGraph("ComputeSeq_192", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_192_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_192", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_193_fir() => TestTools.VerifyComputeGraph("ComputeSeq_193", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_193_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_193", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_194_fir() => TestTools.VerifyComputeGraph("ComputeSeq_194", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_194_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_194", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_195_fir() => TestTools.VerifyComputeGraph("ComputeSeq_195", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_195_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_195", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_196_fir() => TestTools.VerifyComputeGraph("ComputeSeq_196", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_196_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_196", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_197_fir() => TestTools.VerifyComputeGraph("ComputeSeq_197", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_197_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_197", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSeq_198_fir() => TestTools.VerifyComputeGraph("ComputeSeq_198", "fir", false, TestDir);
        [TestMethod] public void ComputeSeq_198_lo_fir() => TestTools.VerifyComputeGraph("ComputeSeq_198", "lo.fir", false, TestDir);


    }
}
