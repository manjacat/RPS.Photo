using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace RPS.Library.API.Utility
{
    public class ImageCropper
    {
        public static Bitmap CropImage(Image source, Rectangle rect)
        {
            var bmp = new Bitmap(rect.Width, rect.Height);
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(source, new Rectangle(0, 0, bmp.Width, bmp.Height), rect, GraphicsUnit.Pixel);
            }
            return bmp;
        }
    }
}
