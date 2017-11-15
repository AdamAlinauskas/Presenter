using Microsoft.AspNetCore.Authorization;

namespace Meetaroo.Hubs
{
    public class HubAuthorizeAttribute : AuthorizeAttribute
    {
        public HubAuthorizeAttribute()
        {
        }

        public HubAuthorizeAttribute(string policy) : base(policy)
        {
        }
    }
}