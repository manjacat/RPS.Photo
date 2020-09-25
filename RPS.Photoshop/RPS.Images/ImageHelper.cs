using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPS.Models;

namespace RPS.Images
{
    public class ImageHelper
    {
        // obsolete class. only used during early phase for testing
        public static string rootfolder = "C:\\A_SampleFolder\\";
        public static string inputFileName = "trafficjam_before.jpg";
        public static string outputFileName = "trafficjam_after.jpg";


        public static void TestMasking()
        {
            Image imageBackground = Image.FromFile(rootfolder + inputFileName);
            Image imageOverlay = Image.FromFile(rootfolder + "reddot.png");
            //Image imageOverlay = Image.FromFile(rootfolder + "catphoto3.jpg");

            //         "vertices": [{
            //	"x": 585,
            //	"y": 397

            //         }, {
            //	"x": 627,
            //	"y": 397
            //}, {
            //	"x": 627,
            //	"y": 414
            //}, {
            //	"x": 585,
            //	"y": 414
            //}]
            PlateBoxModel plateBox1 = new PlateBoxModel();
            plateBox1.vertices.Add(new VerticesModel { x = 585, y = 397 });
            plateBox1.vertices.Add(new VerticesModel { x = 627, y = 397 });
            plateBox1.vertices.Add(new VerticesModel { x = 627, y = 414 });
            plateBox1.vertices.Add(new VerticesModel { x = 585, y = 414 });

            //"vertices": [
            //{
            //  "x": 291,
            //  "y": 447
            //},
            //{
            //  "x": 338,
            //  "y": 447
            //},
            //{
            //  "x": 338,
            //  "y": 464
            //},
            //{
            //  "x": 291,
            //  "y": 464
            //}
            //]

            PlateBoxModel plateBox2 = new PlateBoxModel();
            plateBox2.vertices.Add(new VerticesModel { x = 291, y = 447 });
            plateBox2.vertices.Add(new VerticesModel { x = 338, y = 447 });
            plateBox2.vertices.Add(new VerticesModel { x = 338, y = 464 });
            plateBox2.vertices.Add(new VerticesModel { x = 291, y = 464 });

            PlateBoxModel plateBox3 = new PlateBoxModel();
            plateBox3.vertices.Add(new VerticesModel { x = 540, y = 422 });
            plateBox3.vertices.Add(new VerticesModel { x = 540, y = 532 });
            plateBox3.vertices.Add(new VerticesModel { x = 630, y = 422 });
            plateBox3.vertices.Add(new VerticesModel { x = 630, y = 532 });

            List<PlateBoxModel> plateBoxes = new List<PlateBoxModel>();
            plateBoxes.Add(plateBox1);
            plateBoxes.Add(plateBox2);
            //plateBoxes.Add(plateBox3);
            RunMasking(rootfolder, imageBackground, imageOverlay, plateBoxes);
        }

        private static void RunMasking(string rootfolder,
            Image imageBackground, Image imageOverlay, List<PlateBoxModel> plateBoxes)
        {
            //Create background image
            //Bitmap(imageBackground.Width, imageBackground.Height);
            Image img = new Bitmap(imageBackground);

            //get scale
            int scale = imageBackground.Width / imageOverlay.Width;

            Console.WriteLine(string.Format("Width is {0}, height is {1}",
                imageBackground.Width, imageBackground.Height));

            //create Rectangles from xy
            List<Rectangle> rects = GetRectangles(plateBoxes);

            //draw rectangle to image
            DrawRectangleToImage(img, rects, rootfolder);
        }


        private static List<Rectangle> GetRectangles(List<PlateBoxModel> plateBoxes)
        {
            List<Rectangle> rects = new List<Rectangle>();
            foreach (PlateBoxModel p in plateBoxes)
            {
                int xSize = p.vertices.Select(s => s.x).Max() - p.vertices.Select(s => s.x).Min();
                int ySize = p.vertices.Select(s => s.y).Max() - p.vertices.Select(s => s.y).Min();
                Rectangle rect1 = new Rectangle
                    (p.vertices.Select(s => s.x).Min(),
                    p.vertices.Select(s => s.y).Min(),
                    xSize, ySize);

                rects.Add(rect1);
            }
            return rects;
        }

        private static void DrawRectangleToImage(Image img, List<Rectangle> rects, string rootfolder)
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
                foreach (var rect1 in rects)
                {
                    gr.FillRectangle(blueBrush, rect1);
                }


            }
            string outputFile = rootfolder + outputFileName;
            img.Save(outputFile, ImageFormat.Jpeg);
            //Console.WriteLine("OUTPUT file is at " + outputFile);
        }
    }
}
