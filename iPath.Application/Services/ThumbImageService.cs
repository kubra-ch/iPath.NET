using iPath.Application.Configuration;
using iPath.Data.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace iPath.Application.Services;

public interface IThumbImageService
{
    NodeFile UpdateNode(NodeFile file, string filename);
}

public class ThumbImageService(IOptions<iPathConfig> opts, ILogger<ThumbImageService> logger) : IThumbImageService
{
    public NodeFile UpdateNode(NodeFile file, string filename)
    {
        try
        {
            var originalImage = Image.FromFile(filename);
            file.ImageWidth = originalImage.Width;
            file.ImageHeight = originalImage.Height;

            int thumbSize = opts.Value.ThumbSize;
            var thumbnail = new Bitmap(thumbSize, thumbSize);
            using (var graphics = Graphics.FromImage(thumbnail))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(originalImage, 0, 0, thumbSize, thumbSize);
            }

            byte[] bytearray;
            using (MemoryStream ms = new MemoryStream())
            {
                thumbnail.Save(ms, ImageFormat.Jpeg);
                bytearray = ms.ToArray();
            }

            file.ThumbData = Convert.ToBase64String(bytearray);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
        }

        return file;
    }
}