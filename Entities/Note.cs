﻿using System;

namespace NoteApp.Entities
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
