using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace Lutran.Api
{
    public class Common
    {
        public enum SiteConfigIds
        {
            BlobSuccess = 0,
            BlobFailed = 1,
            RequiredParameter = 2,
            StorageEnvURL = 3,
            BlobClient = 4,
            BlobContainer = 5,
            BlobReference = 6,
            PreadmissionData = 7,
            AdmissionData = 8,
            LeadData = 9,
            PayLoadBlob = 10,
            ErrorLogPath = 11,
            UplodBlobReqParams = 12,
        }
        public static Dictionary<int, string> dictSiteConfigs
        {
            get
            {
                Dictionary<int, string> dictSiteConfigs = new Dictionary<int, string>();
                dictSiteConfigs.Add(0, "Success.");
                dictSiteConfigs.Add(1, "Failed.");
                dictSiteConfigs.Add(2, "Required fields unavailable.");
                dictSiteConfigs.Add(3, "Storagename/Environment Url null.");
                dictSiteConfigs.Add(4, "Blob Client null.");
                dictSiteConfigs.Add(5, "Blob Contianer null.");
                dictSiteConfigs.Add(6, "Blob Refernce null.");
                dictSiteConfigs.Add(7, "preadmissiondata");
                dictSiteConfigs.Add(8, "leaddata");
                dictSiteConfigs.Add(9, "admissiondata");
                dictSiteConfigs.Add(10, "payload.csv");
                dictSiteConfigs.Add(11, "~/Logs/");
                dictSiteConfigs.Add(12, "Upload Blob Method Required Parameters null.");
                return dictSiteConfigs;
            }
        }

        public static string GetSiteConfigById(int SiteConfigId)
        {
            return dictSiteConfigs[SiteConfigId];
        }

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }



        public static string DateTimeNowWithFormat(string strFormatDate)
        {
            string strDateTimeNow = DateTime.Now.ToString(strFormatDate);
            return strDateTimeNow;
        }

        public static void SaveLogsToFile(string strPageUrl, string sLogMessage, string sSource)
        {
            string sFilePath = HttpContext.Current.Server.MapPath(strPageUrl);
            string strProjectName = sSource;
            strProjectName = strProjectName.Trim().Replace(" ", "_").Replace("__", "_");
            if (!(System.IO.Directory.Exists(sFilePath)))
            {
                System.IO.Directory.CreateDirectory(sFilePath);
            }
            sFilePath += "\\" + DateTime.Now.ToString("MMddyyyy") + ".json";
            if (!(System.IO.File.Exists(sFilePath)))
            {
                System.IO.FileStream f = System.IO.File.Create(sFilePath);
                f.Close();
            }

            //set up a streamwriter for adding text

            using (System.IO.StreamWriter strWrite = System.IO.File.AppendText(sFilePath))
            {
                //write my text 

                strWrite.Write("Log Entry : ");
                strWrite.WriteLine("{0}{1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString() + ":");
                strWrite.WriteLine("Message: {0}", sLogMessage);
                strWrite.WriteLine("---------------------------------------------------------------------------");
                strWrite.Flush();
                strWrite.Close();
            }
        }
    }
}