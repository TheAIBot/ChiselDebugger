using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using VCDReader;

namespace VCDReaderTests
{
    [TestClass]
    public class DeclarationCmdTests
    {
        [TestMethod]
        public void ParseComment()
        {
            string vcdString = @"
$comment test $end 
$enddefinitions $end";
            using VCD vcd = Parse.FromString(vcdString);

            Assert.AreEqual(1, vcd.Declarations.Count);
            Assert.IsTrue(vcd.Declarations[0] is Comment);

            var decl = vcd.Declarations[0] as Comment;
            Assert.AreEqual("test", decl.Content);
        }

        [TestMethod]
        public void ParseDate()
        {
            string vcdString = @"
$date test $end 
$enddefinitions $end";
            using VCD vcd = Parse.FromString(vcdString);

            Assert.AreEqual(1, vcd.Declarations.Count);
            Assert.IsTrue(vcd.Declarations[0] is Date);

            var decl = vcd.Declarations[0] as Date;
            Assert.AreEqual("test", decl.Content);
        }

        [TestMethod]
        public void ParseVersion()
        {
            string vcdString = @"
$version test $lol $end 
$enddefinitions $end";
            using VCD vcd = Parse.FromString(vcdString);

            Assert.AreEqual(1, vcd.Declarations.Count);
            Assert.IsTrue(vcd.Declarations[0] is VCDReader.Version);

            var decl = vcd.Declarations[0] as VCDReader.Version;
            Assert.AreEqual("test", decl.VersionText);
            Assert.AreEqual("$lol", decl.SystemTask);
        }

        [TestMethod]
        public void ParseTimeScale()
        {
            foreach (TimeUnit unit in Enum.GetValues(typeof(TimeUnit)))
            {
                string vcdString = @$"
$timescale 100 {unit.ToString().ToLower()} $end 
$enddefinitions $end";

                using VCD vcd = Parse.FromString(vcdString);

                Assert.AreEqual(1, vcd.Declarations.Count);
                Assert.IsTrue(vcd.Declarations[0] is TimeScale);

                var decl = vcd.Declarations[0] as TimeScale;
                Assert.AreEqual(100, decl.Scale);
                Assert.AreEqual(unit, decl.Unit);
            }
        }

        [TestMethod]
        public void ParseScopeOpen1Closed1()
        {
            foreach (ScopeType scopeType in Enum.GetValues(typeof(ScopeType)))
            {
                string vcdString = @$"
$scope {scopeType.ToString().ToLower()} lol $end
$upscope $end
$enddefinitions $end";
                using VCD vcd = Parse.FromString(vcdString);

                Assert.AreEqual(2, vcd.Declarations.Count);
                Assert.IsTrue(vcd.Declarations[0] is Scope);
                Assert.IsTrue(vcd.Declarations[1] is UpScope);

                var decl = vcd.Declarations[0] as Scope;
                Assert.AreEqual(scopeType, decl.Type);
                Assert.AreEqual("lol", decl.Name);
            }
        }

        [TestMethod]
        public void ParseScopeNestedOpen2Closed2()
        {
            string vcdString = @"
$scope module lol $end
$scope module fish $end
$upscope $end
$upscope $end
$enddefinitions $end";
            using VCD vcd = Parse.FromString(vcdString);

            Assert.AreEqual(4, vcd.Declarations.Count);
            Assert.IsTrue(vcd.Declarations[0] is Scope);
            Assert.IsTrue(vcd.Declarations[1] is Scope);
            Assert.IsTrue(vcd.Declarations[2] is UpScope);
            Assert.IsTrue(vcd.Declarations[3] is UpScope);

            var decl = vcd.Declarations[0] as Scope;
            Assert.AreEqual(ScopeType.Module, decl.Type);
            Assert.AreEqual("lol", decl.Name);

            decl = vcd.Declarations[1] as Scope;
            Assert.AreEqual(ScopeType.Module, decl.Type);
            Assert.AreEqual("fish", decl.Name);
        }

        [TestMethod]
        public void ParseScopeOpen2Closed2()
        {
            string vcdString = @"
$scope module lol $end
$upscope $end
$scope module fish $end
$upscope $end
$enddefinitions $end";
            using VCD vcd = Parse.FromString(vcdString);

            Assert.AreEqual(4, vcd.Declarations.Count);
            Assert.IsTrue(vcd.Declarations[0] is Scope);
            Assert.IsTrue(vcd.Declarations[1] is UpScope);
            Assert.IsTrue(vcd.Declarations[2] is Scope);
            Assert.IsTrue(vcd.Declarations[3] is UpScope);

            var decl = vcd.Declarations[0] as Scope;
            Assert.AreEqual(ScopeType.Module, decl.Type);
            Assert.AreEqual("lol", decl.Name);

            decl = vcd.Declarations[2] as Scope;
            Assert.AreEqual(ScopeType.Module, decl.Type);
            Assert.AreEqual("fish", decl.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ParseScopeMismatchOpen1()
        {
            string vcdString = @"
$scope module lol $end
$enddefinitions $end";
            using VCD vcd = Parse.FromString(vcdString);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ParseScopeMismatchOpen2()
        {
            string vcdString = @"
$scope module lol $end
$scope module lol $end
$enddefinitions $end";
            using VCD vcd = Parse.FromString(vcdString);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ParseScopeMismatchOpen2Closed1()
        {
            string vcdString = @"
$scope module lol $end
$upscope $end
$scope module lol $end
$enddefinitions $end";
            using VCD vcd = Parse.FromString(vcdString);
        }

        [TestMethod]
        public void ParseVariable()
        {
            foreach (VarType varType in Enum.GetValues(typeof(VarType)))
            {
                IDeclCmd[] expectedHeader = new IDeclCmd[]
                {
                    new VarDef(varType, 6, "!", "_T_4", Array.Empty<Scope>())
                };

                string vcdString = @$"
$var {varType.ToString().ToLower()} 6 ! _T_4 $end
$enddefinitions $end";
                using VCD vcd = Parse.FromString(vcdString);

                TestTools.VerifyDeclarations(expectedHeader, vcd.Declarations);
            }
        }
    }
}
