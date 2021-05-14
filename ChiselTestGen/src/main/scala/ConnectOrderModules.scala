import chisel3._

class WhenWireConnectOrderUncondFirst extends ModuleWithIO {
    val io = IO(new Bundle{
        val en1 = Input(Bool())
        val din = Input(UInt(8.W))
        val dout1 = Output(UInt(8.W))
    })

    var wire = Wire(UInt(8.W))
    wire := 0.U
    when(io.en1) { wire := io.din }

    io.dout1 := wire
}

class WhenWireConnectOrderUncondLast extends ModuleWithIO {
    val io = IO(new Bundle{
        val en1 = Input(Bool())
        val din = Input(UInt(8.W))
        val dout1 = Output(UInt(8.W))
    })

    var wire = Wire(UInt(8.W))
    wire := 0.U
    when(io.en1) { wire := io.din }
    wire := 1.U

    io.dout1 := wire
}

class WhenWireConnectOrder1To1Mix1 extends ModuleWithIO {
    val io = IO(new Bundle{
        val en1 = Input(Bool())
        val en2 = Input(Bool())
        val en3 = Input(Bool())
        val en4 = Input(Bool())
        val en5 = Input(Bool())
        val din1 = Input(UInt(8.W))
        val din2 = Input(UInt(8.W))
        val din3 = Input(UInt(8.W))
        val din4 = Input(UInt(8.W))
        val dout1 = Output(UInt(8.W))
    })

    var wire1 = Wire(UInt(8.W))
    wire1 := 0.U
    when(io.en1) { wire1 := io.din1 }
    when(io.en2) { wire1 := io.din2 }

    var wire2 = Wire(UInt(8.W))
    wire2 := 1.U
    when(io.en3) { wire2 := wire1 }
    when(io.en4) { wire2 := io.din3 }
    when(io.en5) { wire2 := io.din4 }

    io.dout1 := wire2
}

class WhenWireConnectOrder1To1Mix2 extends ModuleWithIO {
    val io = IO(new Bundle{
        val en1 = Input(Bool())
        val en2 = Input(Bool())
        val en3 = Input(Bool())
        val en4 = Input(Bool())
        val en5 = Input(Bool())
        val din1 = Input(UInt(8.W))
        val din2 = Input(UInt(8.W))
        val din3 = Input(UInt(8.W))
        val din4 = Input(UInt(8.W))
        val dout1 = Output(UInt(8.W))
    })

    var wire1 = Wire(UInt(8.W))
    wire1 := 0.U
    when(io.en1) { wire1 := io.din1 }
    when(io.en2) { wire1 := io.din2 }

    var wire2 = Wire(UInt(8.W))
    wire2 := 1.U
    when(io.en3) { wire2 := io.din3 }
    when(io.en4) { wire2 := wire1 }
    when(io.en5) { wire2 := io.din4 }

    io.dout1 := wire2
}

class WhenWireConnectOrder1To1Mix3 extends ModuleWithIO {
    val io = IO(new Bundle{
        val en1 = Input(Bool())
        val en2 = Input(Bool())
        val en3 = Input(Bool())
        val en4 = Input(Bool())
        val en5 = Input(Bool())
        val din1 = Input(UInt(8.W))
        val din2 = Input(UInt(8.W))
        val din3 = Input(UInt(8.W))
        val din4 = Input(UInt(8.W))
        val dout1 = Output(UInt(8.W))
    })

    var wire1 = Wire(UInt(8.W))
    wire1 := 0.U
    when(io.en1) { wire1 := io.din1 }
    when(io.en2) { wire1 := io.din2 }

    var wire2 = Wire(UInt(8.W))
    wire2 := 1.U
    when(io.en3) { wire2 := io.din3 }
    when(io.en5) { wire2 := io.din4 }
    when(io.en4) { wire2 := wire1 }

    io.dout1 := wire2
}

class WhenWireConnectOrder1To1Mix4 extends ModuleWithIO {
    val io = IO(new Bundle{
        val en1 = Input(Bool())
        val en2 = Input(Bool())
        val en3 = Input(Bool())
        val en4 = Input(Bool())
        val en5 = Input(Bool())
        val din1 = Input(UInt(8.W))
        val din2 = Input(UInt(8.W))
        val din3 = Input(UInt(8.W))
        val din4 = Input(UInt(8.W))
        val dout1 = Output(UInt(8.W))
    })

    var wire1 = Wire(UInt(8.W))
    wire1 := 0.U
    when(io.en1) { wire1 := io.din1 }
    when(io.en2) { wire1 := io.din2 }

    var wire2 = Wire(UInt(8.W))
    when(io.en3) { wire2 := io.din3 }
    wire2 := 1.U
    when(io.en5) { wire2 := io.din4 }
    when(io.en4) { wire2 := wire1 }

    io.dout1 := wire2
}

