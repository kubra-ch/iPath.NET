using iPath.Data.Configuration;
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

            int thumbWidth = opts.Value.ThumbSize;
            int thumbHeight = opts.Value.ThumbSize;

            if (file.ImageWidth > file.ImageHeight )
            {
                thumbHeight = (int)((float)file.ImageHeight / file.ImageWidth * thumbWidth);
            }
            else
            {
                thumbWidth  = (int)((float)file.ImageWidth/ file.ImageHeight * thumbHeight);
            }

            var thumbnail = new Bitmap(thumbWidth, thumbHeight);

            using (var graphics = Graphics.FromImage(thumbnail))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(originalImage, 0, 0, thumbWidth, thumbHeight);
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