using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PengSW.ImageHelper
{
    public static class ImageHelper
    {
        public static BitmapImage CreateBitmapImage(byte[] aBytes)
        {
            BitmapImage aBitmapImage = new BitmapImage();
            aBitmapImage.BeginInit();
            aBitmapImage.StreamSource = new MemoryStream((byte[])aBytes);
            aBitmapImage.EndInit();
            return aBitmapImage;
        }

        public static void SaveBitmapSource(string aFileName, BitmapSource aImage)
        {
            JpegBitmapEncoder aEncoder = new JpegBitmapEncoder();
            aEncoder.Frames.Add(BitmapFrame.Create(aImage));
            using (FileStream aStream = new FileStream(aFileName, FileMode.Create))
            {
                aEncoder.Save(aStream);
                aStream.Close();
            }
        }

        public delegate byte[] ProcessImageDelegate(byte[] aSourceRawData, ref int aPixelWidth, ref int aPixelHeight, int aBytesPerPixel, ref int aStride);

        public static BitmapSource ProcessImage(BitmapImage aSourceImage, ProcessImageDelegate aProcess)
        {
            // 转换为标准Bgr32格式
            FormatConvertedBitmap aFormatedImage = new FormatConvertedBitmap(aSourceImage, PixelFormats.Bgr32, null, 0);

            // 提取图片数据
            int aStride = (aFormatedImage.Format.BitsPerPixel * aFormatedImage.PixelWidth + 7) / 8;
            byte[] aSourceRawData = new byte[aStride * aFormatedImage.PixelHeight];
            aFormatedImage.CopyPixels(aSourceRawData, aStride, 0);

            // 处理图片数据
            int aPixelWidth = aFormatedImage.PixelWidth;
            int aPixelHeight = aFormatedImage.PixelHeight;
            byte[] aResultRawData = aProcess(aSourceRawData, ref aPixelWidth, ref aPixelHeight, (aFormatedImage.Format.BitsPerPixel + 7) / 8, ref aStride);

            // 生成结果图片
            BitmapSource aImageFromRawData = BitmapImage.Create(
                aPixelWidth, aPixelHeight,
                aFormatedImage.DpiX, aFormatedImage.DpiY,
                aFormatedImage.Format, aFormatedImage.Palette,
                aResultRawData, aStride);

            return aImageFromRawData;
        }

        public static BitmapSource ResizeImage(BitmapSource aImage, int aWidth, int aHeight)
        {
            var aDrawingVisual = new DrawingVisual();
            using (DrawingContext dc = aDrawingVisual.RenderOpen())
            {
                dc.DrawImage(aImage, new Rect(0, 0, aWidth, aHeight));
            }
            RenderTargetBitmap aTargetBitmap = new RenderTargetBitmap(aWidth, aHeight, aImage.DpiX, aImage.DpiY, PixelFormats.Default);
            aTargetBitmap.Render(aDrawingVisual);
            return aTargetBitmap;
        }

        public static BitmapSource ResizeImage(BitmapSource aImage, int aWidth)
        {
            int aHeight = aImage.PixelHeight * aWidth / aImage.PixelWidth;
            return ResizeImage(aImage, aWidth, aHeight);
        }
    }
}
