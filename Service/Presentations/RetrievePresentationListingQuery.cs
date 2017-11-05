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
        private readonly IPresentationRepository presentationRepository;

        public RetrievePresentationListingQuery(IDocumentRepository documentRepository, IPresentationRepository presentationRepository)
        {
            this.documentRepository = documentRepository;
            this.presentationRepository = presentationRepository;
        }

        public async Task<PresentationListingDto> Fetch()
        {
            var documents = await documentRepository.All();
            var presentations = await presentationRepository.All();
            return new PresentationListingDto { Documents = documents, Presentations = presentations };
        }
    }
}