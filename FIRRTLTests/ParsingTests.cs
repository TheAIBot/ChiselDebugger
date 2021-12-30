using FIRRTL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FIRRTLTests
{
    [TestClass]
    public class ParsingTests
    {
        [TestMethod]
        public void ParseOneModule()
        {
            string firrtlCode = @"
circuit testCircuit1 : 
  module testModule1 : 
    input clock : Clock";

            Circuit circuit = Parse.FromString(firrtlCode);
            Assert.IsNotNull(circuit);
        }
    }
}