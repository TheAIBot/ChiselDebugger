using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VCDReader.Parsing;

namespace VCDReader
{
    public static class Parse
    {
        public static VCD FromFile(string filepath)
        {
            var fileStream = File.OpenRead(filepath);
            return FromStream(new BinaryReader(fileStream, System.Text.ASCIIEncoding.ASCII));
        }

        public static VCD FromString(string vcdString)
        {
            MemoryStream stream = new MemoryStream(vcdString.Select(x => (byte)x).ToArray());
            return FromStream(new BinaryReader(stream));
        }

        public static VCD FromStream(BinaryReader stream)
        {
            VCDLexer lexer = new VCDLexer(stream);
            return Visitor.VisitVcd(lexer);
        }
    }
}
