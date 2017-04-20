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
    public class ImageProgram
    {
        static string Name = "";
        static string PDFDocPath = "";
        public static void Main(string[] args)
        {
            //ListFilesRecursive(@"C:\Users\Derek\Source\Repos\CWSSv2\cwssWpf\cwssWpf\cwssWpf\bin\Debug");
            Console.Write("Enter Name: ");
            if (args == null)
                Name = Console.ReadLine();
            else
                Name = args[0];

            Console.WriteLine("Waiting For signature Image...");
            var fileList = Directory.GetFiles(Path.Combine(GetFolderPath(SpecialFolder.DesktopDirectory), @"Samsung SM-G930VL\ALL")).Where(f => f.ToString().Contains(".jpg")).ToList();
            var fileCount = 0;
            while(fileCount <= fileList.Count())
            {
                fileCount = Directory.GetFiles(Path.Combine(GetFolderPath(SpecialFolder.DesktopDirectory), @"Samsung SM-G930VL\ALL")).Where(f => f.ToString().Contains(".jpg")).ToList().Count;
            }
            var newFileList = Directory.GetFiles(Path.Combine(GetFolderPath(SpecialFolder.DesktopDirectory), @"Samsung SM-G930VL\ALL")).Where(f => f.ToString().Contains(".jpg")).ToList();
            foreach (var file in fileList)
            {
                newFileList.Remove(file);
            }
            var newFile = newFileList.First();

            Console.WriteLine("Signature Image Found!");
            Console.WriteLine("Converting Document...");

            doImageStuff(newFile);

            Console.WriteLine("Document Ready!");

            if(args == null)
                while (!Console.KeyAvailable) { }

            System.Diagnostics.Process.Start(PDFDocPath);
        }

        private static void ListFilesRecursive(string path)
        {
            var directories = Directory.GetDirectories(path).ToList();
            if(directories.Count > 0)
            {
                foreach (var dir in directories)
                {
                    ListFilesRecursive(dir);
                }
            }

            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                Console.WriteLine(Path.GetFileName(file));
            }
        }

        private static void doImageStuff(string newFile = "")
        {
            var imageFile = newFile;

            // get wavier image
            var file = GetFolderPath(SpecialFolder.DesktopDirectory) + "\\waiver.png";
            byte[] photoBytes = File.ReadAllBytes(file);

            // get signature image
            //var overlay = GetFolderPath(SpecialFolder.DesktopDirectory) + "\\signatures2.jpg";
            byte[] overlayBytes = File.ReadAllBytes(imageFile);

            // temp file paths
            var sigPath = GetFolderPath(SpecialFolder.DesktopDirectory) + "\\signature.jpg";
            var pngPath = GetFolderPath(SpecialFolder.DesktopDirectory) + "\\newImage.png";
            var bmpPath = GetFolderPath(SpecialFolder.DesktopDirectory) + "\\newImage.bmp";

            // final PDF Waiver File
            var pdfFilePath = GetFolderPath(SpecialFolder.DesktopDirectory) + "\\" + Name + "_" + DateTime.Now.ToShortDateString().Replace('/', '-') + ".pdf";

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
        }
    }
}
