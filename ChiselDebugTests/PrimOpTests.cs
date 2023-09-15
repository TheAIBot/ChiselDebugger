using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChiselDebugTests
{
    [TestClass]
    public class PrimOpTests
    {
        [TestMethod] public void PrimOpAdd() => VerifyPrimOpBiArg("add");
        [TestMethod] public void PrimOpSub() => VerifyPrimOpBiArg("sub");
        [TestMethod] public void PrimOpMul() => VerifyPrimOpBiArg("mul");
        [TestMethod] public void PrimOpDiv() => VerifyPrimOpBiArg("div");
        [TestMethod] public void PrimOpMod() => VerifyPrimOpBiArg("rem"); //Name in FIRRTL spec is mod
        [TestMethod] public void PrimOpLt() => VerifyPrimOpBiArg("lt");
        [TestMethod] public void PrimOpLeq() => VerifyPrimOpBiArg("leq");
        [TestMethod] public void PrimOpGt() => VerifyPrimOpBiArg("gt");
        [TestMethod] public void PrimOpGeq() => VerifyPrimOpBiArg("geq");
        [TestMethod] public void PrimOpEq() => VerifyPrimOpBiArg("eq");
        [TestMethod] public void PrimOpNeq() => VerifyPrimOpBiArg("neq");
        [TestMethod] public void PrimOpPad() => VerifyPrimOpMonoArgMonoConst("pad");
        [TestMethod] public void PrimOpAsUInt() => VerifyPrimOpMonoArg("asUInt");
        [TestMethod] public void PrimOpAsSInt() => VerifyPrimOpMonoArg("asSInt");
        [TestMethod] public void PrimOpAsClock() => VerifyPrimOpMonoArg("asClock");
        [TestMethod] public void PrimOpShl() => VerifyPrimOpMonoArgMonoConst("shl");
        [TestMethod] public void PrimOpShr() => VerifyPrimOpMonoArgMonoConst("shr");
        [TestMethod] public void PrimOpDshl() => VerifyPrimOpBiArg("dshl");
        [TestMethod] public void PrimOpDshr() => VerifyPrimOpBiArg("dshr");
        [TestMethod] public void PrimOpCvt() => VerifyPrimOpMonoArg("cvt");
        [TestMethod] public void PrimOpNeg() => VerifyPrimOpMonoArg("neg");
        [TestMethod] public void PrimOpNot() => VerifyPrimOpMonoArg("not");
        [TestMethod] public void PrimOpAnd() => VerifyPrimOpBiArg("and");
        [TestMethod] public void PrimOpOr() => VerifyPrimOpBiArg("or");
        [TestMethod] public void PrimOpXor() => VerifyPrimOpBiArg("xor");
        [TestMethod] public void PrimOpAndr() => VerifyPrimOpMonoArg("andr");
        [TestMethod] public void PrimOpOrr() => VerifyPrimOpMonoArg("orr");
        [TestMethod] public void PrimOpXorr() => VerifyPrimOpMonoArg("xorr");
        [TestMethod] public void PrimOpCat() => VerifyPrimOpBiArg("cat");
        [TestMethod] public void PrimOpBits() => VerifyPrimOpMonoArgBiConst("bits");
        [TestMethod] public void PrimOpHead() => VerifyPrimOpMonoArgMonoConst("head");
        [TestMethod] public void PrimOpTail() => VerifyPrimOpMonoArgMonoConst("tail");


        private static void VerifyPrimOpBiArg(string opName, int outputSize = 8)
        {
            string firrtl = @$"
circuit ModA : 
  module ModA : 
    input a : UInt<8>
    input b : UInt<8>
    output dout : UInt<{outputSize}>
    
    dout <= {opName}(a, b)";

            TestTools.VerifyCanCreateGraph(firrtl);
        }

        private static void VerifyPrimOpMonoArg(string opName, int outputSize = 8)
        {
            string firrtl = @$"
circuit ModA : 
  module ModA : 
    input a : UInt<8>
    input b : UInt<8>
    output dout : UInt<{outputSize}>
    
    dout <= {opName}(a)";

            TestTools.VerifyCanCreateGraph(firrtl);
        }

        private static void VerifyPrimOpMonoArgMonoConst(string opName, int outputSize = 8)
        {
            string firrtl = @$"
circuit ModA : 
  module ModA : 
    input a : UInt<8>
    input b : UInt<8>
    output dout : UInt<{outputSize}>
    
    dout <= {opName}(a, 7)";

            TestTools.VerifyCanCreateGraph(firrtl);
        }

        private static void VerifyPrimOpMonoArgBiConst(string opName, int outputSize = 8)
        {
            string firrtl = @$"
circuit ModA : 
  module ModA : 
    input a : UInt<8>
    input b : UInt<8>
    output dout : UInt<{outputSize}>
    
    dout <= {opName}(a, 7. 4)";

            TestTools.VerifyCanCreateGraph(firrtl);
        }


    }
}
