using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RPS.Library.API.Utility
{
    public class VehicleDetection
    {
        
        /// <summary>
        /// main entrypoint for Vehicle Plate
        /// </summary>
        public static List<RectangleModel> Start(string imageFilePath)
        {  
            if (!File.Exists(imageFilePath))
            {
                throw new Exception(string.Format("File {0} not found.",imageFilePath));
            }
            else
            {
                return RunFunction(imageFilePath);
            }
        }

        //TODO
        //public static void TestDeserializeJson()
        //{
        //    Console.WriteLine("Start!");
        //    string jsonpath = "C:\\A_SampleFolder\\licenceplatejson.txt";
        //    string jsonString = File.ReadAllText(jsonpath);
        //    //List<ObjectTypeModel> results = (List<ObjectTypeModel>)JsonConvert.DeserializeObject<ObjectTypeModel>(jsonpath);
        //    Console.WriteLine("success!");
        //}

        private static List<RectangleModel> RunFunction(string imageFilePath)
        {            
            string imgBase64String = Common.GetBase64StringForImage(imageFilePath);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("image", imgBase64String);
            string json = JsonConvert.SerializeObject(dict, Formatting.Indented);
            byte[] body = Encoding.UTF8.GetBytes(json);

            return CallApi(imageFilePath);
        }

        private static List<RectangleModel> CallApi(string imageFilePath)
        {
            List<RectangleModel> rectangles = new List<RectangleModel>();

            string imgBase64String = Common.GetBase64StringForImage(imageFilePath);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("image", imgBase64String);
            string json = JsonConvert.SerializeObject(dict, Formatting.Indented);
            byte[] body = Encoding.UTF8.GetBytes(json);
            
            WebRequest request = WebRequest.Create(Common.ApiLicenseURL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = json.Length;
            request.Headers["X-Access-Token"] = Common.ApiKey; // "Your-API-Key";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(body, 0, body.Length);
            requestStream.Close();

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(),
                                      ASCIIEncoding.UTF8);
                json = reader.ReadToEnd();
                reader.Close();
                JObject result = JObject.Parse(json);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    JArray objects = (JArray)result["objects"];
                    rectangles = GetRectangles(objects);
                }
            }
            catch (WebException we)
            {
                StreamReader reader = new StreamReader(we.Response.GetResponseStream(),
                                      ASCIIEncoding.UTF8);
                json = reader.ReadToEnd();
                reader.Close();
                JObject info = JObject.Parse(json);
                Console.WriteLine("REQUEST FAILED\nerror: {0}\nreason: '{1}'",
                  info["error"].Value<string>(),
                  info["reason"].Value<string>(),
                  info["reasonCode"].Value<string>());
            }

            return rectangles;
        }
        
        private static List<RectangleModel> GetRectangles(JArray objects)
        {
            List<RectangleModel> rectangles = new List<RectangleModel>();

            //Loop each children (if more than 1 license plate)
            foreach (JObject obj in objects.Children())
            {
                try
                {
                    //Console.WriteLine("Object :");
                    //string oType = obj["objectType"].Value<string>();
                    ////string type = "something"; 
                    //if (oType != null)
                    //{
                    //    Console.WriteLine(oType);
                    //}
                    JObject licencePlateAnnotation = (JObject)obj["licenseplateAnnotation"];
                    JObject bounding = (JObject)licencePlateAnnotation["bounding"];
                    JArray vertices = (JArray)bounding["vertices"];
                    List<int> xcoord = new List<int>();
                    List<int> ycoord = new List<int>();

                    //loop each coordinate (x,y)
                    foreach (JObject vert in vertices.Children())
                    {
                        string x = vert["x"].Value<string>();
                        string y = vert["y"].Value<string>();
                        xcoord.Add(Convert.ToInt32(x));
                        ycoord.Add(Convert.ToInt32(y));
                    }

                    RectangleModel rect1 = new RectangleModel
                    {
                        ObjectType = ObjectTypeEnum.Licenseplate,
                        x = xcoord.Min(),
                        y = ycoord.Min(),
                        height = ycoord.Max() - ycoord.Min(),
                        width = xcoord.Max() - xcoord.Min()
                    };
                    rectangles.Add(rect1);
                }
                catch (Exception ex)
                {
                    //TODO
                    throw new Exception(ex.Message, ex);
                }
               
            }
            return rectangles;
        }
    }
}
