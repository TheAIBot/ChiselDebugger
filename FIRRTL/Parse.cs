using Antlr4.Runtime;
using FIRRTL.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FIRRTL
{
    public static class Parse
    {
        public static Circuit FromString(string firrtl)
        {
            using Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(firrtl));
            return FromStream(stream);
        }

        public static Circuit FromFile(string filepath)
        {
            using Stream stream = File.OpenRead(filepath);
            return FromStream(stream);
        }

        public static Circuit FromStream(Stream stream)
        {
            ICharStream charStream = new AntlrInputStream(stream);
            FIRRTLLexer lexer = new FIRRTLLexer(charStream, true);
            ITokenStream tokenStream = new CommonTokenStream(lexer);
            FIRRTLParser parser = new FIRRTLParser(tokenStream);
            return new Visitor(new UseInfo()).VisitCircuit(parser.circuit());
        }
    }
}
