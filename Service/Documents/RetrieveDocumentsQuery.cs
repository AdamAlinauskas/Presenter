using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess;
using Dto;

namespace Service{
    
    public interface IRetrieveDocumentsQuery{
        Task<DocumentListingDto> Fetch();
    }

    public class RetrieveDocumentsQuery : IRetrieveDocumentsQuery{
        private readonly IDocumentRepository documentRepository;

        public RetrieveDocumentsQuery(IDocumentRepository documentRepository)
        {
            this.documentRepository = documentRepository;
        }

        public async Task<DocumentListingDto> Fetch(){
            var documents = await documentRepository.All();
            return new DocumentListingDto{Documents = documents};
        }
    }
}