using System.IO;
using System.Linq;
using VCDReader.Parsing;

namespace VCDReader
{
    public static class Parse
    {
        public static VCD FromFile(string filepath)
        {
            return FromStream(File.OpenRead(filepath));
        }

        public static VCD FromString(string vcdString)
        {
            MemoryStream stream = new MemoryStream(vcdString.Select(x => (byte)x).ToArray());
            return FromStream(new BinaryReader(stream));
        }

        public static VCD FromStream(Stream stream)
        {
            return FromStream(new BinaryReader(stream, System.Text.Encoding.ASCII));
        }

        public static VCD FromStream(BinaryReader stream)
        {
            return Visitor.VisitVcd(new VCDLexer(stream));
        }
    }
}
