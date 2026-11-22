using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace крош
{
    internal static class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam,
                                                       string lpvParam, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 0x0014;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;

        [STAThread]
        static void Main()
        {
            try
            {
                string imagePath = ExtractEmbeddedImage();
                SetWallpaper(imagePath);
            }
            catch
            {
            }
        }

        private static string ExtractEmbeddedImage()
        {
            const string resourceName = "крош.CroshSzadi.png";
            string tempPath = Path.Combine(Path.GetTempPath(), "CroshSzadi.png");

            Stream stream = Assembly.GetExecutingAssembly()
                                    .GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new FileNotFoundException(
                    "Встроенный ресурс '" + resourceName + "' не найден.");

            using (stream)
            using (var fileStream = File.Create(tempPath))
            {
                stream.CopyTo(fileStream);
            }

            return tempPath;
        }
        private static void SetWallpaper(string imagePath)
        {
            string bmpPath = ConvertToBmp(imagePath);

            SystemParametersInfo(
                SPI_SETDESKWALLPAPER,
                0,
                bmpPath,
                SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        private static string ConvertToBmp(string sourcePath)
        {
            string bmpPath = Path.ChangeExtension(sourcePath, ".bmp");

            using (var img = Image.FromFile(sourcePath))
            using (var bmp = new Bitmap(img.Width, img.Height,
                                        PixelFormat.Format24bppRgb))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(img, 0, 0, img.Width, img.Height);
                }
                bmp.Save(bmpPath, ImageFormat.Bmp);
            }

            return bmpPath;
        }
    }
}