using System.Threading.Tasks;
using DataAccess;
using Dto.Organizations;

namespace Service
{
    public interface IRetrieveOrganizationsQuery
    {
        Task<OrganizationListDto> Fetch();
    }

    public class RetrieveOrganizationsQuery : IRetrieveOrganizationsQuery
    {
        IOrganizationRepository organizations;
        ICurrentSchema schema;

        public RetrieveOrganizationsQuery(IOrganizationRepository organizations, ICurrentSchema schema)
        {
            this.organizations = organizations;
            this.schema = schema;
        }

        public async Task<OrganizationListDto> Fetch()
        {
            return new OrganizationListDto
            {
                Organizations = await organizations.FetchAll(),
                Host = schema.Host
            };
        }
    }
}