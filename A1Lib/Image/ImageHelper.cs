using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace ALib.Image
{
    public static class ImageHelper
    {
        /// <summary>
        /// 将 Bitmap 以指定质量保存为 JPG 文件
        /// </summary>
        /// <param name="bitmap">要保存的 Bitmap 对象</param>
        /// <param name="outputPath">输出文件路径</param>
        /// <param name="quality">压缩质量 (0-100)</param>
        public static void SaveAsJpeg(this Bitmap bitmap, string outputPath, long quality)
        {
            // 1. 获取 JPG 格式的图像编码器
            ImageCodecInfo jpegEncoder = GetEncoder(ImageFormat.Jpeg);

            // 2. 创建并设置编码参数，这里设置压缩质量
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            // 3. 使用指定的编码器和参数保存图像
            bitmap.Save(outputPath, jpegEncoder, encoderParams);
        }

        /// <summary>
        /// 辅助方法：根据 ImageFormat 获取对应的 ImageCodecInfo
        /// </summary>
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
