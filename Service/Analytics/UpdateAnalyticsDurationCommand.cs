using System.Threading.Tasks;
using Dto;
using DataAccess;

namespace Service
{
    public interface IUpdateAnalyticsDurationCommand
    {
        Task Execute(UpdateAnalyticsDurationRequestDto dto);
    }

    public class UpdateAnalyticsDurationCommand : IUpdateAnalyticsDurationCommand
    {
        private readonly IUserAnalyticsSessionRepository userAnalyticsSessionRepository;

        public UpdateAnalyticsDurationCommand(IUserAnalyticsSessionRepository userAnalyticsSessionRepository)
        {
            this.userAnalyticsSessionRepository = userAnalyticsSessionRepository;
        }

        public async Task Execute(UpdateAnalyticsDurationRequestDto dto)
        {
            await userAnalyticsSessionRepository.UpdateDuration(dto.AnalyticsId, dto.Duration);
        }
    }
}