using Antlr4.Runtime;
using FIRRTL.Parsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace FIRRTL
{
    public static class Parse
    {
        public static Circuit FromString(string firrtl)
        {
            ICharStream charStream = new AntlrInputStream(firrtl);
            FIRRTLLexer lexer = new FIRRTLLexer(charStream, true);
            ITokenStream tokenStream = new CommonTokenStream(lexer);
            FIRRTLParser parser = new FIRRTLParser(tokenStream);
            return new Visitor(new UseInfo()).VisitCircuit(parser.circuit());
        }
    }
}
