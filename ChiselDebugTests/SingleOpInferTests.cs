using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebugTests
{
    [TestClass]
    public class SingleOpInferTests
    {
        const string TestDir = "../../../../TestGenerator/OthersFIRRTL";

        [TestMethod] public void ComputeSingle_fir() => TestTools.VerifyInferTypes("ComputeSingle", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_0_fir() => TestTools.VerifyInferTypes("ComputeSingle_0", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_0_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_0", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_1_fir() => TestTools.VerifyInferTypes("ComputeSingle_1", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_1_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_1", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_2_fir() => TestTools.VerifyInferTypes("ComputeSingle_2", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_2_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_2", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_3_fir() => TestTools.VerifyInferTypes("ComputeSingle_3", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_3_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_3", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_4_fir() => TestTools.VerifyInferTypes("ComputeSingle_4", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_4_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_4", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_5_fir() => TestTools.VerifyInferTypes("ComputeSingle_5", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_5_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_5", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_6_fir() => TestTools.VerifyInferTypes("ComputeSingle_6", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_6_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_6", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_7_fir() => TestTools.VerifyInferTypes("ComputeSingle_7", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_7_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_7", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_8_fir() => TestTools.VerifyInferTypes("ComputeSingle_8", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_8_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_8", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_9_fir() => TestTools.VerifyInferTypes("ComputeSingle_9", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_9_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_9", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_10_fir() => TestTools.VerifyInferTypes("ComputeSingle_10", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_10_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_10", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_11_fir() => TestTools.VerifyInferTypes("ComputeSingle_11", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_11_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_11", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_12_fir() => TestTools.VerifyInferTypes("ComputeSingle_12", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_12_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_12", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_13_fir() => TestTools.VerifyInferTypes("ComputeSingle_13", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_13_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_13", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_14_fir() => TestTools.VerifyInferTypes("ComputeSingle_14", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_14_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_14", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_15_fir() => TestTools.VerifyInferTypes("ComputeSingle_15", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_15_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_15", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_16_fir() => TestTools.VerifyInferTypes("ComputeSingle_16", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_16_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_16", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_17_fir() => TestTools.VerifyInferTypes("ComputeSingle_17", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_17_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_17", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_18_fir() => TestTools.VerifyInferTypes("ComputeSingle_18", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_18_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_18", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_19_fir() => TestTools.VerifyInferTypes("ComputeSingle_19", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_19_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_19", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_20_fir() => TestTools.VerifyInferTypes("ComputeSingle_20", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_20_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_20", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_21_fir() => TestTools.VerifyInferTypes("ComputeSingle_21", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_21_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_21", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_22_fir() => TestTools.VerifyInferTypes("ComputeSingle_22", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_22_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_22", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_23_fir() => TestTools.VerifyInferTypes("ComputeSingle_23", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_23_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_23", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_24_fir() => TestTools.VerifyInferTypes("ComputeSingle_24", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_24_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_24", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_25_fir() => TestTools.VerifyInferTypes("ComputeSingle_25", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_25_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_25", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_26_fir() => TestTools.VerifyInferTypes("ComputeSingle_26", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_26_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_26", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_27_fir() => TestTools.VerifyInferTypes("ComputeSingle_27", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_27_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_27", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_28_fir() => TestTools.VerifyInferTypes("ComputeSingle_28", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_28_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_28", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_29_fir() => TestTools.VerifyInferTypes("ComputeSingle_29", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_29_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_29", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_30_fir() => TestTools.VerifyInferTypes("ComputeSingle_30", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_30_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_30", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_31_fir() => TestTools.VerifyInferTypes("ComputeSingle_31", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_31_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_31", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_32_fir() => TestTools.VerifyInferTypes("ComputeSingle_32", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_32_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_32", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_33_fir() => TestTools.VerifyInferTypes("ComputeSingle_33", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_33_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_33", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_34_fir() => TestTools.VerifyInferTypes("ComputeSingle_34", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_34_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_34", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_35_fir() => TestTools.VerifyInferTypes("ComputeSingle_35", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_35_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_35", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_36_fir() => TestTools.VerifyInferTypes("ComputeSingle_36", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_36_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_36", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_37_fir() => TestTools.VerifyInferTypes("ComputeSingle_37", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_37_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_37", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_38_fir() => TestTools.VerifyInferTypes("ComputeSingle_38", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_38_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_38", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_39_fir() => TestTools.VerifyInferTypes("ComputeSingle_39", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_39_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_39", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_40_fir() => TestTools.VerifyInferTypes("ComputeSingle_40", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_40_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_40", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_41_fir() => TestTools.VerifyInferTypes("ComputeSingle_41", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_41_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_41", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_42_fir() => TestTools.VerifyInferTypes("ComputeSingle_42", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_42_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_42", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_43_fir() => TestTools.VerifyInferTypes("ComputeSingle_43", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_43_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_43", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_44_fir() => TestTools.VerifyInferTypes("ComputeSingle_44", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_44_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_44", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_45_fir() => TestTools.VerifyInferTypes("ComputeSingle_45", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_45_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_45", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_46_fir() => TestTools.VerifyInferTypes("ComputeSingle_46", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_46_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_46", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_47_fir() => TestTools.VerifyInferTypes("ComputeSingle_47", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_47_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_47", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_48_fir() => TestTools.VerifyInferTypes("ComputeSingle_48", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_48_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_48", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_49_fir() => TestTools.VerifyInferTypes("ComputeSingle_49", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_49_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_49", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_50_fir() => TestTools.VerifyInferTypes("ComputeSingle_50", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_50_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_50", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_51_fir() => TestTools.VerifyInferTypes("ComputeSingle_51", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_51_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_51", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_52_fir() => TestTools.VerifyInferTypes("ComputeSingle_52", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_52_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_52", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_53_fir() => TestTools.VerifyInferTypes("ComputeSingle_53", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_53_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_53", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_54_fir() => TestTools.VerifyInferTypes("ComputeSingle_54", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_54_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_54", "lo.fir", false, TestDir);

        [TestMethod] public void ComputeSingle_55_fir() => TestTools.VerifyInferTypes("ComputeSingle_55", "fir", false, TestDir);
        [TestMethod] public void ComputeSingle_55_lo_fir() => TestTools.VerifyInferTypes("ComputeSingle_55", "lo.fir", false, TestDir);


    }
}
