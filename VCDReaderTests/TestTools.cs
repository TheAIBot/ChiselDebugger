using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCDReader;

namespace VCDReaderTests
{
    internal static class TestTools
    {
        internal static void VerifyDeclarations(IDeclCmd[] expected, List<IDeclCmd> actual)
        {
            Assert.AreEqual(expected.Length, actual.Count, $"Not same amount of declarations.{Environment.NewLine}Expected: {expected.Length}{Environment.NewLine}Actual: {actual.Count}");

            for (int i = 0; i < expected.Length; i++)
            {
                if (expected[i] is VarDef expectedVar && actual[i] is VarDef actualVar)
                {
                    VerifyVarDef(expectedVar, actualVar);
                }
                else
                {
                    Assert.AreEqual(expected[i], actual[i]);
                }
            }
        }

        internal static void VerifySimCmds(ISimCmd[] expected, ISimCmd[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length, $"Incorrect amount of simulation commands.{Environment.NewLine}Expected: {expected.Length}{Environment.NewLine}Actual: {actual.Length}");

            for (int i = 0; i < expected.Length; i++)
            {
                if (expected[i] is DumpAll expectedAll && actual[i] is DumpAll actualAll)
                {
                    Assert.AreEqual(expectedAll.Values.Count, actualAll.Values.Count);
                    for (int z = 0; z < expectedAll.Values.Count; z++)
                    {
                        VerifyValueChange(expectedAll.Values[z], actualAll.Values[z]);
                    }
                }
                else if (expected[i] is DumpVars expectedVars && actual[i] is DumpVars actualVars)
                {
                    Assert.AreEqual(expectedVars.InitialValues.Count, actualVars.InitialValues.Count);
                    for (int z = 0; z < expectedVars.InitialValues.Count; z++)
                    {
                        VerifyValueChange(expectedVars.InitialValues[z], actualVars.InitialValues[z]);
                    }
                }
                else if (expected[i] is DumpOn expectedOn && actual[i] is DumpOn actualOn)
                {

                }
                else if (expected[i] is DumpOff expectedOff && actual[i] is DumpOff actualOff)
                {

                }
                else if (expected[i] is SimTime expectedTime && actual[i] is SimTime actualTime)
                {
                    Assert.AreEqual(expectedTime.Time, actualTime.Time);
                }
                else if (expected[i] is IValueChange expectedChange && actual[i] is IValueChange actualChange)
                {
                    VerifyValueChange(expectedChange, actualChange);
                }
                else
                {
                    Assert.Fail($"{Environment.NewLine}Expected: {expected[i]}{Environment.NewLine}Actual: {actual[i]}");
                }
            }
        }

        internal static void VerifyValueChange(IValueChange expected, IValueChange actual)
        {
            if (expected is BinaryChange expectedBin && actual is BinaryChange actualBin)
            {
                CollectionAssert.AreEqual(expectedBin.Bits, actualBin.Bits);
                VerifyVarDef(expectedBin.Variable, actualBin.Variable);
            }
            else if (expected is RealChange expectedReal && actual is RealChange actualReal)
            {
                Assert.AreEqual(expectedReal.Value, actualReal.Value);
                VerifyVarDef(expectedReal.Variable, actualReal.Variable);
            }
            else
            {
                Assert.Fail($"{Environment.NewLine}Expected: {expected}{Environment.NewLine}Actual: {actual}");
            }
        }

        internal static void VerifyVarDef(VarDef expected, VarDef actual)
        {
            Assert.AreEqual(expected.Type, actual.Type);
            Assert.AreEqual(expected.Size, actual.Size);
            Assert.AreEqual(expected.ID, actual.ID);
            Assert.AreEqual(expected.Reference, actual.Reference);
            CollectionAssert.AreEqual(expected.Scopes, actual.Scopes);
        }
    }
}
