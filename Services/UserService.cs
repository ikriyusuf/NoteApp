using NoteApp.Security;
using Npgsql;
using System;

namespace NoteApp.Services
{
    public class UserService : DbService
    {
        public UserService() : base()
        {
        }

        public bool AddUser(string userName, string password)
        {
            var conn = GetConnection();
            conn.Open();
            var datetimeNow = DateTime.Now;

            string hashedPassword = PasswordHelper.HashPassword(password);

            string query = "INSERT INTO users (Username, PasswordHash, CreatedAt) VALUES (@Username, @PasswordHash, @CreatedAt)";
            var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("Username", userName);
            cmd.Parameters.AddWithValue("PasswordHash", hashedPassword);
            cmd.Parameters.AddWithValue("CreatedAt", datetimeNow);


            int result = cmd.ExecuteNonQuery();
            return result > 0;
        }

        public bool UserLogin(string userName, string password)
        {
            var conn = GetConnection();
            conn.Open();
            string query = "SELECT PasswordHash FROM users WHERE Username = @Username";
            var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("Username", userName);
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string storedHash = reader.GetString(0);
                return PasswordHelper.VerifyPassword(password, storedHash);
            }
            return false;

        }

        public bool IsUsernameTaken(string username)
        {
            const string query = "SELECT 1 FROM users WHERE Username = @Username LIMIT 1";

            var conn = GetConnection();
            conn.Open();

             var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("Username", username);

            var result = cmd.ExecuteScalar();

            return result != null;
        }
    }
}
