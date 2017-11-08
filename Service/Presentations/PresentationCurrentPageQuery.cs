using System.Threading.Tasks;
using DataAccess;

namespace Service
{
    public interface IPresentationCurrentPageQuery
    {
        Task<int> Fetch(long presentationId);
    }

    public class PresentationCurrentPageQuery : IPresentationCurrentPageQuery
    {
        private readonly IPresentationRepository presentationRepository;

        public PresentationCurrentPageQuery(IPresentationRepository presentationRepository)
        {
            this.presentationRepository = presentationRepository;
        }

        public async Task<int> Fetch(long presentationId)
        {
            var page = await presentationRepository.GetCurrentPageNumber(presentationId);
            return page;
        }
    }
}