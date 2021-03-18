import chisel3._
import chiseltest._
import org.scalatest._
import chisel3.tester.experimental.TestOptionBuilder._
import firrtl.annotations.Annotation
import firrtl.ExecutionOptionsManager
import firrtl.AnnotationSeq

class RandomTester extends FlatSpec with ChiselScalatestTester with Matchers {
    it should "Test A" in {
        test(new ModA()).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
            {_=> {}}
    }
    it should "Test B" in {
        test(new ModB()).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
            {_=> {}}
    }
    it should "Test C" in {
        test(new ModC()).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
            {_=> {}}
    }
    it should "Test D" in {
        test(new ModD()).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
            {_=> {}}
    }
    it should "Test E" in {
        test(new ModE()).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
            {_=> {}}
    }
    it should "Test F" in {
        test(new ModF()).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
            {_=> {}}
    }
    it should "Test G" in {
        test(new ModG()).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
            {_=> {}}
    }
    it should "Test H" in {
        test(new ModH()).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
            {_=> {}}
    }
    it should "Test I" in {
        test(new ModI()).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
            {_=> {}}
    }
}