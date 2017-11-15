using System.Threading.Tasks;
using DataAccess;
using Dto;

namespace Service
{
    public interface IRetrievePresentationToViewQuery
    {
        Task<ViewPresentationDto> Fetch(long presentationId);
    }

    public class RetrievePresentationToViewQuery : IRetrievePresentationToViewQuery
    {
        private readonly IPresentationRepository presentationRepository;
        private readonly ICurrentSchema currentSchema;
        private readonly IRetrieveDocumentUrlQuery retrieveDocumentUrlQuery;

        public RetrievePresentationToViewQuery(IPresentationRepository presentationRepository, ICurrentSchema currentSchema, IRetrieveDocumentUrlQuery retrieveDocumentUrlQuery)
        {
            this.presentationRepository = presentationRepository;
            this.currentSchema = currentSchema;
            this.retrieveDocumentUrlQuery = retrieveDocumentUrlQuery;
        }
        public async Task<ViewPresentationDto> Fetch(long presentationId)
        {
            var presentation = await presentationRepository.Get(presentationId);
            var url = retrieveDocumentUrlQuery.GetDocumentUrlFromAwsKey(presentation.awsKey);

            var dto = new ViewPresentationDto
            {
                PresentationId = presentationId,
                Schema = currentSchema.Name,
                Url = url,
                PresentationName = presentation.Name,
                ConversationId = presentation.ConversationId
            };
            return dto;
        }
    }
}