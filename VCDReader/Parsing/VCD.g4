//IEEE 1800-2017 - IEEE Standard for SystemVerilog--Unified Hardware Design, Specification, and Verification Language
//21.7.2.1 Syntax of 4-state VCD file

grammar VCD;

vcd 
	: declCmdStream simCmdStream? EOF
	;

declCmdStream
	: declCmd declCmdStream
	| '$enddefinitions' '$end'
	;

declCmd 
	: '$comment' AsciiString '$end'
	| '$date' AsciiString '$end'
	| '$scope' scopeType scopeId '$end'
	| '$timescale' timeNumber timeUnit '$end'
	| '$upscope' '$end'
	| '$var' varType size idCode ref '$end'
	| '$version' AsciiString systemTask '$end'
	;

simCmdStream
	: simCmd simCmdStream?
	;

simCmd
	: '$dumpall' valueChangeStream '$end'
	| '$dumpoff' valueChangeStream '$end'
	| '$dumpon' valueChangeStream '$end'
	| '$dumpvars' valueChangeStream '$end'
	| '$comment' AsciiString '$end'
	| simTime
	| valueChange
	;

scopeType
	: 'begin'
	| 'fork'
	| 'function'
	| 'module'
	| 'task'
	;

timeNumber
	: DecimalNumber
	;

timeUnit
	: 's'
	| 'ms'
	| 'us'
	| 'ns'
	| 'ps'
	| 'fs'
	;

varType
	: 'event'
	| 'integer'
	| 'parameter'
	| 'real'
	| 'realtime'
	| 'reg'
	| 'supply0'
	| 'supply1'
	| 'time'
	| 'tri'
	| 'triand'
	| 'trior'
	| 'trireg'
	| 'tri0'
	| 'tri1'
	| 'wand'
	| 'wire'
	| 'wor'
	;

simTime
	: '#' DecimalNumber
	;

valueChangeStream
	: valueChange valueChangeStream?
	;

valueChange
	: scalarValueChange
	| vectorValueChange
	;

scalarValueChange
	: value idCode
	;

value
	: '0'
	| '1'
	| 'x'
	| 'X'
	| 'z'
	| 'Z'
	;

vectorValueChange
	: 'b' binaryNumber idCode
	| 'B' binaryNumber idCode
	| 'r' RealNumber idCode
	| 'R' RealNumber idCode
	;

size
	: DecimalNumber
	;

//Can't use normal Id here because it would also match the [
ref
	: RefId
	| RefId '[' DecimalNumber ']'
	| RefId '[' DecimalNumber ':' DecimalNumber ']'
	;

systemTask
	: '$' AsciiString
	;



idCode
	: ID
	;

scopeId
	: ID
	;

ID
	: (AsciiChar|'$')+
	;

binaryNumber
	: value+
	;

DecimalNumber
	: ('+'|'-')?([1-9][0-9]*|[0-9])
	;

//Matches printf("%.16g")
RealNumber
	: ('+'|'-')?([1-9][0-9]*|[0-9])('.'[0-9]+('e+'[1-9][0-9]*)?)?
	;

WS
	: [ \r\t\n] -> skip
	;


RefId
	: RefChar+
	;

AsciiString : AsciiChar+;

fragment
AsciiChar : (RefChar|'[');

//[!-~] except $ and [
fragment
RefChar
	: ('!'..'#'|'%'..'Z'|'\\'..'~')
	;