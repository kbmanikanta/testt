using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.IO;
using Microsoft.Azure;

namespace Lutran.Api
{
    public class CloudStorageHelper
    {
        public static CloudBlobContainer GetCloudBlobContainer(string strBlobContainerName,bool isBlobPublicAccess)
        {
            // CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("URL_BLOB_CONTAINER_LDW"));

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["URL_BLOB_CONTAINER_LDW"].ToString());

            CloudBlobClient blobclient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer blobcontainer = blobclient.GetContainerReference(strBlobContainerName);

            if (blobcontainer.CreateIfNotExists())
            {
                if (isBlobPublicAccess)
                {
                    blobcontainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }
               
            }
            return blobcontainer;
        }

        public static string DownloadBlob(string strBlobContainerName,string strBlobName)
        {
            // CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("URL_BLOB_CONTAINER_LDW"));

            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["URL_BLOB_CONTAINER_LDW"].ToString());

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(strBlobContainerName);

            // Retrieve reference to a blob named "myblob.csv"
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(strBlobName);

            string text;
            using (var memoryStream = new MemoryStream())
            {
                blockBlob.DownloadToStream(memoryStream);
                text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            return text;
        }

        public static int UploadBlob(string strBlobContainerName, string strSourceBlobName, string strDestinationBlobName, Stream msBlobData)
        {
            int intErrorId = 0;
            if (strBlobContainerName != null && strBlobContainerName != "" && strSourceBlobName != null && strSourceBlobName != "" && strDestinationBlobName != null && strDestinationBlobName != "" && msBlobData != null)
            {
                // Retrieve storage account from connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["URL_BLOB_CONTAINER_LDW"].ToString());

                if (storageAccount != null)
                {
                    // Create the blob client.
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                    if (blobClient != null)
                    {
                        // Retrieve reference to a preadmission created container.
                        CloudBlobContainer container = blobClient.GetContainerReference(strBlobContainerName);

                        if (container != null)
                        {
                            container.CreateIfNotExists(); //if not exists,it will create.

                            // Retrieve reference to a blob named "payload.csv"
                            CloudBlockBlob srcblockPayloadBlob = container.GetBlockBlobReference(strSourceBlobName); // Source "payload.csv blob"

                            CloudBlockBlob destblockPayloadBlob = container.GetBlockBlobReference(strDestinationBlobName); // Destination "MMddyyyy.payload.csv blob"

                            if (srcblockPayloadBlob != null && destblockPayloadBlob != null)
                            {
                                if (srcblockPayloadBlob.Exists())
                                {
                                    destblockPayloadBlob.StartCopy(srcblockPayloadBlob);
                                }
                            }

                            using (Stream memoryStream = msBlobData)
                            {
                                if (memoryStream.Length > 0)
                                {
                                    srcblockPayloadBlob.UploadFromStream(memoryStream);
                                }
                            }
                        }
                        else
                        {
                            intErrorId = Convert.ToInt32(Common.SiteConfigIds.BlobContainer);
                        }
                    }
                    else
                    {
                        intErrorId = Convert.ToInt32(Common.SiteConfigIds.BlobClient);
                    }
                }
                else
                {
                    intErrorId = Convert.ToInt32(Common.SiteConfigIds.StorageEnvURL);
                }
            }
            else
            {
                intErrorId = Convert.ToInt32(Common.SiteConfigIds.UplodBlobReqParams);
            }

            if (intErrorId > 0)
            {
                Common.SaveLogsToFile(Common.GetSiteConfigById(Convert.ToInt32(Common.SiteConfigIds.ErrorLogPath)), Common.GetSiteConfigById(intErrorId), string.Empty);
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}