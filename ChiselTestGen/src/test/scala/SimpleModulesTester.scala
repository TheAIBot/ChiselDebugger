import chisel3._
import chiseltest._
import org.scalatest._
import chisel3.tester.experimental.TestOptionBuilder._
import firrtl.annotations.Annotation
import firrtl.ExecutionOptionsManager
import firrtl.AnnotationSeq

class RandomTester extends FlatSpec with ChiselScalatestTester with Matchers {
    it should "SimpleIO" in {
        autoTester.testRandomWithTreadle(this, _ => new SimpleIO(), 2000)
    }
    it should "Nested1Module" in {
        autoTester.testRandomWithTreadle(this, _ => new Nested1Module(), 2000)
    }
    it should "Nested1Module2x" in {
        autoTester.testRandomWithTreadle(this, _ => new Nested1Module2x(), 2000)
    }
    it should "MuxOnBundles" in {
        autoTester.testRandomWithTreadle(this, _ => new MuxOnBundles(), 2000)
    }
    it should "SimpleVector" in {
        autoTester.testRandomWithTreadle(this, _ => new SimpleVector(), 2000)
    }
    it should "MuxOnBundlesWithVector" in {
        autoTester.testRandomWithTreadle(this, _ => new MuxOnBundlesWithVector(), 2000)
    }
    it should "SimpleBundleWithVector" in {
        autoTester.testRandomWithTreadle(this, _ => new SimpleBundleWithVector(), 2000)
    }
    it should "Nested1ModuleBundleWithVector" in {
        autoTester.testRandomWithTreadle(this, _ => new Nested1ModuleBundleWithVector(), 2000)
    }
    it should "InOutWireVecBundle" in {
        autoTester.testRandomWithTreadle(this, _ => new InOutWireVecBundle(), 2000)
    }
    it should "WireConnectInBeforeOut" in {
        autoTester.testRandomWithTreadle(this, _ => new WireConnectInBeforeOut(), 2000)
    }
    it should "WireConnectOutBeforeIn" in {
        autoTester.testRandomWithTreadle(this, _ => new WireConnectOutBeforeIn(), 2000)
    }
    it should "WireConnectConditionalOrder" in {
        autoTester.testRandomWithTreadle(this, _ => new WireConnectConditionalOrder(), 2000)
    }
    it should "RegConnectVecBundleVec" in {
        autoTester.testRandomWithTreadle(this, _ => new RegConnectVecBundleVec(), 2000)
    }
    it should "RegConnectBundleVec" in {
        autoTester.testRandomWithTreadle(this, _ => new RegConnectBundleVec(), 2000)
    }
    it should "ModOutputAsInput" in {
        autoTester.testRandomWithTreadle(this, _ => new ModOutputAsInput(), 2000)
    }
}
