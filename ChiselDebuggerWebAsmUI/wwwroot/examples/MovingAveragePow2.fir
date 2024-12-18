;buildInfoPackage: chisel3, version: 3.3.2, scalaVersion: 2.12.11, sbtVersion: 1.3.10
circuit MovingAveragePow2 : 
  module MovingAveragePow2 : 
    input clock : Clock
    input reset : UInt<1>
    output io : {flip din : UInt<4>, flip write : UInt<1>, avg : UInt<4>}
    
    wire _T : UInt<4>[4] @[MovingAveragePow2.scala 22:33]
    _T[0] <= UInt<4>("h00") @[MovingAveragePow2.scala 22:33]
    _T[1] <= UInt<4>("h00") @[MovingAveragePow2.scala 22:33]
    _T[2] <= UInt<4>("h00") @[MovingAveragePow2.scala 22:33]
    _T[3] <= UInt<4>("h00") @[MovingAveragePow2.scala 22:33]
    reg values : UInt<4>[4], clock with : (reset => (reset, _T)) @[MovingAveragePow2.scala 22:25]
    reg sum : UInt<6>, clock with : (reset => (reset, UInt<6>("h00"))) @[MovingAveragePow2.scala 25:22]
    when io.write : @[MovingAveragePow2.scala 27:21]
      node _T_1 = sub(io.din, values[3]) @[MovingAveragePow2.scala 28:30]
      node _T_2 = tail(_T_1, 1) @[MovingAveragePow2.scala 28:30]
      node _T_3 = add(sum, _T_2) @[MovingAveragePow2.scala 28:20]
      node _T_4 = tail(_T_3, 1) @[MovingAveragePow2.scala 28:20]
      sum <= _T_4 @[MovingAveragePow2.scala 28:13]
      values[0] <= io.din @[MovingAveragePow2.scala 30:19]
      values[1] <= values[0] @[MovingAveragePow2.scala 32:27]
      values[2] <= values[1] @[MovingAveragePow2.scala 32:27]
      values[3] <= values[2] @[MovingAveragePow2.scala 32:27]
      skip @[MovingAveragePow2.scala 27:21]
    node _T_5 = bits(sum, 5, 2) @[MovingAveragePow2.scala 36:18]
    io.avg <= _T_5 @[MovingAveragePow2.scala 36:12]
    
