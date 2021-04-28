import chisel3._

class ComputeIO[T <: Data](private val dtype: T, val const1: Int, val const2: Int) extends Bundle {
    val a = Input(dtype)
    val b = Input(dtype)
    val c = Input(Bool())
    val out = Output(dtype)
}

object ops {
    val scalarOps = List[(String, ComputeIO[UInt]=>Unit)](
        ("add", io => io.out := io.a + io.b),
        ("sub", io => io.out := io.a - io.b),
        ("mul", io => io.out := io.a * io.b),
        ("div", io => io.out := io.a / io.b),
        ("mod", io => io.out := io.a % io.b),

        ("lt", io => io.out := io.a < io.b),
        ("leq", io => io.out := io.a <= io.b),
        ("gt", io => io.out := io.a > io.b),
        ("geq", io => io.out := io.a >= io.b),
        ("eq", io => io.out := io.a === io.b),
        ("neq", io => io.out := io.a =/= io.b),

        ("asUInt", io => io.out := io.a.asUInt()),
        ("asSInt", io => io.out := io.a.asSInt().asUInt()),
        //("asClock", io => io.out := io.a =/= io.b), //Apparently asClock does not exist

        ("shl", io => io.out := io.a >> io.const1),
        ("shr", io => io.out := io.a << io.const1),
        ("dshl", io => io.out := io.a >> io.b),
        ("dshr", io => io.out := io.a << io.b),

        //("cvt", io => io.out := io.a.), // what code to call this op?

        ("neg", io => io.out := -io.a),

        ("not", io => io.out := ~io.a),
        ("and", io => io.out := io.a & io.b),
        ("or", io => io.out := io.a | io.b),
        ("xor", io => io.out := io.a ^ io.b),

        ("andr", io => io.out := io.a.andR()),
        ("orr", io => io.out := io.a.orR()),
        ("xorr", io => io.out := io.a.xorR()),

        ("cat", io => io.out := io.a ## io.b),
        ("bits", io => io.out := io.a(3, 1)),
        ("head", io => io.out := io.a.head(io.const1)),
        ("tail", io => io.out := io.a.tail(io.const1)),
    )
}

class ComputeSingle[T <: Data](op: (String, ComputeIO[T]=>Unit), modIO: ComputeIO[T]) extends Module {
    val io = IO(modIO)
    op._2(io);
}