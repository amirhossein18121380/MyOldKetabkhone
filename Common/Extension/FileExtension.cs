using System.Drawing;
using Microsoft.AspNetCore.Http;

namespace Common.Extension
{
    public static class FileExtension
    {
        public static byte[]? ToByteArrayFromPath(this string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return null;

                var ret = File.ReadAllBytes(filePath);

                return ret;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<byte[]> ToByteArray(this IFormFile formFile)
        {
            await using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
        public static byte[] ToByteArray(this Image imageIn)
        {
            using var ms = new MemoryStream();
            imageIn.Save(ms, imageIn.RawFormat);
            return ms.ToArray();
        }

        public static byte[] ImageToByte(this Image img)
        {
            using var stream = new MemoryStream();
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }
    }
}
