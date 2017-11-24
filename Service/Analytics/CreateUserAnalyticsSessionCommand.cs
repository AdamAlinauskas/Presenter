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
        private readonly IRetrieveLocation retrieveLocation;

        public CreateUserAnalyticsSessionCommand(IUserAnalyticsSessionRepository userAnalyticsSessionRepository, IRetrieveLocation retrieveLocation)
        {
            this.userAnalyticsSessionRepository = userAnalyticsSessionRepository;
            this.retrieveLocation = retrieveLocation;
        }

        public async Task<long> Execute(TrackRequestDto dto)
        {
            dto.Location = await retrieveLocation.Fetch(dto);
            return await userAnalyticsSessionRepository.CreateForEitherDocumentOrPresentation(dto);
        }
    }
}