using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace VCDReader.Parsing
{
    public static class Visitor
    {
        internal static VCD VisitVcd(VCDLexer lexer)
        {
            var declAndVarID = VisitDeclCmdStream(lexer);
            return new VCD(declAndVarID.declarations, declAndVarID.idToVariable, lexer);
        }

        internal static (List<IDeclCmd> declarations, IDToVarDef idToVariable) VisitDeclCmdStream(VCDLexer lexer)
        {
            List<IDeclCmd> declarations = new List<IDeclCmd>();
            IDToVarDef idToVariable = new IDToVarDef();
            Stack<Scope> scopes = new Stack<Scope>();

            while (!lexer.IsEmpty())
            {
                IDeclCmd? decl = VisitDeclCmd(lexer, idToVariable, scopes);
                if (decl == null)
                {
                    break;
                }
                declarations.Add(decl);
            }

            if (scopes.Count != 0)
            {
                throw new Exception("Not all declaration scopes were closed.");
            }

            return (declarations, idToVariable);
        }

        internal static IDeclCmd? VisitDeclCmd(VCDLexer lexer, IDToVarDef idToVariable, Stack<Scope> scopes)
        {
            ReadOnlySpan<char> declWord = lexer.NextWord();
            if (declWord.SequenceEqual("$comment"))
            {
                string text = lexer.NextUntil("$end").ToString();

                lexer.ExpectNextWord("$end");
                return new Comment(text);
            }
            else if (declWord.SequenceEqual("$date"))
            {
                string text = lexer.NextUntil("$end").ToString();

                lexer.ExpectNextWord("$end");
                return new Date(text);
            }
            else if (declWord.SequenceEqual("$scope"))
            {
                ScopeType type = VisitScopeType(lexer);
                string id = lexer.NextWord().ToString();

                Scope scope = new Scope(type, id);
                scopes.Push(scope);

                lexer.ExpectNextWord("$end");
                return scope;
            }
            else if (declWord.SequenceEqual("$timescale"))
            {
                int scale = VisitTimeNumber(lexer);
                TimeUnit unit = VisitTimeUnit(lexer);

                lexer.ExpectNextWord("$end");
                return new TimeScale(scale, unit);
            }
            else if (declWord.SequenceEqual("$upscope"))
            {
                scopes.Pop();

                lexer.ExpectNextWord("$end");
                return new UpScope();
            }
            else if (declWord.SequenceEqual("$var"))
            {
                VarType type = VisitVarType(lexer);
                int size = VisitSize(lexer);
                string id = lexer.NextWord().ToString();
                string reference = lexer.NextWord().ToString();

                VarDef variable = new VarDef(type, size, id, reference, scopes.Reverse().ToArray());
                idToVariable.AddVariable(variable);

                //Skip this stuff because it's not currently supported
                char nextChar = lexer.PeekNextChar();
                if (nextChar == '[')
                {
                    lexer.NextUntil("]");
                    lexer.SkipChar();
                }

                lexer.ExpectNextWord("$end");
                return variable;
            }
            else if (declWord.SequenceEqual("$version"))
            {
                string versionTxt = lexer.NextUntil("$").ToString();
                string systemTaskString = string.Empty;

                ReadOnlySpan<char> systemTask = lexer.PeekNextWord().Span;
                if (systemTask.StartsWith("$") && !systemTask.SequenceEqual("$end"))
                {
                    lexer.SkipWord(systemTask);
                    systemTaskString = systemTask.ToString();
                }

                lexer.ExpectNextWord("$end");
                return new Version(versionTxt, systemTaskString);
            }
            else if (declWord.SequenceEqual("$enddefinitions"))
            {
                lexer.ExpectNextWord("$end");
                return null;
            }
            else
            {
                throw new Exception($"Invalid declaration command: {declWord.ToString()}");
            }
        }

        internal static void VisitSimCmd(VCDLexer lexer, IDToVarDef idToVariable, SimPass pass, BitAllocator bitAlloc)
        {
            ReadOnlyMemory<char> declWordMem = lexer.NextWordAsMem();
            ReadOnlySpan<char> declWord = declWordMem.Span;

            //It may be the case that it's first discovered now that
            // the end of the file has been reached.
            if (declWord.Length == 0)
            {
                if (lexer.IsWordsRemaining())
                {
                    throw new Exception("Invalid simulation command.");
                }

                return;
            }

            if (declWord.SequenceEqual("$comment"))
            {
                string text = lexer.NextUntil("$end").ToString();

                lexer.ExpectNextWord("$end");
                pass.SimCmd = new Comment(text);
            }
            else if (declWord.SequenceEqual("$dumpall"))
            {
                pass.SimCmd = new DumpAll(VisitValueChangeStream(lexer, idToVariable, pass, bitAlloc));
            }
            else if (declWord.SequenceEqual("$dumpoff"))
            {
                pass.SimCmd = new DumpOff(VisitValueChangeStream(lexer, idToVariable, pass, bitAlloc));
            }
            else if (declWord.SequenceEqual("$dumpon"))
            {
                pass.SimCmd = new DumpOn(VisitValueChangeStream(lexer, idToVariable, pass, bitAlloc));
            }
            else if (declWord.SequenceEqual("$dumpvars"))
            {
                pass.SimCmd = new DumpVars(VisitValueChangeStream(lexer, idToVariable, pass, bitAlloc));
            }
            else if (declWord.StartsWith("#"))
            {
                pass.SimCmd = VisitSimTime(declWord);
            }
            else
            {
                VisitValueChange(lexer, declWordMem, idToVariable, pass, bitAlloc);
            }
        }

        internal static List<VarValue> VisitValueChangeStream(VCDLexer lexer, IDToVarDef idToVariable, SimPass pass, BitAllocator bitAlloc)
        {
            List<VarValue> changes = new List<VarValue>();

            while (!lexer.IsEmpty())
            {
                ReadOnlyMemory<char> text = lexer.NextWordAsMem();
                if (text.Span.SequenceEqual("$end"))
                {
                    break;
                }

                VisitValueChange(lexer, text, idToVariable, pass, bitAlloc);
                if (pass.BinValue.HasValue)
                {
                    changes.Add(pass.BinValue);
                }
                else if (pass.RealValue.HasValue)
                {
                    changes.Add(pass.RealValue);
                }
                else
                {
                    throw new Exception("Expected to read a value change but found none.");
                }
            }

            pass.Reset();
            return changes;
        }

        internal static void VisitValueChange(VCDLexer lexer, ReadOnlyMemory<char> text, IDToVarDef idToVariable, SimPass pass, BitAllocator bitAlloc)
        {
            if (text.Length < 2)
            {
                throw new Exception($"Invalid value change: {text.ToString()}");
            }

            ReadOnlySpan<char> textSpan = text.Span;
            if (textSpan[0] == 'b' || textSpan[0] == 'B')
            {
                VisitBinaryVectorValueChange(lexer, textSpan.Slice(1), idToVariable, pass, bitAlloc);
            }
            else if (textSpan[0] == 'r' || textSpan[0] == 'R')
            {
                VisitRealVectorValueChange(lexer, textSpan.Slice(1), idToVariable, pass, bitAlloc);
            }
            else
            {
                VisitScalarValueChange(text, idToVariable, pass, bitAlloc);
            }
        }

        internal static void VisitBinaryVectorValueChange(VCDLexer lexer, ReadOnlySpan<char> valueText, IDToVarDef idToVariable, SimPass pass, BitAllocator bitAlloc)
        {
            (UnsafeMemory<BitState> bits, bool isValidBinary) = ToBitStates(valueText, bitAlloc);
            var id = lexer.NextWordAsMem();

            if (idToVariable.TryGetValue(id, out List<VarDef>? variables))
            {
                pass.BinValue = new BinaryVarValue(bits, variables, isValidBinary);
            }
            else
            {
                throw new Exception($"Unknown variable identifier: {id}");
            }
        }

        internal static void VisitRealVectorValueChange(VCDLexer lexer, ReadOnlySpan<char> valueText, IDToVarDef idToVariable, SimPass pass, BitAllocator bitAlloc)
        {
            double value = double.Parse(valueText, NumberStyles.Float, CultureInfo.InvariantCulture);
            var id = lexer.NextWordAsMem();

            if (idToVariable.TryGetValue(id, out List<VarDef>? variable))
            {
                pass.RealValue = new RealVarValue(value, variable);
            }
            else
            {
                throw new Exception($"Unknown variable identifier: {id}");
            }
        }

        internal static void VisitScalarValueChange(ReadOnlyMemory<char> text, IDToVarDef idToVariable, SimPass pass, BitAllocator bitAlloc)
        {
            UnsafeMemory<BitState> bits = bitAlloc.GetBits(1);
            BitState bit = ToBitState(text.Span[0]);
            var id = text.Slice(1);

            bits.Span[0] = bit;

            if (idToVariable.TryGetValue(id, out List<VarDef>? variable))
            {
                pass.BinValue = new BinaryVarValue(bits, variable, ((int)bit & 0b10) == 0);
            }
            else
            {
                throw new Exception($"Unknown variable identifier: {id}");
            }
        }

        private static Vector128<byte> onlyFirstBit = Vector128.Create((byte)1);
        private static Vector128<byte> onlySecondBit = Vector128.Create((byte)2);
        private static Vector128<byte> shuffleIdxs = Vector128.Create(0x00_02_04_06_08_0A_0C_0E, 0x80_80_80_80_80_80_80_80).AsByte();

        internal static unsafe (UnsafeMemory<BitState> bits, bool isValidBinary) ToBitStates(ReadOnlySpan<char> valueText, BitAllocator bitAlloc)
        {
            UnsafeMemory<BitState> bitsMem = bitAlloc.GetBits(valueText.Length);
            Span<BitState> bits = bitsMem.Span;

            ulong isValidBinary = 0;
            int index = 0;
            if (bits.Length >= Vector128<ushort>.Count)
            {
                int vecBitCount = bits.Length / Vector128<ushort>.Count;
                fixed (BitState* bitsPtr = bits)
                {
                    fixed (char* textPtr = valueText)
                    {
                        ulong* bits4x = (ulong*)bitsPtr;

                        Vector128<ulong> isValidBin = Vector128<ulong>.Zero;
                        for (; index < vecBitCount; index++)
                        {
                            var charText = Avx.LoadVector128((byte*)textPtr + index * Vector128<byte>.Count);
                            var byteText = Avx.Shuffle(charText, shuffleIdxs);

                            var firstBit = Avx.And(onlyFirstBit, Avx.Or(byteText, Avx.ShiftRightLogical(byteText.AsInt32(), 1).AsByte()));
                            var secondBit = Avx.And(onlySecondBit, Avx.ShiftRightLogical(byteText.AsInt32(), 5).AsByte());
                            var bytesAsBitStates = Avx.Or(firstBit, secondBit);

                            Avx.StoreScalar((ulong*)(bitsPtr + bits.Length) - index - 1, bytesAsBitStates.AsUInt64());
                            isValidBin = Avx.Or(isValidBin, secondBit.AsUInt64());
                        }

                        isValidBinary = isValidBin.ToScalar();
                    }
                }

                index *= Vector128<ushort>.Count;
            }

            for (; index < bits.Length; index++)
            {
                BitState bit = ToBitState(valueText[index]);
                bits[bits.Length - index - 1] = bit;
                isValidBinary |= (uint)bit & 0b10;
            }

            return (bitsMem, isValidBinary == 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static BitState ToBitState(int value)
        {
            int firstBit = (value | (value >> 1)) & 0b1;
            int secondBit = (value >> 5) & 0b10;
            return (BitState)(secondBit | firstBit);
        }

        internal static ScopeType VisitScopeType(VCDLexer lexer)
        {
            string text = lexer.NextWord().ToString();
            if (Enum.TryParse(text, true, out ScopeType result))
            {
                return result;
            }
            else
            {
                throw new Exception($"Invalid scope type: {text}");
            }
        }

        internal static SimTime VisitSimTime(ReadOnlySpan<char> text)
        {
            if (text.Length < 2 || text[0] != '#')
            {
                throw new Exception($"Invalid simulation time: {text.ToString()}");
            }
            return new SimTime(ParseULong(text.Slice(1)));
        }

        private static ulong ParseULong(ReadOnlySpan<char> text)
        {
            if (ulong.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out ulong result))
            {
                return result;
            }
            else
            {
                throw new Exception($"Failed to parse unsigned long: {text.ToString()}");
            }
        }

        private static int ParseInt(ReadOnlySpan<char> text)
        {
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
            {
                return result;
            }
            else
            {
                throw new Exception($"Failed to parse integer: {text.ToString()}");
            }
        }

        internal static int VisitSize(VCDLexer lexer)
        {
            return ParseInt(lexer.NextWord());
        }

        internal static int VisitTimeNumber(VCDLexer lexer)
        {
            ReadOnlySpan<char> text = lexer.NextInteger();
            if (text.SequenceEqual("1"))
            {
                return 1;
            }
            else if (text.SequenceEqual("10"))
            {
                return 10;
            }
            else if (text.SequenceEqual("100"))
            {
                return 100;
            }
            else
            {
                throw new Exception($"Invalid time scale: {text.ToString()}");
            }
        }

        internal static TimeUnit VisitTimeUnit(VCDLexer lexer)
        {
            string text = lexer.NextWord().ToString();
            if (Enum.TryParse(text, true, out TimeUnit result))
            {
                return result;
            }
            else
            {
                throw new Exception($"Invalid time unit: {text}");
            }
        }

        internal static VarType VisitVarType(VCDLexer lexer)
        {
            string text = lexer.NextWord().ToString();
            if (Enum.TryParse(text, true, out VarType result))
            {
                return result;
            }
            else
            {
                throw new Exception($"Invalid variable type: {text}");
            }
        }
    }
}
