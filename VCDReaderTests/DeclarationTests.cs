using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using VCDReader;

namespace VCDReaderTests
{
    [TestClass]
    public class DeclarationTests
    {
        [TestMethod]
        public void ParseComment()
        {
            string vcdString = @"
$comment test $end 
$enddefinitions $end";
            VCD vcd = Parse.FromString(vcdString);

            Assert.AreEqual(1, vcd.Declarations.Count);
            Assert.IsTrue(vcd.Declarations[0] is Comment);

            var decl = vcd.Declarations[0] as Comment;
            Assert.AreEqual(" test ", decl.Content);
        }

        [TestMethod]
        public void ParseDate()
        {
            string vcdString = @"
$date test $end 
$enddefinitions $end";
            VCD vcd = Parse.FromString(vcdString);

            Assert.AreEqual(1, vcd.Declarations.Count);
            Assert.IsTrue(vcd.Declarations[0] is Date);

            var decl = vcd.Declarations[0] as Date;
            Assert.AreEqual(" test ", decl.Content);
        }

        [TestMethod]
        public void ParseVersion()
        {
            string vcdString = @"
$version test $lol $end 
$enddefinitions $end";
            VCD vcd = Parse.FromString(vcdString);

            Assert.AreEqual(1, vcd.Declarations.Count);
            Assert.IsTrue(vcd.Declarations[0] is VCDReader.Version);

            var decl = vcd.Declarations[0] as VCDReader.Version;
            Assert.AreEqual(" test ", decl.VersionText);
            Assert.AreEqual("$lol ", decl.SystemTask);
        }

        [TestMethod]
        public void ParseTimeScale()
        {
            foreach (TimeUnit unit in Enum.GetValues(typeof(TimeUnit)))
            {
                string vcdString = @$"
$timescale 100 {unit.ToString().ToLower()} $end 
$enddefinitions $end";

                VCD vcd = Parse.FromString(vcdString);

                Assert.AreEqual(1, vcd.Declarations.Count);
                Assert.IsTrue(vcd.Declarations[0] is TimeScale);

                var decl = vcd.Declarations[0] as TimeScale;
                Assert.AreEqual(100, decl.Scale);
                Assert.AreEqual(unit, decl.Unit);
            }
        }

        [TestMethod]
        public void ParseScope()
        {
            string vcdString = @"
$scope module lol $end
$upscope $end
$enddefinitions $end";
            VCD vcd = Parse.FromString(vcdString);

            Assert.AreEqual(1, vcd.Declarations.Count);
            Assert.IsTrue(vcd.Declarations[0] is VCDReader.Version);

            var decl = vcd.Declarations[0] as VCDReader.Version;
            Assert.AreEqual(" test ", decl.VersionText);
            Assert.AreEqual("$lol ", decl.SystemTask);
        }
    }
}
