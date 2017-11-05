using DataAccess;

namespace Service
{
    public interface ICreatePresentationCommand
    {

    }
    public class CreatePresentationCommand : ICreatePresentationCommand
    {
        private readonly IPresentationRepository presentationRepository;

        public CreatePresentationCommand(IPresentationRepository presentationRepository)
        {
            this.presentationRepository = presentationRepository;
        }
    }
}