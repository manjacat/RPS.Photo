using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace RPS.Library.API.Utility
{
    public class ImageUtils
    {
        public static List<Bitmap> CutImage(Image img)
        {
            Bitmap bmp = new Bitmap(img);
            Rectangle rect1 = new Rectangle(0, 0, bmp.Width / 3, bmp.Height);
            Rectangle rect2 = new Rectangle(rect1.Width, 0, bmp.Width / 3, bmp.Height);
            Rectangle rect3 = new Rectangle(rect2.Width * 2, 0, bmp.Width / 3, bmp.Height);
            Bitmap bmp1 = new Bitmap(rect1.Width, rect1.Height);
            Bitmap bmp2 = new Bitmap(rect2.Width, rect2.Height);
            Bitmap bmp3 = new Bitmap(rect3.Width, rect3.Height);
            using (var gr = Graphics.FromImage(bmp1))
            {
                gr.DrawImage(img, new Rectangle(0, 0, bmp1.Width, bmp1.Height), rect1, GraphicsUnit.Pixel);
            }
            using (var gr = Graphics.FromImage(bmp2))
            {
                gr.DrawImage(img, new Rectangle(0, 0, bmp2.Width, bmp2.Height), rect2, GraphicsUnit.Pixel);
            }
            using (var gr = Graphics.FromImage(bmp3))
            {
                gr.DrawImage(img, new Rectangle(0, 0, bmp3.Width, bmp3.Height), rect3, GraphicsUnit.Pixel);
            }
            List<Bitmap> arr = new List<Bitmap>();
            arr.Add(bmp1);
            arr.Add(bmp2);
            arr.Add(bmp3);
            return arr;
        }

        public static void CombineImgAndSave(List<Bitmap> img, string outputfolder, string fileName)
        {
            Bitmap canvas = new Bitmap(img[0].Width * img.Count, img[0].Height);
            for (int j = 0; j < img.Count; j++)
            {
                using (var gr = Graphics.FromImage(canvas))
                {
                    if (j == 0)
                    {
                        gr.DrawImage(img[j], new Rectangle(0, 0, img[j].Width, img[j].Height));
                    }
                    else
                    {
                        gr.DrawImage(img[j], new Rectangle(img[j].Width * j, 0, img[j].Width, img[j].Height));
                    }
                }
            }
            canvas.Save(outputfolder + fileName, ImageFormat.Jpeg);
            canvas.Dispose();
        }
    }
}
