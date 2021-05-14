import chisel3._
import chiseltest._
import org.scalatest._

class ConnectOrderTester extends FlatSpec with ChiselScalatestTester with Matchers {
    it should "Test WhenWireConnectOrderUncondFirst" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrderUncondFirst(), 2000)
    }
    it should "Test WhenWireConnectOrderUncondLast" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrderUncondLast(), 2000)
    }
    it should "Test WhenWireConnectOrder1To1Mix1" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder1To1Mix1(), 2000)
    }
    it should "Test WhenWireConnectOrder1To1Mix2" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder1To1Mix2(), 2000)
    }
    it should "Test WhenWireConnectOrder1To1Mix3" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder1To1Mix3(), 2000)
    }
    it should "Test WhenWireConnectOrder1To1Mix4" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder1To1Mix4(), 2000)
    }
    it should "Test WhenWireConnectOrder1To1Mix5" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder1To1Mix5(), 2000)
    }
    it should "Test WhenWireConnectOrder2To1Mix1" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder2To1Mix1(), 2000)
    }
    it should "Test WhenWireConnectOrder2To1Mix2" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder2To1Mix2(), 2000)
    }
    it should "Test WhenWireConnectOrder2To1Mix3" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder2To1Mix3(), 2000)
    }
    it should "Test WhenWireConnectOrder2To1Mix4" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder2To1Mix4(), 2000)
    }
    it should "Test WhenWireConnectOrder2To1Mix5" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder2To1Mix5(), 2000)
    }
    it should "Test WhenWireConnectOrder2To1Mix6" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder2To1Mix6(), 2000)
    }
    it should "Test WhenWireConnectOrder2To1Mix7" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder2To1Mix7(), 2000)
    }
}
