using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace FIRRTL.Parsing
{
    public static class Parse
    {
        public static Circuit ParseString(string firrtl)
        {
            ICharStream charStream = new AntlrInputStream(firrtl);
            FIRRTLLexer lexer = new FIRRTLLexer(charStream);
            ITokenStream tokenStream = new CommonTokenStream(lexer);
            FIRRTLParser parser = new FIRRTLParser(tokenStream);
            return new Visitor().VisitCircuit(parser.circuit());
            
        }
    }
}
