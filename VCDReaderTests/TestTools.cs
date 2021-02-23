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
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}
