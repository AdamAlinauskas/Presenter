using System.Threading.Tasks;
using DataAccess;
using Dto;

namespace Service
{
    public interface IRetrievePresentationListingQuery
    {
        Task<PresentationListingDto> Fetch();
    }
    public class RetrievePresentationListingQuery : IRetrievePresentationListingQuery
    {
        private readonly IDocumentRepository documentRepository;

        public RetrievePresentationListingQuery(IDocumentRepository documentRepository)
        {
            this.documentRepository = documentRepository;
        }

        public async Task<PresentationListingDto> Fetch()
        {
            var documents = await documentRepository.All();
            return new PresentationListingDto { Documents = documents };
        }
    }
}