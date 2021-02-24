using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace VCDReader.Parsing
{
    internal class VCDLexer
    {
        private readonly TextReader Reader;
        private char[] Buffer; 
        private bool ReachedEOF;
        private Memory<char> AvailableChars;
        private const int DefaultBufferSize = 1024;
        private const string SkipChars = " \t\r\n";

        public VCDLexer(TextReader reader)
        {
            this.Reader = reader;
            this.Buffer = new char[DefaultBufferSize];
            this.ReachedEOF = false;
            this.AvailableChars = new Memory<char>();

            FillBuffer(true);
        }

        public ReadOnlySpan<char> NextInteger()
        {
            SkipStart();

            int wordLength = 0;
            while (!IsEmpty() && wordLength < AvailableChars.Length)
            {
                if (!char.IsDigit(AvailableChars.Span[wordLength]))
                {
                    break;
                }

                wordLength++;
                if (AvailableChars.Length == wordLength && !ReachedEOF)
                {
                    FillBuffer(false);
                }
            }

            ReadOnlySpan<char> numbers = AvailableChars.Span.Slice(0, wordLength);
            AvailableChars = AvailableChars.Slice(numbers.Length);
            return numbers;
        }

        public ReadOnlySpan<char> PeekNextWord()
        {
            SkipStart();

            int wordLength = 0;
            while (!IsEmpty() && wordLength < AvailableChars.Length)
            {
                if (!IsWordChar(AvailableChars.Span[wordLength]))
                {
                    break;
                }

                wordLength++;
                if (AvailableChars.Length == wordLength && !ReachedEOF)
                {
                    FillBuffer(false);
                }
            }

            return AvailableChars.Span.Slice(0, wordLength);
        }

        public void SkipWord(ReadOnlySpan<char> word)
        {
            AvailableChars = AvailableChars.Slice(word.Length);
        }

        public ReadOnlySpan<char> NextWord()
        {
            ReadOnlySpan<char> word = PeekNextWord();
            SkipWord(word);

            return word;
        }

        public ReadOnlySpan<char> NextUntil(ReadOnlySpan<char> stopAt)
        {
            SkipStart();

            const int notFound = -1;
            int foundIndex;
            do
            {
                foundIndex = AvailableChars.Span.IndexOf(stopAt);
                if (foundIndex == notFound)
                {
                    if (ReachedEOF)
                    {
                        throw new Exception($"Failed to find pattern before EOF. Pattern: {new string(stopAt)}");
                    }
                    FillBuffer(false);
                }
            } while (foundIndex == notFound);

            ReadOnlySpan<char> text = AvailableChars.Span.Slice(0, foundIndex);
            AvailableChars = AvailableChars.Slice(foundIndex);

            return text.TrimEnd(SkipChars);
        }

        public void ExpectNextWord(ReadOnlySpan<char> expectedWord)
        {
            ReadOnlySpan<char> actualword = NextWord();
            if (!expectedWord.SequenceEqual(actualword))
            {
                throw new Exception($"Expected next word to be {expectedWord.ToString()} but got {actualword.ToString()}");
            }
        }

        private bool IsWordChar(char value)
        {
            return value >= '!' && value <= '~';
        }

        private void SkipStart()
        {
            ReadOnlySpan<char> ToSkip = SkipChars;
            while (true)
            {
                AvailableChars = AvailableChars.TrimStart(ToSkip);
                if (AvailableChars.Length == 0 && !ReachedEOF)
                {
                    FillBuffer(true);
                }
                else
                {
                    break;
                }
            }
        }

        private void FillBuffer(bool overwriteExisting)
        {
            if (overwriteExisting)
            {
                int copiedLength = Reader.ReadBlock(Buffer, 0, Buffer.Length);
                ReachedEOF = copiedLength < Buffer.Length;
                AvailableChars = new Memory<char>(Buffer, 0, copiedLength);
            }
            else
            {
                //If there is still available chars equal to the
                //size of the buffer then that means the buffer
                //isn't large enough to contain all the chars
                //required for some operation.
                if (AvailableChars.Length == Buffer.Length)
                {
                    char[] biggerBuffer = new char[Buffer.Length * 2];
                    AvailableChars.CopyTo(biggerBuffer);
                    Buffer = biggerBuffer;
                }
                else
                {
                    //Move remaining text to start of buffer so more
                    //text can be added to the end of it
                    Memory<char> from = AvailableChars;
                    Memory<char> to = new Memory<char>(Buffer, 0, AvailableChars.Length);
                    from.CopyTo(to);
                    AvailableChars = to;
                }

                int spaceLeft = Buffer.Length - AvailableChars.Length;
                int copiedLength = Reader.ReadBlock(Buffer, AvailableChars.Length, spaceLeft);
                ReachedEOF = copiedLength < spaceLeft;
                AvailableChars = new Memory<char>(Buffer, 0, AvailableChars.Length + copiedLength);
            }
        }

        internal bool IsEmpty()
        {
            return ReachedEOF && (AvailableChars.Length == 0);
        }
    }

    public static class Visitor
    {
        internal static VCD VisitVcd(VCDLexer lexer)
        {
            var declAndVarID = VisitDeclCmdStream(lexer);
            return new VCD(declAndVarID.declarations, declAndVarID.idToVariable, lexer);
        }

        internal static (List<IDeclCmd> declarations, Dictionary<string, VarDef> idToVariable) VisitDeclCmdStream(VCDLexer lexer)
        {
            List<IDeclCmd> declarations = new List<IDeclCmd>();
            Dictionary<string, VarDef> idToVariable = new Dictionary<string, VarDef>();
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

        internal static IDeclCmd? VisitDeclCmd(VCDLexer lexer, Dictionary<string, VarDef> idToVariable, Stack<Scope> scopes)
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

                VarDef variable = new VarDef(type, size, id, reference, scopes.ToArray());
                idToVariable.Add(id, variable);

                lexer.ExpectNextWord("$end");
                return variable;
            }
            else if (declWord.SequenceEqual("$version"))
            {
                string versionTxt = lexer.NextWord().ToString();
                string systemTaskString = string.Empty;

                ReadOnlySpan<char> systemTask = lexer.PeekNextWord();
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

        internal static ISimCmd VisitSimCmd(VCDLexer lexer, Dictionary<string, VarDef> idToVariable)
        {
            ReadOnlySpan<char> declWord = lexer.NextWord();
            if (declWord.SequenceEqual("$comment"))
            {
                string text = lexer.NextUntil("$end").ToString();

                lexer.ExpectNextWord("$end");
                return new Comment(text);
            }
            else if (declWord.SequenceEqual("$dumpall"))
            {
                return new DumpAll(VisitValueChangeStream(lexer, idToVariable));
            }
            else if (declWord.SequenceEqual("$dumpoff"))
            {
                lexer.ExpectNextWord("$end");
                return new DumpOff();
            }
            else if (declWord.SequenceEqual("$dumpon"))
            {
                lexer.ExpectNextWord("$end");
                return new DumpOn();
            }
            else if (declWord.SequenceEqual("$dumpvars"))
            {
                return new DumpVars(VisitValueChangeStream(lexer, idToVariable));
            }
            else if (declWord.StartsWith("#"))
            {
                return  VisitSimTime(declWord);
            }
            else
            {
                return VisitValueChange(lexer, declWord, idToVariable);
            }
        }

        internal static List<IValueChange> VisitValueChangeStream(VCDLexer lexer, Dictionary<string, VarDef> idToVariable)
        {
            List<IValueChange> changes = new List<IValueChange>();

            while (!lexer.IsEmpty())
            {
                ReadOnlySpan<char> text = lexer.NextWord();
                if (text.SequenceEqual("$end"))
                {
                    break;
                }
                changes.Add(VisitValueChange(lexer, text, idToVariable));
            }

            return changes;
        }

        internal static IValueChange VisitValueChange(VCDLexer lexer, ReadOnlySpan<char> text, Dictionary<string, VarDef> idToVariable)
        {
            if (text.Length < 2)
            {
                throw new Exception($"Invalid value change: {text.ToString()}");
            }

            if (text[0] == 'b' || text[0] == 'B')
            {
                return VisitBinaryVectorValueChange(lexer, text.Slice(1), idToVariable);
            }
            else if (text[0] == 'r' || text[0] == 'R')
            {
                return VisitRealVectorValueChange(lexer, text.Slice(1), idToVariable);
            }
            else
            {
                return VisitScalarValueChange(text, idToVariable);
            }
        }

        internal static IValueChange VisitBinaryVectorValueChange(VCDLexer lexer, ReadOnlySpan<char> valueText, Dictionary<string, VarDef> idToVariable)
        {
            BitState[] bits = ToBitStates(valueText);
            string id = lexer.NextWord().ToString();

            if (idToVariable.TryGetValue(id, out VarDef variable))
            {
                return new BinaryChange(bits, variable);
            }
            else
            {
                throw new Exception($"Unknown variable identifier: {id}");
            }
        }

        internal static IValueChange VisitRealVectorValueChange(VCDLexer lexer, ReadOnlySpan<char> valueText, Dictionary<string, VarDef> idToVariable)
        {
            double value = double.Parse(valueText, NumberStyles.Float, CultureInfo.InvariantCulture);
            string id = lexer.NextWord().ToString();

            if (idToVariable.TryGetValue(id, out VarDef variable))
            {
                return new RealChange(value, variable);
            }
            else
            {
                throw new Exception($"Unknown variable identifier: {id}");
            }
        }

        internal static IValueChange VisitScalarValueChange(ReadOnlySpan<char> text, Dictionary<string, VarDef> idToVariable)
        {
            BitState bit = ToBitState(text[0]);
            string id = text.Slice(1).ToString();

            BitState[] bits = new BitState[] { bit };

            if (idToVariable.TryGetValue(id, out VarDef variable))
            {
                return new BinaryChange(bits, variable);
            }
            else
            {
                throw new Exception($"Unknown variable identifier: {id}");
            }
        }

        internal static BitState[] ToBitStates(ReadOnlySpan<char> valueText)
        {
            BitState[] bits = new BitState[valueText.Length];
            for (int i = 0; i < bits.Length; i++)
            {
                bits[i] = ToBitState(valueText[i]);
            }

            return bits;
        }

        internal static BitState ToBitState(char value)
        {
            return value switch
            {
                '0' => BitState.Zero,
                '1' => BitState.One,
                'x' => BitState.X,
                'X' => BitState.X,
                'z' => BitState.Z,
                'Z' => BitState.Z,
                var error => throw new Exception($"Invalid bit state: {error}")
            };
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
            return new SimTime(ParseInt(text.Slice(1)));
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
