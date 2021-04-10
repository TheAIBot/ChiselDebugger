import chisel3._
import chiseltest._
import org.scalatest._
import chisel3.tester.experimental.TestOptionBuilder._
import firrtl.annotations.Annotation
import firrtl.ExecutionOptionsManager
import firrtl.AnnotationSeq

class WhenTester extends FlatSpec with ChiselScalatestTester with Matchers {
    it should "Test When1xVectorAccess" in {
        test(new When1xVectorAccess()).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
            {_=> {}}
    }
    it should "Test When2xVectorAccess" in {
        test(new When2xVectorAccess()).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
            {_=> {}}
    }
    it should "Test When3xVectorAccess" in {
        test(new When3xVectorAccess()).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
            {_=> {}}
    }
    it should "Test When1xMemAccess" in {
        test(new When1xMemAccess()).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
            {_=> {}}
    }
    it should "Test When2xMemAccess" in {
        test(new When2xMemAccess()).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
            {_=> {}}
    }
    it should "Test When3xMemAccess" in {
        test(new When3xMemAccess()).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
            {_=> {}}
    }
    it should "Test When1xDuplexInput" in {
        test(new When1xDuplexInput()).withFlags(Array("--tr-write-vcd", "--tr-vcd-show-underscored-vars", "--tr-save-firrtl-at-load"))
            {_=> {}}
    }
}
