using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPS.Models
{
    public class ObjectsListModel
    {
        List<ObjectsModel> objects { get; set; }
    }

    public class ObjectsModel
    {
        public string objectType { get; set; }
        //public LicensePlateModel licenseplateAnnotation { get; set; }
    }

    public class LicensePlateModel
    {
        public BoundingModel bounding { get; set; }
    }

    public class BoundingModel
    {
        public VerticesModel vertices { get; set; }
    }

    public class PlateBoxModel
    {
        public PlateBoxModel()
        {
            vertices = new List<VerticesModel>();
        }
        public List<VerticesModel> vertices { get; set; }
    }


    public class VerticesModel
    {
        public int x { get; set; }
        public int y { get; set; }
    }
}
