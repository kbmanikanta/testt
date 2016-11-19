using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lutran.Api.Models
{
    public class PreadmissionsData
    {
        [JsonProperty(PropertyName = "id")] // <-- need to add this mapping to avoid issues
        public string Id { get; set; }
        public bool preadmission_client_signed_agreement { get; set; }
        public DateTime preadmission_client_agreement_date { get; set; }
        public bool preadmission_sponsor_signed_agreement { get; set; }
        public DateTime preadmission_sponsor_agreement_date { get; set; }
       
    }
}