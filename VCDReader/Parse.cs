﻿using System.IO;
using System.Linq;
using VCDReader.Parsing;

namespace VCDReader
{
    public static class Parse
    {
        public static VCD FromFile(string filepath)
        {
            var fileStream = File.OpenRead(filepath);
            return FromStream(fileStream);
        }

        public static VCD FromString(string vcdString)
        {
            MemoryStream stream = new MemoryStream(vcdString.Select(x => (byte)x).ToArray());
            return FromStream(new BinaryReader(stream));
        }

        public static VCD FromStream(Stream stream)
        {
            return FromStream(new BinaryReader(stream, System.Text.ASCIIEncoding.ASCII));
        }

        public static VCD FromStream(BinaryReader stream)
        {
            VCDLexer lexer = new VCDLexer(stream);
            return Visitor.VisitVcd(lexer);
        }
    }
}
