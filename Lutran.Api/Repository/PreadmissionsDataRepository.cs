using Lutran.Api.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Lutran.Api.Repository
{
    public class PreadmissionsDataRepository : DocumentDb
    {
        public PreadmissionsDataRepository() : base("ramub", "ddl_ldw") { }

        public Task<List<PreadmissionsData>> GetPreadmissionsDataAsync()
        {
            return Task<List<PreadmissionsData>>.Run(() =>
                Client.CreateDocumentQuery<PreadmissionsData>(Collection.DocumentsLink)
                .ToList());
        }
        
        public Task<PreadmissionsData> GetPreadmissionsDataAsync(string id)
        {
            return Task<PreadmissionsData>.Run(() =>
                Client.CreateDocumentQuery<PreadmissionsData>(Collection.DocumentsLink)
                .Where(p => p.Id == id)
                .AsEnumerable()
                .FirstOrDefault());
        }

        public Task<ResourceResponse<Document>> CreatePreadmissionsDataAsync(dynamic PreadmissionsData)
        {
            return Client.CreateDocumentAsync(Collection.DocumentsLink, PreadmissionsData);
        }

        public Task<ResourceResponse<Document>> UpdatePreadmissionsDataAsync(PreadmissionsData PreadmissionsData)
        {
            var doc = Client.CreateDocumentQuery<Document>(Collection.DocumentsLink)
                .Where(d => d.Id == PreadmissionsData.Id)
                .AsEnumerable() 
                .FirstOrDefault();

            return Client.ReplaceDocumentAsync(doc.SelfLink, PreadmissionsData);
        }

        public Task<ResourceResponse<Document>> DeletePreadmissionsDataAsync(string id)
        {
            var doc = Client.CreateDocumentQuery<Document>(Collection.DocumentsLink)
                .Where(d => d.Id == id)
                .AsEnumerable()
                .FirstOrDefault();

            return Client.DeleteDocumentAsync(doc.SelfLink);
        }

    }
}