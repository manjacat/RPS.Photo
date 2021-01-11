using RPS.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using RPS.Library.API.Utility;

namespace RPS.Images
{
    
    public class ImageProcessor
    {
        //public static string outputFileName = "trafficjam_008.jpg";

        // this class will get all the rectangles from Face/License Plate
        // and overlay them to the original image to redact the face and license plate
        // it will then save the new image to output folder with the same filename

        public static Bitmap Start(
            Bitmap img,
            List<RectangleModel> rectangles)
        {
            //Create background image
            //Bitmap(imageBackground.Width, imageBackground.Height);
            //Image img = Image.FromFile(inputFileName);
            //Image img = new Bitmap(imageBackground);

            //draw rectangle to image
            return DrawRectangleToImage(img, rectangles);
        }

        private static Rectangle ConvertToRectangle(RectangleModel p)
        {
            Rectangle rect1 = new Rectangle(p.x, p.y, p.width, p.height);
            return rect1;
        }

        

        // Apply blur to image
        private static Bitmap Blur(Bitmap image, Int32 blurSize)
        {
            return Blur(image, new Rectangle(0, 0, image.Width, image.Height), blurSize);
        }

        private unsafe static Bitmap Blur(Bitmap image, Rectangle rectangle, Int32 blurSize)
        {
            Bitmap blurred = new Bitmap(image.Width, image.Height);

            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

            // Lock the bitmap's bits
            BitmapData blurredData = blurred.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, blurred.PixelFormat);

            // Get bits per pixel for current PixelFormat
            int bitsPerPixel = Image.GetPixelFormatSize(blurred.PixelFormat);

            // Get pointer to first line
            byte* scan0 = (byte*)blurredData.Scan0.ToPointer();

            // look at every pixel in the blur rectangle
            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    // average the color of the red, green and blue for each pixel in the
                    // blur size while making sure you don't go outside the image bounds
                    for (int x = xx; (x < xx + blurSize && x < image.Width); x++)
                    {
                        for (int y = yy; (y < yy + blurSize && y < image.Height); y++)
                        {
                            // Get pointer to RGB
                            byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;

                            avgB += data[0]; // Blue
                            avgG += data[1]; // Green
                            avgR += data[2]; // Red

                            blurPixelCount++;
                        }
                    }

                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;

                    // now that we know the average for the blur size, set each pixel to that color
                    for (int x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
                    {
                        for (int y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
                        {
                            // Get pointer to RGB
                            byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;

                            // Change values
                            data[0] = (byte)avgB;
                            data[1] = (byte)avgG;
                            data[2] = (byte)avgR;
                        }
                    }
                }
            }

            // Unlock the bits
            blurred.UnlockBits(blurredData);

            return blurred;
        }

        private static Bitmap DrawRectangleToImage(Bitmap img, 
            List<RectangleModel> rectangles)
        {
            using (Graphics gr = Graphics.FromImage(img))
            {
                foreach (var r in rectangles)
                {
                    Rectangle rect1 = ConvertToRectangle(r);
                    Bitmap croppedImg = ImageCropper.CropImage(img, rect1);
                    if (r.ObjectType == ObjectTypeEnum.Face)
                    {
                        croppedImg = Blur(croppedImg, 6);
                        gr.DrawImage(croppedImg, rect1);
                    }
                    else if (r.ObjectType == ObjectTypeEnum.Licenseplate)
                    {
                        croppedImg = Blur(croppedImg, 8);
                        gr.DrawImage(croppedImg, rect1);
                    }
                }
            }
            return img;
            //img.Save(outputfileName, ImageFormat.Jpeg);
            //Console.WriteLine("OUTPUT file is at " + outputFile);
        }
    }
}
