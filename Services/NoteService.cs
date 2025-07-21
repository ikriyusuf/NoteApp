using NoteApp.Entities;
using System;
using System.Collections.Generic;

namespace NoteApp.Services
{
    public class NoteService : DbService
    {
        public NoteService() : base()
        {
        }
        public bool CreateNote(Note note)
        {
            var conn = GetConnection();
            conn.Open();
            string query = "INSERT INTO notes (title, content, priority, userid) VALUES (@Title, @Content, @Priority, @UserId)";
            var cmd = new Npgsql.NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("Title", note.Title);
            cmd.Parameters.AddWithValue("Content", note.Content);
            cmd.Parameters.AddWithValue("Priority", note.Priority);
            cmd.Parameters.AddWithValue("UserId", note.UserId);
            int result = cmd.ExecuteNonQuery();
            return result > 0;
        }

        public List<NoteDto> GetAllNotes(int userId = 1)
        {
            var notes = new List<NoteDto>();

            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM notes WHERE userid = @userId";

                using (var cmd = new Npgsql.NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("userId", userId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var note = new NoteDto
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("noteid")),
                                Title = reader.GetString(reader.GetOrdinal("title")),
                                Content = reader.GetString(reader.GetOrdinal("content")),
                                Priority = reader.GetString(reader.GetOrdinal("priority")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("createddate")),
                                UpdateDate = reader.IsDBNull(reader.GetOrdinal("updatedate"))
                                    ? (DateTime?)null
                                    : reader.GetDateTime(reader.GetOrdinal("updatedate"))
                            };

                            notes.Add(note);
                        }
                    }
                }
            }

            return notes;
        }

        public bool UpdateNote(Note note)
        {
            var conn = GetConnection();
            conn.Open();
            string query = "UPDATE notes SET title = @Title, content = @Content, priority = @Priority, updatedate = @UpdateDate WHERE noteid = @noteid";
            var cmd = new Npgsql.NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("noteid", note.Id);
            cmd.Parameters.AddWithValue("Title", note.Title);
            cmd.Parameters.AddWithValue("Content", note.Content);
            cmd.Parameters.AddWithValue("Priority", note.Priority);
            cmd.Parameters.AddWithValue("UpdateDate", DateTime.Now);
            int result = cmd.ExecuteNonQuery();
            return result > 0;
        }

        public bool DeleteNote(int noteId)
        {
            var conn = GetConnection();
            conn.Open();
            string query = "DELETE FROM notes WHERE noteid = @noteid";
            var cmd = new Npgsql.NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("noteid", noteId);
            int result = cmd.ExecuteNonQuery();
            return result > 0;
        }
    }
}
