import chisel3._

class When1xVectorAccess extends Module {
    val io = IO(new Bundle{
        val read = Input(Bool())
        val write = Input(Bool())
        val index1 = Input(UInt(2.W))
        val din = Input(UInt(8.W))
        val dout = Output(UInt(8.W))
    })

    val reg = Reg(Vec(4, UInt(8.W)))
    io.dout := 0.U

    when(io.read) {
        io.dout := reg(io.index1)
    }

    when(io.write) {
        reg(io.index1) := io.din
    }
}

class When2xVectorAccess extends Module {
    val io = IO(new Bundle{
        val read1 = Input(Bool())
        val read2 = Input(Bool())
        val write1 = Input(Bool())
        val write2 = Input(Bool())
        val index1 = Input(UInt(2.W))
        val din = Input(UInt(8.W))
        val dout = Output(UInt(8.W))
    })

    val reg = Reg(Vec(4, UInt(8.W)))
    io.dout := 0.U

    when(io.read1) {
        when(io.read2) {
            io.dout := reg(io.index1)
        }
    }

    when(io.write1) {
        when(io.write2) {
            reg(io.index1) := io.din
        }
    }
}

class When3xVectorAccess extends Module {
    val io = IO(new Bundle{
        val read1 = Input(Bool())
        val read2 = Input(Bool())
        val read3 = Input(Bool())
        val write1 = Input(Bool())
        val write2 = Input(Bool())
        val write3 = Input(Bool())
        val index1 = Input(UInt(2.W))
        val din = Input(UInt(8.W))
        val dout = Output(UInt(8.W))
    })

    val reg = Reg(Vec(4, UInt(8.W)))
    io.dout := 0.U

    when(io.read1) {
        when(io.read2) {
            when(io.read3) {
                io.dout := reg(io.index1)
            }
        }
    }

    when(io.write1) {
        when(io.write2) {
            when(io.write3) {
                reg(io.index1) := io.din
            }
        }
    }
}

class When1xMemAccess extends Module {
    val io = IO(new Bundle{
        val read = Input(Bool())
        val write = Input(Bool())
        val index1 = Input(UInt(2.W))
        val din = Input(UInt(8.W))
        val dout = Output(UInt(8.W))
    })

    val mem = Mem(4, UInt(8.W))
    io.dout := 0.U

    when(io.read) {
        io.dout := mem(io.index1)
    }

    when(io.write) {
        mem(io.index1) := io.din
    }
}

class When2xMemAccess extends Module {
    val io = IO(new Bundle{
        val read1 = Input(Bool())
        val read2 = Input(Bool())
        val write1 = Input(Bool())
        val write2 = Input(Bool())
        val index1 = Input(UInt(2.W))
        val din = Input(UInt(8.W))
        val dout = Output(UInt(8.W))
    })

    val mem = Mem(4, UInt(8.W))
    io.dout := 0.U

    when(io.read1) {
        when(io.read2) {
            io.dout := mem(io.index1)
        }
    }

    when(io.write1) {
        when(io.write2) {
            mem(io.index1) := io.din
        }
    }
}

class When3xMemAccess extends Module {
    val io = IO(new Bundle{
        val read1 = Input(Bool())
        val read2 = Input(Bool())
        val read3 = Input(Bool())
        val write1 = Input(Bool())
        val write2 = Input(Bool())
        val write3 = Input(Bool())
        val index1 = Input(UInt(2.W))
        val din = Input(UInt(8.W))
        val dout = Output(UInt(8.W))
    })

    val mem = Mem(4, UInt(8.W))
    io.dout := 0.U

    when(io.read1) {
        when(io.read2) {
            when(io.read3) {
                io.dout := mem(io.index1)
            }
        }
    }

    when(io.write1) {
        when(io.write2) {
            when(io.write3) {
                mem(io.index1) := io.din
            }
        }
    }
}