using System;
using System.IO;
using System.Linq;

namespace VCDReader.Parsing
{
    internal sealed class VCDLexer : IDisposable
    {
        private readonly BinaryReader Reader;
        private byte[] Buffer;
        private bool ReachedEOF;
        private Memory<byte> AvailableChars;
        private const int DefaultBufferSize = 1024;
        private static readonly byte[] SkipChars = new byte[] { (byte)' ', (byte)'\t', (byte)'\r', (byte)'\n' };

        public VCDLexer(BinaryReader reader)
        {
            this.Reader = reader;
            this.Buffer = new byte[DefaultBufferSize];
            this.ReachedEOF = false;
            this.AvailableChars = new Memory<byte>();

            FillBuffer(true);
        }

        public ReadOnlySpan<byte> NextInteger()
        {
            SkipStart();

            int wordLength = 0;
            while (!IsEmpty() && wordLength < AvailableChars.Length)
            {
                if (!char.IsDigit((char)AvailableChars.Span[wordLength]))
                {
                    break;
                }

                wordLength++;
                if (AvailableChars.Length == wordLength && !ReachedEOF)
                {
                    FillBuffer(false);
                }
            }

            ReadOnlySpan<byte> numbers = AvailableChars.Span.Slice(0, wordLength);
            AvailableChars = AvailableChars.Slice(numbers.Length);
            return numbers;
        }

        public ReadOnlyMemory<byte> PeekNextWord()
        {
            SkipStart();

            int wordLength = 0;
            while (!IsEmpty() && wordLength < AvailableChars.Length)
            {
                ReadOnlySpan<byte> chars = AvailableChars.Span;
                while (wordLength < chars.Length)
                {
                    if (!IsWordChar((char)chars[wordLength]))
                    {
                        goto skipStop;
                    }

                    wordLength++;
                }

                if (AvailableChars.Length == wordLength && !ReachedEOF)
                {
                    FillBuffer(false);
                }
            }

        skipStop:
            return AvailableChars.Slice(0, wordLength);
        }

        public char PeekNextChar()
        {
            SkipStart();

            return (char)AvailableChars.Span[0];
        }

        public void SkipWord(ReadOnlySpan<byte> word)
        {
            AvailableChars = AvailableChars.Slice(word.Length);
        }

        public void SkipChar()
        {
            AvailableChars = AvailableChars.Slice(1);
        }

        public ReadOnlySpan<byte> NextWord()
        {
            ReadOnlySpan<byte> word = PeekNextWord().Span;
            SkipWord(word);

            return word;
        }

        public ReadOnlyMemory<byte> NextWordAsMem()
        {
            ReadOnlyMemory<byte> wordMem = PeekNextWord();
            SkipWord(wordMem.Span);

            return wordMem;
        }

        public ReadOnlySpan<byte> NextUntil(ReadOnlySpan<byte> stopAt)
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
                        throw new Exception($"Failed to find pattern before EOF. Pattern: {new string(stopAt.ToString())}");
                    }
                    FillBuffer(false);
                }
            } while (foundIndex == notFound);

            ReadOnlySpan<byte> text = AvailableChars.Span.Slice(0, foundIndex);
            AvailableChars = AvailableChars.Slice(foundIndex);

            return text.TrimEnd(SkipChars);
        }

        public void ExpectNextWord(ReadOnlySpan<byte> expectedWord)
        {
            ReadOnlySpan<byte> actualword = NextWord();
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
                ReadOnlySpan<byte> chars = AvailableChars.Span;
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
                int copiedLength = Reader.Read(Buffer, 0, Buffer.Length);
                ReachedEOF = copiedLength < Buffer.Length;
                AvailableChars = new Memory<byte>(Buffer, 0, copiedLength);
            }
            else
            {
                //If there is still available chars equal to the
                //size of the buffer then that means the buffer
                //isn't large enough to contain all the chars
                //required for some operation.
                if (AvailableChars.Length == Buffer.Length)
                {
                    byte[] biggerBuffer = new byte[Buffer.Length * 2];
                    AvailableChars.CopyTo(biggerBuffer);
                    Buffer = biggerBuffer;
                }
                else
                {
                    //Move remaining text to start of buffer so more
                    //text can be added to the end of it
                    Memory<byte> from = AvailableChars;
                    Memory<byte> to = new Memory<byte>(Buffer, 0, AvailableChars.Length);
                    from.CopyTo(to);
                    AvailableChars = to;
                }

                int spaceLeft = Buffer.Length - AvailableChars.Length;
                int copiedLength = Reader.Read(Buffer, AvailableChars.Length, spaceLeft);
                ReachedEOF = copiedLength < spaceLeft;
                AvailableChars = new Memory<byte>(Buffer, 0, AvailableChars.Length + copiedLength);
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

        internal string BufferToString()
        {
            ReadOnlySpan<byte> bufferBytes = Buffer.AsSpan();
            return bufferBytes.ToCharString();
        }

        public void Dispose()
        {
#pragma warning disable IDISP007 // Don't dispose injected
            Reader.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected
        }
    }
}
