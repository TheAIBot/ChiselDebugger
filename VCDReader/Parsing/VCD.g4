//IEEE 1800-2017 - IEEE Standard for SystemVerilog--Unified Hardware Design, Specification, and Verification Language
//21.7.2.1 Syntax of 4-state VCD file

grammar VCD;

vcd 
	: declCmd*
	| simCmd*
	;

declCmd 
	: '$comment' asciiString '$end'
	| '$date' asciiString '$end'
	| '$enddefinitions' '$end'
	| '$scope' scopeType scopeId '$end'
	| '$timescale' timeNumber timeUnit '$end'
	| '$upscope' '$end'
	| '$var' varType size idCode ref '$end'
	| '$version' asciiString systemTask '$end'
	;

simCmd
	: '$dumpall' valueChange* '$end'
	| '$dumpoff' valueChange* '$end'
	| '$dumpon' valueChange* '$end'
	| '$dumpvars' valueChange* '$end'
	| '$comment' asciiString '$end'
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
	: '1'
	| '10'
	| '100'
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
	: '#' decimalNumber
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
	: 'b' binaryNumber ' ' idCode
	| 'B' binaryNumber ' ' idCode
	| 'r' realNumber ' ' idCode
	| 'R' realNumber ' ' idCode
	;

size
	: decimalNumber
	;

ref
	: id
	| id '[' decimalNumber ']'
	| id '[' decimalNumber ':' decimalNumber ']'
	;

systemTask
	: '$' asciiString
	;



idCode
	: id
	;

scopeId
	: id
	;

id
	: asciiChar+
	;

decimalNumber
	: ('+' | '-')? [1-9] [0-9]*
	;

binaryNumber
	: value+
	;

// Matches printf("%.16g")
realNumber
	: ('+'|'-')?([1-9][0-9]*|[0-9])('.'[0-9]+('e+'[1-9][0-9]*)?)?
	;

fragment
asciiChar : [!-~];

asciiString : (asciiChar | [ \t\r\n])*;