using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChiselDebug;
using FIRRTL;
using VCDReader;


namespace ChiselDebugTests
{
    [TestClass]
    public class FIRRTLGraphTests
    {
        [TestMethod]
        public void SimpleIO1()
        {
            string firrtl = @"
circuit ModA : 
  module ModA : 
    input clock : Clock
    input reset : UInt<1>
    output io : {flip din : UInt<8>, dout : UInt<8>}
    
    io.dout <= io.din @[Random.scala 15:13]";

            Circuit circuit = FIRRTL.Parse.FromString(firrtl);
            CircuitToGraph.GetAsGraph(circuit);
        }

        [TestMethod]
        public void Nested1Module()
        {
            string firrtl = @"
circuit ModB : 
  module ModA : 
    input clock : Clock
    input reset : Reset
    output io : {flip din : UInt<8>, dout : UInt<8>}
    
    io.dout <= io.din @[Random.scala 15:13]
    
  module ModB : 
    input clock : Clock
    input reset : UInt<1>
    output io : {flip din : UInt<8>, dout : UInt<8>}
    
    inst a of ModA @[Random.scala 24:19]
    a.clock <= clock
    a.reset <= reset
    a.io.din <= io.din @[Random.scala 25:14]
    io.dout <= a.io.dout @[Random.scala 26:13]";

            Circuit circuit = FIRRTL.Parse.FromString(firrtl);
            CircuitToGraph.GetAsGraph(circuit);
        }

        [TestMethod]
        public void Nested1Module2x()
        {
            string firrtl = @"
circuit ModC : 
  module ModA : 
    input clock : Clock
    input reset : Reset
    output io : {flip din : UInt<8>, dout : UInt<8>}
    
    io.dout <= io.din @[Random.scala 15:13]
    
  module ModA_1 : 
    input clock : Clock
    input reset : Reset
    output io : {flip din : UInt<8>, dout : UInt<8>}
    
    io.dout <= io.din @[Random.scala 15:13]
    
  module ModC : 
    input clock : Clock
    input reset : UInt<1>
    output io : {flip din : UInt<8>, dout : UInt<8>}
    
    inst a1 of ModA @[Random.scala 35:20]
    a1.clock <= clock
    a1.reset <= reset
    inst a2 of ModA_1 @[Random.scala 36:20]
    a2.clock <= clock
    a2.reset <= reset
    a1.io.din <= io.din @[Random.scala 37:15]
    a2.io.din <= a1.io.dout @[Random.scala 38:15]
    io.dout <= a2.io.dout @[Random.scala 39:13]";

            Circuit circuit = FIRRTL.Parse.FromString(firrtl);
            CircuitToGraph.GetAsGraph(circuit);
        }

        [TestMethod]
        public void MuxOnBundles()
        {
            string firrtl = @"
circuit ModD : 
  module ModD : 
    input clock : Clock
    input reset : UInt<1>
    output io : {flip a : {a1 : UInt<8>, a2 : SInt<4>}, flip b : {a1 : UInt<8>, a2 : SInt<4>}, flip cond : UInt<1>, c : {a1 : UInt<8>, a2 : SInt<4>}}
    
    node _T = mux(io.cond, io.a, io.b) @[Random.scala 55:16]
    io.c.a2 <= _T.a2 @[Random.scala 55:10]
    io.c.a1 <= _T.a1 @[Random.scala 55:10]";

            Circuit circuit = FIRRTL.Parse.FromString(firrtl);
            CircuitToGraph.GetAsGraph(circuit);
        }
    }
}
