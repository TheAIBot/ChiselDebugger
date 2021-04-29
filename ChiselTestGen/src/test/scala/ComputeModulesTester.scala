import chisel3._
import chiseltest._
import org.scalatest._
import chisel3.tester.experimental.TestOptionBuilder._
import firrtl.annotations.Annotation
import firrtl.ExecutionOptionsManager
import firrtl.AnnotationSeq
import firrtl.options.TargetDirAnnotation

class ComputeTester extends FlatSpec with ChiselScalatestTester with Matchers {
    def testModuleUInt(io: ComputeIO[UInt], clock: Clock) {
        for (a <- 0 to 63) {
            for (b <- 0 to 63) {
                io.a.poke(a.U)
                io.b.poke(b.U)
                io.c.poke(true.B)
                clock.step()

                io.a.poke(a.U)
                io.b.poke(b.U)
                io.c.poke(false.B)
                clock.step()
            }
        }
    }
    def testModuleSInt(io: ComputeIO[SInt], clock: Clock) {
        for (a <- 0 to 63) {
            for (b <- 0 to 63) {
                io.a.poke(a.S)
                io.b.poke(b.S)
                io.c.poke(true.B)
                clock.step()

                io.a.poke(a.S)
                io.b.poke(b.S)
                io.c.poke(false.B)
                clock.step()
            }
        }
    }

    ops.scalarUIntOps.foreach(x => {
        it should "Test single uint " + x._1 in {
            val testDir = "test_comp_dir/UInt_" + x._1
            test(new ComputeSingle(x, new ComputeIO[UInt](UInt(6.W), 1, 3))).withAnnotations(Seq(TargetDirAnnotation(testDir))).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
                {dut=> {
                    testModuleUInt(dut.io, dut.clock)
                }
            }
        }
    })

    ops.scalarSIntOps.foreach(x => {
        it should "Test single sint " + x._1 in {
            val testDir = "test_comp_dir/SInt_" + x._1
            test(new ComputeSingle(x, new ComputeIO[SInt](SInt(6.W), 1, 3))).withAnnotations(Seq(TargetDirAnnotation(testDir))).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
                {dut=> {
                    testModuleSInt(dut.io, dut.clock)
                }
            }
        }
    })
}