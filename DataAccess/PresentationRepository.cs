using Npgsql;

namespace DataAccess
{
    public interface IPresentationRepository
    {

    }
    public class PresentationRepository : BaseRepository, IPresentationRepository
    {
        private readonly NpgsqlConnection connection;
        private readonly ICurrentSchema currentSchema;

        public PresentationRepository(NpgsqlConnection connection, ICurrentSchema currentSchema): base(connection, currentSchema)
        {
            this.connection = connection;
            this.currentSchema = currentSchema;
        }
    }
}