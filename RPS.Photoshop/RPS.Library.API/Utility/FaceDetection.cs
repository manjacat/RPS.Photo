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
using System.Drawing;

namespace RPS.Library.API.Utility
{
    public class FaceDetection
    {
        // this is the class used to call the License Plate API
        // read the json output
        // and convert them to List of rectangles 

        /// <summary>
        /// main entrypoint for Vehicle Plate
        /// </summary>
        public static List<RectangleModel> Start(Bitmap imageFilePath)
        {
                return CallApi(imageFilePath);
        }

        private static List<RectangleModel> CallApi(Bitmap imageFilePath)
        {
            List<RectangleModel> rectangles = new List<RectangleModel>();
            string imgBase64String = Common.GetBase64StringForImage(imageFilePath);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("image", imgBase64String);
            string json = JsonConvert.SerializeObject(dict, Formatting.Indented);
            byte[] body = Encoding.UTF8.GetBytes(json);

            WebRequest request = WebRequest.Create(Common.ApiFaceDetectURL);
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

            //Loop each children (if more than 1 face)
            foreach (JObject obj in objects.Children())
            {
                try
                {
                    string type = obj["type"].Value<string>();
                    if(type.ToLower() == "face")
                    {
                        JObject bbox = (JObject)obj["boundingBox"];
                        string width = bbox["width"].Value<string>();
                        string height = bbox["height"].Value<string>();
                        string xStr = bbox["x"].Value<string>();
                        string yStr = bbox["y"].Value<string>();

                        RectangleModel rect1 = new RectangleModel
                        {
                            ObjectType = ObjectTypeEnum.Face,
                            x = Convert.ToInt32(xStr),
                            y = Convert.ToInt32(yStr),
                            height = Convert.ToInt32(height),
                            width = Convert.ToInt32(width)
                        };
                        rectangles.Add(rect1);
                    }
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
