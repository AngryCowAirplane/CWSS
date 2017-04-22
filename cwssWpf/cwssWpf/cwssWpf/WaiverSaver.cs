using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Filters.Photo;
using ImageProcessor.Imaging.Formats;
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

namespace cwssWpf
{
    public class WaiverSaver
    {
        static string Name = "";
        static string PDFDocPath = "";
        static string SigPath = System.IO.Path.Combine(Environment.CurrentDirectory,"AppData", "Images");
        static string WorkPath = System.IO.Path.Combine(Environment.CurrentDirectory, "AppData", "Images", "Temp");
        static string WaiverPath = System.IO.Path.Combine(Environment.CurrentDirectory, "AppData", "Waivers");

        public static void Start(string[] args)
        {
            Name = args[0];
            checkAndCreateDirs();

            var fileList = Directory.GetFiles(SigPath).ToList();
            var fileCount = 0;

            // Wait for new image.
            while (fileCount <= fileList.Count())
            {
                fileCount = Directory.GetFiles(SigPath).ToList().Count;
            }

            var newFileList = Directory.GetFiles(SigPath).ToList();
            var newFile = newFileList.First();

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));

            doImageStuff(newFile);

            if (args == null)
                while (!Console.KeyAvailable) { }

            System.Diagnostics.Process.Start(PDFDocPath);
        }

        private static void checkAndCreateDirs()
        {
            if (!Directory.Exists(SigPath))
                Directory.CreateDirectory(SigPath);
            if (!Directory.Exists(WorkPath))
                Directory.CreateDirectory(WorkPath);
            if (!Directory.Exists(WaiverPath))
                Directory.CreateDirectory(WaiverPath);
            deleteAllSigFiles();
        }

        private static void deleteAllSigFiles()
        {
            DirectoryInfo di = new DirectoryInfo(SigPath);
            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }
        }

        private static void doImageStuff(string newFile = "")
        {
            var imageFile = newFile;

            // get wavier image
            var file = Path.Combine(Environment.CurrentDirectory,"Images", @"waiver.png");
            byte[] photoBytes = File.ReadAllBytes(file);

            // get signature image
            byte[] overlayBytes = File.ReadAllBytes(imageFile);

            // temp file paths
            var sigPath = Path.Combine(WorkPath, @"sig.bmp");
            var pngPath = Path.Combine(WorkPath, @"newImage.png");
            var bmpPath = Path.Combine(WorkPath, @"newImage.bmp");

            // final PDF Waiver File
            var pdfFilePath = WaiverPath + "\\" + Name + " " + DateTime.Now.ToShortDateString().Replace('/', '-') + ".pdf";

            ISupportedImageFormat format = new PngFormat { Quality = 70 };
            var size = new Size(1200, 1365);

            // read base waiver image and add signature
            using (MemoryStream inStream = new MemoryStream(photoBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        // load and overlay black and white version of the sigature image to the waiver image
                        imageFactory.Load(imageFile).EntropyCrop(100).Contrast(100).Filter(MatrixFilters.BlackWhite).Save(sigPath);
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
                    graphics.DrawString(Name, arialFont, Brushes.Black, new Point(50, 1250));
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

            PDFDocPath = pdfFilePath;

            // delete miscalaneous created images
            File.Delete(pngPath);
            File.Delete(bmpPath);
            File.Delete(sigPath);
            deleteAllSigFiles();
        }
    }
}