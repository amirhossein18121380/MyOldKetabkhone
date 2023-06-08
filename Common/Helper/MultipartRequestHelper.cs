using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Common.Helper;

public static class MultipartRequestHelper
{
    public static async Task<MultiPartModel?> GetMultiPart(HttpRequest request, HttpContext context)
    {
        var model = new  MultiPartModel();

        if (!MultipartRequestHelper.IsMultipartContentType(request.ContentType))
        {
            return null;
        }

        var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(request.ContentType), 500000);

        var reader = new MultipartReader(boundary, context.Request.Body);

        Stream input;
        var section = await reader.ReadNextSectionAsync();
        while (section != null)
        {
            ContentDispositionHeaderValue? contentDisposition;
            var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

            if (hasContentDispositionHeader)
            {
                if (HasFileContentDisposition(contentDisposition))
                {
                    #region File Content
                    var thisFileName = contentDisposition.FileName.ToString().Trim('\"');
                    model.FileExtension = thisFileName.Remove(0, thisFileName.LastIndexOf('.'));
                    input = section.Body;
                    MemoryStream ms = new MemoryStream();
                    input.CopyTo(ms);
                    model.ImageArrays = ms.ToArray();
                    ms.Dispose();
                    #endregion

                    model.TargetFilePath = Path.GetTempFileName();
                }
                else if (HasFormDataContentDisposition(contentDisposition))
                {
                    using (var streamReader = new StreamReader(section.Body))
                    {
                        // The value length limit is enforced by MultipartBodyLengthLimit
                        model.TextContents = await streamReader.ReadToEndAsync();
                        if (String.Equals(model.TextContents, "undefined", StringComparison.OrdinalIgnoreCase))
                        {
                            model.TextContents = String.Empty;
                        }
                    }
                }
            }
            section = await reader.ReadNextSectionAsync();
        }
        return model;
    }

    public static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
    {
        var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary);
        if (string.IsNullOrWhiteSpace(boundary.ToString()))
        {
            throw new InvalidDataException("Missing content-type boundary.");
        }

        if (boundary.Length > lengthLimit)
        {
            throw new InvalidDataException(
                $"Multipart boundary length limit {lengthLimit} exceeded.");
        }

        return boundary.ToString();
    }

    public static bool IsMultipartContentType(string contentType)
    {
        return !string.IsNullOrEmpty(contentType)
               && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static bool HasFormDataContentDisposition(ContentDispositionHeaderValue? contentDisposition)
    {
        // Content-Disposition: form-data; name="key";
        return contentDisposition != null
               && contentDisposition.DispositionType.Equals("form-data")
               && string.IsNullOrEmpty(contentDisposition.FileName.ToString())
               && string.IsNullOrEmpty(contentDisposition.FileNameStar.ToString());
    }

    public static bool HasFileContentDisposition(ContentDispositionHeaderValue? contentDisposition)
    {
        // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
        return contentDisposition != null
               && contentDisposition.DispositionType.Equals("form-data")
               && (!string.IsNullOrEmpty(contentDisposition.FileName.ToString())
                   || !string.IsNullOrEmpty(contentDisposition.FileNameStar.ToString()));
    }
}

public class MultiPartModel
{
    public string? TargetFilePath { set; get; }
    public string? FileExtension { set; get; }
    public string? TextContents { set; get; }
    public byte[]? ImageArrays { set; get; }
}

