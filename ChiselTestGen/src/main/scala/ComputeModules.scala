import chisel3._
import scala.collection.mutable.ListBuffer
import scala.util.Random

class ComputeIO[T <: Data](private val dtype: T, val const1: Int, val const2: Int) extends Bundle {
    val a = Input(dtype)
    val b = Input(dtype)
    val c = Input(Bool())
    val out = Output(dtype)
}

object ops {
    val scalarUIntOps = List[(String, ComputeIO[UInt]=>Unit)](
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

        ("shr", io => io.out := io.a >> io.const1),
        ("shl", io => io.out := io.a << io.const1),
        ("dshr", io => io.out := io.a >> (io.b & 31.U)),
        ("dshl", io => io.out := io.a << io.b),

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

        ("mux", io => io.out := Mux(io.c, io.a, io.b)),
    )

    val scalarSIntOps = List[(String, ComputeIO[SInt]=>Unit)](
        ("add", io => io.out := io.a + io.b),
        ("sub", io => io.out := io.a - io.b),
        ("mul", io => io.out := io.a * io.b),
        ("div", io => io.out := io.a / io.b),
        ("mod", io => io.out := io.a % io.b),

        ("lt", io => io.out := (io.a < io.b).asSInt()),
        ("leq", io => io.out := (io.a <= io.b).asSInt()),
        ("gt", io => io.out := (io.a > io.b).asSInt()),
        ("geq", io => io.out := (io.a >= io.b).asSInt()),
        ("eq", io => io.out := (io.a === io.b).asSInt()),
        ("neq", io => io.out := (io.a =/= io.b).asSInt()),

        ("asUInt", io => io.out := io.a.asUInt().asSInt()),
        ("asSInt", io => io.out := io.a.asSInt()),
        //("asClock", io => io.out := io.a =/= io.b), //Apparently asClock does not exist

        ("shr", io => io.out := io.a >> io.const1),
        ("shl", io => io.out := io.a << io.const1),
        ("dshr", io => io.out := io.a >> (io.b.asUInt() & 31.U)),
        ("dshl", io => io.out := io.a << io.b.asUInt()),

        //("cvt", io => io.out := io.a.), // what code to call this op?

        ("neg", io => io.out := -io.a),

        ("not", io => io.out := ~io.a),
        ("and", io => io.out := io.a & io.b),
        ("or", io => io.out := io.a | io.b),
        ("xor", io => io.out := io.a ^ io.b),

        ("cat", io => io.out := (io.a ## io.b).asSInt()),
        ("bits", io => io.out := io.a(3, 1).asSInt()),
        ("head", io => io.out := io.a.head(io.const1).asSInt()),
        ("tail", io => io.out := io.a.tail(io.const1).asSInt()),

        ("mux", io => io.out := Mux(io.c, io.a, io.b)),
    )
}

class ComputeSingle[T <: Data](op: (String, ComputeIO[T]=>Unit), modIO: ComputeIO[T]) extends Module {
    val io = IO(modIO)
    op._2(io);
}

class ComputeSeq(opCount: Int, modIO: ComputeIO[UInt], rng: Random) extends Module {
    val io = IO(modIO)
    
    val outputs = ListBuffer[UInt]()
    outputs += io.a
    outputs += io.b

    for (x <- 0 until opCount) {
        var wire = Wire(new ComputeIO[UInt](UInt(modIO.a.getWidth.W), modIO.const1, modIO.const2))
        wire.a := outputs(rng.nextInt(outputs.length))
        wire.b := outputs(rng.nextInt(outputs.length))
        wire.c := outputs(rng.nextInt(outputs.length))

        val index = rng.nextInt(2)
        if (index == 0) {
            wire.a := outputs(outputs.length - 1)
        }
        else {
            wire.b := outputs(outputs.length - 1)
        }

        ops.scalarUIntOps(rng.nextInt(ops.scalarUIntOps.length))._2(wire);
        outputs += wire.out
    }

    io.out := outputs(outputs.length - 1)
}