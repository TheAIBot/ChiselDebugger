import chisel3._

class ModA extends Module {
    val io = IO(new Bundle{
        val din = Input(UInt(8.W))
        val dout = Output(UInt(8.W))
    })

    io.dout := io.din
}

class ModB extends Module {
    val io = IO(new Bundle{
        val din = Input(Bits(8.W))
        val dout = Output(Bits(8.W))
    })

    val a = Module(new ModA())
    a.io.din := io.din
    io.dout := a.io.dout
}

class ModC extends Module {
    val io = IO(new Bundle{
        val din = Input(Bits(8.W))
        val dout = Output(Bits(8.W))
    })

    val a1 = Module(new ModA())
    val a2 = Module(new ModA())
    a1.io.din := io.din
    a2.io.din := a1.io.dout
    io.dout := a2.io.dout
}

class BunA extends Bundle {
    val a1 = Input(UInt(8.W))
    val a2 = Input(SInt(4.W))
}

class ModD extends Module {
    val io = IO(new Bundle{
        val a = Input(new BunA())
        val b = Input(new BunA())
        val cond = Input(Bool())
        val c = Output(new BunA())
    })

    io.c := Mux(io.cond, io.a, io.b)
}

class ModE extends Module {
    val io = IO(new Bundle{
        val din = Input(Vec(5, UInt(8.W)))
        val dout = Output(Vec(5, UInt(8.W)))
    })

    io.dout := io.din
}

class BunB extends Bundle {
    val a1 = Input(Vec(5, UInt(8.W)))
    val a2 = Input(Vec(7, UInt(3.W)))
}

class ModF extends Module {
    val io = IO(new Bundle{
        val a = Input(new BunB())
        val b = Input(new BunB())
        val cond = Input(Bool())
        val c = Output(new BunB())
    })

    io.c := Mux(io.cond, io.a, io.b)
}

class BunC extends Bundle {
    val a1 = Vec(5, UInt(8.W))
    val a2 = Vec(7, UInt(3.W))
}

class ModG extends Module {
    val io = IO(new Bundle{
        val din = Input(new BunC())
        val dout = Output(new BunC())
    })

    io.dout <> io.din
}

class ModH extends Module {
    val io = IO(new Bundle{
        val din = Input(new BunC())
        val dout = Output(new BunC())
    })

    val a = Module(new ModG())

    io <> a.io
}

class BunD extends Bundle {
    val a1 = Input(Vec(5, UInt(8.W)))
    val a2 = Output(Vec(7, UInt(3.W)))
}

class ModI extends Module {
    val io = IO(new Bundle {
        val a = new BunD()
        val b = Flipped(new BunD())
    })

    val vecA = Wire(Vec(1, new BunD()))
    vecA(0) <> io.a
    io.b <> vecA(0)
}

class ModJ extends Module {
    val io = IO(new Bundle{
        val din1 = Input(UInt(8.W))
        val din2 = Input(UInt(8.W))
        val din3 = Input(UInt(8.W))
        val din4 = Input(UInt(8.W))
        val din5 = Input(UInt(8.W))
        val dout = Output(UInt(8.W))
    })

    val wir = Wire(UInt(8.W))
    wir := io.din1
    wir := io.din2
    wir := io.din3
    wir := io.din4
    wir := io.din5

    io.dout := wir
}

class ModK extends Module {
    val io = IO(new Bundle{
        val din1 = Input(UInt(8.W))
        val din2 = Input(UInt(8.W))
        val din3 = Input(UInt(8.W))
        val din4 = Input(UInt(8.W))
        val din5 = Input(UInt(8.W))
        val dout = Output(UInt(8.W))
    })

    val wir = Wire(UInt(8.W))
    io.dout := wir

    wir := io.din1
    wir := io.din2
    wir := io.din3
    wir := io.din4
    wir := io.din5
}

class ModL extends Module {
    val io = IO(new Bundle{
        val en1 = Input(Bool())
        val en2 = Input(Bool())
        val din1 = Input(UInt(8.W))
        val dout1 = Output(UInt(8.W))
        val en3 = Input(Bool())
        val en4 = Input(Bool())
        val din2 = Input(UInt(8.W))
        val dout2 = Output(UInt(8.W))
    })

    val wir = Wire(UInt(8.W))
    wir := 0.U

    when(io.en1) {
        wir := io.din1
    }

    when(io.en2) {
        io.dout1 := wir
    }.otherwise {
        io.dout1 := 3.U
    }

    when(io.en3) {
        wir := io.din2
    }

    when(io.en4) {
        io.dout2 := wir
    }.otherwise {
        io.dout2 := 7.U
    }
}