using RPS.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPS.Images
{
    
    public class ImageProcessor
    {
        //public static string outputFileName = "trafficjam_008.jpg";

        public static void Start(
            string inputFileName,
            List<RectangleModel> rectangles,
            string outputFolder)
        {
            //Create background image
            //Bitmap(imageBackground.Width, imageBackground.Height);
            Image img = Image.FromFile(inputFileName);
            //Image img = new Bitmap(imageBackground);

            //draw rectangle to image
            DrawRectangleToImage(img, rectangles, outputFolder);
        }

        private static List<Rectangle> ConvertToRectangle(List<RectangleModel> rectangles)
        {
            List<Rectangle> rects = new List<Rectangle>();
            foreach (RectangleModel p in rectangles)
            {
                Rectangle rect1 = new Rectangle(p.x, p.y, p.width, p.height);
                rects.Add(rect1);
            }
            return rects;
        }

        private static Rectangle ConvertToRectangle(RectangleModel p)
        {
            Rectangle rect1 = new Rectangle(p.x, p.y, p.width, p.height);
            return rect1;
        }

        private static void DrawRectangleToImage(Image img, 
            //List<Rectangle> rects,
            List<RectangleModel> rectangles,
            string outputfileName)
        {
            using (Graphics gr = Graphics.FromImage(img))
            {
                //Create a red pen
                //Pen redpen = new Pen(Color.Red, 4);
                //gr.DrawRectangles(redpen, rects.ToArray());

                //create a solid brush
                SolidBrush blueBrush = new SolidBrush(Color.Blue);
                SolidBrush redBrush = new SolidBrush(Color.Red);
                SolidBrush blackbrush = new SolidBrush(Color.Black);
                Brush brush1 = new SolidBrush(Color.FromArgb(230, Color.Black));
                //gr.FillRectangles(brush1, rects.ToArray());
                foreach (var r in rectangles)
                {
                    Rectangle rect1 = ConvertToRectangle(r);
                    if (r.ObjectType == ObjectTypeEnum.Face)
                    {   
                        gr.FillEllipse(redBrush, rect1);
                    }
                    else if(r.ObjectType == ObjectTypeEnum.Licenseplate)
                    {
                        gr.FillRectangle(redBrush, rect1);
                    }
                }
            }
            img.Save(outputfileName, ImageFormat.Jpeg);
            //Console.WriteLine("OUTPUT file is at " + outputFile);
        }
    }
}
