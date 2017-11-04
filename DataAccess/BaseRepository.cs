using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace DataAccess{
    public class BaseRepository{
        protected readonly NpgsqlConnection connection;
        private readonly ICurrentSchema currentSchema;

        public BaseRepository(NpgsqlConnection connection, ICurrentSchema currentSchema)
        {
            this.connection = connection;
            this.currentSchema = currentSchema;
        }
         

         public async Task ConnectAndSetSchema()
        {
            await connection.OpenAsync();

            // Can't parameterize this
            await connection.ExecuteAsync(
                "set search_path = " + currentSchema.Name
            );
        }
    }
}
