using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCDReader;

namespace VCDReaderTests
{
    [TestClass]
    public class SimulationCmdTests
    {
        [TestMethod]
        public void ParseScalarValueChange()
        {
            foreach (BitState bitState in Enum.GetValues(typeof(BitState)))
            {

                string vcdString = @$"
$var wire 1 ! _T_4 $end
$enddefinitions $end
{bitState.ToChar()}!";
                VCD vcd = Parse.FromString(vcdString);

                Assert.AreEqual(1, vcd.Declarations.Count);
                Assert.IsTrue(vcd.Declarations[0] is VarDef);

                var variable = vcd.Declarations[0] as VarDef;

                ISimCmd[] simCmds = vcd.GetSimulationCommands().ToArray();
                Assert.AreEqual(1, simCmds.Length);
                Assert.IsTrue(simCmds[0] is BinaryChange);

                var valueChange = simCmds[0] as BinaryChange;
                Assert.AreEqual(1, valueChange.Bits.Length);
                Assert.AreEqual(bitState, valueChange.Bits[0]);
                Assert.AreEqual(variable, valueChange.Variable);
            }
        }

        [TestMethod]
        public void ParseVectorBinarySize1ValueChange()
        {
            foreach (BitState bitState in Enum.GetValues(typeof(BitState)))
            {

                string vcdString = @$"
$var wire 1 ! _T_4 $end
$enddefinitions $end
b{bitState.ToChar()} !";
                VCD vcd = Parse.FromString(vcdString);

                Assert.AreEqual(1, vcd.Declarations.Count);
                Assert.IsTrue(vcd.Declarations[0] is VarDef);

                var variable = vcd.Declarations[0] as VarDef;

                ISimCmd[] simCmds = vcd.GetSimulationCommands().ToArray();
                Assert.AreEqual(1, simCmds.Length);
                Assert.IsTrue(simCmds[0] is BinaryChange);

                var valueChange = simCmds[0] as BinaryChange;
                Assert.AreEqual(1, valueChange.Bits.Length);
                Assert.AreEqual(bitState, valueChange.Bits[0]);
                Assert.AreEqual(variable, valueChange.Variable);
            }
        }

        [TestMethod]
        public void ParseVectorBinarySize4ValueChange()
        {
            BitState[] expectedBits = new BitState[] { BitState.Zero, BitState.One, BitState.X, BitState.Z };

            string vcdString = @$"
$var wire {expectedBits.Length} ! _T_4 $end
$enddefinitions $end
b{expectedBits.BitsToString()} !";
            VCD vcd = Parse.FromString(vcdString);

            Assert.AreEqual(1, vcd.Declarations.Count);
            Assert.IsTrue(vcd.Declarations[0] is VarDef);

            var variable = vcd.Declarations[0] as VarDef;

            ISimCmd[] simCmds = vcd.GetSimulationCommands().ToArray();
            Assert.AreEqual(1, simCmds.Length);
            Assert.IsTrue(simCmds[0] is BinaryChange);

            var valueChange = simCmds[0] as BinaryChange;
            CollectionAssert.AreEqual(expectedBits, valueChange.Bits);
            Assert.AreEqual(variable, valueChange.Variable);
        }

        [TestMethod]
        public void ParseVectorRealSize4ValueChange()
        {
            double expectedValue = 3.1415;
            string vcdString = @$"
$var wire 5 ! _T_4 $end
$enddefinitions $end
r{expectedValue.ToString(CultureInfo.InvariantCulture)} !";
            VCD vcd = Parse.FromString(vcdString);

            Assert.AreEqual(1, vcd.Declarations.Count);
            Assert.IsTrue(vcd.Declarations[0] is VarDef);

            var variable = vcd.Declarations[0] as VarDef;

            ISimCmd[] simCmds = vcd.GetSimulationCommands().ToArray();
            Assert.AreEqual(1, simCmds.Length);
            Assert.IsTrue(simCmds[0] is RealChange);

            var valueChange = simCmds[0] as RealChange;
            Assert.AreEqual(expectedValue, valueChange.Value);
            Assert.AreEqual(variable, valueChange.Variable);
        }
    }
}
