using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebugTests
{
    [TestClass]
    public class SingleOpCompTests
    {
        const string TestDir = "../../../../TestGenerator/OthersFIRRTL";

        [TestMethod] public void ComputeSingle_fir() => TestTools.VerifyComputeGraph("ComputeSingle", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_0_fir() => TestTools.VerifyComputeGraph("ComputeSingle_0", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_0_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_0", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_1_fir() => TestTools.VerifyComputeGraph("ComputeSingle_1", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_1_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_1", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_2_fir() => TestTools.VerifyComputeGraph("ComputeSingle_2", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_2_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_2", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_3_fir() => TestTools.VerifyComputeGraph("ComputeSingle_3", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_3_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_3", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_4_fir() => TestTools.VerifyComputeGraph("ComputeSingle_4", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_4_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_4", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_5_fir() => TestTools.VerifyComputeGraph("ComputeSingle_5", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_5_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_5", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_6_fir() => TestTools.VerifyComputeGraph("ComputeSingle_6", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_6_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_6", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_7_fir() => TestTools.VerifyComputeGraph("ComputeSingle_7", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_7_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_7", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_8_fir() => TestTools.VerifyComputeGraph("ComputeSingle_8", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_8_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_8", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_9_fir() => TestTools.VerifyComputeGraph("ComputeSingle_9", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_9_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_9", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_10_fir() => TestTools.VerifyComputeGraph("ComputeSingle_10", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_10_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_10", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_11_fir() => TestTools.VerifyComputeGraph("ComputeSingle_11", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_11_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_11", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_12_fir() => TestTools.VerifyComputeGraph("ComputeSingle_12", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_12_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_12", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_13_fir() => TestTools.VerifyComputeGraph("ComputeSingle_13", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_13_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_13", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_14_fir() => TestTools.VerifyComputeGraph("ComputeSingle_14", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_14_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_14", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_15_fir() => TestTools.VerifyComputeGraph("ComputeSingle_15", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_15_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_15", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_16_fir() => TestTools.VerifyComputeGraph("ComputeSingle_16", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_16_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_16", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_17_fir() => TestTools.VerifyComputeGraph("ComputeSingle_17", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_17_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_17", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_18_fir() => TestTools.VerifyComputeGraph("ComputeSingle_18", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_18_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_18", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_19_fir() => TestTools.VerifyComputeGraph("ComputeSingle_19", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_19_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_19", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_20_fir() => TestTools.VerifyComputeGraph("ComputeSingle_20", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_20_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_20", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_21_fir() => TestTools.VerifyComputeGraph("ComputeSingle_21", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_21_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_21", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_22_fir() => TestTools.VerifyComputeGraph("ComputeSingle_22", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_22_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_22", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_23_fir() => TestTools.VerifyComputeGraph("ComputeSingle_23", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_23_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_23", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_24_fir() => TestTools.VerifyComputeGraph("ComputeSingle_24", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_24_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_24", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_25_fir() => TestTools.VerifyComputeGraph("ComputeSingle_25", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_25_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_25", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_26_fir() => TestTools.VerifyComputeGraph("ComputeSingle_26", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_26_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_26", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_27_fir() => TestTools.VerifyComputeGraph("ComputeSingle_27", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_27_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_27", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_28_fir() => TestTools.VerifyComputeGraph("ComputeSingle_28", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_28_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_28", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_29_fir() => TestTools.VerifyComputeGraph("ComputeSingle_29", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_29_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_29", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_30_fir() => TestTools.VerifyComputeGraph("ComputeSingle_30", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_30_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_30", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_31_fir() => TestTools.VerifyComputeGraph("ComputeSingle_31", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_31_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_31", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_32_fir() => TestTools.VerifyComputeGraph("ComputeSingle_32", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_32_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_32", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_33_fir() => TestTools.VerifyComputeGraph("ComputeSingle_33", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_33_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_33", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_34_fir() => TestTools.VerifyComputeGraph("ComputeSingle_34", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_34_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_34", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_35_fir() => TestTools.VerifyComputeGraph("ComputeSingle_35", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_35_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_35", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_36_fir() => TestTools.VerifyComputeGraph("ComputeSingle_36", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_36_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_36", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_37_fir() => TestTools.VerifyComputeGraph("ComputeSingle_37", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_37_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_37", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_38_fir() => TestTools.VerifyComputeGraph("ComputeSingle_38", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_38_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_38", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_39_fir() => TestTools.VerifyComputeGraph("ComputeSingle_39", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_39_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_39", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_40_fir() => TestTools.VerifyComputeGraph("ComputeSingle_40", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_40_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_40", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_41_fir() => TestTools.VerifyComputeGraph("ComputeSingle_41", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_41_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_41", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_42_fir() => TestTools.VerifyComputeGraph("ComputeSingle_42", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_42_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_42", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_43_fir() => TestTools.VerifyComputeGraph("ComputeSingle_43", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_43_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_43", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_44_fir() => TestTools.VerifyComputeGraph("ComputeSingle_44", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_44_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_44", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_45_fir() => TestTools.VerifyComputeGraph("ComputeSingle_45", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_45_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_45", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_46_fir() => TestTools.VerifyComputeGraph("ComputeSingle_46", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_46_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_46", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_47_fir() => TestTools.VerifyComputeGraph("ComputeSingle_47", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_47_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_47", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_48_fir() => TestTools.VerifyComputeGraph("ComputeSingle_48", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_48_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_48", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_49_fir() => TestTools.VerifyComputeGraph("ComputeSingle_49", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_49_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_49", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_50_fir() => TestTools.VerifyComputeGraph("ComputeSingle_50", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_50_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_50", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_51_fir() => TestTools.VerifyComputeGraph("ComputeSingle_51", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_51_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_51", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_52_fir() => TestTools.VerifyComputeGraph("ComputeSingle_52", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_52_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_52", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_53_fir() => TestTools.VerifyComputeGraph("ComputeSingle_53", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_53_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_53", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_54_fir() => TestTools.VerifyComputeGraph("ComputeSingle_54", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_54_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_54", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_55_fir() => TestTools.VerifyComputeGraph("ComputeSingle_55", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_55_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_55", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_56_fir() => TestTools.VerifyComputeGraph("ComputeSingle_56", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_56_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_56", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_57_fir() => TestTools.VerifyComputeGraph("ComputeSingle_57", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_57_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_57", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_58_fir() => TestTools.VerifyComputeGraph("ComputeSingle_58", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_58_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_58", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_59_fir() => TestTools.VerifyComputeGraph("ComputeSingle_59", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_59_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_59", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_60_fir() => TestTools.VerifyComputeGraph("ComputeSingle_60", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_60_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_60", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_61_fir() => TestTools.VerifyComputeGraph("ComputeSingle_61", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_61_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_61", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_62_fir() => TestTools.VerifyComputeGraph("ComputeSingle_62", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_62_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_62", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_63_fir() => TestTools.VerifyComputeGraph("ComputeSingle_63", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_63_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_63", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_64_fir() => TestTools.VerifyComputeGraph("ComputeSingle_64", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_64_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_64", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_65_fir() => TestTools.VerifyComputeGraph("ComputeSingle_65", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_65_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_65", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_66_fir() => TestTools.VerifyComputeGraph("ComputeSingle_66", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_66_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_66", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_67_fir() => TestTools.VerifyComputeGraph("ComputeSingle_67", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_67_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_67", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_68_fir() => TestTools.VerifyComputeGraph("ComputeSingle_68", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_68_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_68", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_69_fir() => TestTools.VerifyComputeGraph("ComputeSingle_69", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_69_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_69", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_70_fir() => TestTools.VerifyComputeGraph("ComputeSingle_70", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_70_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_70", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_71_fir() => TestTools.VerifyComputeGraph("ComputeSingle_71", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_71_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_71", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_72_fir() => TestTools.VerifyComputeGraph("ComputeSingle_72", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_72_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_72", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_73_fir() => TestTools.VerifyComputeGraph("ComputeSingle_73", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_73_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_73", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_74_fir() => TestTools.VerifyComputeGraph("ComputeSingle_74", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_74_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_74", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_75_fir() => TestTools.VerifyComputeGraph("ComputeSingle_75", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_75_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_75", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_76_fir() => TestTools.VerifyComputeGraph("ComputeSingle_76", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_76_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_76", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_77_fir() => TestTools.VerifyComputeGraph("ComputeSingle_77", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_77_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_77", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_78_fir() => TestTools.VerifyComputeGraph("ComputeSingle_78", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_78_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_78", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_79_fir() => TestTools.VerifyComputeGraph("ComputeSingle_79", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_79_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_79", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_80_fir() => TestTools.VerifyComputeGraph("ComputeSingle_80", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_80_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_80", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_81_fir() => TestTools.VerifyComputeGraph("ComputeSingle_81", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_81_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_81", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_82_fir() => TestTools.VerifyComputeGraph("ComputeSingle_82", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_82_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_82", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_83_fir() => TestTools.VerifyComputeGraph("ComputeSingle_83", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_83_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_83", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_84_fir() => TestTools.VerifyComputeGraph("ComputeSingle_84", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_84_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_84", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_85_fir() => TestTools.VerifyComputeGraph("ComputeSingle_85", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_85_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_85", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_86_fir() => TestTools.VerifyComputeGraph("ComputeSingle_86", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_86_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_86", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_87_fir() => TestTools.VerifyComputeGraph("ComputeSingle_87", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_87_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_87", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_88_fir() => TestTools.VerifyComputeGraph("ComputeSingle_88", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_88_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_88", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_89_fir() => TestTools.VerifyComputeGraph("ComputeSingle_89", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_89_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_89", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_90_fir() => TestTools.VerifyComputeGraph("ComputeSingle_90", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_90_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_90", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_91_fir() => TestTools.VerifyComputeGraph("ComputeSingle_91", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_91_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_91", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_92_fir() => TestTools.VerifyComputeGraph("ComputeSingle_92", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_92_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_92", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_93_fir() => TestTools.VerifyComputeGraph("ComputeSingle_93", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_93_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_93", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_94_fir() => TestTools.VerifyComputeGraph("ComputeSingle_94", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_94_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_94", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_95_fir() => TestTools.VerifyComputeGraph("ComputeSingle_95", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_95_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_95", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_96_fir() => TestTools.VerifyComputeGraph("ComputeSingle_96", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_96_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_96", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_97_fir() => TestTools.VerifyComputeGraph("ComputeSingle_97", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_97_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_97", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_98_fir() => TestTools.VerifyComputeGraph("ComputeSingle_98", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_98_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_98", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_99_fir() => TestTools.VerifyComputeGraph("ComputeSingle_99", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_99_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_99", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_100_fir() => TestTools.VerifyComputeGraph("ComputeSingle_100", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_100_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_100", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_101_fir() => TestTools.VerifyComputeGraph("ComputeSingle_101", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_101_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_101", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_102_fir() => TestTools.VerifyComputeGraph("ComputeSingle_102", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_102_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_102", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_103_fir() => TestTools.VerifyComputeGraph("ComputeSingle_103", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_103_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_103", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_104_fir() => TestTools.VerifyComputeGraph("ComputeSingle_104", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_104_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_104", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_105_fir() => TestTools.VerifyComputeGraph("ComputeSingle_105", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_105_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_105", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_106_fir() => TestTools.VerifyComputeGraph("ComputeSingle_106", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_106_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_106", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_107_fir() => TestTools.VerifyComputeGraph("ComputeSingle_107", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_107_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_107", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_108_fir() => TestTools.VerifyComputeGraph("ComputeSingle_108", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_108_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_108", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_109_fir() => TestTools.VerifyComputeGraph("ComputeSingle_109", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_109_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_109", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_110_fir() => TestTools.VerifyComputeGraph("ComputeSingle_110", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_110_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_110", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_111_fir() => TestTools.VerifyComputeGraph("ComputeSingle_111", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_111_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_111", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_112_fir() => TestTools.VerifyComputeGraph("ComputeSingle_112", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_112_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_112", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_113_fir() => TestTools.VerifyComputeGraph("ComputeSingle_113", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_113_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_113", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_114_fir() => TestTools.VerifyComputeGraph("ComputeSingle_114", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_114_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_114", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_115_fir() => TestTools.VerifyComputeGraph("ComputeSingle_115", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_115_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_115", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_116_fir() => TestTools.VerifyComputeGraph("ComputeSingle_116", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_116_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_116", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_117_fir() => TestTools.VerifyComputeGraph("ComputeSingle_117", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_117_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_117", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_118_fir() => TestTools.VerifyComputeGraph("ComputeSingle_118", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_118_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_118", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_119_fir() => TestTools.VerifyComputeGraph("ComputeSingle_119", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_119_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_119", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_120_fir() => TestTools.VerifyComputeGraph("ComputeSingle_120", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_120_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_120", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_121_fir() => TestTools.VerifyComputeGraph("ComputeSingle_121", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_121_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_121", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_122_fir() => TestTools.VerifyComputeGraph("ComputeSingle_122", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_122_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_122", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_123_fir() => TestTools.VerifyComputeGraph("ComputeSingle_123", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_123_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_123", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_124_fir() => TestTools.VerifyComputeGraph("ComputeSingle_124", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_124_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_124", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_125_fir() => TestTools.VerifyComputeGraph("ComputeSingle_125", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_125_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_125", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_126_fir() => TestTools.VerifyComputeGraph("ComputeSingle_126", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_126_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_126", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_127_fir() => TestTools.VerifyComputeGraph("ComputeSingle_127", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_127_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_127", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_128_fir() => TestTools.VerifyComputeGraph("ComputeSingle_128", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_128_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_128", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_129_fir() => TestTools.VerifyComputeGraph("ComputeSingle_129", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_129_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_129", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_130_fir() => TestTools.VerifyComputeGraph("ComputeSingle_130", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_130_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_130", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_131_fir() => TestTools.VerifyComputeGraph("ComputeSingle_131", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_131_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_131", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_132_fir() => TestTools.VerifyComputeGraph("ComputeSingle_132", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_132_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_132", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_133_fir() => TestTools.VerifyComputeGraph("ComputeSingle_133", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_133_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_133", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_134_fir() => TestTools.VerifyComputeGraph("ComputeSingle_134", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_134_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_134", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_135_fir() => TestTools.VerifyComputeGraph("ComputeSingle_135", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_135_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_135", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_136_fir() => TestTools.VerifyComputeGraph("ComputeSingle_136", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_136_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_136", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_137_fir() => TestTools.VerifyComputeGraph("ComputeSingle_137", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_137_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_137", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_138_fir() => TestTools.VerifyComputeGraph("ComputeSingle_138", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_138_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_138", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_139_fir() => TestTools.VerifyComputeGraph("ComputeSingle_139", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_139_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_139", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_140_fir() => TestTools.VerifyComputeGraph("ComputeSingle_140", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_140_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_140", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_141_fir() => TestTools.VerifyComputeGraph("ComputeSingle_141", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_141_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_141", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_142_fir() => TestTools.VerifyComputeGraph("ComputeSingle_142", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_142_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_142", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_143_fir() => TestTools.VerifyComputeGraph("ComputeSingle_143", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_143_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_143", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_144_fir() => TestTools.VerifyComputeGraph("ComputeSingle_144", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_144_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_144", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_145_fir() => TestTools.VerifyComputeGraph("ComputeSingle_145", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_145_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_145", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_146_fir() => TestTools.VerifyComputeGraph("ComputeSingle_146", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_146_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_146", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_147_fir() => TestTools.VerifyComputeGraph("ComputeSingle_147", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_147_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_147", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_148_fir() => TestTools.VerifyComputeGraph("ComputeSingle_148", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_148_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_148", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_149_fir() => TestTools.VerifyComputeGraph("ComputeSingle_149", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_149_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_149", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_150_fir() => TestTools.VerifyComputeGraph("ComputeSingle_150", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_150_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_150", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_151_fir() => TestTools.VerifyComputeGraph("ComputeSingle_151", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_151_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_151", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_152_fir() => TestTools.VerifyComputeGraph("ComputeSingle_152", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_152_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_152", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_153_fir() => TestTools.VerifyComputeGraph("ComputeSingle_153", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_153_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_153", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_154_fir() => TestTools.VerifyComputeGraph("ComputeSingle_154", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_154_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_154", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_155_fir() => TestTools.VerifyComputeGraph("ComputeSingle_155", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_155_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_155", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_156_fir() => TestTools.VerifyComputeGraph("ComputeSingle_156", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_156_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_156", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_157_fir() => TestTools.VerifyComputeGraph("ComputeSingle_157", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_157_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_157", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_158_fir() => TestTools.VerifyComputeGraph("ComputeSingle_158", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_158_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_158", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_159_fir() => TestTools.VerifyComputeGraph("ComputeSingle_159", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_159_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_159", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_160_fir() => TestTools.VerifyComputeGraph("ComputeSingle_160", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_160_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_160", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_161_fir() => TestTools.VerifyComputeGraph("ComputeSingle_161", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_161_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_161", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_162_fir() => TestTools.VerifyComputeGraph("ComputeSingle_162", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_162_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_162", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_163_fir() => TestTools.VerifyComputeGraph("ComputeSingle_163", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_163_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_163", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_164_fir() => TestTools.VerifyComputeGraph("ComputeSingle_164", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_164_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_164", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_165_fir() => TestTools.VerifyComputeGraph("ComputeSingle_165", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_165_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_165", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_166_fir() => TestTools.VerifyComputeGraph("ComputeSingle_166", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_166_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_166", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_167_fir() => TestTools.VerifyComputeGraph("ComputeSingle_167", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_167_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_167", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_168_fir() => TestTools.VerifyComputeGraph("ComputeSingle_168", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_168_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_168", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_169_fir() => TestTools.VerifyComputeGraph("ComputeSingle_169", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_169_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_169", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_170_fir() => TestTools.VerifyComputeGraph("ComputeSingle_170", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_170_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_170", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_171_fir() => TestTools.VerifyComputeGraph("ComputeSingle_171", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_171_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_171", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_172_fir() => TestTools.VerifyComputeGraph("ComputeSingle_172", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_172_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_172", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_173_fir() => TestTools.VerifyComputeGraph("ComputeSingle_173", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_173_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_173", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_174_fir() => TestTools.VerifyComputeGraph("ComputeSingle_174", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_174_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_174", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_175_fir() => TestTools.VerifyComputeGraph("ComputeSingle_175", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_175_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_175", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_176_fir() => TestTools.VerifyComputeGraph("ComputeSingle_176", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_176_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_176", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_177_fir() => TestTools.VerifyComputeGraph("ComputeSingle_177", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_177_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_177", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_178_fir() => TestTools.VerifyComputeGraph("ComputeSingle_178", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_178_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_178", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_179_fir() => TestTools.VerifyComputeGraph("ComputeSingle_179", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_179_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_179", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_180_fir() => TestTools.VerifyComputeGraph("ComputeSingle_180", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_180_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_180", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_181_fir() => TestTools.VerifyComputeGraph("ComputeSingle_181", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_181_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_181", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_182_fir() => TestTools.VerifyComputeGraph("ComputeSingle_182", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_182_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_182", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_183_fir() => TestTools.VerifyComputeGraph("ComputeSingle_183", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_183_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_183", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_184_fir() => TestTools.VerifyComputeGraph("ComputeSingle_184", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_184_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_184", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_185_fir() => TestTools.VerifyComputeGraph("ComputeSingle_185", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_185_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_185", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_186_fir() => TestTools.VerifyComputeGraph("ComputeSingle_186", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_186_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_186", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_187_fir() => TestTools.VerifyComputeGraph("ComputeSingle_187", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_187_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_187", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_188_fir() => TestTools.VerifyComputeGraph("ComputeSingle_188", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_188_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_188", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_189_fir() => TestTools.VerifyComputeGraph("ComputeSingle_189", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_189_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_189", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_190_fir() => TestTools.VerifyComputeGraph("ComputeSingle_190", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_190_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_190", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_191_fir() => TestTools.VerifyComputeGraph("ComputeSingle_191", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_191_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_191", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_192_fir() => TestTools.VerifyComputeGraph("ComputeSingle_192", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_192_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_192", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_193_fir() => TestTools.VerifyComputeGraph("ComputeSingle_193", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_193_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_193", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_194_fir() => TestTools.VerifyComputeGraph("ComputeSingle_194", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_194_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_194", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_195_fir() => TestTools.VerifyComputeGraph("ComputeSingle_195", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_195_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_195", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_196_fir() => TestTools.VerifyComputeGraph("ComputeSingle_196", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_196_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_196", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_197_fir() => TestTools.VerifyComputeGraph("ComputeSingle_197", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_197_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_197", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_198_fir() => TestTools.VerifyComputeGraph("ComputeSingle_198", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_198_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_198", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_199_fir() => TestTools.VerifyComputeGraph("ComputeSingle_199", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_199_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_199", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_200_fir() => TestTools.VerifyComputeGraph("ComputeSingle_200", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_200_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_200", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_201_fir() => TestTools.VerifyComputeGraph("ComputeSingle_201", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_201_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_201", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_202_fir() => TestTools.VerifyComputeGraph("ComputeSingle_202", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_202_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_202", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_203_fir() => TestTools.VerifyComputeGraph("ComputeSingle_203", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_203_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_203", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_204_fir() => TestTools.VerifyComputeGraph("ComputeSingle_204", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_204_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_204", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_205_fir() => TestTools.VerifyComputeGraph("ComputeSingle_205", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_205_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_205", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_206_fir() => TestTools.VerifyComputeGraph("ComputeSingle_206", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_206_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_206", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_207_fir() => TestTools.VerifyComputeGraph("ComputeSingle_207", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_207_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_207", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_208_fir() => TestTools.VerifyComputeGraph("ComputeSingle_208", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_208_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_208", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_209_fir() => TestTools.VerifyComputeGraph("ComputeSingle_209", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_209_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_209", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_210_fir() => TestTools.VerifyComputeGraph("ComputeSingle_210", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_210_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_210", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_211_fir() => TestTools.VerifyComputeGraph("ComputeSingle_211", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_211_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_211", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_212_fir() => TestTools.VerifyComputeGraph("ComputeSingle_212", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_212_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_212", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_213_fir() => TestTools.VerifyComputeGraph("ComputeSingle_213", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_213_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_213", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_214_fir() => TestTools.VerifyComputeGraph("ComputeSingle_214", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_214_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_214", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_215_fir() => TestTools.VerifyComputeGraph("ComputeSingle_215", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_215_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_215", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_216_fir() => TestTools.VerifyComputeGraph("ComputeSingle_216", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_216_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_216", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_217_fir() => TestTools.VerifyComputeGraph("ComputeSingle_217", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_217_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_217", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_218_fir() => TestTools.VerifyComputeGraph("ComputeSingle_218", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_218_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_218", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_219_fir() => TestTools.VerifyComputeGraph("ComputeSingle_219", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_219_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_219", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_220_fir() => TestTools.VerifyComputeGraph("ComputeSingle_220", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_220_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_220", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_221_fir() => TestTools.VerifyComputeGraph("ComputeSingle_221", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_221_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_221", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_222_fir() => TestTools.VerifyComputeGraph("ComputeSingle_222", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_222_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_222", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_223_fir() => TestTools.VerifyComputeGraph("ComputeSingle_223", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_223_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_223", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_224_fir() => TestTools.VerifyComputeGraph("ComputeSingle_224", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_224_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_224", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_225_fir() => TestTools.VerifyComputeGraph("ComputeSingle_225", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_225_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_225", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_226_fir() => TestTools.VerifyComputeGraph("ComputeSingle_226", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_226_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_226", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_227_fir() => TestTools.VerifyComputeGraph("ComputeSingle_227", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_227_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_227", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_228_fir() => TestTools.VerifyComputeGraph("ComputeSingle_228", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_228_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_228", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_229_fir() => TestTools.VerifyComputeGraph("ComputeSingle_229", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_229_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_229", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_230_fir() => TestTools.VerifyComputeGraph("ComputeSingle_230", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_230_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_230", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_231_fir() => TestTools.VerifyComputeGraph("ComputeSingle_231", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_231_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_231", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_232_fir() => TestTools.VerifyComputeGraph("ComputeSingle_232", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_232_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_232", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_233_fir() => TestTools.VerifyComputeGraph("ComputeSingle_233", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_233_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_233", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_234_fir() => TestTools.VerifyComputeGraph("ComputeSingle_234", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_234_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_234", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_235_fir() => TestTools.VerifyComputeGraph("ComputeSingle_235", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_235_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_235", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_236_fir() => TestTools.VerifyComputeGraph("ComputeSingle_236", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_236_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_236", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_237_fir() => TestTools.VerifyComputeGraph("ComputeSingle_237", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_237_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_237", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_238_fir() => TestTools.VerifyComputeGraph("ComputeSingle_238", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_238_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_238", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_239_fir() => TestTools.VerifyComputeGraph("ComputeSingle_239", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_239_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_239", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_240_fir() => TestTools.VerifyComputeGraph("ComputeSingle_240", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_240_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_240", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_241_fir() => TestTools.VerifyComputeGraph("ComputeSingle_241", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_241_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_241", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_242_fir() => TestTools.VerifyComputeGraph("ComputeSingle_242", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_242_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_242", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_243_fir() => TestTools.VerifyComputeGraph("ComputeSingle_243", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_243_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_243", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_244_fir() => TestTools.VerifyComputeGraph("ComputeSingle_244", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_244_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_244", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_245_fir() => TestTools.VerifyComputeGraph("ComputeSingle_245", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_245_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_245", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_246_fir() => TestTools.VerifyComputeGraph("ComputeSingle_246", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_246_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_246", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_247_fir() => TestTools.VerifyComputeGraph("ComputeSingle_247", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_247_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_247", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_248_fir() => TestTools.VerifyComputeGraph("ComputeSingle_248", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_248_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_248", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_249_fir() => TestTools.VerifyComputeGraph("ComputeSingle_249", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_249_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_249", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_250_fir() => TestTools.VerifyComputeGraph("ComputeSingle_250", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_250_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_250", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_251_fir() => TestTools.VerifyComputeGraph("ComputeSingle_251", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_251_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_251", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_252_fir() => TestTools.VerifyComputeGraph("ComputeSingle_252", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_252_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_252", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_253_fir() => TestTools.VerifyComputeGraph("ComputeSingle_253", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_253_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_253", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_254_fir() => TestTools.VerifyComputeGraph("ComputeSingle_254", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_254_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_254", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_255_fir() => TestTools.VerifyComputeGraph("ComputeSingle_255", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_255_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_255", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_256_fir() => TestTools.VerifyComputeGraph("ComputeSingle_256", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_256_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_256", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_257_fir() => TestTools.VerifyComputeGraph("ComputeSingle_257", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_257_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_257", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_258_fir() => TestTools.VerifyComputeGraph("ComputeSingle_258", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_258_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_258", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_259_fir() => TestTools.VerifyComputeGraph("ComputeSingle_259", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_259_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_259", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_260_fir() => TestTools.VerifyComputeGraph("ComputeSingle_260", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_260_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_260", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_261_fir() => TestTools.VerifyComputeGraph("ComputeSingle_261", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_261_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_261", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_262_fir() => TestTools.VerifyComputeGraph("ComputeSingle_262", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_262_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_262", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_263_fir() => TestTools.VerifyComputeGraph("ComputeSingle_263", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_263_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_263", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_264_fir() => TestTools.VerifyComputeGraph("ComputeSingle_264", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_264_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_264", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_265_fir() => TestTools.VerifyComputeGraph("ComputeSingle_265", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_265_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_265", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_266_fir() => TestTools.VerifyComputeGraph("ComputeSingle_266", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_266_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_266", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_267_fir() => TestTools.VerifyComputeGraph("ComputeSingle_267", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_267_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_267", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_268_fir() => TestTools.VerifyComputeGraph("ComputeSingle_268", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_268_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_268", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_269_fir() => TestTools.VerifyComputeGraph("ComputeSingle_269", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_269_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_269", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_270_fir() => TestTools.VerifyComputeGraph("ComputeSingle_270", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_270_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_270", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_271_fir() => TestTools.VerifyComputeGraph("ComputeSingle_271", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_271_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_271", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_272_fir() => TestTools.VerifyComputeGraph("ComputeSingle_272", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_272_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_272", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_273_fir() => TestTools.VerifyComputeGraph("ComputeSingle_273", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_273_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_273", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_274_fir() => TestTools.VerifyComputeGraph("ComputeSingle_274", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_274_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_274", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_275_fir() => TestTools.VerifyComputeGraph("ComputeSingle_275", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_275_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_275", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_276_fir() => TestTools.VerifyComputeGraph("ComputeSingle_276", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_276_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_276", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_277_fir() => TestTools.VerifyComputeGraph("ComputeSingle_277", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_277_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_277", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_278_fir() => TestTools.VerifyComputeGraph("ComputeSingle_278", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_278_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_278", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_279_fir() => TestTools.VerifyComputeGraph("ComputeSingle_279", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_279_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_279", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_280_fir() => TestTools.VerifyComputeGraph("ComputeSingle_280", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_280_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_280", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_281_fir() => TestTools.VerifyComputeGraph("ComputeSingle_281", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_281_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_281", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_282_fir() => TestTools.VerifyComputeGraph("ComputeSingle_282", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_282_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_282", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_283_fir() => TestTools.VerifyComputeGraph("ComputeSingle_283", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_283_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_283", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_284_fir() => TestTools.VerifyComputeGraph("ComputeSingle_284", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_284_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_284", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_285_fir() => TestTools.VerifyComputeGraph("ComputeSingle_285", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_285_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_285", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_286_fir() => TestTools.VerifyComputeGraph("ComputeSingle_286", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_286_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_286", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_287_fir() => TestTools.VerifyComputeGraph("ComputeSingle_287", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_287_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_287", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_288_fir() => TestTools.VerifyComputeGraph("ComputeSingle_288", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_288_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_288", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_289_fir() => TestTools.VerifyComputeGraph("ComputeSingle_289", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_289_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_289", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_290_fir() => TestTools.VerifyComputeGraph("ComputeSingle_290", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_290_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_290", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_291_fir() => TestTools.VerifyComputeGraph("ComputeSingle_291", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_291_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_291", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_292_fir() => TestTools.VerifyComputeGraph("ComputeSingle_292", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_292_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_292", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_293_fir() => TestTools.VerifyComputeGraph("ComputeSingle_293", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_293_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_293", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_294_fir() => TestTools.VerifyComputeGraph("ComputeSingle_294", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_294_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_294", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_295_fir() => TestTools.VerifyComputeGraph("ComputeSingle_295", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_295_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_295", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_296_fir() => TestTools.VerifyComputeGraph("ComputeSingle_296", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_296_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_296", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_297_fir() => TestTools.VerifyComputeGraph("ComputeSingle_297", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_297_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_297", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_298_fir() => TestTools.VerifyComputeGraph("ComputeSingle_298", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_298_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_298", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_299_fir() => TestTools.VerifyComputeGraph("ComputeSingle_299", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_299_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_299", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_300_fir() => TestTools.VerifyComputeGraph("ComputeSingle_300", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_300_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_300", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_301_fir() => TestTools.VerifyComputeGraph("ComputeSingle_301", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_301_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_301", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_302_fir() => TestTools.VerifyComputeGraph("ComputeSingle_302", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_302_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_302", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_303_fir() => TestTools.VerifyComputeGraph("ComputeSingle_303", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_303_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_303", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_304_fir() => TestTools.VerifyComputeGraph("ComputeSingle_304", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_304_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_304", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_305_fir() => TestTools.VerifyComputeGraph("ComputeSingle_305", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_305_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_305", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_306_fir() => TestTools.VerifyComputeGraph("ComputeSingle_306", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_306_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_306", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_307_fir() => TestTools.VerifyComputeGraph("ComputeSingle_307", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_307_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_307", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_308_fir() => TestTools.VerifyComputeGraph("ComputeSingle_308", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_308_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_308", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_309_fir() => TestTools.VerifyComputeGraph("ComputeSingle_309", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_309_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_309", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_310_fir() => TestTools.VerifyComputeGraph("ComputeSingle_310", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_310_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_310", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_311_fir() => TestTools.VerifyComputeGraph("ComputeSingle_311", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_311_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_311", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_312_fir() => TestTools.VerifyComputeGraph("ComputeSingle_312", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_312_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_312", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_313_fir() => TestTools.VerifyComputeGraph("ComputeSingle_313", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_313_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_313", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_314_fir() => TestTools.VerifyComputeGraph("ComputeSingle_314", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_314_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_314", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_315_fir() => TestTools.VerifyComputeGraph("ComputeSingle_315", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_315_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_315", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_316_fir() => TestTools.VerifyComputeGraph("ComputeSingle_316", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_316_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_316", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_317_fir() => TestTools.VerifyComputeGraph("ComputeSingle_317", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_317_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_317", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_318_fir() => TestTools.VerifyComputeGraph("ComputeSingle_318", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_318_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_318", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_319_fir() => TestTools.VerifyComputeGraph("ComputeSingle_319", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_319_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_319", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_320_fir() => TestTools.VerifyComputeGraph("ComputeSingle_320", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_320_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_320", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_321_fir() => TestTools.VerifyComputeGraph("ComputeSingle_321", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_321_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_321", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_322_fir() => TestTools.VerifyComputeGraph("ComputeSingle_322", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_322_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_322", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_323_fir() => TestTools.VerifyComputeGraph("ComputeSingle_323", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_323_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_323", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_324_fir() => TestTools.VerifyComputeGraph("ComputeSingle_324", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_324_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_324", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_325_fir() => TestTools.VerifyComputeGraph("ComputeSingle_325", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_325_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_325", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_326_fir() => TestTools.VerifyComputeGraph("ComputeSingle_326", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_326_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_326", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_327_fir() => TestTools.VerifyComputeGraph("ComputeSingle_327", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_327_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_327", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_328_fir() => TestTools.VerifyComputeGraph("ComputeSingle_328", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_328_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_328", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_329_fir() => TestTools.VerifyComputeGraph("ComputeSingle_329", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_329_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_329", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_330_fir() => TestTools.VerifyComputeGraph("ComputeSingle_330", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_330_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_330", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_331_fir() => TestTools.VerifyComputeGraph("ComputeSingle_331", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_331_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_331", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_332_fir() => TestTools.VerifyComputeGraph("ComputeSingle_332", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_332_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_332", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_333_fir() => TestTools.VerifyComputeGraph("ComputeSingle_333", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_333_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_333", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_334_fir() => TestTools.VerifyComputeGraph("ComputeSingle_334", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_334_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_334", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_335_fir() => TestTools.VerifyComputeGraph("ComputeSingle_335", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_335_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_335", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_336_fir() => TestTools.VerifyComputeGraph("ComputeSingle_336", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_336_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_336", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_337_fir() => TestTools.VerifyComputeGraph("ComputeSingle_337", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_337_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_337", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_338_fir() => TestTools.VerifyComputeGraph("ComputeSingle_338", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_338_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_338", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_339_fir() => TestTools.VerifyComputeGraph("ComputeSingle_339", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_339_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_339", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_340_fir() => TestTools.VerifyComputeGraph("ComputeSingle_340", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_340_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_340", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_341_fir() => TestTools.VerifyComputeGraph("ComputeSingle_341", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_341_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_341", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_342_fir() => TestTools.VerifyComputeGraph("ComputeSingle_342", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_342_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_342", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_343_fir() => TestTools.VerifyComputeGraph("ComputeSingle_343", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_343_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_343", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_344_fir() => TestTools.VerifyComputeGraph("ComputeSingle_344", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_344_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_344", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_345_fir() => TestTools.VerifyComputeGraph("ComputeSingle_345", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_345_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_345", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_346_fir() => TestTools.VerifyComputeGraph("ComputeSingle_346", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_346_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_346", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_347_fir() => TestTools.VerifyComputeGraph("ComputeSingle_347", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_347_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_347", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_348_fir() => TestTools.VerifyComputeGraph("ComputeSingle_348", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_348_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_348", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_349_fir() => TestTools.VerifyComputeGraph("ComputeSingle_349", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_349_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_349", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_350_fir() => TestTools.VerifyComputeGraph("ComputeSingle_350", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_350_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_350", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_351_fir() => TestTools.VerifyComputeGraph("ComputeSingle_351", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_351_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_351", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_352_fir() => TestTools.VerifyComputeGraph("ComputeSingle_352", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_352_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_352", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_353_fir() => TestTools.VerifyComputeGraph("ComputeSingle_353", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_353_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_353", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_354_fir() => TestTools.VerifyComputeGraph("ComputeSingle_354", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_354_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_354", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_355_fir() => TestTools.VerifyComputeGraph("ComputeSingle_355", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_355_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_355", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_356_fir() => TestTools.VerifyComputeGraph("ComputeSingle_356", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_356_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_356", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_357_fir() => TestTools.VerifyComputeGraph("ComputeSingle_357", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_357_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_357", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_358_fir() => TestTools.VerifyComputeGraph("ComputeSingle_358", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_358_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_358", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_359_fir() => TestTools.VerifyComputeGraph("ComputeSingle_359", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_359_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_359", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_360_fir() => TestTools.VerifyComputeGraph("ComputeSingle_360", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_360_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_360", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_361_fir() => TestTools.VerifyComputeGraph("ComputeSingle_361", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_361_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_361", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_362_fir() => TestTools.VerifyComputeGraph("ComputeSingle_362", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_362_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_362", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_363_fir() => TestTools.VerifyComputeGraph("ComputeSingle_363", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_363_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_363", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_364_fir() => TestTools.VerifyComputeGraph("ComputeSingle_364", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_364_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_364", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_365_fir() => TestTools.VerifyComputeGraph("ComputeSingle_365", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_365_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_365", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_366_fir() => TestTools.VerifyComputeGraph("ComputeSingle_366", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_366_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_366", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_367_fir() => TestTools.VerifyComputeGraph("ComputeSingle_367", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_367_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_367", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_368_fir() => TestTools.VerifyComputeGraph("ComputeSingle_368", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_368_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_368", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_369_fir() => TestTools.VerifyComputeGraph("ComputeSingle_369", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_369_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_369", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_370_fir() => TestTools.VerifyComputeGraph("ComputeSingle_370", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_370_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_370", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_371_fir() => TestTools.VerifyComputeGraph("ComputeSingle_371", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_371_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_371", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_372_fir() => TestTools.VerifyComputeGraph("ComputeSingle_372", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_372_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_372", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_373_fir() => TestTools.VerifyComputeGraph("ComputeSingle_373", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_373_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_373", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_374_fir() => TestTools.VerifyComputeGraph("ComputeSingle_374", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_374_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_374", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_375_fir() => TestTools.VerifyComputeGraph("ComputeSingle_375", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_375_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_375", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_376_fir() => TestTools.VerifyComputeGraph("ComputeSingle_376", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_376_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_376", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_377_fir() => TestTools.VerifyComputeGraph("ComputeSingle_377", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_377_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_377", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_378_fir() => TestTools.VerifyComputeGraph("ComputeSingle_378", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_378_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_378", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_379_fir() => TestTools.VerifyComputeGraph("ComputeSingle_379", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_379_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_379", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_380_fir() => TestTools.VerifyComputeGraph("ComputeSingle_380", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_380_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_380", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_381_fir() => TestTools.VerifyComputeGraph("ComputeSingle_381", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_381_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_381", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_382_fir() => TestTools.VerifyComputeGraph("ComputeSingle_382", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_382_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_382", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_383_fir() => TestTools.VerifyComputeGraph("ComputeSingle_383", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_383_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_383", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_384_fir() => TestTools.VerifyComputeGraph("ComputeSingle_384", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_384_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_384", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_385_fir() => TestTools.VerifyComputeGraph("ComputeSingle_385", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_385_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_385", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_386_fir() => TestTools.VerifyComputeGraph("ComputeSingle_386", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_386_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_386", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_387_fir() => TestTools.VerifyComputeGraph("ComputeSingle_387", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_387_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_387", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_388_fir() => TestTools.VerifyComputeGraph("ComputeSingle_388", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_388_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_388", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_389_fir() => TestTools.VerifyComputeGraph("ComputeSingle_389", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_389_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_389", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_390_fir() => TestTools.VerifyComputeGraph("ComputeSingle_390", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_390_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_390", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_391_fir() => TestTools.VerifyComputeGraph("ComputeSingle_391", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_391_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_391", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_392_fir() => TestTools.VerifyComputeGraph("ComputeSingle_392", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_392_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_392", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_393_fir() => TestTools.VerifyComputeGraph("ComputeSingle_393", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_393_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_393", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_394_fir() => TestTools.VerifyComputeGraph("ComputeSingle_394", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_394_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_394", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_395_fir() => TestTools.VerifyComputeGraph("ComputeSingle_395", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_395_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_395", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_396_fir() => TestTools.VerifyComputeGraph("ComputeSingle_396", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_396_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_396", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_397_fir() => TestTools.VerifyComputeGraph("ComputeSingle_397", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_397_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_397", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_398_fir() => TestTools.VerifyComputeGraph("ComputeSingle_398", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_398_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_398", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_399_fir() => TestTools.VerifyComputeGraph("ComputeSingle_399", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_399_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_399", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_400_fir() => TestTools.VerifyComputeGraph("ComputeSingle_400", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_400_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_400", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_401_fir() => TestTools.VerifyComputeGraph("ComputeSingle_401", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_401_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_401", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_402_fir() => TestTools.VerifyComputeGraph("ComputeSingle_402", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_402_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_402", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_403_fir() => TestTools.VerifyComputeGraph("ComputeSingle_403", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_403_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_403", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_404_fir() => TestTools.VerifyComputeGraph("ComputeSingle_404", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_404_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_404", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_405_fir() => TestTools.VerifyComputeGraph("ComputeSingle_405", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_405_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_405", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_406_fir() => TestTools.VerifyComputeGraph("ComputeSingle_406", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_406_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_406", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_407_fir() => TestTools.VerifyComputeGraph("ComputeSingle_407", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_407_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_407", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_408_fir() => TestTools.VerifyComputeGraph("ComputeSingle_408", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_408_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_408", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_409_fir() => TestTools.VerifyComputeGraph("ComputeSingle_409", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_409_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_409", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_410_fir() => TestTools.VerifyComputeGraph("ComputeSingle_410", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_410_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_410", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_411_fir() => TestTools.VerifyComputeGraph("ComputeSingle_411", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_411_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_411", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_412_fir() => TestTools.VerifyComputeGraph("ComputeSingle_412", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_412_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_412", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_413_fir() => TestTools.VerifyComputeGraph("ComputeSingle_413", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_413_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_413", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_414_fir() => TestTools.VerifyComputeGraph("ComputeSingle_414", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_414_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_414", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_415_fir() => TestTools.VerifyComputeGraph("ComputeSingle_415", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_415_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_415", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_416_fir() => TestTools.VerifyComputeGraph("ComputeSingle_416", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_416_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_416", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_417_fir() => TestTools.VerifyComputeGraph("ComputeSingle_417", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_417_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_417", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_418_fir() => TestTools.VerifyComputeGraph("ComputeSingle_418", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_418_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_418", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_419_fir() => TestTools.VerifyComputeGraph("ComputeSingle_419", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_419_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_419", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_420_fir() => TestTools.VerifyComputeGraph("ComputeSingle_420", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_420_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_420", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_421_fir() => TestTools.VerifyComputeGraph("ComputeSingle_421", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_421_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_421", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_422_fir() => TestTools.VerifyComputeGraph("ComputeSingle_422", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_422_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_422", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_423_fir() => TestTools.VerifyComputeGraph("ComputeSingle_423", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_423_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_423", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_424_fir() => TestTools.VerifyComputeGraph("ComputeSingle_424", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_424_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_424", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_425_fir() => TestTools.VerifyComputeGraph("ComputeSingle_425", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_425_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_425", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_426_fir() => TestTools.VerifyComputeGraph("ComputeSingle_426", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_426_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_426", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_427_fir() => TestTools.VerifyComputeGraph("ComputeSingle_427", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_427_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_427", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_428_fir() => TestTools.VerifyComputeGraph("ComputeSingle_428", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_428_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_428", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_429_fir() => TestTools.VerifyComputeGraph("ComputeSingle_429", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_429_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_429", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_430_fir() => TestTools.VerifyComputeGraph("ComputeSingle_430", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_430_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_430", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_431_fir() => TestTools.VerifyComputeGraph("ComputeSingle_431", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_431_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_431", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_432_fir() => TestTools.VerifyComputeGraph("ComputeSingle_432", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_432_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_432", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_433_fir() => TestTools.VerifyComputeGraph("ComputeSingle_433", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_433_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_433", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_434_fir() => TestTools.VerifyComputeGraph("ComputeSingle_434", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_434_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_434", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_435_fir() => TestTools.VerifyComputeGraph("ComputeSingle_435", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_435_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_435", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_436_fir() => TestTools.VerifyComputeGraph("ComputeSingle_436", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_436_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_436", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_437_fir() => TestTools.VerifyComputeGraph("ComputeSingle_437", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_437_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_437", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_438_fir() => TestTools.VerifyComputeGraph("ComputeSingle_438", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_438_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_438", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_439_fir() => TestTools.VerifyComputeGraph("ComputeSingle_439", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_439_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_439", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_440_fir() => TestTools.VerifyComputeGraph("ComputeSingle_440", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_440_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_440", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_441_fir() => TestTools.VerifyComputeGraph("ComputeSingle_441", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_441_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_441", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_442_fir() => TestTools.VerifyComputeGraph("ComputeSingle_442", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_442_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_442", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_443_fir() => TestTools.VerifyComputeGraph("ComputeSingle_443", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_443_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_443", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_444_fir() => TestTools.VerifyComputeGraph("ComputeSingle_444", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_444_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_444", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_445_fir() => TestTools.VerifyComputeGraph("ComputeSingle_445", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_445_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_445", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_446_fir() => TestTools.VerifyComputeGraph("ComputeSingle_446", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_446_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_446", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_447_fir() => TestTools.VerifyComputeGraph("ComputeSingle_447", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_447_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_447", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_448_fir() => TestTools.VerifyComputeGraph("ComputeSingle_448", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_448_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_448", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_449_fir() => TestTools.VerifyComputeGraph("ComputeSingle_449", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_449_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_449", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_450_fir() => TestTools.VerifyComputeGraph("ComputeSingle_450", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_450_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_450", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_451_fir() => TestTools.VerifyComputeGraph("ComputeSingle_451", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_451_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_451", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_452_fir() => TestTools.VerifyComputeGraph("ComputeSingle_452", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_452_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_452", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_453_fir() => TestTools.VerifyComputeGraph("ComputeSingle_453", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_453_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_453", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_454_fir() => TestTools.VerifyComputeGraph("ComputeSingle_454", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_454_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_454", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_455_fir() => TestTools.VerifyComputeGraph("ComputeSingle_455", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_455_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_455", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_456_fir() => TestTools.VerifyComputeGraph("ComputeSingle_456", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_456_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_456", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_457_fir() => TestTools.VerifyComputeGraph("ComputeSingle_457", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_457_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_457", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_458_fir() => TestTools.VerifyComputeGraph("ComputeSingle_458", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_458_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_458", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_459_fir() => TestTools.VerifyComputeGraph("ComputeSingle_459", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_459_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_459", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_460_fir() => TestTools.VerifyComputeGraph("ComputeSingle_460", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_460_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_460", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_461_fir() => TestTools.VerifyComputeGraph("ComputeSingle_461", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_461_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_461", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_462_fir() => TestTools.VerifyComputeGraph("ComputeSingle_462", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_462_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_462", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_463_fir() => TestTools.VerifyComputeGraph("ComputeSingle_463", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_463_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_463", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_464_fir() => TestTools.VerifyComputeGraph("ComputeSingle_464", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_464_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_464", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_465_fir() => TestTools.VerifyComputeGraph("ComputeSingle_465", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_465_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_465", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_466_fir() => TestTools.VerifyComputeGraph("ComputeSingle_466", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_466_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_466", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_467_fir() => TestTools.VerifyComputeGraph("ComputeSingle_467", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_467_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_467", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_468_fir() => TestTools.VerifyComputeGraph("ComputeSingle_468", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_468_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_468", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_469_fir() => TestTools.VerifyComputeGraph("ComputeSingle_469", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_469_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_469", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_470_fir() => TestTools.VerifyComputeGraph("ComputeSingle_470", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_470_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_470", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_471_fir() => TestTools.VerifyComputeGraph("ComputeSingle_471", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_471_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_471", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_472_fir() => TestTools.VerifyComputeGraph("ComputeSingle_472", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_472_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_472", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_473_fir() => TestTools.VerifyComputeGraph("ComputeSingle_473", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_473_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_473", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_474_fir() => TestTools.VerifyComputeGraph("ComputeSingle_474", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_474_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_474", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_475_fir() => TestTools.VerifyComputeGraph("ComputeSingle_475", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_475_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_475", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_476_fir() => TestTools.VerifyComputeGraph("ComputeSingle_476", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_476_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_476", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_477_fir() => TestTools.VerifyComputeGraph("ComputeSingle_477", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_477_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_477", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_478_fir() => TestTools.VerifyComputeGraph("ComputeSingle_478", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_478_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_478", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_479_fir() => TestTools.VerifyComputeGraph("ComputeSingle_479", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_479_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_479", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_480_fir() => TestTools.VerifyComputeGraph("ComputeSingle_480", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_480_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_480", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_481_fir() => TestTools.VerifyComputeGraph("ComputeSingle_481", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_481_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_481", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_482_fir() => TestTools.VerifyComputeGraph("ComputeSingle_482", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_482_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_482", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_483_fir() => TestTools.VerifyComputeGraph("ComputeSingle_483", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_483_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_483", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_484_fir() => TestTools.VerifyComputeGraph("ComputeSingle_484", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_484_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_484", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_485_fir() => TestTools.VerifyComputeGraph("ComputeSingle_485", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_485_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_485", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_486_fir() => TestTools.VerifyComputeGraph("ComputeSingle_486", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_486_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_486", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_487_fir() => TestTools.VerifyComputeGraph("ComputeSingle_487", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_487_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_487", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_488_fir() => TestTools.VerifyComputeGraph("ComputeSingle_488", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_488_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_488", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_489_fir() => TestTools.VerifyComputeGraph("ComputeSingle_489", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_489_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_489", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_490_fir() => TestTools.VerifyComputeGraph("ComputeSingle_490", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_490_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_490", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_491_fir() => TestTools.VerifyComputeGraph("ComputeSingle_491", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_491_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_491", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_492_fir() => TestTools.VerifyComputeGraph("ComputeSingle_492", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_492_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_492", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_493_fir() => TestTools.VerifyComputeGraph("ComputeSingle_493", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_493_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_493", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_494_fir() => TestTools.VerifyComputeGraph("ComputeSingle_494", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_494_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_494", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_495_fir() => TestTools.VerifyComputeGraph("ComputeSingle_495", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_495_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_495", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_496_fir() => TestTools.VerifyComputeGraph("ComputeSingle_496", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_496_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_496", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_497_fir() => TestTools.VerifyComputeGraph("ComputeSingle_497", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_497_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_497", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_498_fir() => TestTools.VerifyComputeGraph("ComputeSingle_498", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_498_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_498", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_499_fir() => TestTools.VerifyComputeGraph("ComputeSingle_499", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_499_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_499", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_500_fir() => TestTools.VerifyComputeGraph("ComputeSingle_500", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_500_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_500", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_501_fir() => TestTools.VerifyComputeGraph("ComputeSingle_501", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_501_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_501", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_502_fir() => TestTools.VerifyComputeGraph("ComputeSingle_502", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_502_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_502", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_503_fir() => TestTools.VerifyComputeGraph("ComputeSingle_503", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_503_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_503", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_504_fir() => TestTools.VerifyComputeGraph("ComputeSingle_504", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_504_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_504", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_505_fir() => TestTools.VerifyComputeGraph("ComputeSingle_505", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_505_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_505", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_506_fir() => TestTools.VerifyComputeGraph("ComputeSingle_506", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_506_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_506", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_507_fir() => TestTools.VerifyComputeGraph("ComputeSingle_507", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_507_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_507", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_508_fir() => TestTools.VerifyComputeGraph("ComputeSingle_508", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_508_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_508", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_509_fir() => TestTools.VerifyComputeGraph("ComputeSingle_509", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_509_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_509", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_510_fir() => TestTools.VerifyComputeGraph("ComputeSingle_510", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_510_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_510", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_511_fir() => TestTools.VerifyComputeGraph("ComputeSingle_511", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_511_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_511", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_512_fir() => TestTools.VerifyComputeGraph("ComputeSingle_512", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_512_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_512", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_513_fir() => TestTools.VerifyComputeGraph("ComputeSingle_513", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_513_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_513", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_514_fir() => TestTools.VerifyComputeGraph("ComputeSingle_514", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_514_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_514", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_515_fir() => TestTools.VerifyComputeGraph("ComputeSingle_515", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_515_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_515", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_516_fir() => TestTools.VerifyComputeGraph("ComputeSingle_516", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_516_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_516", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_517_fir() => TestTools.VerifyComputeGraph("ComputeSingle_517", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_517_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_517", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_518_fir() => TestTools.VerifyComputeGraph("ComputeSingle_518", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_518_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_518", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_519_fir() => TestTools.VerifyComputeGraph("ComputeSingle_519", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_519_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_519", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_520_fir() => TestTools.VerifyComputeGraph("ComputeSingle_520", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_520_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_520", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_521_fir() => TestTools.VerifyComputeGraph("ComputeSingle_521", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_521_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_521", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_522_fir() => TestTools.VerifyComputeGraph("ComputeSingle_522", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_522_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_522", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_523_fir() => TestTools.VerifyComputeGraph("ComputeSingle_523", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_523_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_523", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_524_fir() => TestTools.VerifyComputeGraph("ComputeSingle_524", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_524_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_524", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_525_fir() => TestTools.VerifyComputeGraph("ComputeSingle_525", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_525_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_525", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_526_fir() => TestTools.VerifyComputeGraph("ComputeSingle_526", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_526_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_526", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_527_fir() => TestTools.VerifyComputeGraph("ComputeSingle_527", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_527_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_527", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_528_fir() => TestTools.VerifyComputeGraph("ComputeSingle_528", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_528_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_528", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_529_fir() => TestTools.VerifyComputeGraph("ComputeSingle_529", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_529_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_529", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_530_fir() => TestTools.VerifyComputeGraph("ComputeSingle_530", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_530_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_530", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_531_fir() => TestTools.VerifyComputeGraph("ComputeSingle_531", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_531_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_531", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_532_fir() => TestTools.VerifyComputeGraph("ComputeSingle_532", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_532_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_532", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_533_fir() => TestTools.VerifyComputeGraph("ComputeSingle_533", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_533_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_533", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_534_fir() => TestTools.VerifyComputeGraph("ComputeSingle_534", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_534_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_534", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_535_fir() => TestTools.VerifyComputeGraph("ComputeSingle_535", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_535_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_535", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_536_fir() => TestTools.VerifyComputeGraph("ComputeSingle_536", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_536_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_536", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_537_fir() => TestTools.VerifyComputeGraph("ComputeSingle_537", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_537_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_537", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_538_fir() => TestTools.VerifyComputeGraph("ComputeSingle_538", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_538_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_538", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_539_fir() => TestTools.VerifyComputeGraph("ComputeSingle_539", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_539_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_539", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_540_fir() => TestTools.VerifyComputeGraph("ComputeSingle_540", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_540_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_540", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_541_fir() => TestTools.VerifyComputeGraph("ComputeSingle_541", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_541_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_541", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_542_fir() => TestTools.VerifyComputeGraph("ComputeSingle_542", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_542_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_542", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_543_fir() => TestTools.VerifyComputeGraph("ComputeSingle_543", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_543_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_543", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_544_fir() => TestTools.VerifyComputeGraph("ComputeSingle_544", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_544_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_544", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_545_fir() => TestTools.VerifyComputeGraph("ComputeSingle_545", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_545_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_545", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_546_fir() => TestTools.VerifyComputeGraph("ComputeSingle_546", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_546_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_546", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_547_fir() => TestTools.VerifyComputeGraph("ComputeSingle_547", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_547_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_547", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_548_fir() => TestTools.VerifyComputeGraph("ComputeSingle_548", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_548_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_548", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_549_fir() => TestTools.VerifyComputeGraph("ComputeSingle_549", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_549_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_549", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_550_fir() => TestTools.VerifyComputeGraph("ComputeSingle_550", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_550_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_550", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_551_fir() => TestTools.VerifyComputeGraph("ComputeSingle_551", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_551_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_551", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_552_fir() => TestTools.VerifyComputeGraph("ComputeSingle_552", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_552_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_552", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_553_fir() => TestTools.VerifyComputeGraph("ComputeSingle_553", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_553_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_553", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_554_fir() => TestTools.VerifyComputeGraph("ComputeSingle_554", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_554_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_554", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_555_fir() => TestTools.VerifyComputeGraph("ComputeSingle_555", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_555_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_555", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_556_fir() => TestTools.VerifyComputeGraph("ComputeSingle_556", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_556_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_556", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_557_fir() => TestTools.VerifyComputeGraph("ComputeSingle_557", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_557_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_557", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_558_fir() => TestTools.VerifyComputeGraph("ComputeSingle_558", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_558_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_558", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_559_fir() => TestTools.VerifyComputeGraph("ComputeSingle_559", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_559_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_559", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_560_fir() => TestTools.VerifyComputeGraph("ComputeSingle_560", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_560_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_560", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_561_fir() => TestTools.VerifyComputeGraph("ComputeSingle_561", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_561_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_561", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_562_fir() => TestTools.VerifyComputeGraph("ComputeSingle_562", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_562_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_562", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_563_fir() => TestTools.VerifyComputeGraph("ComputeSingle_563", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_563_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_563", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_564_fir() => TestTools.VerifyComputeGraph("ComputeSingle_564", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_564_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_564", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_565_fir() => TestTools.VerifyComputeGraph("ComputeSingle_565", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_565_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_565", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_566_fir() => TestTools.VerifyComputeGraph("ComputeSingle_566", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_566_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_566", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_567_fir() => TestTools.VerifyComputeGraph("ComputeSingle_567", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_567_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_567", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_568_fir() => TestTools.VerifyComputeGraph("ComputeSingle_568", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_568_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_568", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_569_fir() => TestTools.VerifyComputeGraph("ComputeSingle_569", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_569_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_569", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_570_fir() => TestTools.VerifyComputeGraph("ComputeSingle_570", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_570_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_570", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_571_fir() => TestTools.VerifyComputeGraph("ComputeSingle_571", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_571_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_571", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_572_fir() => TestTools.VerifyComputeGraph("ComputeSingle_572", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_572_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_572", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_573_fir() => TestTools.VerifyComputeGraph("ComputeSingle_573", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_573_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_573", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_574_fir() => TestTools.VerifyComputeGraph("ComputeSingle_574", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_574_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_574", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_575_fir() => TestTools.VerifyComputeGraph("ComputeSingle_575", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_575_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_575", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_576_fir() => TestTools.VerifyComputeGraph("ComputeSingle_576", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_576_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_576", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_577_fir() => TestTools.VerifyComputeGraph("ComputeSingle_577", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_577_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_577", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_578_fir() => TestTools.VerifyComputeGraph("ComputeSingle_578", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_578_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_578", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_579_fir() => TestTools.VerifyComputeGraph("ComputeSingle_579", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_579_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_579", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_580_fir() => TestTools.VerifyComputeGraph("ComputeSingle_580", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_580_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_580", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_581_fir() => TestTools.VerifyComputeGraph("ComputeSingle_581", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_581_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_581", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_582_fir() => TestTools.VerifyComputeGraph("ComputeSingle_582", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_582_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_582", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_583_fir() => TestTools.VerifyComputeGraph("ComputeSingle_583", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_583_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_583", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_584_fir() => TestTools.VerifyComputeGraph("ComputeSingle_584", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_584_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_584", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_585_fir() => TestTools.VerifyComputeGraph("ComputeSingle_585", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_585_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_585", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_586_fir() => TestTools.VerifyComputeGraph("ComputeSingle_586", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_586_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_586", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_587_fir() => TestTools.VerifyComputeGraph("ComputeSingle_587", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_587_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_587", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_588_fir() => TestTools.VerifyComputeGraph("ComputeSingle_588", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_588_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_588", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_589_fir() => TestTools.VerifyComputeGraph("ComputeSingle_589", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_589_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_589", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_590_fir() => TestTools.VerifyComputeGraph("ComputeSingle_590", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_590_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_590", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_591_fir() => TestTools.VerifyComputeGraph("ComputeSingle_591", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_591_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_591", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_592_fir() => TestTools.VerifyComputeGraph("ComputeSingle_592", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_592_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_592", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_593_fir() => TestTools.VerifyComputeGraph("ComputeSingle_593", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_593_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_593", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_594_fir() => TestTools.VerifyComputeGraph("ComputeSingle_594", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_594_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_594", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_595_fir() => TestTools.VerifyComputeGraph("ComputeSingle_595", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_595_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_595", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_596_fir() => TestTools.VerifyComputeGraph("ComputeSingle_596", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_596_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_596", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_597_fir() => TestTools.VerifyComputeGraph("ComputeSingle_597", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_597_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_597", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_598_fir() => TestTools.VerifyComputeGraph("ComputeSingle_598", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_598_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_598", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_599_fir() => TestTools.VerifyComputeGraph("ComputeSingle_599", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_599_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_599", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_600_fir() => TestTools.VerifyComputeGraph("ComputeSingle_600", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_600_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_600", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_601_fir() => TestTools.VerifyComputeGraph("ComputeSingle_601", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_601_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_601", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_602_fir() => TestTools.VerifyComputeGraph("ComputeSingle_602", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_602_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_602", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_603_fir() => TestTools.VerifyComputeGraph("ComputeSingle_603", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_603_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_603", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_604_fir() => TestTools.VerifyComputeGraph("ComputeSingle_604", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_604_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_604", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_605_fir() => TestTools.VerifyComputeGraph("ComputeSingle_605", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_605_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_605", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_606_fir() => TestTools.VerifyComputeGraph("ComputeSingle_606", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_606_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_606", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_607_fir() => TestTools.VerifyComputeGraph("ComputeSingle_607", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_607_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_607", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_608_fir() => TestTools.VerifyComputeGraph("ComputeSingle_608", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_608_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_608", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_609_fir() => TestTools.VerifyComputeGraph("ComputeSingle_609", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_609_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_609", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_610_fir() => TestTools.VerifyComputeGraph("ComputeSingle_610", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_610_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_610", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_611_fir() => TestTools.VerifyComputeGraph("ComputeSingle_611", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_611_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_611", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_612_fir() => TestTools.VerifyComputeGraph("ComputeSingle_612", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_612_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_612", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_613_fir() => TestTools.VerifyComputeGraph("ComputeSingle_613", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_613_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_613", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_614_fir() => TestTools.VerifyComputeGraph("ComputeSingle_614", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_614_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_614", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_615_fir() => TestTools.VerifyComputeGraph("ComputeSingle_615", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_615_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_615", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_616_fir() => TestTools.VerifyComputeGraph("ComputeSingle_616", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_616_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_616", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_617_fir() => TestTools.VerifyComputeGraph("ComputeSingle_617", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_617_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_617", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_618_fir() => TestTools.VerifyComputeGraph("ComputeSingle_618", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_618_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_618", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_619_fir() => TestTools.VerifyComputeGraph("ComputeSingle_619", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_619_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_619", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_620_fir() => TestTools.VerifyComputeGraph("ComputeSingle_620", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_620_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_620", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_621_fir() => TestTools.VerifyComputeGraph("ComputeSingle_621", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_621_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_621", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_622_fir() => TestTools.VerifyComputeGraph("ComputeSingle_622", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_622_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_622", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_623_fir() => TestTools.VerifyComputeGraph("ComputeSingle_623", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_623_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_623", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_624_fir() => TestTools.VerifyComputeGraph("ComputeSingle_624", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_624_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_624", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_625_fir() => TestTools.VerifyComputeGraph("ComputeSingle_625", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_625_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_625", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_626_fir() => TestTools.VerifyComputeGraph("ComputeSingle_626", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_626_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_626", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_627_fir() => TestTools.VerifyComputeGraph("ComputeSingle_627", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_627_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_627", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_628_fir() => TestTools.VerifyComputeGraph("ComputeSingle_628", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_628_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_628", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_629_fir() => TestTools.VerifyComputeGraph("ComputeSingle_629", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_629_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_629", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_630_fir() => TestTools.VerifyComputeGraph("ComputeSingle_630", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_630_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_630", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_631_fir() => TestTools.VerifyComputeGraph("ComputeSingle_631", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_631_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_631", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_632_fir() => TestTools.VerifyComputeGraph("ComputeSingle_632", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_632_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_632", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_633_fir() => TestTools.VerifyComputeGraph("ComputeSingle_633", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_633_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_633", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_634_fir() => TestTools.VerifyComputeGraph("ComputeSingle_634", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_634_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_634", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_635_fir() => TestTools.VerifyComputeGraph("ComputeSingle_635", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_635_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_635", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_636_fir() => TestTools.VerifyComputeGraph("ComputeSingle_636", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_636_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_636", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_637_fir() => TestTools.VerifyComputeGraph("ComputeSingle_637", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_637_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_637", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_638_fir() => TestTools.VerifyComputeGraph("ComputeSingle_638", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_638_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_638", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_639_fir() => TestTools.VerifyComputeGraph("ComputeSingle_639", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_639_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_639", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_640_fir() => TestTools.VerifyComputeGraph("ComputeSingle_640", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_640_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_640", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_641_fir() => TestTools.VerifyComputeGraph("ComputeSingle_641", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_641_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_641", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_642_fir() => TestTools.VerifyComputeGraph("ComputeSingle_642", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_642_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_642", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_643_fir() => TestTools.VerifyComputeGraph("ComputeSingle_643", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_643_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_643", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_644_fir() => TestTools.VerifyComputeGraph("ComputeSingle_644", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_644_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_644", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_645_fir() => TestTools.VerifyComputeGraph("ComputeSingle_645", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_645_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_645", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_646_fir() => TestTools.VerifyComputeGraph("ComputeSingle_646", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_646_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_646", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_647_fir() => TestTools.VerifyComputeGraph("ComputeSingle_647", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_647_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_647", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_648_fir() => TestTools.VerifyComputeGraph("ComputeSingle_648", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_648_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_648", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_649_fir() => TestTools.VerifyComputeGraph("ComputeSingle_649", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_649_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_649", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_650_fir() => TestTools.VerifyComputeGraph("ComputeSingle_650", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_650_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_650", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_651_fir() => TestTools.VerifyComputeGraph("ComputeSingle_651", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_651_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_651", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_652_fir() => TestTools.VerifyComputeGraph("ComputeSingle_652", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_652_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_652", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_653_fir() => TestTools.VerifyComputeGraph("ComputeSingle_653", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_653_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_653", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_654_fir() => TestTools.VerifyComputeGraph("ComputeSingle_654", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_654_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_654", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_655_fir() => TestTools.VerifyComputeGraph("ComputeSingle_655", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_655_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_655", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_656_fir() => TestTools.VerifyComputeGraph("ComputeSingle_656", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_656_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_656", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_657_fir() => TestTools.VerifyComputeGraph("ComputeSingle_657", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_657_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_657", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_658_fir() => TestTools.VerifyComputeGraph("ComputeSingle_658", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_658_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_658", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_659_fir() => TestTools.VerifyComputeGraph("ComputeSingle_659", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_659_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_659", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_660_fir() => TestTools.VerifyComputeGraph("ComputeSingle_660", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_660_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_660", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_661_fir() => TestTools.VerifyComputeGraph("ComputeSingle_661", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_661_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_661", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_662_fir() => TestTools.VerifyComputeGraph("ComputeSingle_662", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_662_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_662", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_663_fir() => TestTools.VerifyComputeGraph("ComputeSingle_663", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_663_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_663", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_664_fir() => TestTools.VerifyComputeGraph("ComputeSingle_664", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_664_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_664", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_665_fir() => TestTools.VerifyComputeGraph("ComputeSingle_665", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_665_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_665", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_666_fir() => TestTools.VerifyComputeGraph("ComputeSingle_666", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_666_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_666", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_667_fir() => TestTools.VerifyComputeGraph("ComputeSingle_667", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_667_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_667", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_668_fir() => TestTools.VerifyComputeGraph("ComputeSingle_668", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_668_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_668", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_669_fir() => TestTools.VerifyComputeGraph("ComputeSingle_669", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_669_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_669", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_670_fir() => TestTools.VerifyComputeGraph("ComputeSingle_670", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_670_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_670", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_671_fir() => TestTools.VerifyComputeGraph("ComputeSingle_671", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_671_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_671", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_672_fir() => TestTools.VerifyComputeGraph("ComputeSingle_672", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_672_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_672", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_673_fir() => TestTools.VerifyComputeGraph("ComputeSingle_673", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_673_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_673", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_674_fir() => TestTools.VerifyComputeGraph("ComputeSingle_674", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_674_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_674", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_675_fir() => TestTools.VerifyComputeGraph("ComputeSingle_675", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_675_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_675", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_676_fir() => TestTools.VerifyComputeGraph("ComputeSingle_676", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_676_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_676", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_677_fir() => TestTools.VerifyComputeGraph("ComputeSingle_677", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_677_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_677", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_678_fir() => TestTools.VerifyComputeGraph("ComputeSingle_678", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_678_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_678", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_679_fir() => TestTools.VerifyComputeGraph("ComputeSingle_679", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_679_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_679", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_680_fir() => TestTools.VerifyComputeGraph("ComputeSingle_680", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_680_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_680", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_681_fir() => TestTools.VerifyComputeGraph("ComputeSingle_681", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_681_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_681", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_682_fir() => TestTools.VerifyComputeGraph("ComputeSingle_682", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_682_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_682", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_683_fir() => TestTools.VerifyComputeGraph("ComputeSingle_683", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_683_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_683", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_684_fir() => TestTools.VerifyComputeGraph("ComputeSingle_684", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_684_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_684", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_685_fir() => TestTools.VerifyComputeGraph("ComputeSingle_685", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_685_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_685", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_686_fir() => TestTools.VerifyComputeGraph("ComputeSingle_686", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_686_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_686", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_687_fir() => TestTools.VerifyComputeGraph("ComputeSingle_687", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_687_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_687", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_688_fir() => TestTools.VerifyComputeGraph("ComputeSingle_688", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_688_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_688", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_689_fir() => TestTools.VerifyComputeGraph("ComputeSingle_689", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_689_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_689", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_690_fir() => TestTools.VerifyComputeGraph("ComputeSingle_690", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_690_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_690", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_691_fir() => TestTools.VerifyComputeGraph("ComputeSingle_691", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_691_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_691", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_692_fir() => TestTools.VerifyComputeGraph("ComputeSingle_692", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_692_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_692", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_693_fir() => TestTools.VerifyComputeGraph("ComputeSingle_693", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_693_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_693", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_694_fir() => TestTools.VerifyComputeGraph("ComputeSingle_694", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_694_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_694", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_695_fir() => TestTools.VerifyComputeGraph("ComputeSingle_695", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_695_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_695", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_696_fir() => TestTools.VerifyComputeGraph("ComputeSingle_696", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_696_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_696", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_697_fir() => TestTools.VerifyComputeGraph("ComputeSingle_697", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_697_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_697", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_698_fir() => TestTools.VerifyComputeGraph("ComputeSingle_698", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_698_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_698", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_699_fir() => TestTools.VerifyComputeGraph("ComputeSingle_699", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_699_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_699", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_700_fir() => TestTools.VerifyComputeGraph("ComputeSingle_700", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_700_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_700", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_701_fir() => TestTools.VerifyComputeGraph("ComputeSingle_701", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_701_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_701", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_702_fir() => TestTools.VerifyComputeGraph("ComputeSingle_702", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_702_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_702", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_703_fir() => TestTools.VerifyComputeGraph("ComputeSingle_703", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_703_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_703", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_704_fir() => TestTools.VerifyComputeGraph("ComputeSingle_704", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_704_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_704", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_705_fir() => TestTools.VerifyComputeGraph("ComputeSingle_705", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_705_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_705", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_706_fir() => TestTools.VerifyComputeGraph("ComputeSingle_706", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_706_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_706", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_707_fir() => TestTools.VerifyComputeGraph("ComputeSingle_707", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_707_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_707", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_708_fir() => TestTools.VerifyComputeGraph("ComputeSingle_708", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_708_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_708", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_709_fir() => TestTools.VerifyComputeGraph("ComputeSingle_709", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_709_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_709", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_710_fir() => TestTools.VerifyComputeGraph("ComputeSingle_710", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_710_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_710", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_711_fir() => TestTools.VerifyComputeGraph("ComputeSingle_711", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_711_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_711", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_712_fir() => TestTools.VerifyComputeGraph("ComputeSingle_712", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_712_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_712", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_713_fir() => TestTools.VerifyComputeGraph("ComputeSingle_713", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_713_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_713", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_714_fir() => TestTools.VerifyComputeGraph("ComputeSingle_714", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_714_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_714", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_715_fir() => TestTools.VerifyComputeGraph("ComputeSingle_715", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_715_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_715", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_716_fir() => TestTools.VerifyComputeGraph("ComputeSingle_716", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_716_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_716", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_717_fir() => TestTools.VerifyComputeGraph("ComputeSingle_717", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_717_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_717", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_718_fir() => TestTools.VerifyComputeGraph("ComputeSingle_718", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_718_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_718", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_719_fir() => TestTools.VerifyComputeGraph("ComputeSingle_719", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_719_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_719", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_720_fir() => TestTools.VerifyComputeGraph("ComputeSingle_720", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_720_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_720", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_721_fir() => TestTools.VerifyComputeGraph("ComputeSingle_721", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_721_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_721", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_722_fir() => TestTools.VerifyComputeGraph("ComputeSingle_722", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_722_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_722", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_723_fir() => TestTools.VerifyComputeGraph("ComputeSingle_723", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_723_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_723", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_724_fir() => TestTools.VerifyComputeGraph("ComputeSingle_724", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_724_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_724", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_725_fir() => TestTools.VerifyComputeGraph("ComputeSingle_725", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_725_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_725", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_726_fir() => TestTools.VerifyComputeGraph("ComputeSingle_726", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_726_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_726", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_727_fir() => TestTools.VerifyComputeGraph("ComputeSingle_727", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_727_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_727", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_728_fir() => TestTools.VerifyComputeGraph("ComputeSingle_728", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_728_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_728", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_729_fir() => TestTools.VerifyComputeGraph("ComputeSingle_729", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_729_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_729", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_730_fir() => TestTools.VerifyComputeGraph("ComputeSingle_730", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_730_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_730", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_731_fir() => TestTools.VerifyComputeGraph("ComputeSingle_731", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_731_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_731", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_732_fir() => TestTools.VerifyComputeGraph("ComputeSingle_732", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_732_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_732", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_733_fir() => TestTools.VerifyComputeGraph("ComputeSingle_733", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_733_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_733", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_734_fir() => TestTools.VerifyComputeGraph("ComputeSingle_734", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_734_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_734", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_735_fir() => TestTools.VerifyComputeGraph("ComputeSingle_735", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_735_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_735", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_736_fir() => TestTools.VerifyComputeGraph("ComputeSingle_736", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_736_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_736", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_737_fir() => TestTools.VerifyComputeGraph("ComputeSingle_737", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_737_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_737", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_738_fir() => TestTools.VerifyComputeGraph("ComputeSingle_738", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_738_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_738", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_739_fir() => TestTools.VerifyComputeGraph("ComputeSingle_739", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_739_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_739", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_740_fir() => TestTools.VerifyComputeGraph("ComputeSingle_740", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_740_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_740", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_741_fir() => TestTools.VerifyComputeGraph("ComputeSingle_741", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_741_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_741", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_742_fir() => TestTools.VerifyComputeGraph("ComputeSingle_742", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_742_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_742", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_743_fir() => TestTools.VerifyComputeGraph("ComputeSingle_743", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_743_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_743", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_744_fir() => TestTools.VerifyComputeGraph("ComputeSingle_744", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_744_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_744", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_745_fir() => TestTools.VerifyComputeGraph("ComputeSingle_745", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_745_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_745", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_746_fir() => TestTools.VerifyComputeGraph("ComputeSingle_746", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_746_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_746", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_747_fir() => TestTools.VerifyComputeGraph("ComputeSingle_747", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_747_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_747", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_748_fir() => TestTools.VerifyComputeGraph("ComputeSingle_748", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_748_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_748", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_749_fir() => TestTools.VerifyComputeGraph("ComputeSingle_749", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_749_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_749", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_750_fir() => TestTools.VerifyComputeGraph("ComputeSingle_750", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_750_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_750", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_751_fir() => TestTools.VerifyComputeGraph("ComputeSingle_751", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_751_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_751", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_752_fir() => TestTools.VerifyComputeGraph("ComputeSingle_752", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_752_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_752", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_753_fir() => TestTools.VerifyComputeGraph("ComputeSingle_753", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_753_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_753", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_754_fir() => TestTools.VerifyComputeGraph("ComputeSingle_754", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_754_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_754", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_755_fir() => TestTools.VerifyComputeGraph("ComputeSingle_755", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_755_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_755", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_756_fir() => TestTools.VerifyComputeGraph("ComputeSingle_756", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_756_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_756", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_757_fir() => TestTools.VerifyComputeGraph("ComputeSingle_757", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_757_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_757", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_758_fir() => TestTools.VerifyComputeGraph("ComputeSingle_758", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_758_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_758", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_759_fir() => TestTools.VerifyComputeGraph("ComputeSingle_759", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_759_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_759", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_760_fir() => TestTools.VerifyComputeGraph("ComputeSingle_760", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_760_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_760", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_761_fir() => TestTools.VerifyComputeGraph("ComputeSingle_761", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_761_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_761", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_762_fir() => TestTools.VerifyComputeGraph("ComputeSingle_762", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_762_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_762", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_763_fir() => TestTools.VerifyComputeGraph("ComputeSingle_763", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_763_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_763", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_764_fir() => TestTools.VerifyComputeGraph("ComputeSingle_764", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_764_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_764", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_765_fir() => TestTools.VerifyComputeGraph("ComputeSingle_765", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_765_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_765", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_766_fir() => TestTools.VerifyComputeGraph("ComputeSingle_766", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_766_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_766", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_767_fir() => TestTools.VerifyComputeGraph("ComputeSingle_767", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_767_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_767", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_768_fir() => TestTools.VerifyComputeGraph("ComputeSingle_768", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_768_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_768", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_769_fir() => TestTools.VerifyComputeGraph("ComputeSingle_769", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_769_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_769", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_770_fir() => TestTools.VerifyComputeGraph("ComputeSingle_770", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_770_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_770", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_771_fir() => TestTools.VerifyComputeGraph("ComputeSingle_771", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_771_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_771", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_772_fir() => TestTools.VerifyComputeGraph("ComputeSingle_772", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_772_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_772", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_773_fir() => TestTools.VerifyComputeGraph("ComputeSingle_773", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_773_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_773", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_774_fir() => TestTools.VerifyComputeGraph("ComputeSingle_774", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_774_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_774", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_775_fir() => TestTools.VerifyComputeGraph("ComputeSingle_775", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_775_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_775", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_776_fir() => TestTools.VerifyComputeGraph("ComputeSingle_776", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_776_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_776", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_777_fir() => TestTools.VerifyComputeGraph("ComputeSingle_777", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_777_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_777", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_778_fir() => TestTools.VerifyComputeGraph("ComputeSingle_778", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_778_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_778", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_779_fir() => TestTools.VerifyComputeGraph("ComputeSingle_779", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_779_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_779", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_780_fir() => TestTools.VerifyComputeGraph("ComputeSingle_780", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_780_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_780", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_781_fir() => TestTools.VerifyComputeGraph("ComputeSingle_781", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_781_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_781", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_782_fir() => TestTools.VerifyComputeGraph("ComputeSingle_782", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_782_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_782", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_783_fir() => TestTools.VerifyComputeGraph("ComputeSingle_783", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_783_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_783", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_784_fir() => TestTools.VerifyComputeGraph("ComputeSingle_784", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_784_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_784", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_785_fir() => TestTools.VerifyComputeGraph("ComputeSingle_785", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_785_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_785", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_786_fir() => TestTools.VerifyComputeGraph("ComputeSingle_786", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_786_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_786", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_787_fir() => TestTools.VerifyComputeGraph("ComputeSingle_787", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_787_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_787", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_788_fir() => TestTools.VerifyComputeGraph("ComputeSingle_788", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_788_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_788", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_789_fir() => TestTools.VerifyComputeGraph("ComputeSingle_789", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_789_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_789", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_790_fir() => TestTools.VerifyComputeGraph("ComputeSingle_790", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_790_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_790", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_791_fir() => TestTools.VerifyComputeGraph("ComputeSingle_791", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_791_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_791", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_792_fir() => TestTools.VerifyComputeGraph("ComputeSingle_792", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_792_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_792", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_793_fir() => TestTools.VerifyComputeGraph("ComputeSingle_793", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_793_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_793", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_794_fir() => TestTools.VerifyComputeGraph("ComputeSingle_794", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_794_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_794", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_795_fir() => TestTools.VerifyComputeGraph("ComputeSingle_795", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_795_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_795", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_796_fir() => TestTools.VerifyComputeGraph("ComputeSingle_796", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_796_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_796", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_797_fir() => TestTools.VerifyComputeGraph("ComputeSingle_797", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_797_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_797", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_798_fir() => TestTools.VerifyComputeGraph("ComputeSingle_798", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_798_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_798", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_799_fir() => TestTools.VerifyComputeGraph("ComputeSingle_799", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_799_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_799", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_800_fir() => TestTools.VerifyComputeGraph("ComputeSingle_800", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_800_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_800", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_801_fir() => TestTools.VerifyComputeGraph("ComputeSingle_801", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_801_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_801", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_802_fir() => TestTools.VerifyComputeGraph("ComputeSingle_802", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_802_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_802", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_803_fir() => TestTools.VerifyComputeGraph("ComputeSingle_803", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_803_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_803", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_804_fir() => TestTools.VerifyComputeGraph("ComputeSingle_804", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_804_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_804", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_805_fir() => TestTools.VerifyComputeGraph("ComputeSingle_805", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_805_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_805", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_806_fir() => TestTools.VerifyComputeGraph("ComputeSingle_806", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_806_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_806", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_807_fir() => TestTools.VerifyComputeGraph("ComputeSingle_807", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_807_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_807", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_808_fir() => TestTools.VerifyComputeGraph("ComputeSingle_808", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_808_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_808", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_809_fir() => TestTools.VerifyComputeGraph("ComputeSingle_809", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_809_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_809", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_810_fir() => TestTools.VerifyComputeGraph("ComputeSingle_810", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_810_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_810", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_811_fir() => TestTools.VerifyComputeGraph("ComputeSingle_811", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_811_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_811", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_812_fir() => TestTools.VerifyComputeGraph("ComputeSingle_812", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_812_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_812", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_813_fir() => TestTools.VerifyComputeGraph("ComputeSingle_813", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_813_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_813", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_814_fir() => TestTools.VerifyComputeGraph("ComputeSingle_814", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_814_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_814", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_815_fir() => TestTools.VerifyComputeGraph("ComputeSingle_815", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_815_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_815", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_816_fir() => TestTools.VerifyComputeGraph("ComputeSingle_816", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_816_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_816", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_817_fir() => TestTools.VerifyComputeGraph("ComputeSingle_817", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_817_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_817", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_818_fir() => TestTools.VerifyComputeGraph("ComputeSingle_818", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_818_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_818", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_819_fir() => TestTools.VerifyComputeGraph("ComputeSingle_819", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_819_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_819", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_820_fir() => TestTools.VerifyComputeGraph("ComputeSingle_820", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_820_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_820", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_821_fir() => TestTools.VerifyComputeGraph("ComputeSingle_821", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_821_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_821", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_822_fir() => TestTools.VerifyComputeGraph("ComputeSingle_822", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_822_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_822", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_823_fir() => TestTools.VerifyComputeGraph("ComputeSingle_823", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_823_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_823", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_824_fir() => TestTools.VerifyComputeGraph("ComputeSingle_824", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_824_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_824", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_825_fir() => TestTools.VerifyComputeGraph("ComputeSingle_825", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_825_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_825", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_826_fir() => TestTools.VerifyComputeGraph("ComputeSingle_826", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_826_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_826", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_827_fir() => TestTools.VerifyComputeGraph("ComputeSingle_827", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_827_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_827", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_828_fir() => TestTools.VerifyComputeGraph("ComputeSingle_828", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_828_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_828", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_829_fir() => TestTools.VerifyComputeGraph("ComputeSingle_829", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_829_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_829", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_830_fir() => TestTools.VerifyComputeGraph("ComputeSingle_830", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_830_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_830", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_831_fir() => TestTools.VerifyComputeGraph("ComputeSingle_831", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_831_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_831", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_832_fir() => TestTools.VerifyComputeGraph("ComputeSingle_832", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_832_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_832", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_833_fir() => TestTools.VerifyComputeGraph("ComputeSingle_833", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_833_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_833", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_834_fir() => TestTools.VerifyComputeGraph("ComputeSingle_834", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_834_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_834", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_835_fir() => TestTools.VerifyComputeGraph("ComputeSingle_835", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_835_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_835", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_836_fir() => TestTools.VerifyComputeGraph("ComputeSingle_836", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_836_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_836", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_837_fir() => TestTools.VerifyComputeGraph("ComputeSingle_837", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_837_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_837", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_838_fir() => TestTools.VerifyComputeGraph("ComputeSingle_838", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_838_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_838", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_839_fir() => TestTools.VerifyComputeGraph("ComputeSingle_839", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_839_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_839", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_840_fir() => TestTools.VerifyComputeGraph("ComputeSingle_840", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_840_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_840", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_841_fir() => TestTools.VerifyComputeGraph("ComputeSingle_841", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_841_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_841", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_842_fir() => TestTools.VerifyComputeGraph("ComputeSingle_842", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_842_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_842", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_843_fir() => TestTools.VerifyComputeGraph("ComputeSingle_843", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_843_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_843", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_844_fir() => TestTools.VerifyComputeGraph("ComputeSingle_844", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_844_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_844", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_845_fir() => TestTools.VerifyComputeGraph("ComputeSingle_845", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_845_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_845", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_846_fir() => TestTools.VerifyComputeGraph("ComputeSingle_846", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_846_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_846", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_847_fir() => TestTools.VerifyComputeGraph("ComputeSingle_847", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_847_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_847", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_848_fir() => TestTools.VerifyComputeGraph("ComputeSingle_848", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_848_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_848", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_849_fir() => TestTools.VerifyComputeGraph("ComputeSingle_849", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_849_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_849", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_850_fir() => TestTools.VerifyComputeGraph("ComputeSingle_850", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_850_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_850", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_851_fir() => TestTools.VerifyComputeGraph("ComputeSingle_851", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_851_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_851", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_852_fir() => TestTools.VerifyComputeGraph("ComputeSingle_852", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_852_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_852", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_853_fir() => TestTools.VerifyComputeGraph("ComputeSingle_853", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_853_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_853", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_854_fir() => TestTools.VerifyComputeGraph("ComputeSingle_854", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_854_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_854", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_855_fir() => TestTools.VerifyComputeGraph("ComputeSingle_855", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_855_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_855", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_856_fir() => TestTools.VerifyComputeGraph("ComputeSingle_856", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_856_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_856", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_857_fir() => TestTools.VerifyComputeGraph("ComputeSingle_857", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_857_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_857", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_858_fir() => TestTools.VerifyComputeGraph("ComputeSingle_858", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_858_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_858", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_859_fir() => TestTools.VerifyComputeGraph("ComputeSingle_859", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_859_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_859", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_860_fir() => TestTools.VerifyComputeGraph("ComputeSingle_860", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_860_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_860", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_861_fir() => TestTools.VerifyComputeGraph("ComputeSingle_861", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_861_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_861", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_862_fir() => TestTools.VerifyComputeGraph("ComputeSingle_862", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_862_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_862", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_863_fir() => TestTools.VerifyComputeGraph("ComputeSingle_863", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_863_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_863", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_864_fir() => TestTools.VerifyComputeGraph("ComputeSingle_864", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_864_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_864", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_865_fir() => TestTools.VerifyComputeGraph("ComputeSingle_865", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_865_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_865", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_866_fir() => TestTools.VerifyComputeGraph("ComputeSingle_866", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_866_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_866", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_867_fir() => TestTools.VerifyComputeGraph("ComputeSingle_867", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_867_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_867", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_868_fir() => TestTools.VerifyComputeGraph("ComputeSingle_868", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_868_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_868", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_869_fir() => TestTools.VerifyComputeGraph("ComputeSingle_869", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_869_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_869", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_870_fir() => TestTools.VerifyComputeGraph("ComputeSingle_870", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_870_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_870", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_871_fir() => TestTools.VerifyComputeGraph("ComputeSingle_871", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_871_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_871", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_872_fir() => TestTools.VerifyComputeGraph("ComputeSingle_872", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_872_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_872", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_873_fir() => TestTools.VerifyComputeGraph("ComputeSingle_873", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_873_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_873", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_874_fir() => TestTools.VerifyComputeGraph("ComputeSingle_874", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_874_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_874", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_875_fir() => TestTools.VerifyComputeGraph("ComputeSingle_875", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_875_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_875", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_876_fir() => TestTools.VerifyComputeGraph("ComputeSingle_876", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_876_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_876", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_877_fir() => TestTools.VerifyComputeGraph("ComputeSingle_877", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_877_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_877", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_878_fir() => TestTools.VerifyComputeGraph("ComputeSingle_878", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_878_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_878", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_879_fir() => TestTools.VerifyComputeGraph("ComputeSingle_879", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_879_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_879", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_880_fir() => TestTools.VerifyComputeGraph("ComputeSingle_880", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_880_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_880", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_881_fir() => TestTools.VerifyComputeGraph("ComputeSingle_881", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_881_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_881", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_882_fir() => TestTools.VerifyComputeGraph("ComputeSingle_882", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_882_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_882", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_883_fir() => TestTools.VerifyComputeGraph("ComputeSingle_883", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_883_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_883", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_884_fir() => TestTools.VerifyComputeGraph("ComputeSingle_884", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_884_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_884", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_885_fir() => TestTools.VerifyComputeGraph("ComputeSingle_885", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_885_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_885", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_886_fir() => TestTools.VerifyComputeGraph("ComputeSingle_886", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_886_lo_fir() => TestTools.VerifyComputeGraph("ComputeSingle_886", "lo.fir", false, TestDir);


    }
}
