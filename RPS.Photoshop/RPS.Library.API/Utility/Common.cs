using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPS.Library.API.Utility
{
    public class Common
    {
        //properties
        #region Properties
        public static string ApiKey
        {
            get
            {
                string _apiKey = "leFN2Zyom3aeLctlXsphLZDqbvCffYcRCXEN";
                return _apiKey;
            }
        }

        public static string ApiLicenseURL
        {
            get
            {
                string urlLicence = "https://dev.sighthoundapi.com/v1/recognition?objectType=licenseplate";
                return urlLicence;
            }
        }

        public static string ApiFaceDetectURL
        {
            get
            {
                //string urlFaceDetect = "https://dev.sighthoundapi.com/v1/detections?type=face,person&faceOption=landmark,gender";
                string urlFaceDetect = "https://dev.sighthoundapi.com/v1/detections?type=face";
                return urlFaceDetect;
            }
        }
        #endregion

        #region methods
        public static string GetBase64StringForImage(string imgPath)
        {
            byte[] imageBytes = System.IO.File.ReadAllBytes(imgPath);
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }
        #endregion
    }
}
