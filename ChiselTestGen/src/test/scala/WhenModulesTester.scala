import chisel3._
import chiseltest._
import org.scalatest._

class WhenTester extends FlatSpec with ChiselScalatestTester with Matchers {
    it should "Test When1xVectorAccess" in {
        autoTester.testRandomWithTreadle(this, _ => new When1xVectorAccess(), 200)
    }
    it should "Test When2xVectorAccess" in {
        autoTester.testRandomWithTreadle(this, _ => new When2xVectorAccess(), 200)
    }
    it should "Test When3xVectorAccess" in {
        autoTester.testRandomWithTreadle(this, _ => new When3xVectorAccess(), 200)
    }
    it should "Test When1xMemAccess" in {
        autoTester.testRandomWithTreadle(this, _ => new When1xMemAccess(), 200)
    }
    it should "Test When2xMemAccess" in {
        autoTester.testRandomWithTreadle(this, _ => new When2xMemAccess(), 200)
    }
    it should "Test When3xMemAccess" in {
        autoTester.testRandomWithTreadle(this, _ => new When3xMemAccess(), 200)
    }
    it should "Test When1xDuplexInput" in {
        autoTester.testRandomWithTreadle(this, _ => new When1xDuplexInput(), 200)
    }
    it should "Test WhenWireCondInput" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenWireCondInput(), 200)
    }
    it should "Test WhenConstCondInput" in {
        autoTester.testRandomWithTreadle(this, _ => new WhenConstCondInput(), 200)
    }
}
