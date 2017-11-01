using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace DataAccess{
    public class BaseRepository{
        private readonly NpgsqlConnection connection;
        private readonly ICurrentSchema currentSchema;

        public BaseRepository(NpgsqlConnection connection, ICurrentSchema currentSchema)
         {
            this.connection = connection;
            this.currentSchema = currentSchema;
        }
         
         private async Task Connect()
        {
            await connection.OpenAsync();

            // Can't parameterize this
            await connection.ExecuteAsync(
                "set search_path = " + currentSchema.Name
            );
        }

        // private async Task<int> GetUserIdAsync()
        // {
        //     var userIdentifier = User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
        //     var user = await connection.QueryFirstAsync<dynamic>(
        //         "SELECT id FROM meetaroo_shared.users WHERE identifier = @identifier",
        //         new { identifier = userIdentifier }
        //     );
        //     return (int) user.id;
        // }
    }
}
