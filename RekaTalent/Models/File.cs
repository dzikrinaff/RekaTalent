﻿namespace RekaTalent.Models
{
    public class FileModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public string ContentType { get; set; }
    }
}