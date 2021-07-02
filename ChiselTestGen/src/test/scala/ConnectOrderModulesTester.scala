import chisel3._
import chiseltest._
import org.scalatest._

class ConnectOrderTester extends FlatSpec with ChiselScalatestTester with Matchers {
    it should "Test WhenWireConnectOrderUncondFirst" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrderUncondFirst(), 2000, "test_con_order_dir/WhenWireConnectOrderUncondFirst")
    }
    it should "Test WhenWireConnectOrderUncondLast" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrderUncondLast(), 2000, "test_con_order_dir/WhenWireConnectOrderUncondLast")
    }
    it should "Test WhenWireConnectOrder1To1Mix1" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder1To1Mix1(), 2000, "test_con_order_dir/WhenWireConnectOrder1To1Mix1")
    }
    it should "Test WhenWireConnectOrder1To1Mix2" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder1To1Mix2(), 2000, "test_con_order_dir/WhenWireConnectOrder1To1Mix2")
    }
    it should "Test WhenWireConnectOrder1To1Mix3" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder1To1Mix3(), 2000, "test_con_order_dir/WhenWireConnectOrder1To1Mix3")
    }
    it should "Test WhenWireConnectOrder1To1Mix4" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder1To1Mix4(), 2000, "test_con_order_dir/WhenWireConnectOrder1To1Mix4")
    }
    it should "Test WhenWireConnectOrder1To1Mix5" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder1To1Mix5(), 2000, "test_con_order_dir/WhenWireConnectOrder1To1Mix5")
    }
    it should "Test WhenWireConnectOrder2To1Mix1" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder2To1Mix1(), 2000, "test_con_order_dir/WhenWireConnectOrder2To1Mix1")
    }
    it should "Test WhenWireConnectOrder2To1Mix2" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder2To1Mix2(), 2000, "test_con_order_dir/WhenWireConnectOrder2To1Mix2")
    }
    it should "Test WhenWireConnectOrder2To1Mix3" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder2To1Mix3(), 2000, "test_con_order_dir/WhenWireConnectOrder2To1Mix3")
    }
    it should "Test WhenWireConnectOrder2To1Mix4" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder2To1Mix4(), 2000, "test_con_order_dir/WhenWireConnectOrder2To1Mix4")
    }
    it should "Test WhenWireConnectOrder2To1Mix5" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder2To1Mix5(), 2000, "test_con_order_dir/WhenWireConnectOrder2To1Mix5")
    }
    it should "Test WhenWireConnectOrder2To1Mix6" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder2To1Mix6(), 2000, "test_con_order_dir/WhenWireConnectOrder2To1Mix6")
    }
    it should "Test WhenWireConnectOrder2To1Mix7" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectOrder2To1Mix7(), 2000, "test_con_order_dir/WhenWireConnectOrder2To1Mix7")
    }
    it should "Test WhenWireConnectMultiSameSource" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireConnectMultiSameSource(), 2000, "test_con_order_dir/WhenWireConnectMultiSameSource")
    }
}
