using System.Threading.Tasks;
using DataAccess;
using Domain;

namespace Service
{
    public interface IUpdatePresentationStatusCommand
    {
        Task Execute(long presentationId, PresentationStatus status);
    }
    
    public class UpdatePresentationStatusCommand : IUpdatePresentationStatusCommand
    {
        private readonly IPresentationRepository presentationRepository;

        public UpdatePresentationStatusCommand(IPresentationRepository presentationRepository)
        {
            this.presentationRepository = presentationRepository;
        }

        public async Task Execute(long presentationId, PresentationStatus status)
        {
            await presentationRepository.UpdateStatus(presentationId, status);
        }
    }
}