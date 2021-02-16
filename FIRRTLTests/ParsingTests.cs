using Microsoft.VisualStudio.TestTools.UnitTesting;
using FIRRTL;
using FIRRTL.Parsing;

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

            Circuit circuit = Parse.ParseString(firrtlCode);
            Assert.IsNotNull(circuit);
        }
    }
}