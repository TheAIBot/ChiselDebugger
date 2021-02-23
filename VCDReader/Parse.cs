using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using VCDReader.Parsing;
using VCDReader.Parsing.Antlr;

namespace VCDReader
{
    public static class Parse
    {
        public static VCD FromFile(string filepath)
        {
            using var fileStream = File.OpenText(filepath);
            return FromStream(fileStream);
        }

        public static VCD FromString(string vcdString)
        {
            using var stringStream = new StringReader(vcdString);
            return FromStream(stringStream);
        }

        public static VCD FromStream(TextReader stream)
        {
            ICharStream charStream = new UnbufferedCharStream(stream);
            VCDLexer lexer = new VCDLexer(charStream);
            lexer.TokenFactory = new CommonTokenFactory(true);
            ITokenStream tokenStream = new UnbufferedTokenStream(lexer);
            VCDParser parser = new VCDParser(tokenStream);
            return Visitor.VisitVcd(parser.vcd());
        }
    }
}
