using Npgsql;

namespace NoteApp.Services
{
    public class DbService
    {
        private string _connectionString = "Server=localhost;Port=5432;Database=NoteAppDb;User Id=postgres;Password=147258369";
        protected NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
