using Lutran.Api.Models;
using Lutran.Api.Repository;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Lutran.Api.Controllers
{
    public class PreadmissionsDataController : ApiController
    {
        private PreadmissionsDataRepository _repo;

        public PreadmissionsDataController()
        {
            Initialization = InitializeAsync();
        }

        public Task Initialization { get; private set; }

        private async Task InitializeAsync()
        {
            _repo = new PreadmissionsDataRepository();
            await _repo.Initialization;
        }

        public async Task<IHttpActionResult> Post([FromBody]string request)
        {
            await Initialization;

            string strresponse = string.Empty;

            string errorresponse = string.Empty;

            int interrorid = 0;

            List<string> dictErrorList = null;


            if (request == null || request.ToString() == "" || request.ToString() == string.Empty)
            {
                return BadRequest(Common.GetSiteConfigById(Convert.ToInt32(Common.SiteConfigIds.RequiredParameter)));
            }
            else
            {
                //Input Payload Fields Validation Process.

                #region Validation Process

                request = request.Replace("\r",string.Empty);

                //Check if each line of input payload Exists
                //if (request.Contains("\n"))
                //{
                    string[] strarryReqPayload = request.Split('\n');
                    if (strarryReqPayload == null && strarryReqPayload.Length <= 0)
                    {
                        return BadRequest(Common.GetSiteConfigById(Convert.ToInt32(Common.SiteConfigIds.RequiredParameter)));
                    }
                    else
                    {                   
                        dictErrorList = new List<string>();
                        int intCurrentIndex = 0;
                        foreach (string strReqPayload in strarryReqPayload)
                        {
                            intCurrentIndex += 1;

                            //Check Payload: 5 fields separated by ^ character.

                            if (strReqPayload.Contains("^"))
                            {
                                string[] strarryallReqParam = strReqPayload.Split('^');

                                // Check if each line of input payload has 5 fields

                                if (strarryallReqParam != null && strarryallReqParam.Length != 5)
                                {
                                    dictErrorList.Add(intCurrentIndex + " Line," + Common.GetSiteConfigById(Convert.ToInt32(Common.SiteConfigIds.RequiredParameter)));
                                }
                            }
                            else
                            {
                                //BADREQUEST StatusCode - if new line character(\n) not exist in request payload.
                                return BadRequest(Common.GetSiteConfigById(Convert.ToInt32(Common.SiteConfigIds.RequiredParameter)));
                            }
                        }
                        if (dictErrorList != null && dictErrorList.Count > 0)
                        {

                            //BADREQUEST StatusCode - If Request Object Validation Fails. 

                            return BadRequest(dictErrorList);
                        }
                        else
                        {

                            // If Request Object Validation Success.  

                            #region PAYLOAD UPLOAD TO AZURE BLOB

                            //Parameters Passing to UploadBlob Method : (BlobContainer,Soruce Blob Name, Destination Blob Name,Blob Stream Data .
                            using (Stream StreamBlobData = Common.GenerateStreamFromString(request))
                            {
                                interrorid = CloudStorageHelper.UploadBlob(Common.GetSiteConfigById(Convert.ToInt32(Common.SiteConfigIds.PreadmissionData)), Common.GetSiteConfigById(Convert.ToInt32(Common.SiteConfigIds.PayLoadBlob)), Common.DateTimeNowWithFormat("MMddyyyyHHmm") + "." + Common.GetSiteConfigById(Convert.ToInt32(Common.SiteConfigIds.PayLoadBlob)), StreamBlobData);
                            };
                            if (interrorid > 0)
                            {
                                //BADREQUEST StatusCode - if Payload Uploads to Azure Blob Fails.
                                return BadRequest(Common.GetSiteConfigById(interrorid));

                            }
                            else
                            {
                                //CREATED StatusCode - if Payload Uploads to Azure Blob Success.
                                return Created("Success", (Common.GetSiteConfigById(interrorid)));
                            }

                            #endregion
                        }
                    }
                  
                //}
                //else
                //{
                //    //BADREQUEST StatusCode - if new line character(\n) not exist in request payload.
                //    return BadRequest(Common.GetSiteConfigById(Convert.ToInt32(Common.SiteConfigIds.RequiredParameter)));
                //}

                #endregion
            }
        }

        private IHttpActionResult BadRequest<T>(T value)
        {
            // Get default content negotiator and negotiate type.  
            var defaultNegotiator = Configuration.Services.GetContentNegotiator();
            var negotationResult = defaultNegotiator.Negotiate(typeof(T), Request, Configuration.Formatters);
            // Create a 400 response message with negotiated content.  
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new ObjectContent<T>(value, negotationResult.Formatter, negotationResult.MediaType)
            };
            return ResponseMessage(response);
        }
    }
}