import chisel3._
import chiseltest._
import chisel3.tester.experimental.TestOptionBuilder._
import firrtl.AnnotationSeq
import scala.util.Random
import chisel3.experimental._

object autoTester {
    def testWithRandomInputs(io: Bundle, clock: Clock, count: Int) {
        var inputs = Seq[Data]()

        io.elements.foreach(x => {
            val dir = DataMirror.directionOf(x._2)
            if (dir == Direction.Input) {
                inputs = inputs :+ x._2
            }
        });

        val rng = new Random(37)
        for (z <- 0 to count) {
            inputs.foreach(x => {
                x match {
                    case y: Bool => x.poke(rng.nextBoolean().B)
                    case y: UInt => x.poke(BigInt.apply(y.getWidth, rng).U)
                    case y: SInt => x.poke(BigInt.apply(y.getWidth, rng).S)
                }
            })
            clock.step()
        }
    }

    def testRandomWithTreadle(tester: ChiselScalatestTester, mod: Unit => ModuleWithIO, count: Int) {
        tester.test(mod()).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
        { dut=> { autoTester.testWithRandomInputs(dut.io, dut.clock, 200) } }
    }
}