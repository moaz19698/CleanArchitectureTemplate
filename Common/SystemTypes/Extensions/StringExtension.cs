using System.IO.Compression;
using System.Text;

namespace CleanArchitecture.Common.SystemTypes.Extensions
{
    public static class StringExtension
    {
        public static string ToBase64(this string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            string returnValue = Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;

        }

        public static string FromBase64(this string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            string returnValue = Encoding.ASCII.GetString(encodedDataAsBytes);

            return returnValue;

        }
        public static string ToCompress(this string text)
        {
            byte[] dataToCompress = Encoding.ASCII.GetBytes(text);
            byte[] compressedData = Compress(dataToCompress);
            string compressedString = Encoding.ASCII.GetString(compressedData);

            return compressedString;

        }
        public static string ToDecompress(this string text)
        {
            byte[] dataToCompress = Encoding.ASCII.GetBytes(text);
            byte[] decompressedData = Decompress(dataToCompress);
            string deCompressedString = Encoding.ASCII.GetString(decompressedData);

            return deCompressedString;

        }

        public static byte[] Compress(byte[] bytes)
        {
            using var memoryStream = new MemoryStream();
            using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
            {
                gzipStream.Write(bytes, 0, bytes.Length);
            }
            return memoryStream.ToArray();
        }

        public static byte[] Decompress(byte[] bytes)
        {
            using var memoryStream = new MemoryStream(bytes);

            using var outputStream = new MemoryStream();
            using (var decompressStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                decompressStream.CopyTo(outputStream);
            }
            return outputStream.ToArray();
        }
    }
}
