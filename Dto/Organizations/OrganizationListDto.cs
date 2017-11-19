using System.Collections.Generic;

namespace Dto.Organizations
{
    public class OrganizationListDto
    {
        public IEnumerable<OrganizationSummaryDto> Organizations { get; set; }
        public string Host { get; set; }
    }
}