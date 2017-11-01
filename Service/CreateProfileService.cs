using System;
using System.Threading.Tasks;
using DataAccess;
using Domain;

namespace Service {
    public interface ICreateProfileService {
        Task EnsureExists(User userProfile);
    }

    public class CreateProfileService : ICreateProfileService
    {
        private readonly IUserRepository users;

        public CreateProfileService(IUserRepository users) {
            this.users = users;
        }

        public async Task EnsureExists(User userProfile)
        {
            if (userProfile.Identifier == null)
                throw new Exception("User is authenticated but has no identifier");

            var profileExists = await users.DoesUserExist(userProfile.Identifier);
            if (!profileExists) {
                await users.CreateUser(userProfile);
            }

            return;
        }
    }
}