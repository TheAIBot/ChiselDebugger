import chisel3._
import chiseltest._
import chisel3.tester.experimental.TestOptionBuilder._
import chisel3.experimental._
import firrtl.AnnotationSeq
import firrtl.options.TargetDirAnnotation
import scala.util.Random

object autoTester {
    def testWithRandomInputs(io: Bundle, clock: Clock, count: Int) {
        var inputs = Seq[Data]()

        def flattenInputs(elems: Iterable[Data]) {
            elems.foreach(x => x match {
                case a: Aggregate => flattenInputs(a.getElements)
                case g: Data => {
                    val dir = DataMirror.directionOf(g)
                    if (dir == Direction.Input) {
                        inputs = inputs :+ g
                    }
                }
            })
        }

        flattenInputs(io.elements.values)

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
        testRandomWithTreadle(tester, mod, count, None)
    }

    def testRandomWithTreadle(tester: ChiselScalatestTester, mod: Unit => ModuleWithIO, count: Int, testDir: String) {
        testRandomWithTreadle(tester, mod, count, Some(testDir))
    }

    private def testRandomWithTreadle(tester: ChiselScalatestTester, mod: Unit => ModuleWithIO, count: Int, testDir: Option[String]) {
        var test : ChiselScalatestTester#TestBuilder[ModuleWithIO] = tester.test(mod())
        if (testDir.isDefined) {
            test = test.withAnnotations(Seq(TargetDirAnnotation(testDir.get)))
        }
        
        test.withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
        { dut=> { autoTester.testWithRandomInputs(dut.io, dut.clock, count) } }
    }
}