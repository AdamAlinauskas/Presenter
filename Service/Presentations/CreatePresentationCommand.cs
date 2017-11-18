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
        private readonly IConversationRepository conversations;

        public CreatePresentationCommand(
            IPresentationRepository presentationRepository,
            IConversationRepository conversations
        )
        {
            this.presentationRepository = presentationRepository;
            this.conversations = conversations;
        }

        public async Task Execute(PresentationDto dto)
        {
            var conversationId = await conversations.CreateConversation(dto.Name, dto.CreatedBy);
            dto.ConversationId = conversationId;

            await presentationRepository.Create(dto);
        }
    }
}