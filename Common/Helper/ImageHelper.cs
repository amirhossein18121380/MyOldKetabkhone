using System.Drawing;
using System.Drawing.Drawing2D;

namespace Common.Helper;

public static class ImageHelper
{
    public static bool SaveImage(this MemoryStream stream, string fileName, string subFolderName = "ProfilePictures")
    {
        try
        {
            if (ApplicationSettings.ResourceFolderPath != null)
            {
                var folderPath = !string.IsNullOrEmpty(subFolderName) ? Path.Combine(ApplicationSettings.ResourceFolderPath, subFolderName) : ApplicationSettings.ResourceFolderPath;
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var imagePath = Path.Combine(folderPath, fileName);

                var image = Image.FromStream(stream);

                image.Save(imagePath);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public static bool SaveImage(this byte[] imgArray, string fileName, string subFolderName = "ProfilePictures")
    {
        try
        {
            if (ApplicationSettings.ResourceFolderPath != null)
            {
                var folderPath = !string.IsNullOrEmpty(subFolderName) ? Path.Combine(ApplicationSettings.ResourceFolderPath, subFolderName) : ApplicationSettings.ResourceFolderPath;
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var imagePath = Path.Combine(folderPath, fileName);

                Image image;
                using (var ms = new MemoryStream(imgArray))
                {
                    image = Image.FromStream(ms);
                }

                image.Save(imagePath);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool DeleteFile(string fileName, string subFolderName = "ProfilePictures")
    {
        try
        {
            if (ApplicationSettings.ResourceFolderPath != null)
            {
                var folderPath = !string.IsNullOrEmpty(subFolderName) ? Path.Combine(ApplicationSettings.ResourceFolderPath, subFolderName) : ApplicationSettings.ResourceFolderPath;
                var imagePath = Path.Combine(folderPath, fileName);

                if (!File.Exists(imagePath))
                    return false;

                File.Delete(imagePath);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static KeyValuePair<string, string> GetFilePath(this string fileName, string subFolderName = "ProfilePictures")
    {
        try
        {
            if (ApplicationSettings.ResourceFolderPath != null)
            {
                var folderPath = !string.IsNullOrEmpty(subFolderName) ? Path.Combine(ApplicationSettings.ResourceFolderPath, subFolderName) : ApplicationSettings.ResourceFolderPath;
                var imagePath = Path.Combine(folderPath, fileName);

                if (!File.Exists(imagePath))
                    return new KeyValuePair<string, string>();

                var ext = Path.GetExtension(imagePath);
                return new KeyValuePair<string, string>(imagePath, ext);
            }

            return new KeyValuePair<string, string>();
        }
        catch (Exception)
        {
            return new KeyValuePair<string, string>();
        }
    }

    public static Image ResizeImage(Image image, Size size, bool preserveAspectRatio = true)
    {
        int newWidth;
        int newHeight;
        if (preserveAspectRatio)
        {
            var originalWidth = image.Width;
            var originalHeight = image.Height;
            var percentWidth = size.Width / (float)originalWidth;
            var percentHeight = size.Height / (float)originalHeight;
            var percent = percentHeight < percentWidth ? percentHeight : percentWidth;
            newWidth = (int)(originalWidth * percent);
            newHeight = (int)(originalHeight * percent);
        }
        else
        {
            newWidth = size.Width;
            newHeight = size.Height;
        }
        Image newImage = new Bitmap(newWidth, newHeight);
        using (var graphicsHandle = Graphics.FromImage(newImage))
        {
            graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphicsHandle.SmoothingMode = SmoothingMode.HighQuality;
            graphicsHandle.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
        }

        return newImage;
    }
}