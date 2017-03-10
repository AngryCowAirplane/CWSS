using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Filters.Photo;
using ImageProcessor.Imaging.Formats;
using ImageProcessor.Processors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Environment;

namespace ImageManipTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = GetFolderPath(SpecialFolder.DesktopDirectory) + "\\waiver.png";
            byte[] photoBytes = File.ReadAllBytes(file);

            var overlay = GetFolderPath(SpecialFolder.DesktopDirectory) + "\\signatures.jpg";
            byte[] overlayBytes = File.ReadAllBytes(overlay);

            ISupportedImageFormat format = new PngFormat { Quality = 70 };
            var size = new Size(1200, 1365);

            using (MemoryStream inStream = new MemoryStream(photoBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData:true))
                    {
                        // signature
                        imageFactory.Load(overlay).Contrast(100).Filter(MatrixFilters.BlackWhite).Save(overlay);


                        var layer = new ImageLayer();
                        layer.Image = Image.FromFile(overlay);
                        layer.Position = new Point(400, 1225);
                        layer.Size = new Size(250, 900);

                        imageFactory.Load(inStream).Overlay(layer).Save(outStream);
                    }
                    using (FileStream fileStream = new FileStream(GetFolderPath(SpecialFolder.DesktopDirectory) + "\\newImage.png", FileMode.Create))
                    {
                        outStream.CopyTo(fileStream);
                    }
                }
            }

            Bitmap bitmap = (Bitmap)Image.FromFile(GetFolderPath(SpecialFolder.DesktopDirectory) + "\\newImage.png");

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                using (Font arialFont = new Font("Arial", 25))
                {
                    graphics.DrawString("Name: Derek Meyer", arialFont, Brushes.Black, new Point(50, 1250));
                    graphics.DrawString("Date: " + DateTime.Now.ToShortDateString(), arialFont, Brushes.Black, new Point(900, 1250));
                }
            }

            bitmap.Save(GetFolderPath(SpecialFolder.DesktopDirectory) + "\\newImage.bmp");
        }
    }
}
