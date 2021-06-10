import chisel3._
import chiseltest._
import org.scalatest._
import chisel3.tester.experimental.TestOptionBuilder._
import firrtl.annotations.Annotation
import firrtl.ExecutionOptionsManager
import firrtl.AnnotationSeq
import firrtl.options.TargetDirAnnotation
import scala.util.Random
import org.scalatest.{FunSuite, ParallelTestExecution}
import org.scalatest.concurrent.Eventually

class BenchTester extends FlatSpec with ChiselScalatestTester with Matchers {
    def testModuleUInt(io: ComputeIO[UInt], clock: Clock) {
        for (a <- 0 to 512) {
            for (b <- 0 to 512) {
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

    it should "Benchmark long sequential module" in {
        val rng = new Random(37)
        test(new ComputeSeq(1000, new ComputeIO[UInt](UInt(10.W), UInt(10.W), UInt(6.W), 1, 3), rng)).withFlags(Array("--tr-save-firrtl-at-load"))
            {dut=> {
                val startTime = System.nanoTime()
                testModuleUInt(dut.io, dut.clock)
                val endTime = System.nanoTime()
                print(endTime - startTime)
            }
        }
    }
}

class ComputeTester extends FlatSpec with ChiselScalatestTester with Matchers with Eventually with ParallelTestExecution {
    val widths = List(1, 2, 5, 6)
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
        widths.foreach(aWidth => {
            widths.foreach(bWidth => {
                val testName = "UInt" + aWidth + "_UInt" + bWidth + "_" + x._1
                val testDir = "test_comp_dir/" + testName
                if (!(x._1 == "bits" && (aWidth < 4 || bWidth < 4)))
                {                
                    it should "Test single " + testName in {
                        test(new ComputeSingle(x, new ComputeIO[UInt](UInt(aWidth.W), UInt(bWidth.W), UInt(6.W), 1, 3))).withAnnotations(Seq(TargetDirAnnotation(testDir))).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
                            {dut=> {
                                testModuleUInt(dut.io, dut.clock)
                            }
                        }
                    }
                }
            })
        })
    })

    ops.scalarSIntOps.foreach(x => {
        widths.foreach(aWidth => {
            widths.foreach(bWidth => {
                val testName = "SInt" + aWidth + "_SInt" + bWidth + "_" + x._1
                val testDir = "test_comp_dir/" + testName
                if (!(x._1 == "bits" && (aWidth < 4 || bWidth < 4)))
                {                
                    it should "Test single " + testName in {
                        test(new ComputeSingle(x, new ComputeIO[SInt](SInt(aWidth.W), SInt(bWidth.W), SInt(6.W), 1, 3))).withAnnotations(Seq(TargetDirAnnotation(testDir))).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
                            {dut=> {
                                testModuleSInt(dut.io, dut.clock)
                            }
                        }
                    }
                }
            })
        })
    })
    val opsCounts = List(5, 10, 20, 50)

    val rng = new Random(37)
    for (opCountIdx <- 0 until opsCounts.length) {
        val opCount = opsCounts(opCountIdx)
        for (i <- 0 until 50) {
            it should "Test multi uint ops: " + opCount + " test: " + i in {
                val testDir = "test_multi_comp_dir/UInt/ops_" + opCount + "/test_" + i
                test(new ComputeSeq(opCount, new ComputeIO[UInt](UInt(6.W), UInt(6.W), UInt(6.W), 1, 3), rng)).withAnnotations(Seq(TargetDirAnnotation(testDir))).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
                    {dut=> {
                        testModuleUInt(dut.io, dut.clock)
                    }
                }
            }
        }
    }
}