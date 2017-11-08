using System;
using System.Threading.Tasks;
using DataAccess;

namespace Service
{
    public interface IUpdatePresentationCurrentPage
    {
        Task Execute(long presentationId, int pageNumber);
    }

    public class UpdatePresentationCurrentPage : IUpdatePresentationCurrentPage
    {
        private readonly IPresentationRepository presentationRepository;

        public UpdatePresentationCurrentPage(IPresentationRepository presentationRepository)
        {
            this.presentationRepository = presentationRepository;
        }

        public async Task Execute(long presentationId, int pageNumber)
        {
            await presentationRepository.UpdatePageNumber(presentationId, pageNumber);
        }
    }
}