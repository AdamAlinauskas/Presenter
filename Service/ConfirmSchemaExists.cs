using System.Threading.Tasks;
using DataAccess;

namespace Service{
    
    public interface IConfirmSchemaExists{
        Task<bool> For(string schemaName);
    }
    
    public class ConfirmSchemaExists : IConfirmSchemaExists
    {
        private readonly IOrganizationRepository organizationRepository;

        public ConfirmSchemaExists(IOrganizationRepository organizationRepository)
        {
            this.organizationRepository = organizationRepository;
        }

        public async Task<bool> For(string schemaName){
            if(string.IsNullOrWhiteSpace(schemaName))
                return false;

            return await organizationRepository.DoesSchemaExist(schemaName);
        }
    }
}