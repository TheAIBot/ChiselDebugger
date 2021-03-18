using ChiselDebug;
using FIRRTL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChiselDebugTests
{
    [TestClass]
    public class RegisterTests
    {
        [TestMethod]
        public void RegGroundType()
        {
            string firrtl = @"
circuit ModA : 
  module ModA : 
    input clock : Clock
    input reset : UInt<1>
    output io : {flip din : UInt<8>, dout : UInt<8>}
    
    reg rr : UInt<8>, clock
    rr <= io.din
    io.dout <= rr";

            TestTools.VerifyCanCreateGraph(firrtl);
        }

        [TestMethod]
        public void RegBundle()
        {
            string firrtl = @"
circuit ModA : 
  module ModA : 
    input clock : Clock
    input reset : UInt<1>
    output io : {flip din : {a1 : UInt<8>, a2 : SInt<4>}, dout : {a1 : UInt<8>, a2 : SInt<4>}}
    
    reg rr : {a1 : UInt<8>, a2 : SInt<4>}, clock
    rr <= io.din
    io.dout <= rr";

            TestTools.VerifyCanCreateGraph(firrtl);
        }

        [TestMethod]
        public void RegVector()
        {
            string firrtl = @"
circuit ModA : 
  module ModA : 
    input clock : Clock
    input reset : UInt<1>
    output io : {flip din : UInt<8>[5], dout : UInt<8>[5]}
    
    reg rr : UInt<8>[5], clock
    rr <= io.din
    io.dout <= rr";

            TestTools.VerifyCanCreateGraph(firrtl);
        }
    }
}
