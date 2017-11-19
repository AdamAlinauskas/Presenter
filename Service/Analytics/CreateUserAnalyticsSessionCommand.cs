using System.Threading.Tasks;
using DataAccess;
using Dto;

namespace Service
{
    public interface ICreateUserAnalyticsSessionCommand
    {
        Task<long> Execute(TrackRequestDto dto);
    }

    public class CreateUserAnalyticsSessionCommand : ICreateUserAnalyticsSessionCommand
    {
        private readonly IUserAnalyticsSessionRepository userAnalyticsSessionRepository;

        public CreateUserAnalyticsSessionCommand(IUserAnalyticsSessionRepository userAnalyticsSessionRepository)
        {
            this.userAnalyticsSessionRepository = userAnalyticsSessionRepository;
        }

        public async Task<long> Execute(TrackRequestDto dto)
        {
            return await userAnalyticsSessionRepository.CreateForEitherDocumentOrPresentation(dto);
        }
    }
}