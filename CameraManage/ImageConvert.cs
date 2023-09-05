using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace CameraManage
{
    public static class ImageConvert
    {
        /// <summary>
        /// 将图像数据转为bitmap
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap ConvertImageDataToBitmap(byte[] imageData, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            // 设置调色板
            ColorPalette palette = bitmap.Palette;
            for (int i = 0; i < 256; i++)
                palette.Entries[i] = Color.FromArgb(i, i, i);
            bitmap.Palette = palette;

            // 将图像数据写入Bitmap对象
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            System.Runtime.InteropServices.Marshal.Copy(imageData, 0, bmpData.Scan0, imageData.Length);
            bitmap.UnlockBits(bmpData);

            return bitmap;
        }

        /// <summary>
        /// 将图像数据转为bitmapImage
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static BitmapImage ConvertImageDataToBitmapImage(byte[] imageData, int width, int height)
        {
            return ConvertBitmapToBitmapImage(ConvertImageDataToBitmap(imageData, width, height));
        }

        private static BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            //// 保存Bitmap图像为其他格式（如PNG）
            //PngBitmapEncoder encoder = new PngBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            //using (FileStream fs = new FileStream("bitmap_as_png.png", FileMode.Create))
            //{
            //    encoder.Save(fs);
            //}

            return bitmapImage;
        }
    }
}
