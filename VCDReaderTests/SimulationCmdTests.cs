﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
                using VCD vcd = Parse.FromString(vcdString);

                Assert.AreEqual(1, vcd.Declarations.Count);
                Assert.IsTrue(vcd.Declarations[0] is VarDef);

                var variable = vcd.Declarations[0] as VarDef;

                ISimCmd[] simCmds = vcd.GetSimulationCommands().ToArray();
                Assert.AreEqual(1, simCmds.Length);
                Assert.IsTrue(simCmds[0] is BinaryVarValue);

                var valueChange = (BinaryVarValue)simCmds[0];
                Assert.AreEqual(1, valueChange.Bits.Length);
                Assert.AreEqual(bitState, valueChange.Bits[0]);
                Assert.AreEqual(variable, valueChange.Variables[0]);
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
                using VCD vcd = Parse.FromString(vcdString);

                Assert.AreEqual(1, vcd.Declarations.Count);
                Assert.IsTrue(vcd.Declarations[0] is VarDef);

                var variable = vcd.Declarations[0] as VarDef;

                ISimCmd[] simCmds = vcd.GetSimulationCommands().ToArray();
                Assert.AreEqual(1, simCmds.Length);
                Assert.IsTrue(simCmds[0] is BinaryVarValue);

                var valueChange = (BinaryVarValue)simCmds[0];
                Assert.AreEqual(1, valueChange.Bits.Length);
                Assert.AreEqual(bitState, valueChange.Bits[0]);
                Assert.AreEqual(variable, valueChange.Variables[0]);
            }
        }

        [TestMethod]
        public void ParseVectorBinarySize2ValueChange()
        {
            IDeclCmd[] expectedDecls = new IDeclCmd[]
            {
                new VarDef(VarType.Wire, 2, "b1", "b1", Array.Empty<Scope>())
            };
            ISimCmd[] expectedSimCmds = new ISimCmd[]
            {
                new BinaryVarValue(new BitState[] { BitState.Zero, BitState.Zero }, new List<VarDef>() {(VarDef)expectedDecls[0] }, true)
            };

            string vcdString = @$"
$var wire 2 b1 b1 $end
$enddefinitions $end
b00 b1";
            using VCD vcd = Parse.FromString(vcdString);

            TestTools.VerifyDeclarations(expectedDecls, vcd.Declarations);
            TestTools.VerifySimCmds(expectedSimCmds, vcd.GetSimulationCommands().ToArray());
        }

        [TestMethod]
        public void ParseVCDWithEndAsSkipChars()
        {
            string vcdString = @$"
$var wire 2 b1 b1 $end
$enddefinitions $end
b0 b1
 
   
";
            using VCD vcd = Parse.FromString(vcdString);
        }

        [TestMethod]
        public void ParseVectorBinarySize4ValueChange()
        {
            BitState[] expectedBits = new BitState[] { BitState.Zero, BitState.One, BitState.X, BitState.Z };

            string vcdString = @$"
$var wire {expectedBits.Length} ! _T_4 $end
$enddefinitions $end
b{expectedBits.BitsToString()} !";
            using VCD vcd = Parse.FromString(vcdString);

            Assert.AreEqual(1, vcd.Declarations.Count);
            Assert.IsTrue(vcd.Declarations[0] is VarDef);

            var variable = vcd.Declarations[0] as VarDef;

            ISimCmd[] simCmds = vcd.GetSimulationCommands().ToArray();
            Assert.AreEqual(1, simCmds.Length);
            Assert.IsTrue(simCmds[0] is BinaryVarValue);

            var valueChange = (BinaryVarValue)simCmds[0];
            CollectionAssert.AreEqual(expectedBits, valueChange.Bits.ToArray());
            Assert.AreEqual(variable, valueChange.Variables[0]);
        }

        [TestMethod]
        public void ParseVectorBinarySize22ValueChange()
        {
            BitState[] expectedBits = new BitState[]
            {
                BitState.One, BitState.One,
                BitState.Zero, BitState.Zero,
                BitState.Zero, BitState.One,
                BitState.Zero, BitState.Zero,
                BitState.Zero, BitState.One,
                BitState.Zero, BitState.One,
                BitState.One, BitState.Zero,
                BitState.Zero, BitState.One,
                BitState.Zero, BitState.One,
                BitState.One, BitState.One,
                BitState.Zero, BitState.One
            };

            string vcdString = @$"
$var wire {expectedBits.Length} ! _T_4 $end
$enddefinitions $end
b{expectedBits.BitsToString()} !";
            using VCD vcd = Parse.FromString(vcdString);

            Assert.AreEqual(1, vcd.Declarations.Count);
            Assert.IsTrue(vcd.Declarations[0] is VarDef);

            var variable = vcd.Declarations[0] as VarDef;

            ISimCmd[] simCmds = vcd.GetSimulationCommands().ToArray();
            Assert.AreEqual(1, simCmds.Length);
            Assert.IsTrue(simCmds[0] is BinaryVarValue);

            var valueChange = (BinaryVarValue)simCmds[0];
            CollectionAssert.AreEqual(expectedBits, valueChange.Bits.ToArray());
            Assert.AreEqual(variable, valueChange.Variables[0]);
        }

        [TestMethod]
        public void ParseVectorRealSize4ValueChange()
        {
            double expectedValue = 3.1415;
            string vcdString = @$"
$var wire 5 ! _T_4 $end
$enddefinitions $end
r{expectedValue.ToString(CultureInfo.InvariantCulture)} !";
            using VCD vcd = Parse.FromString(vcdString);

            Assert.AreEqual(1, vcd.Declarations.Count);
            Assert.IsTrue(vcd.Declarations[0] is VarDef);

            var variable = vcd.Declarations[0] as VarDef;

            ISimCmd[] simCmds = vcd.GetSimulationCommands().ToArray();
            Assert.AreEqual(1, simCmds.Length);
            Assert.IsTrue(simCmds[0] is RealVarValue);

            var valueChange = (RealVarValue)simCmds[0];
            Assert.AreEqual(expectedValue, valueChange.Value);
            Assert.AreEqual(variable, valueChange.Variables[0]);
        }

        [TestMethod]
        public void ParseDumpVars()
        {
            Scope scope = new Scope(ScopeType.Module, "tester1");
            Scope[] scopes = { scope };

            IDeclCmd[] expectedDecls = new IDeclCmd[]
            {
                scope,
                new VarDef(VarType.Wire, 4, "'", "values_2/in", scopes),
                new VarDef(VarType.Wire, 4, "(", "values_1", scopes),
                new VarDef(VarType.Wire, 1, "-", "clock", scopes),
                new VarDef(VarType.Wire, 4, ".", "values_0", scopes),
                new UpScope()
            };

            ISimCmd[] expectedSimCmds = new ISimCmd[]
            {
                new DumpVars(new List<VarValue>()
                {
                    new BinaryVarValue(new BitState[] { BitState.Zero, BitState.Zero, BitState.Zero, BitState.Zero }, new List<VarDef>() { (VarDef)expectedDecls[1] }, true),
                    new BinaryVarValue(new BitState[] { BitState.Zero, BitState.Zero, BitState.Zero, BitState.Zero }, new List<VarDef>() { (VarDef)expectedDecls[4] }, true),
                    new BinaryVarValue(new BitState[] { BitState.Zero, BitState.Zero, BitState.Zero, BitState.Zero }, new List<VarDef>() { (VarDef)expectedDecls[2] }, true),
                    new BinaryVarValue(new BitState[] { BitState.Zero }, new List<VarDef>() { (VarDef)expectedDecls[3] }, true)
                })
            };

            string vcdString = @"
$scope module tester1 $end
$var wire 4 ' values_2/in $end
$var wire 4 ( values_1 $end
$var wire 1 - clock $end
$var wire 4 . values_0 $end
$upscope $end
$enddefinitions $end
$dumpvars
b0000 '
b0000 .
b0000 (
0-
$end";
            using VCD vcd = Parse.FromString(vcdString);

            TestTools.VerifyDeclarations(expectedDecls, vcd.Declarations);
            TestTools.VerifySimCmds(expectedSimCmds, vcd.GetSimulationCommands().ToArray());
        }

        [TestMethod]
        public void ParseSimTime()
        {
            ISimCmd[] expectedSimCmds = new ISimCmd[]
            {
                new SimTime(0),
                new SimTime(53)
            };

            string vcdString = @"
$enddefinitions $end
#0
#53";
            using VCD vcd = Parse.FromString(vcdString);

            TestTools.VerifySimCmds(expectedSimCmds, vcd.GetSimulationCommands().ToArray());
        }
    }
}
