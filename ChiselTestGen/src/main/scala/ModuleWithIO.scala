import chisel3._

abstract class ModuleWithIO extends Module {
    def io: Bundle
}