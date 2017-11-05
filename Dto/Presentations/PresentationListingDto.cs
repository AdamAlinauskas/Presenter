using System.Collections.Generic;

namespace Dto
{
    public class PresentationListingDto
    {
        public IList<DocumentDto> Documents { get; set; } = new List<DocumentDto>();
        public IList<PresentationDto> Presentations { get; set; } = new List<PresentationDto>();
    }
}