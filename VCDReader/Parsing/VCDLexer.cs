using System;
using System.IO;
using System.Linq;

namespace VCDReader.Parsing
{
    internal class VCDLexer : IDisposable
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

        public char PeekNextChar()
        {
            SkipStart();

            return AvailableChars.Span[0];
        }

        public void SkipWord(ReadOnlySpan<char> word)
        {
            AvailableChars = AvailableChars.Slice(word.Length);
        }

        public void SkipChar()
        {
            AvailableChars = AvailableChars.Slice(1);
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
            while (true)
            {
                int skip = 0;
                ReadOnlySpan<char> chars = AvailableChars.Span;
                while (skip < chars.Length && chars[skip] < '!')
                {
                    skip++;
                }

                AvailableChars = AvailableChars.Slice(skip);
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

        internal bool IsWordsRemaining()
        {
            if (!ReachedEOF)
            {
                return true;
            }

            return AvailableChars.Length > 0 && AvailableChars.Span.TrimStart(SkipChars).Length > 0;
        }

        public void Dispose()
        {
            Reader.Dispose();
        }
    }
}
