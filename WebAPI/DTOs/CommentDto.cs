﻿namespace WebAPI.DTOs
{
    public class CommentDto
    {
        public int CommentID { get; set; }
        public string Content { get; set; }
        public int UserID { get; set; }
        public int HeritageID { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}