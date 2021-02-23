﻿//IEEE 1800-2017 - IEEE Standard for SystemVerilog--Unified Hardware Design, Specification, and Verification Language
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
	| '$scope' scopeType AsciiString '$end'
	| '$timescale' timeNumber timeUnit '$end'
	| '$upscope' '$end'
	| '$var' varType AsciiString AsciiString AsciiString '$end'
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
	: '100'
	| '10'
	| '1'
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

valueChangeStream
	: valueChange valueChangeStream?
	;

valueChange
	: AsciiString
	| AsciiString AsciiString
	;

systemTask
	: AsciiString
	;

WS
	: [ \r\t\n] -> skip
	;

AsciiString : AsciiChar+;

//[!-~] except $ 
fragment
AsciiChar : [!-~];