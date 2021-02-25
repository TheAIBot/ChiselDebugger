using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using VCDReader.Parsing;

namespace VCDReader
{
    public static class Parse
    {
        public static VCD FromFile(string filepath)
        {
            var fileStream = File.OpenText(filepath);
            return FromStream(fileStream);
        }

        public static VCD FromString(string vcdString)
        {
            var stringStream = new StringReader(vcdString);
            return FromStream(stringStream);
        }

        public static VCD FromStream(TextReader stream)
        {
            VCDLexer lexer = new VCDLexer(stream);
            return Visitor.VisitVcd(lexer);
        }
    }
}
