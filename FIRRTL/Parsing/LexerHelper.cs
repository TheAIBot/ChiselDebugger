// SPDX-License-Identifier: Apache-2.0

using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FIRRTL.Parsing
{
    internal abstract class LexerHelper
    {
        private Queue<IToken> TokenBuffer = new Queue<IToken>();
        private Stack<int> Indentations = new Stack<int>();
        private bool ReachedEOF = false;

        private IToken EOFhandler(IToken t)
        {
            IToken ret;
            if (Indentations.Count == 0)
            {
                ret = CreateToken(FIRRTLParser.NEWLINE, t);
            }
            else
            {
                ret = UnwindTo(0, t);
            }

            TokenBuffer.Enqueue(t);
            ReachedEOF = true;

            return ret;
        }

        internal IToken NextToken()
        {
            if (Indentations.Count == 0)
            {
                Indentations.Push(0);

                IToken FindFirstRead()
                {
                    IToken t1 = PullToken();
                    if (t1.Type != FIRRTLParser.NEWLINE)
                    {
                        return t1;
                    }
                    else
                    {
                        return FindFirstRead();
                    }
                }

                IToken firstRealToken = FindFirstRead();

                if (firstRealToken.Column > 0)
                {
                    Indentations.Push(firstRealToken.Column);
                    TokenBuffer.Enqueue(CreateToken(FIRRTLParser.INDENT, firstRealToken));
                }
                TokenBuffer.Enqueue(firstRealToken);
            }

            IToken HandleNewLineToken(IToken token)
            {
                (IToken, IToken) NonNewline(IToken token1)
                {
                    IToken nextNext1 = PullToken();
                    if (nextNext1.Type == FIRRTLParser.NEWLINE)
                    {
                        return NonNewline(nextNext1);
                    }
                    else
                    {
                        return (token1, nextNext1);
                    }
                }

                (IToken nxtToken, IToken nextNext) = NonNewline(token);

                if (nextNext.Type == FIRRTLParser.Eof) // might be wrong
                {
                    return EOFhandler(nextNext);
                }
                else
                {
                    string nlText = nxtToken.Text;
                    int indent;
                    if (nlText.Length > 0 && nlText[0] == '\r')
                    {
                        indent = nlText.Length - 2;
                    }
                    else
                    {
                        indent = nlText.Length - 1;
                    }

                    int prevIndent = Indentations.First(); // might be wrong

                    IToken retToken;
                    if (indent == prevIndent)
                    {
                        retToken = nxtToken;
                    }
                    else if (indent > prevIndent)
                    {
                        Indentations.Push(indent);
                        retToken = CreateToken(FIRRTLParser.INDENT, nxtToken);
                    }
                    else
                    {
                        retToken = UnwindTo(indent, nxtToken);
                    }

                    TokenBuffer.Enqueue(nextNext);
                    return retToken;
                }
            }

            IToken t;
            if (TokenBuffer.Count == 0)
            {
                t = PullToken();
            }
            else
            {
                t = TokenBuffer.Dequeue();
            }

            if (ReachedEOF)
            {
                return t;
            }
            else if (t.Type == FIRRTLParser.NEWLINE)
            {
                return HandleNewLineToken(t);
            }
            else if (t.Type == FIRRTLParser.Eof) // might be wrong
            {
                return EOFhandler(t);
            }
            else
            {
                return t;
            }
        }

        internal abstract IToken PullToken();

        private IToken CreateToken(int tokenType, IToken copyFrom)
        {
            string text;
            if (tokenType == FIRRTLParser.NEWLINE)
            {
                text = "<NEWLINE>";
            }
            else if (tokenType == FIRRTLParser.INDENT)
            {
                text = "<INDENT>";
            }
            else if (tokenType == FIRRTLParser.DEDENT)
            {
                text = "<DEDENT>";
            }
            else
            {
                throw new Exception("");
            }

            return new CommonToken(copyFrom)
            {
                Type = tokenType,
                Text = text
            };
        }

        private IToken UnwindTo(int targetIndent, IToken copyFrom)
        {
            Debug.Assert(TokenBuffer.Count == 0);
            TokenBuffer.Enqueue(CreateToken(FIRRTLParser.NEWLINE, copyFrom));

            void doPop()
            {
                int prevIndent = Indentations.Pop();
                if (prevIndent < targetIndent)
                {
                    Indentations.Push(prevIndent);
                    TokenBuffer.Enqueue(CreateToken(FIRRTLParser.INDENT, copyFrom));
                }
                else if (prevIndent > targetIndent)
                {
                    TokenBuffer.Enqueue(CreateToken(FIRRTLParser.DEDENT, copyFrom));
                    doPop();
                }
            }

            doPop();

            Indentations.Push(targetIndent);
            return TokenBuffer.Dequeue();
        }
    }
}
