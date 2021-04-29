using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebugTests
{
    [TestClass]
    public class MultiOpLoadTests
    {
        const string TestDir = "../../../../TestGenerator/OthersFIRRTL";

        [TestMethod] public void ComputeSeq_fir() => TestTools.VerifyMakeGraph("ComputeSeq", "fir", TestDir);
        [TestMethod] public void ComputeSeq_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_0_fir() => TestTools.VerifyMakeGraph("ComputeSeq_0", "fir", TestDir);
        [TestMethod] public void ComputeSeq_0_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_0", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_1_fir() => TestTools.VerifyMakeGraph("ComputeSeq_1", "fir", TestDir);
        [TestMethod] public void ComputeSeq_1_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_1", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_2_fir() => TestTools.VerifyMakeGraph("ComputeSeq_2", "fir", TestDir);
        [TestMethod] public void ComputeSeq_2_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_2", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_3_fir() => TestTools.VerifyMakeGraph("ComputeSeq_3", "fir", TestDir);
        [TestMethod] public void ComputeSeq_3_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_3", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_4_fir() => TestTools.VerifyMakeGraph("ComputeSeq_4", "fir", TestDir);
        [TestMethod] public void ComputeSeq_4_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_4", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_5_fir() => TestTools.VerifyMakeGraph("ComputeSeq_5", "fir", TestDir);
        [TestMethod] public void ComputeSeq_5_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_5", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_6_fir() => TestTools.VerifyMakeGraph("ComputeSeq_6", "fir", TestDir);
        [TestMethod] public void ComputeSeq_6_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_6", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_7_fir() => TestTools.VerifyMakeGraph("ComputeSeq_7", "fir", TestDir);
        [TestMethod] public void ComputeSeq_7_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_7", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_8_fir() => TestTools.VerifyMakeGraph("ComputeSeq_8", "fir", TestDir);
        [TestMethod] public void ComputeSeq_8_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_8", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_9_fir() => TestTools.VerifyMakeGraph("ComputeSeq_9", "fir", TestDir);
        [TestMethod] public void ComputeSeq_9_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_9", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_10_fir() => TestTools.VerifyMakeGraph("ComputeSeq_10", "fir", TestDir);
        [TestMethod] public void ComputeSeq_10_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_10", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_11_fir() => TestTools.VerifyMakeGraph("ComputeSeq_11", "fir", TestDir);
        [TestMethod] public void ComputeSeq_11_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_11", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_12_fir() => TestTools.VerifyMakeGraph("ComputeSeq_12", "fir", TestDir);
        [TestMethod] public void ComputeSeq_12_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_12", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_13_fir() => TestTools.VerifyMakeGraph("ComputeSeq_13", "fir", TestDir);
        [TestMethod] public void ComputeSeq_13_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_13", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_14_fir() => TestTools.VerifyMakeGraph("ComputeSeq_14", "fir", TestDir);
        [TestMethod] public void ComputeSeq_14_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_14", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_15_fir() => TestTools.VerifyMakeGraph("ComputeSeq_15", "fir", TestDir);
        [TestMethod] public void ComputeSeq_15_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_15", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_16_fir() => TestTools.VerifyMakeGraph("ComputeSeq_16", "fir", TestDir);
        [TestMethod] public void ComputeSeq_16_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_16", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_17_fir() => TestTools.VerifyMakeGraph("ComputeSeq_17", "fir", TestDir);
        [TestMethod] public void ComputeSeq_17_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_17", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_18_fir() => TestTools.VerifyMakeGraph("ComputeSeq_18", "fir", TestDir);
        [TestMethod] public void ComputeSeq_18_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_18", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_19_fir() => TestTools.VerifyMakeGraph("ComputeSeq_19", "fir", TestDir);
        [TestMethod] public void ComputeSeq_19_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_19", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_20_fir() => TestTools.VerifyMakeGraph("ComputeSeq_20", "fir", TestDir);
        [TestMethod] public void ComputeSeq_20_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_20", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_21_fir() => TestTools.VerifyMakeGraph("ComputeSeq_21", "fir", TestDir);
        [TestMethod] public void ComputeSeq_21_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_21", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_22_fir() => TestTools.VerifyMakeGraph("ComputeSeq_22", "fir", TestDir);
        [TestMethod] public void ComputeSeq_22_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_22", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_23_fir() => TestTools.VerifyMakeGraph("ComputeSeq_23", "fir", TestDir);
        [TestMethod] public void ComputeSeq_23_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_23", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_24_fir() => TestTools.VerifyMakeGraph("ComputeSeq_24", "fir", TestDir);
        [TestMethod] public void ComputeSeq_24_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_24", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_25_fir() => TestTools.VerifyMakeGraph("ComputeSeq_25", "fir", TestDir);
        [TestMethod] public void ComputeSeq_25_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_25", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_26_fir() => TestTools.VerifyMakeGraph("ComputeSeq_26", "fir", TestDir);
        [TestMethod] public void ComputeSeq_26_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_26", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_27_fir() => TestTools.VerifyMakeGraph("ComputeSeq_27", "fir", TestDir);
        [TestMethod] public void ComputeSeq_27_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_27", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_28_fir() => TestTools.VerifyMakeGraph("ComputeSeq_28", "fir", TestDir);
        [TestMethod] public void ComputeSeq_28_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_28", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_29_fir() => TestTools.VerifyMakeGraph("ComputeSeq_29", "fir", TestDir);
        [TestMethod] public void ComputeSeq_29_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_29", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_30_fir() => TestTools.VerifyMakeGraph("ComputeSeq_30", "fir", TestDir);
        [TestMethod] public void ComputeSeq_30_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_30", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_31_fir() => TestTools.VerifyMakeGraph("ComputeSeq_31", "fir", TestDir);
        [TestMethod] public void ComputeSeq_31_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_31", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_32_fir() => TestTools.VerifyMakeGraph("ComputeSeq_32", "fir", TestDir);
        [TestMethod] public void ComputeSeq_32_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_32", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_33_fir() => TestTools.VerifyMakeGraph("ComputeSeq_33", "fir", TestDir);
        [TestMethod] public void ComputeSeq_33_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_33", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_34_fir() => TestTools.VerifyMakeGraph("ComputeSeq_34", "fir", TestDir);
        [TestMethod] public void ComputeSeq_34_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_34", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_35_fir() => TestTools.VerifyMakeGraph("ComputeSeq_35", "fir", TestDir);
        [TestMethod] public void ComputeSeq_35_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_35", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_36_fir() => TestTools.VerifyMakeGraph("ComputeSeq_36", "fir", TestDir);
        [TestMethod] public void ComputeSeq_36_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_36", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_37_fir() => TestTools.VerifyMakeGraph("ComputeSeq_37", "fir", TestDir);
        [TestMethod] public void ComputeSeq_37_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_37", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_38_fir() => TestTools.VerifyMakeGraph("ComputeSeq_38", "fir", TestDir);
        [TestMethod] public void ComputeSeq_38_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_38", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_39_fir() => TestTools.VerifyMakeGraph("ComputeSeq_39", "fir", TestDir);
        [TestMethod] public void ComputeSeq_39_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_39", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_40_fir() => TestTools.VerifyMakeGraph("ComputeSeq_40", "fir", TestDir);
        [TestMethod] public void ComputeSeq_40_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_40", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_41_fir() => TestTools.VerifyMakeGraph("ComputeSeq_41", "fir", TestDir);
        [TestMethod] public void ComputeSeq_41_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_41", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_42_fir() => TestTools.VerifyMakeGraph("ComputeSeq_42", "fir", TestDir);
        [TestMethod] public void ComputeSeq_42_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_42", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_43_fir() => TestTools.VerifyMakeGraph("ComputeSeq_43", "fir", TestDir);
        [TestMethod] public void ComputeSeq_43_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_43", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_44_fir() => TestTools.VerifyMakeGraph("ComputeSeq_44", "fir", TestDir);
        [TestMethod] public void ComputeSeq_44_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_44", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_45_fir() => TestTools.VerifyMakeGraph("ComputeSeq_45", "fir", TestDir);
        [TestMethod] public void ComputeSeq_45_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_45", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_46_fir() => TestTools.VerifyMakeGraph("ComputeSeq_46", "fir", TestDir);
        [TestMethod] public void ComputeSeq_46_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_46", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_47_fir() => TestTools.VerifyMakeGraph("ComputeSeq_47", "fir", TestDir);
        [TestMethod] public void ComputeSeq_47_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_47", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_48_fir() => TestTools.VerifyMakeGraph("ComputeSeq_48", "fir", TestDir);
        [TestMethod] public void ComputeSeq_48_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_48", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_49_fir() => TestTools.VerifyMakeGraph("ComputeSeq_49", "fir", TestDir);
        [TestMethod] public void ComputeSeq_49_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_49", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_50_fir() => TestTools.VerifyMakeGraph("ComputeSeq_50", "fir", TestDir);
        [TestMethod] public void ComputeSeq_50_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_50", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_51_fir() => TestTools.VerifyMakeGraph("ComputeSeq_51", "fir", TestDir);
        [TestMethod] public void ComputeSeq_51_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_51", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_52_fir() => TestTools.VerifyMakeGraph("ComputeSeq_52", "fir", TestDir);
        [TestMethod] public void ComputeSeq_52_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_52", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_53_fir() => TestTools.VerifyMakeGraph("ComputeSeq_53", "fir", TestDir);
        [TestMethod] public void ComputeSeq_53_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_53", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_54_fir() => TestTools.VerifyMakeGraph("ComputeSeq_54", "fir", TestDir);
        [TestMethod] public void ComputeSeq_54_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_54", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_55_fir() => TestTools.VerifyMakeGraph("ComputeSeq_55", "fir", TestDir);
        [TestMethod] public void ComputeSeq_55_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_55", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_56_fir() => TestTools.VerifyMakeGraph("ComputeSeq_56", "fir", TestDir);
        [TestMethod] public void ComputeSeq_56_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_56", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_57_fir() => TestTools.VerifyMakeGraph("ComputeSeq_57", "fir", TestDir);
        [TestMethod] public void ComputeSeq_57_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_57", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_58_fir() => TestTools.VerifyMakeGraph("ComputeSeq_58", "fir", TestDir);
        [TestMethod] public void ComputeSeq_58_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_58", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_59_fir() => TestTools.VerifyMakeGraph("ComputeSeq_59", "fir", TestDir);
        [TestMethod] public void ComputeSeq_59_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_59", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_60_fir() => TestTools.VerifyMakeGraph("ComputeSeq_60", "fir", TestDir);
        [TestMethod] public void ComputeSeq_60_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_60", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_61_fir() => TestTools.VerifyMakeGraph("ComputeSeq_61", "fir", TestDir);
        [TestMethod] public void ComputeSeq_61_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_61", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_62_fir() => TestTools.VerifyMakeGraph("ComputeSeq_62", "fir", TestDir);
        [TestMethod] public void ComputeSeq_62_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_62", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_63_fir() => TestTools.VerifyMakeGraph("ComputeSeq_63", "fir", TestDir);
        [TestMethod] public void ComputeSeq_63_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_63", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_64_fir() => TestTools.VerifyMakeGraph("ComputeSeq_64", "fir", TestDir);
        [TestMethod] public void ComputeSeq_64_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_64", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_65_fir() => TestTools.VerifyMakeGraph("ComputeSeq_65", "fir", TestDir);
        [TestMethod] public void ComputeSeq_65_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_65", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_66_fir() => TestTools.VerifyMakeGraph("ComputeSeq_66", "fir", TestDir);
        [TestMethod] public void ComputeSeq_66_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_66", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_67_fir() => TestTools.VerifyMakeGraph("ComputeSeq_67", "fir", TestDir);
        [TestMethod] public void ComputeSeq_67_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_67", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_68_fir() => TestTools.VerifyMakeGraph("ComputeSeq_68", "fir", TestDir);
        [TestMethod] public void ComputeSeq_68_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_68", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_69_fir() => TestTools.VerifyMakeGraph("ComputeSeq_69", "fir", TestDir);
        [TestMethod] public void ComputeSeq_69_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_69", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_70_fir() => TestTools.VerifyMakeGraph("ComputeSeq_70", "fir", TestDir);
        [TestMethod] public void ComputeSeq_70_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_70", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_71_fir() => TestTools.VerifyMakeGraph("ComputeSeq_71", "fir", TestDir);
        [TestMethod] public void ComputeSeq_71_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_71", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_72_fir() => TestTools.VerifyMakeGraph("ComputeSeq_72", "fir", TestDir);
        [TestMethod] public void ComputeSeq_72_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_72", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_73_fir() => TestTools.VerifyMakeGraph("ComputeSeq_73", "fir", TestDir);
        [TestMethod] public void ComputeSeq_73_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_73", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_74_fir() => TestTools.VerifyMakeGraph("ComputeSeq_74", "fir", TestDir);
        [TestMethod] public void ComputeSeq_74_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_74", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_75_fir() => TestTools.VerifyMakeGraph("ComputeSeq_75", "fir", TestDir);
        [TestMethod] public void ComputeSeq_75_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_75", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_76_fir() => TestTools.VerifyMakeGraph("ComputeSeq_76", "fir", TestDir);
        [TestMethod] public void ComputeSeq_76_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_76", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_77_fir() => TestTools.VerifyMakeGraph("ComputeSeq_77", "fir", TestDir);
        [TestMethod] public void ComputeSeq_77_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_77", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_78_fir() => TestTools.VerifyMakeGraph("ComputeSeq_78", "fir", TestDir);
        [TestMethod] public void ComputeSeq_78_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_78", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_79_fir() => TestTools.VerifyMakeGraph("ComputeSeq_79", "fir", TestDir);
        [TestMethod] public void ComputeSeq_79_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_79", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_80_fir() => TestTools.VerifyMakeGraph("ComputeSeq_80", "fir", TestDir);
        [TestMethod] public void ComputeSeq_80_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_80", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_81_fir() => TestTools.VerifyMakeGraph("ComputeSeq_81", "fir", TestDir);
        [TestMethod] public void ComputeSeq_81_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_81", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_82_fir() => TestTools.VerifyMakeGraph("ComputeSeq_82", "fir", TestDir);
        [TestMethod] public void ComputeSeq_82_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_82", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_83_fir() => TestTools.VerifyMakeGraph("ComputeSeq_83", "fir", TestDir);
        [TestMethod] public void ComputeSeq_83_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_83", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_84_fir() => TestTools.VerifyMakeGraph("ComputeSeq_84", "fir", TestDir);
        [TestMethod] public void ComputeSeq_84_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_84", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_85_fir() => TestTools.VerifyMakeGraph("ComputeSeq_85", "fir", TestDir);
        [TestMethod] public void ComputeSeq_85_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_85", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_86_fir() => TestTools.VerifyMakeGraph("ComputeSeq_86", "fir", TestDir);
        [TestMethod] public void ComputeSeq_86_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_86", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_87_fir() => TestTools.VerifyMakeGraph("ComputeSeq_87", "fir", TestDir);
        [TestMethod] public void ComputeSeq_87_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_87", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_88_fir() => TestTools.VerifyMakeGraph("ComputeSeq_88", "fir", TestDir);
        [TestMethod] public void ComputeSeq_88_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_88", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_89_fir() => TestTools.VerifyMakeGraph("ComputeSeq_89", "fir", TestDir);
        [TestMethod] public void ComputeSeq_89_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_89", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_90_fir() => TestTools.VerifyMakeGraph("ComputeSeq_90", "fir", TestDir);
        [TestMethod] public void ComputeSeq_90_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_90", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_91_fir() => TestTools.VerifyMakeGraph("ComputeSeq_91", "fir", TestDir);
        [TestMethod] public void ComputeSeq_91_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_91", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_92_fir() => TestTools.VerifyMakeGraph("ComputeSeq_92", "fir", TestDir);
        [TestMethod] public void ComputeSeq_92_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_92", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_93_fir() => TestTools.VerifyMakeGraph("ComputeSeq_93", "fir", TestDir);
        [TestMethod] public void ComputeSeq_93_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_93", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_94_fir() => TestTools.VerifyMakeGraph("ComputeSeq_94", "fir", TestDir);
        [TestMethod] public void ComputeSeq_94_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_94", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_95_fir() => TestTools.VerifyMakeGraph("ComputeSeq_95", "fir", TestDir);
        [TestMethod] public void ComputeSeq_95_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_95", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_96_fir() => TestTools.VerifyMakeGraph("ComputeSeq_96", "fir", TestDir);
        [TestMethod] public void ComputeSeq_96_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_96", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_97_fir() => TestTools.VerifyMakeGraph("ComputeSeq_97", "fir", TestDir);
        [TestMethod] public void ComputeSeq_97_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_97", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_98_fir() => TestTools.VerifyMakeGraph("ComputeSeq_98", "fir", TestDir);
        [TestMethod] public void ComputeSeq_98_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_98", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_99_fir() => TestTools.VerifyMakeGraph("ComputeSeq_99", "fir", TestDir);
        [TestMethod] public void ComputeSeq_99_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_99", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_100_fir() => TestTools.VerifyMakeGraph("ComputeSeq_100", "fir", TestDir);
        [TestMethod] public void ComputeSeq_100_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_100", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_101_fir() => TestTools.VerifyMakeGraph("ComputeSeq_101", "fir", TestDir);
        [TestMethod] public void ComputeSeq_101_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_101", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_102_fir() => TestTools.VerifyMakeGraph("ComputeSeq_102", "fir", TestDir);
        [TestMethod] public void ComputeSeq_102_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_102", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_103_fir() => TestTools.VerifyMakeGraph("ComputeSeq_103", "fir", TestDir);
        [TestMethod] public void ComputeSeq_103_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_103", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_104_fir() => TestTools.VerifyMakeGraph("ComputeSeq_104", "fir", TestDir);
        [TestMethod] public void ComputeSeq_104_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_104", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_105_fir() => TestTools.VerifyMakeGraph("ComputeSeq_105", "fir", TestDir);
        [TestMethod] public void ComputeSeq_105_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_105", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_106_fir() => TestTools.VerifyMakeGraph("ComputeSeq_106", "fir", TestDir);
        [TestMethod] public void ComputeSeq_106_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_106", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_107_fir() => TestTools.VerifyMakeGraph("ComputeSeq_107", "fir", TestDir);
        [TestMethod] public void ComputeSeq_107_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_107", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_108_fir() => TestTools.VerifyMakeGraph("ComputeSeq_108", "fir", TestDir);
        [TestMethod] public void ComputeSeq_108_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_108", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_109_fir() => TestTools.VerifyMakeGraph("ComputeSeq_109", "fir", TestDir);
        [TestMethod] public void ComputeSeq_109_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_109", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_110_fir() => TestTools.VerifyMakeGraph("ComputeSeq_110", "fir", TestDir);
        [TestMethod] public void ComputeSeq_110_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_110", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_111_fir() => TestTools.VerifyMakeGraph("ComputeSeq_111", "fir", TestDir);
        [TestMethod] public void ComputeSeq_111_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_111", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_112_fir() => TestTools.VerifyMakeGraph("ComputeSeq_112", "fir", TestDir);
        [TestMethod] public void ComputeSeq_112_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_112", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_113_fir() => TestTools.VerifyMakeGraph("ComputeSeq_113", "fir", TestDir);
        [TestMethod] public void ComputeSeq_113_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_113", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_114_fir() => TestTools.VerifyMakeGraph("ComputeSeq_114", "fir", TestDir);
        [TestMethod] public void ComputeSeq_114_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_114", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_115_fir() => TestTools.VerifyMakeGraph("ComputeSeq_115", "fir", TestDir);
        [TestMethod] public void ComputeSeq_115_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_115", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_116_fir() => TestTools.VerifyMakeGraph("ComputeSeq_116", "fir", TestDir);
        [TestMethod] public void ComputeSeq_116_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_116", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_117_fir() => TestTools.VerifyMakeGraph("ComputeSeq_117", "fir", TestDir);
        [TestMethod] public void ComputeSeq_117_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_117", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_118_fir() => TestTools.VerifyMakeGraph("ComputeSeq_118", "fir", TestDir);
        [TestMethod] public void ComputeSeq_118_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_118", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_119_fir() => TestTools.VerifyMakeGraph("ComputeSeq_119", "fir", TestDir);
        [TestMethod] public void ComputeSeq_119_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_119", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_120_fir() => TestTools.VerifyMakeGraph("ComputeSeq_120", "fir", TestDir);
        [TestMethod] public void ComputeSeq_120_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_120", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_121_fir() => TestTools.VerifyMakeGraph("ComputeSeq_121", "fir", TestDir);
        [TestMethod] public void ComputeSeq_121_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_121", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_122_fir() => TestTools.VerifyMakeGraph("ComputeSeq_122", "fir", TestDir);
        [TestMethod] public void ComputeSeq_122_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_122", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_123_fir() => TestTools.VerifyMakeGraph("ComputeSeq_123", "fir", TestDir);
        [TestMethod] public void ComputeSeq_123_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_123", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_124_fir() => TestTools.VerifyMakeGraph("ComputeSeq_124", "fir", TestDir);
        [TestMethod] public void ComputeSeq_124_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_124", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_125_fir() => TestTools.VerifyMakeGraph("ComputeSeq_125", "fir", TestDir);
        [TestMethod] public void ComputeSeq_125_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_125", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_126_fir() => TestTools.VerifyMakeGraph("ComputeSeq_126", "fir", TestDir);
        [TestMethod] public void ComputeSeq_126_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_126", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_127_fir() => TestTools.VerifyMakeGraph("ComputeSeq_127", "fir", TestDir);
        [TestMethod] public void ComputeSeq_127_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_127", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_128_fir() => TestTools.VerifyMakeGraph("ComputeSeq_128", "fir", TestDir);
        [TestMethod] public void ComputeSeq_128_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_128", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_129_fir() => TestTools.VerifyMakeGraph("ComputeSeq_129", "fir", TestDir);
        [TestMethod] public void ComputeSeq_129_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_129", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_130_fir() => TestTools.VerifyMakeGraph("ComputeSeq_130", "fir", TestDir);
        [TestMethod] public void ComputeSeq_130_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_130", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_131_fir() => TestTools.VerifyMakeGraph("ComputeSeq_131", "fir", TestDir);
        [TestMethod] public void ComputeSeq_131_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_131", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_132_fir() => TestTools.VerifyMakeGraph("ComputeSeq_132", "fir", TestDir);
        [TestMethod] public void ComputeSeq_132_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_132", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_133_fir() => TestTools.VerifyMakeGraph("ComputeSeq_133", "fir", TestDir);
        [TestMethod] public void ComputeSeq_133_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_133", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_134_fir() => TestTools.VerifyMakeGraph("ComputeSeq_134", "fir", TestDir);
        [TestMethod] public void ComputeSeq_134_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_134", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_135_fir() => TestTools.VerifyMakeGraph("ComputeSeq_135", "fir", TestDir);
        [TestMethod] public void ComputeSeq_135_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_135", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_136_fir() => TestTools.VerifyMakeGraph("ComputeSeq_136", "fir", TestDir);
        [TestMethod] public void ComputeSeq_136_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_136", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_137_fir() => TestTools.VerifyMakeGraph("ComputeSeq_137", "fir", TestDir);
        [TestMethod] public void ComputeSeq_137_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_137", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_138_fir() => TestTools.VerifyMakeGraph("ComputeSeq_138", "fir", TestDir);
        [TestMethod] public void ComputeSeq_138_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_138", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_139_fir() => TestTools.VerifyMakeGraph("ComputeSeq_139", "fir", TestDir);
        [TestMethod] public void ComputeSeq_139_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_139", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_140_fir() => TestTools.VerifyMakeGraph("ComputeSeq_140", "fir", TestDir);
        [TestMethod] public void ComputeSeq_140_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_140", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_141_fir() => TestTools.VerifyMakeGraph("ComputeSeq_141", "fir", TestDir);
        [TestMethod] public void ComputeSeq_141_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_141", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_142_fir() => TestTools.VerifyMakeGraph("ComputeSeq_142", "fir", TestDir);
        [TestMethod] public void ComputeSeq_142_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_142", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_143_fir() => TestTools.VerifyMakeGraph("ComputeSeq_143", "fir", TestDir);
        [TestMethod] public void ComputeSeq_143_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_143", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_144_fir() => TestTools.VerifyMakeGraph("ComputeSeq_144", "fir", TestDir);
        [TestMethod] public void ComputeSeq_144_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_144", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_145_fir() => TestTools.VerifyMakeGraph("ComputeSeq_145", "fir", TestDir);
        [TestMethod] public void ComputeSeq_145_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_145", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_146_fir() => TestTools.VerifyMakeGraph("ComputeSeq_146", "fir", TestDir);
        [TestMethod] public void ComputeSeq_146_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_146", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_147_fir() => TestTools.VerifyMakeGraph("ComputeSeq_147", "fir", TestDir);
        [TestMethod] public void ComputeSeq_147_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_147", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_148_fir() => TestTools.VerifyMakeGraph("ComputeSeq_148", "fir", TestDir);
        [TestMethod] public void ComputeSeq_148_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_148", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_149_fir() => TestTools.VerifyMakeGraph("ComputeSeq_149", "fir", TestDir);
        [TestMethod] public void ComputeSeq_149_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_149", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_150_fir() => TestTools.VerifyMakeGraph("ComputeSeq_150", "fir", TestDir);
        [TestMethod] public void ComputeSeq_150_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_150", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_151_fir() => TestTools.VerifyMakeGraph("ComputeSeq_151", "fir", TestDir);
        [TestMethod] public void ComputeSeq_151_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_151", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_152_fir() => TestTools.VerifyMakeGraph("ComputeSeq_152", "fir", TestDir);
        [TestMethod] public void ComputeSeq_152_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_152", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_153_fir() => TestTools.VerifyMakeGraph("ComputeSeq_153", "fir", TestDir);
        [TestMethod] public void ComputeSeq_153_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_153", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_154_fir() => TestTools.VerifyMakeGraph("ComputeSeq_154", "fir", TestDir);
        [TestMethod] public void ComputeSeq_154_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_154", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_155_fir() => TestTools.VerifyMakeGraph("ComputeSeq_155", "fir", TestDir);
        [TestMethod] public void ComputeSeq_155_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_155", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_156_fir() => TestTools.VerifyMakeGraph("ComputeSeq_156", "fir", TestDir);
        [TestMethod] public void ComputeSeq_156_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_156", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_157_fir() => TestTools.VerifyMakeGraph("ComputeSeq_157", "fir", TestDir);
        [TestMethod] public void ComputeSeq_157_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_157", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_158_fir() => TestTools.VerifyMakeGraph("ComputeSeq_158", "fir", TestDir);
        [TestMethod] public void ComputeSeq_158_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_158", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_159_fir() => TestTools.VerifyMakeGraph("ComputeSeq_159", "fir", TestDir);
        [TestMethod] public void ComputeSeq_159_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_159", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_160_fir() => TestTools.VerifyMakeGraph("ComputeSeq_160", "fir", TestDir);
        [TestMethod] public void ComputeSeq_160_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_160", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_161_fir() => TestTools.VerifyMakeGraph("ComputeSeq_161", "fir", TestDir);
        [TestMethod] public void ComputeSeq_161_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_161", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_162_fir() => TestTools.VerifyMakeGraph("ComputeSeq_162", "fir", TestDir);
        [TestMethod] public void ComputeSeq_162_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_162", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_163_fir() => TestTools.VerifyMakeGraph("ComputeSeq_163", "fir", TestDir);
        [TestMethod] public void ComputeSeq_163_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_163", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_164_fir() => TestTools.VerifyMakeGraph("ComputeSeq_164", "fir", TestDir);
        [TestMethod] public void ComputeSeq_164_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_164", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_165_fir() => TestTools.VerifyMakeGraph("ComputeSeq_165", "fir", TestDir);
        [TestMethod] public void ComputeSeq_165_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_165", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_166_fir() => TestTools.VerifyMakeGraph("ComputeSeq_166", "fir", TestDir);
        [TestMethod] public void ComputeSeq_166_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_166", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_167_fir() => TestTools.VerifyMakeGraph("ComputeSeq_167", "fir", TestDir);
        [TestMethod] public void ComputeSeq_167_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_167", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_168_fir() => TestTools.VerifyMakeGraph("ComputeSeq_168", "fir", TestDir);
        [TestMethod] public void ComputeSeq_168_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_168", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_169_fir() => TestTools.VerifyMakeGraph("ComputeSeq_169", "fir", TestDir);
        [TestMethod] public void ComputeSeq_169_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_169", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_170_fir() => TestTools.VerifyMakeGraph("ComputeSeq_170", "fir", TestDir);
        [TestMethod] public void ComputeSeq_170_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_170", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_171_fir() => TestTools.VerifyMakeGraph("ComputeSeq_171", "fir", TestDir);
        [TestMethod] public void ComputeSeq_171_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_171", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_172_fir() => TestTools.VerifyMakeGraph("ComputeSeq_172", "fir", TestDir);
        [TestMethod] public void ComputeSeq_172_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_172", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_173_fir() => TestTools.VerifyMakeGraph("ComputeSeq_173", "fir", TestDir);
        [TestMethod] public void ComputeSeq_173_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_173", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_174_fir() => TestTools.VerifyMakeGraph("ComputeSeq_174", "fir", TestDir);
        [TestMethod] public void ComputeSeq_174_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_174", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_175_fir() => TestTools.VerifyMakeGraph("ComputeSeq_175", "fir", TestDir);
        [TestMethod] public void ComputeSeq_175_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_175", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_176_fir() => TestTools.VerifyMakeGraph("ComputeSeq_176", "fir", TestDir);
        [TestMethod] public void ComputeSeq_176_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_176", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_177_fir() => TestTools.VerifyMakeGraph("ComputeSeq_177", "fir", TestDir);
        [TestMethod] public void ComputeSeq_177_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_177", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_178_fir() => TestTools.VerifyMakeGraph("ComputeSeq_178", "fir", TestDir);
        [TestMethod] public void ComputeSeq_178_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_178", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_179_fir() => TestTools.VerifyMakeGraph("ComputeSeq_179", "fir", TestDir);
        [TestMethod] public void ComputeSeq_179_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_179", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_180_fir() => TestTools.VerifyMakeGraph("ComputeSeq_180", "fir", TestDir);
        [TestMethod] public void ComputeSeq_180_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_180", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_181_fir() => TestTools.VerifyMakeGraph("ComputeSeq_181", "fir", TestDir);
        [TestMethod] public void ComputeSeq_181_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_181", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_182_fir() => TestTools.VerifyMakeGraph("ComputeSeq_182", "fir", TestDir);
        [TestMethod] public void ComputeSeq_182_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_182", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_183_fir() => TestTools.VerifyMakeGraph("ComputeSeq_183", "fir", TestDir);
        [TestMethod] public void ComputeSeq_183_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_183", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_184_fir() => TestTools.VerifyMakeGraph("ComputeSeq_184", "fir", TestDir);
        [TestMethod] public void ComputeSeq_184_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_184", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_185_fir() => TestTools.VerifyMakeGraph("ComputeSeq_185", "fir", TestDir);
        [TestMethod] public void ComputeSeq_185_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_185", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_186_fir() => TestTools.VerifyMakeGraph("ComputeSeq_186", "fir", TestDir);
        [TestMethod] public void ComputeSeq_186_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_186", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_187_fir() => TestTools.VerifyMakeGraph("ComputeSeq_187", "fir", TestDir);
        [TestMethod] public void ComputeSeq_187_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_187", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_188_fir() => TestTools.VerifyMakeGraph("ComputeSeq_188", "fir", TestDir);
        [TestMethod] public void ComputeSeq_188_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_188", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_189_fir() => TestTools.VerifyMakeGraph("ComputeSeq_189", "fir", TestDir);
        [TestMethod] public void ComputeSeq_189_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_189", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_190_fir() => TestTools.VerifyMakeGraph("ComputeSeq_190", "fir", TestDir);
        [TestMethod] public void ComputeSeq_190_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_190", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_191_fir() => TestTools.VerifyMakeGraph("ComputeSeq_191", "fir", TestDir);
        [TestMethod] public void ComputeSeq_191_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_191", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_192_fir() => TestTools.VerifyMakeGraph("ComputeSeq_192", "fir", TestDir);
        [TestMethod] public void ComputeSeq_192_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_192", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_193_fir() => TestTools.VerifyMakeGraph("ComputeSeq_193", "fir", TestDir);
        [TestMethod] public void ComputeSeq_193_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_193", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_194_fir() => TestTools.VerifyMakeGraph("ComputeSeq_194", "fir", TestDir);
        [TestMethod] public void ComputeSeq_194_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_194", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_195_fir() => TestTools.VerifyMakeGraph("ComputeSeq_195", "fir", TestDir);
        [TestMethod] public void ComputeSeq_195_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_195", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_196_fir() => TestTools.VerifyMakeGraph("ComputeSeq_196", "fir", TestDir);
        [TestMethod] public void ComputeSeq_196_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_196", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_197_fir() => TestTools.VerifyMakeGraph("ComputeSeq_197", "fir", TestDir);
        [TestMethod] public void ComputeSeq_197_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_197", "lo.fir", TestDir);

        [TestMethod] public void ComputeSeq_198_fir() => TestTools.VerifyMakeGraph("ComputeSeq_198", "fir", TestDir);
        [TestMethod] public void ComputeSeq_198_lo_fir() => TestTools.VerifyMakeGraph("ComputeSeq_198", "lo.fir", TestDir);


    }
}
