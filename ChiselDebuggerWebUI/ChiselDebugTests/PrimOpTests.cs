using ChiselDebug;
using FIRRTL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebugTests
{
    [TestClass]
    public class PrimOpTests
    {
        [TestMethod] public void PrimOpAdd() => VerifyPrimOpBiArg("add");
        [TestMethod] public void PrimOpSub() => VerifyPrimOpBiArg("sub");
        [TestMethod] public void PrimOpMul() => VerifyPrimOpBiArg("mul");
        [TestMethod] public void PrimOpDiv() => VerifyPrimOpBiArg("div");
        [TestMethod] public void PrimOpMod() => VerifyPrimOpBiArg("mod");
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


        private void VerifyPrimOpBiArg(string opName, int outputSize = 8)
        {
            string firrtl = @$"
circuit ModA : 
  module ModA : 
    input a : UInt<8>
    input b : UInt<8>
    output dout : UInt<{outputSize}>
    
    dout <= {opName}(a, b)";

            VerifyCanCreateGraph(firrtl);
        }

        private void VerifyPrimOpMonoArg(string opName, int outputSize = 8)
        {
            string firrtl = @$"
circuit ModA : 
  module ModA : 
    input a : UInt<8>
    input b : UInt<8>
    output dout : UInt<{outputSize}>
    
    dout <= {opName}(a)";

            VerifyCanCreateGraph(firrtl);
        }

        private void VerifyPrimOpMonoArgMonoConst(string opName, int outputSize = 8)
        {
            string firrtl = @$"
circuit ModA : 
  module ModA : 
    input a : UInt<8>
    input b : UInt<8>
    output dout : UInt<{outputSize}>
    
    dout <= {opName}(a, 7)";

            VerifyCanCreateGraph(firrtl);
        }

        private void VerifyPrimOpMonoArgBiConst(string opName, int outputSize = 8)
        {
            string firrtl = @$"
circuit ModA : 
  module ModA : 
    input a : UInt<8>
    input b : UInt<8>
    output dout : UInt<{outputSize}>
    
    dout <= {opName}(a, 7. 4)";

            VerifyCanCreateGraph(firrtl);
        }

        private void VerifyCanCreateGraph(string firrtl)
        {
            Circuit circuit = FIRRTL.Parse.FromString(firrtl);
            CircuitGraph graph = CircuitToGraph.GetAsGraph(circuit);
            graph.InferTypes();
        }
    }
}
