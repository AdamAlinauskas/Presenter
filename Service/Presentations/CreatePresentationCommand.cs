using System.Threading.Tasks;
using DataAccess;
using Dto;

namespace Service
{
    public interface ICreatePresentationCommand
    {
        Task Execute(PresentationDto dto);
    }
    public class CreatePresentationCommand : ICreatePresentationCommand
    {
        private readonly IPresentationRepository presentationRepository;

        public CreatePresentationCommand(IPresentationRepository presentationRepository)
        {
            this.presentationRepository = presentationRepository;
        }

        public async Task Execute(PresentationDto dto)
        {
            await presentationRepository.Create(dto);
        }
    }
}