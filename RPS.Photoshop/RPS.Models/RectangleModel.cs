using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPS.Models
{
    public enum ObjectTypeEnum
    {
        Unknown = 0,
        Licenseplate = 1,
        Face = 2
    }

    public class RectangleModel
    {
        public RectangleModel()
        {
            ObjectType = ObjectTypeEnum.Unknown;
            x = 0;
            y = 0;
            height = 0;
            width = 0;
        }

        public ObjectTypeEnum ObjectType { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }
}
