using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Game.Foundation.Random;

namespace Game.Infrastructure.Persistence
{
    // Bounded strict decoder for persisted mdc1 images. Canonical re-encoding rejects alternate encodings.
    internal sealed class MdcReader
    {
        private readonly byte[] data;
        private int offset;
        private static readonly UTF8Encoding Utf8 = new UTF8Encoding(false, true);
        private MdcReader(byte[] data) { this.data = data; offset = 4; }
        internal static object? Decode(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 5 || bytes.Length > 64 * 1024 * 1024 || Encoding.ASCII.GetString(bytes,0,4) != "MDC1") throw new FormatException("INVALID_MDC1");
            var reader = new MdcReader(bytes); var result = reader.Read(0);
            if (reader.offset != bytes.Length || !CanonicalCodec.Encode(result).SequenceEqual(bytes)) throw new FormatException("NONCANONICAL_MDC1");
            return result;
        }
        private byte Take() => offset < data.Length ? data[offset++] : throw new FormatException("TRUNCATED_MDC1");
        private string Until(byte separator)
        {
            var start = offset;
            while (Take() != separator) if (offset - start > 22) throw new FormatException("MDC1_NUMBER_RANGE");
            return Encoding.ASCII.GetString(data,start,offset-start-1);
        }
        private int Count()
        {
            var text = Until((byte)':');
            if (!int.TryParse(text,NumberStyles.None,CultureInfo.InvariantCulture,out var n) || n > data.Length - offset) throw new FormatException("MDC1_LENGTH");
            return n;
        }
        private object? Read(int depth)
        {
            if (depth > 48) throw new FormatException("MDC1_DEPTH");
            switch ((char)Take())
            {
                case 'n': return null;
                case 't': return true;
                case 'f': return false;
                case 'i':
                    var text = Until((byte)';');
                    if (long.TryParse(text,NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture,out var signed)) return signed;
                    if (ulong.TryParse(text,NumberStyles.None,CultureInfo.InvariantCulture,out var unsigned)) return unsigned;
                    throw new FormatException("MDC1_INTEGER");
                case 's':
                    var length = Count(); var value = Utf8.GetString(data,offset,length); offset += length; return value;
                case 'a':
                    var count = Count(); var array = new List<object?>();
                    for (int i=0;i<count;i++) array.Add(Read(depth+1));
                    return array;
                case 'o':
                    var fields = Count(); var map = new Dictionary<string,object?>(StringComparer.Ordinal);
                    for (int i=0;i<fields;i++)
                    {
                        var key = Read(depth+1) as string ?? throw new FormatException("MDC1_KEY");
                        if (map.ContainsKey(key)) throw new FormatException("MDC1_DUPLICATE_KEY");
                        map.Add(key,Read(depth+1));
                    }
                    return map;
                default: throw new FormatException("MDC1_TAG");
            }
        }
    }
}
