﻿using System;
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
            var (declarations, idToVariable) = VisitDeclCmdStream(lexer);

            return new VCD(declarations, idToVariable.GetIdToVarDef(), lexer);
        }

        internal static (List<IDeclCmd> declarations, IDCollection idCollection) VisitDeclCmdStream(VCDLexer lexer)
        {
            List<IDeclCmd> declarations = new List<IDeclCmd>();
            IDCollection idCollection = new IDCollection();
            Stack<Scope> scopes = new Stack<Scope>();

            while (!lexer.IsEmpty())
            {
                IDeclCmd? decl = VisitDeclCmd(lexer, idCollection, scopes);
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

            return (declarations, idCollection);
        }

        internal static IDeclCmd? VisitDeclCmd(VCDLexer lexer, IDCollection idCollection, Stack<Scope> scopes)
        {
            ReadOnlySpan<byte> declWord = lexer.NextWord();

            ReadOnlySpan<byte> endToken = "$end"u8; ;
            ReadOnlySpan<byte> dollarSign = "$"u8; ;

            if (declWord.SequenceEqual("$comment"u8))
            {
                string text = lexer.NextUntil(endToken).ToCharString();

                lexer.ExpectNextWord(endToken);
                return new Comment(text);
            }
            else if (declWord.SequenceEqual("$date"u8))
            {
                string text = lexer.NextUntil(endToken).ToCharString();

                lexer.ExpectNextWord(endToken);
                return new Date(text);
            }
            else if (declWord.SequenceEqual("$scope"u8))
            {
                ScopeType type = VisitScopeType(lexer);
                string id = lexer.NextWord().ToCharString();

                Scope scope = new Scope(type, id);
                scopes.Push(scope);

                lexer.ExpectNextWord(endToken);
                return scope;
            }
            else if (declWord.SequenceEqual("$timescale"u8))
            {
                int scale = VisitTimeNumber(lexer);
                TimeUnit unit = VisitTimeUnit(lexer);

                lexer.ExpectNextWord(endToken);
                return new TimeScale(scale, unit);
            }
            else if (declWord.SequenceEqual("$upscope"u8))
            {
                scopes.Pop();

                lexer.ExpectNextWord(endToken);
                return new UpScope();
            }
            else if (declWord.SequenceEqual("$var"u8))
            {
                VarType type = VisitVarType(lexer);
                int size = VisitSize(lexer);
                string id = lexer.NextWord().ToCharString();
                string reference = lexer.NextWord().ToCharString();

                VarDef variable = new VarDef(type, size, id, reference, scopes.Reverse().ToArray());
                idCollection.AddVariable(variable);

                //Skip this stuff because it's not currently supported
                char nextChar = lexer.PeekNextChar();
                if (nextChar == '[')
                {
                    ReadOnlySpan<byte> endBracket = new byte[] { (byte)']' };
                    lexer.NextUntil(endBracket);
                    lexer.SkipChar();
                }

                lexer.ExpectNextWord(endToken);
                return variable;
            }
            else if (declWord.SequenceEqual("$version"u8))
            {
                string versionTxt = lexer.NextUntil(dollarSign).ToCharString();
                string systemTaskString = string.Empty;

                ReadOnlySpan<byte> systemTask = lexer.PeekNextWord();
                if (systemTask.StartsWith(dollarSign) && !systemTask.SequenceEqual(endToken))
                {
                    lexer.SkipWord(systemTask);
                    systemTaskString = systemTask.ToCharString();
                }

                lexer.ExpectNextWord(endToken);
                return new Version(versionTxt, systemTaskString);
            }
            else if (declWord.SequenceEqual("$enddefinitions"u8))
            {
                lexer.ExpectNextWord(endToken);
                return null;
            }
            else
            {
                throw new Exception($"Invalid declaration command: {declWord.ToCharString()}\nBuffer: {lexer.BufferToString()}");
            }
        }

        internal static void VisitSimCmd(VCDLexer lexer, IDToVarDef idToVariable, SimPass pass, BitAllocator bitAlloc)
        {
            ReadOnlySpan<byte> declWord = lexer.NextWord();

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
            char firstChar = (char)declWord[0];
            char firstCharLowered = (char)(firstChar | 0b10_0000);
            if (firstCharLowered == 'b')
            {
                VisitBinaryVectorValueChange(lexer, declWord.Slice(1), idToVariable, pass, bitAlloc);
            }
            else if (firstCharLowered == 'r')
            {
                VisitRealVectorValueChange(lexer, declWord.Slice(1), idToVariable, pass, bitAlloc);
            }
            else if (firstChar != '#' && firstChar != '$')
            {
                VisitScalarValueChange(declWord, idToVariable, pass, bitAlloc);
            }
            else if (firstChar == '#')
            {
                pass.SimCmd = VisitSimTime(declWord);
            }
            else if (declWord.SequenceEqual("$comment"u8))
            {
                ReadOnlySpan<byte> endToken = "$end"u8;
                string text = lexer.NextUntil(endToken).ToCharString();

                lexer.ExpectNextWord(endToken);
                pass.SimCmd = new Comment(text);
            }
            else if (declWord.SequenceEqual("$dumpall"u8))
            {
                pass.SimCmd = new DumpAll(VisitValueChangeStream(lexer, idToVariable, pass, bitAlloc));
            }
            else if (declWord.SequenceEqual("$dumpoff"u8))
            {
                pass.SimCmd = new DumpOff(VisitValueChangeStream(lexer, idToVariable, pass, bitAlloc));
            }
            else if (declWord.SequenceEqual("$dumpon"u8))
            {
                pass.SimCmd = new DumpOn(VisitValueChangeStream(lexer, idToVariable, pass, bitAlloc));
            }
            else if (declWord.SequenceEqual("$dumpvars"u8))
            {
                pass.SimCmd = new DumpVars(VisitValueChangeStream(lexer, idToVariable, pass, bitAlloc));
            }

            else
            {
                VisitValueChange(lexer, declWord, idToVariable, pass, bitAlloc);
            }
        }

        internal static List<VarValue> VisitValueChangeStream(VCDLexer lexer, IDToVarDef idToVariable, SimPass pass, BitAllocator bitAlloc)
        {
            List<VarValue> changes = new List<VarValue>();

            while (!lexer.IsEmpty())
            {
                ReadOnlySpan<byte> text = lexer.NextWord();
                if (text.SequenceEqual("$end"u8))
                {
                    break;
                }

                VisitValueChange(lexer, text, idToVariable, pass, bitAlloc);
                if (pass.HasBinValue)
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

        internal static void VisitValueChange(VCDLexer lexer, ReadOnlySpan<byte> text, IDToVarDef idToVariable, SimPass pass, BitAllocator bitAlloc)
        {
            if (text.Length < 2)
            {
                throw new Exception($"Invalid value change: {text.ToCharString()}");
            }

            char firstCharLowered = (char)(text[0] | 0b10_0000);
            if (firstCharLowered == 'b')
            {
                VisitBinaryVectorValueChange(lexer, text.Slice(1), idToVariable, pass, bitAlloc);
            }
            else if (firstCharLowered == 'r')
            {
                VisitRealVectorValueChange(lexer, text.Slice(1), idToVariable, pass, bitAlloc);
            }
            else
            {
                VisitScalarValueChange(text, idToVariable, pass, bitAlloc);
            }
        }

        internal static void VisitBinaryVectorValueChange(VCDLexer lexer, ReadOnlySpan<byte> valueText, IDToVarDef idToVariable, SimPass pass, BitAllocator bitAlloc)
        {
            Memory<BitState> bits = bitAlloc.GetBits(valueText.Length);
            bool isValidBinary = ToBitStates(valueText, in bits);
            var id = lexer.NextWord();

            if (idToVariable.TryGetValue(id, out List<VarDef>? variables))
            {
                pass.BinValue = new BinaryVarValue(bits, variables, isValidBinary);
                pass.HasBinValue = true;
            }
            else
            {
                throw new Exception($"Unknown variable identifier: {id.ToCharString()}");
            }
        }

        internal static void VisitRealVectorValueChange(VCDLexer lexer, ReadOnlySpan<byte> valueText, IDToVarDef idToVariable, SimPass pass, BitAllocator bitAlloc)
        {
            double value = double.Parse(valueText, NumberStyles.Float, CultureInfo.InvariantCulture);
            var id = lexer.NextWord();

            if (idToVariable.TryGetValue(id, out List<VarDef>? variable))
            {
                pass.RealValue = new RealVarValue(value, variable);
            }
            else
            {
                throw new Exception($"Unknown variable identifier: {id.ToCharString()}");
            }
        }

        internal static void VisitScalarValueChange(ReadOnlySpan<byte> text, IDToVarDef idToVariable, SimPass pass, BitAllocator bitAlloc)
        {
            Memory<BitState> bits = bitAlloc.GetBits(1);
            BitState bit = ToBitState(text[0]);
            var id = text.Slice(1);

            bits.Span[0] = bit;

            if (idToVariable.TryGetValue(id, out List<VarDef>? variable))
            {
                pass.BinValue = new BinaryVarValue(bits, variable, ((int)bit & 0b10) == 0);
                pass.HasBinValue = true;
            }
            else
            {
                throw new Exception($"Unknown variable identifier: {id.ToCharString()}");
            }
        }

        internal static unsafe bool ToBitStates(ReadOnlySpan<byte> valueText, in Memory<BitState> bitsMem)
        {
            Span<BitState> bits = bitsMem.Span;

            var shuffleIdxs = Vector128.Create(15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0).AsByte();
            var onlyFirstBit = Vector128.Create((byte)1);
            var onlySecondBit = Vector128.Create((byte)2);

            ulong isValidBinary = 0;
            int index = 0;
            if (Ssse3.IsSupported && bits.Length >= Vector128<byte>.Count)
            {
                int vecBitCount = bits.Length - Vector128<byte>.Count;
                fixed (BitState* bitsPtr = bits)
                {
                    fixed (byte* textPtr = valueText)
                    {
                        byte* bitsStorePtr = (byte*)bitsPtr + bits.Length - Vector128<byte>.Count;
                        Vector128<ulong> isValidBin = Vector128<ulong>.Zero;
                        for (; index <= vecBitCount; index += Vector128<byte>.Count)
                        {
                            var charText = Avx.LoadVector128(textPtr + index);
                            var byteText = Avx.Shuffle(charText, shuffleIdxs);

                            var firstBit = Avx.And(onlyFirstBit, Avx.Or(byteText, Avx.ShiftRightLogical(byteText.AsInt32(), 1).AsByte()));
                            var secondBit = Avx.And(onlySecondBit, Avx.ShiftRightLogical(byteText.AsInt32(), 5).AsByte());
                            var bytesAsBitStates = Avx.Or(firstBit, secondBit);

                            Avx.Store(bitsStorePtr, bytesAsBitStates);
                            bitsStorePtr -= Vector128<byte>.Count;
                            isValidBin = Avx.Or(isValidBin, secondBit.AsUInt64());
                        }

                        isValidBinary = isValidBin.GetElement(0) | isValidBin.GetElement(1);
                    }
                }
            }

            for (; index < bits.Length; index++)
            {
                BitState bit = ToBitState(valueText[index]);
                bits[bits.Length - index - 1] = bit;
                isValidBinary |= (uint)bit & 0b10;
            }

            return isValidBinary == 0;
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
            string text = lexer.NextWord().ToCharString();
            if (Enum.TryParse(text, true, out ScopeType result))
            {
                return result;
            }
            else
            {
                throw new Exception($"Invalid scope type: {text}");
            }
        }

        internal static SimTime VisitSimTime(ReadOnlySpan<byte> text)
        {
            if (text.Length < 2 || text[0] != '#')
            {
                throw new Exception($"Invalid simulation time: {text.ToCharString()}");
            }
            return new SimTime(ParseULong(text.Slice(1)));
        }

        private static ulong ParseULong(ReadOnlySpan<byte> text)
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

        private static int ParseInt(ReadOnlySpan<byte> text)
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
            ReadOnlySpan<byte> text = lexer.NextInteger();

            if (text.SequenceEqual("1"u8))
            {
                return 1;
            }
            else if (text.SequenceEqual("10"u8))
            {
                return 10;
            }
            else if (text.SequenceEqual("100"u8))
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
            string text = lexer.NextWord().ToCharString();
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
            string text = lexer.NextWord().ToCharString();
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
