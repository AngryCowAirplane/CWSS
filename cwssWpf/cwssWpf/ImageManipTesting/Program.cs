using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Filters.Photo;
using ImageProcessor.Imaging.Formats;
using ImageProcessor.Processors;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
            // get wavier image
            var file = GetFolderPath(SpecialFolder.DesktopDirectory) + "\\waiver.png";
            byte[] photoBytes = File.ReadAllBytes(file);

            // get signature image
            var overlay = GetFolderPath(SpecialFolder.DesktopDirectory) + "\\signatures2.jpg";
            byte[] overlayBytes = File.ReadAllBytes(overlay);

            // temp file paths
            var sigPath = GetFolderPath(SpecialFolder.DesktopDirectory) + "\\signature.jpg";
            var pngPath = GetFolderPath(SpecialFolder.DesktopDirectory) + "\\newImage.png";
            var bmpPath = GetFolderPath(SpecialFolder.DesktopDirectory) + "\\newImage.bmp";

            // final PDF Waiver File
            var pdfFilePath = GetFolderPath(SpecialFolder.DesktopDirectory) + "\\" + UserName + "_" + DateTime.Now.ToShortDateString().Replace('/', '-') + ".pdf";

            ISupportedImageFormat format = new PngFormat { Quality = 70 };
            var size = new Size(1200, 1365);

            // read base waiver image and add signature
            using (MemoryStream inStream = new MemoryStream(photoBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData:true))
                    {
                        // load and overlay black and white version of the sigature image to the waiver image
                        imageFactory.Load(overlay).EntropyCrop(100).Contrast(100).Filter(MatrixFilters.BlackWhite).Save(sigPath);
                        var layer = new ImageLayer();
                        layer.Image = System.Drawing.Image.FromFile(sigPath);
                        layer.Position = new Point(400, 1225);
                        layer.Size = new Size(250, 900);
                        imageFactory.Load(inStream).Overlay(layer).Format(format).Resize(size).Save(outStream);
                        layer.Image.Dispose();
                    }
                    using (FileStream fileStream = new FileStream(pngPath, FileMode.Create))
                    {
                        outStream.CopyTo(fileStream);
                    }
                }
            }


            // create and save a bitmap with added user name and date text
            Bitmap bitmap = (Bitmap)System.Drawing.Image.FromFile(pngPath);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                using (System.Drawing.Font arialFont = new System.Drawing.Font("Arial", 25))
                {
                    graphics.DrawString("Name: Derek Meyer", arialFont, Brushes.Black, new Point(50, 1250));
                    graphics.DrawString("Date: " + DateTime.Now.ToShortDateString(), arialFont, Brushes.Black, new Point(900, 1250));
                }
            }
            bitmap.Save(bmpPath);
            bitmap.Dispose();


            // convert saved sign waiver bitmap to pdf document
            Document pdfDoc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, new FileStream(pdfFilePath, FileMode.OpenOrCreate));
            pdfDoc.Open();
            var imageUri = new Uri(bmpPath);
            var image = iTextSharp.text.Image.GetInstance(imageUri);
            var percentage = 600f / image.Height;
            image.ScalePercent(percentage * 100);
            pdfDoc.Add(image);
            pdfDoc.Close();


            // delete miscalaneous created images
            File.Delete(pngPath);
            File.Delete(bmpPath);
            File.Delete(sigPath);
        }
    }
}