class WhenWireConnectOrder1To1Mix5 extends ModuleWithIO {
    val io = IO(new Bundle{
        val en1 = Input(Bool())
        val en2 = Input(Bool())
        val en3 = Input(Bool())
        val en4 = Input(Bool())
        val en5 = Input(Bool())
        val din1 = Input(UInt(8.W))
        val din2 = Input(UInt(8.W))
        val din3 = Input(UInt(8.W))
        val din4 = Input(UInt(8.W))
        val dout1 = Output(UInt(8.W))
    })

    var wire1 = Wire(UInt(8.W))
    wire1 := 0.U
    when(io.en1) { wire1 := io.din1 }
    when(io.en2) { wire1 := io.din2 }

    var wire2 = Wire(UInt(8.W))
    when(io.en3) { wire2 := io.din3 }
    when(io.en5) { wire2 := io.din4 }
    when(io.en4) { wire2 := wire1 }
    wire2 := 1.U

    io.dout1 := wire2
}

class WhenWireConnectOrder2To1Mix1 extends ModuleWithIO {
    val io = IO(new Bundle{
        val en1 = Input(Bool())
        val en2 = Input(Bool())
        val en3 = Input(Bool())
        val en4 = Input(Bool())
        val en5 = Input(Bool())
        val en6 = Input(Bool())
        val en7 = Input(Bool())
        val din1 = Input(UInt(8.W))
        val din2 = Input(UInt(8.W))
        val din3 = Input(UInt(8.W))
        val din4 = Input(UInt(8.W))
        val din5 = Input(UInt(8.W))
        val dout1 = Output(UInt(8.W))
    })

    var wire1 = Wire(UInt(8.W))
    wire1 := 0.U
    when(io.en1) { wire1 := io.din1 }
    when(io.en2) { wire1 := io.din2 }

    var wire2 = Wire(UInt(8.W))
    wire2 := 1.U
    when(io.en3) { wire2 := io.din3 }
    when(io.en4) { wire2 := io.din4 }

    var wire3 = Wire(UInt(8.W))
    wire3 := 2.U
    when(io.en5) { wire3 := wire1 }
    when(io.en6) { wire3 := wire2 }
    when(io.en7) { wire3 := io.din5 }

    io.dout1 := wire3
}

class WhenWireConnectOrder2To1Mix2 extends ModuleWithIO {
    val io = IO(new Bundle{
        val en1 = Input(Bool())
        val en2 = Input(Bool())
        val en3 = Input(Bool())
        val en4 = Input(Bool())
        val en5 = Input(Bool())
        val en6 = Input(Bool())
        val en7 = Input(Bool())
        val din1 = Input(UInt(8.W))
        val din2 = Input(UInt(8.W))
        val din3 = Input(UInt(8.W))
        val din4 = Input(UInt(8.W))
        val din5 = Input(UInt(8.W))
        val dout1 = Output(UInt(8.W))
    })

    var wire1 = Wire(UInt(8.W))
    when(io.en1) { wire1 := io.din1 }
    when(io.en2) { wire1 := io.din2 }
    wire1 := 0.U

    var wire2 = Wire(UInt(8.W))
    wire2 := 1.U
    when(io.en3) { wire2 := io.din3 }
    when(io.en4) { wire2 := io.din4 }

    var wire3 = Wire(UInt(8.W))
    wire3 := 2.U
    when(io.en5) { wire3 := wire1 }
    when(io.en6) { wire3 := wire2 }
    when(io.en7) { wire3 := io.din5 }

    io.dout1 := wire3
}

class WhenWireConnectOrder2To1Mix3 extends ModuleWithIO {
    val io = IO(new Bundle{
        val en1 = Input(Bool())
        val en2 = Input(Bool())
        val en3 = Input(Bool())
        val en4 = Input(Bool())
        val en5 = Input(Bool())
        val en6 = Input(Bool())
        val en7 = Input(Bool())
        val din1 = Input(UInt(8.W))
        val din2 = Input(UInt(8.W))
        val din3 = Input(UInt(8.W))
        val din4 = Input(UInt(8.W))
        val din5 = Input(UInt(8.W))
        val dout1 = Output(UInt(8.W))
    })

    var wire1 = Wire(UInt(8.W))
    wire1 := 0.U
    when(io.en1) { wire1 := io.din1 }
    when(io.en2) { wire1 := io.din2 }

    var wire2 = Wire(UInt(8.W))
    wire2 := 1.U
    when(io.en3) { wire2 := io.din3 }
    when(io.en4) { wire2 := io.din4 }

    var wire3 = Wire(UInt(8.W))
    wire3 := 2.U
    when(io.en5) { wire3 := wire1 }
    when(io.en7) { wire3 := io.din5 }
    when(io.en6) { wire3 := wire2 }

    io.dout1 := wire3
}

