using System.Collections.Generic;

namespace Dto{
    public class DocumentListingDto
    {
        public IList<DocumentDto> Documents {get;set;} = new List<DocumentDto>();
    }
}