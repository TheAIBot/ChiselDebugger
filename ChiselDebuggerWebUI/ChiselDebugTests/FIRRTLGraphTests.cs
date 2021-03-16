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
            CircuitGraph graph = CircuitToGraph.GetAsGraph(circuit);
        }
    }
}
