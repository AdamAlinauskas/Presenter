using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Meetaroo.Services
{
    public static class ControllerExtensions
    {
        public static async Task<User> GetCurrentUser(this Controller controller) {
            var userIdentifier = controller.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var repository = (IUserRepository)controller.HttpContext.RequestServices.GetService(typeof(IUserRepository));
            return await repository.GetByIdentifier(userIdentifier);
        }
    }
}