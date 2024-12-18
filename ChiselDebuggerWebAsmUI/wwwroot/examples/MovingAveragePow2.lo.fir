circuit MovingAveragePow2 :
  module MovingAveragePow2 :
    input clock : Clock
    input reset : UInt<1>
    input io_din : UInt<4>
    input io_write : UInt<1>
    output io_avg : UInt<4>
  
    reg values_0 : UInt<4>, clock with :
      reset => (UInt<1>("h0"), values_0) @[MovingAveragePow2.scala 22:25]
    reg values_1 : UInt<4>, clock with :
      reset => (UInt<1>("h0"), values_1) @[MovingAveragePow2.scala 22:25]
    reg values_2 : UInt<4>, clock with :
      reset => (UInt<1>("h0"), values_2) @[MovingAveragePow2.scala 22:25]
    reg values_3 : UInt<4>, clock with :
      reset => (UInt<1>("h0"), values_3) @[MovingAveragePow2.scala 22:25]
    reg sum : UInt<6>, clock with :
      reset => (UInt<1>("h0"), sum) @[MovingAveragePow2.scala 25:22]
    node _T_1 = sub(io_din, values_3) @[MovingAveragePow2.scala 28:30]
    node _T_2 = tail(_T_1, 1) @[MovingAveragePow2.scala 28:30]
    node _GEN_5 = pad(_T_2, 6) @[MovingAveragePow2.scala 28:20]
    node _T_3 = add(sum, _GEN_5) @[MovingAveragePow2.scala 28:20]
    node _T_4 = tail(_T_3, 1) @[MovingAveragePow2.scala 28:20]
    node _GEN_0 = mux(io_write, _T_4, sum) @[MovingAveragePow2.scala 27:21]
    node _GEN_1 = mux(io_write, io_din, values_0) @[MovingAveragePow2.scala 27:21]
    node _GEN_2 = mux(io_write, values_0, values_1) @[MovingAveragePow2.scala 27:21]
    node _GEN_3 = mux(io_write, values_1, values_2) @[MovingAveragePow2.scala 27:21]
    node _GEN_4 = mux(io_write, values_2, values_3) @[MovingAveragePow2.scala 27:21]
    io_avg <= bits(sum, 5, 2) @[MovingAveragePow2.scala 36:12]
    values_0 <= mux(reset, UInt<4>("h0"), _GEN_1) @[MovingAveragePow2.scala 30:19]
    values_1 <= mux(reset, UInt<4>("h0"), _GEN_2) @[MovingAveragePow2.scala 32:27]
    values_2 <= mux(reset, UInt<4>("h0"), _GEN_3) @[MovingAveragePow2.scala 32:27]
    values_3 <= mux(reset, UInt<4>("h0"), _GEN_4) @[MovingAveragePow2.scala 32:27]
    sum <= mux(reset, UInt<6>("h0"), _GEN_0) @[MovingAveragePow2.scala 28:13]