class WhenWireConnectOrder2To1Mix4 extends ModuleWithIO {
    val io = IO(new Bundle{
        val en1 = Input(Bool())
        val en2 = Input(Bool())
        val en3 = Input(Bool())
        val en4 = Input(Bool())
        val en5 = Input(Bool())
        val en6 = Input(Bool())
        val en7 = Input(Bool())
        val din1 = Input(UInt(8.W))
        val din2 = Input(UInt(8.W))
        val din3 = Input(UInt(8.W))
        val din4 = Input(UInt(8.W))
        val din5 = Input(UInt(8.W))
        val dout1 = Output(UInt(8.W))
    })

    var wire1 = Wire(UInt(8.W))
    wire1 := 0.U
    when(io.en1) { wire1 := io.din1 }
    when(io.en2) { wire1 := io.din2 }

    var wire2 = Wire(UInt(8.W))
    wire2 := 1.U
    when(io.en3) { wire2 := io.din3 }
    when(io.en4) { wire2 := io.din4 }

    var wire3 = Wire(UInt(8.W))
    when(io.en5) { wire3 := wire1 }
    wire3 := 2.U
    when(io.en7) { wire3 := io.din5 }
    when(io.en6) { wire3 := wire2 }

    io.dout1 := wire3
}

class WhenWireConnectOrder2To1Mix5 extends ModuleWithIO {
    val io = IO(new Bundle{
        val en1 = Input(Bool())
        val en2 = Input(Bool())
        val en3 = Input(Bool())
        val en4 = Input(Bool())
        val en5 = Input(Bool())
        val en6 = Input(Bool())
        val en7 = Input(Bool())
        val din1 = Input(UInt(8.W))
        val din2 = Input(UInt(8.W))
        val din3 = Input(UInt(8.W))
        val din4 = Input(UInt(8.W))
        val din5 = Input(UInt(8.W))
        val dout1 = Output(UInt(8.W))
    })

    var wire1 = Wire(UInt(8.W))
    wire1 := 0.U
    when(io.en1) { wire1 := io.din1 }
    when(io.en2) { wire1 := io.din2 }

    var wire2 = Wire(UInt(8.W))
    wire2 := 1.U
    when(io.en3) { wire2 := io.din3 }
    when(io.en4) { wire2 := io.din4 }

    var wire3 = Wire(UInt(8.W))
    when(io.en5) { wire3 := wire1 }
    when(io.en7) { wire3 := io.din5 }
    when(io.en6) { wire3 := wire2 }
    wire3 := 2.U

    io.dout1 := wire3
}

class WhenWireConnectOrder2To1Mix6 extends ModuleWithIO {
    val io = IO(new Bundle{
        val en1 = Input(Bool())
        val en2 = Input(Bool())
        val en3 = Input(Bool())
        val en4 = Input(Bool())
        val en5 = Input(Bool())
        val en6 = Input(Bool())
        val en7 = Input(Bool())
        val din1 = Input(UInt(8.W))
        val din2 = Input(UInt(8.W))
        val din3 = Input(UInt(8.W))
        val din4 = Input(UInt(8.W))
        val din5 = Input(UInt(8.W))
        val dout1 = Output(UInt(8.W))
    })

    var wire1 = Wire(UInt(8.W))
    when(io.en1) { wire1 := io.din1 }
    when(io.en2) { wire1 := io.din2 }
    wire1 := 0.U

    var wire2 = Wire(UInt(8.W))
    wire2 := 1.U
    when(io.en3) { wire2 := io.din3 }
    when(io.en4) { wire2 := io.din4 }

    var wire3 = Wire(UInt(8.W))
    when(io.en5) { wire3 := wire1 }
    when(io.en7) { wire3 := io.din5 }
    when(io.en6) { wire3 := wire2 }
    wire3 := 2.U

    io.dout1 := wire3
}

class WhenWireConnectOrder2To1Mix7 extends ModuleWithIO {
    val io = IO(new Bundle{
        val en1 = Input(Bool())
        val en2 = Input(Bool())
        val en3 = Input(Bool())
        val en4 = Input(Bool())
        val en5 = Input(Bool())
        val en6 = Input(Bool())
        val en7 = Input(Bool())
        val din1 = Input(UInt(8.W))
        val din2 = Input(UInt(8.W))
        val din3 = Input(UInt(8.W))
        val din4 = Input(UInt(8.W))
        val din5 = Input(UInt(8.W))
        val dout1 = Output(UInt(8.W))
    })

    var wire1 = Wire(UInt(8.W))
    when(io.en1) { wire1 := io.din1 }
    when(io.en2) { wire1 := io.din2 }
    wire1 := 0.U

    var wire2 = Wire(UInt(8.W))
    when(io.en3) { wire2 := io.din3 }
    when(io.en4) { wire2 := io.din4 }
    wire2 := 1.U

    var wire3 = Wire(UInt(8.W))
    when(io.en5) { wire3 := wire1 }
    when(io.en7) { wire3 := io.din5 }
    when(io.en6) { wire3 := wire2 }
    wire3 := 2.U

    io.dout1 := wire3
}