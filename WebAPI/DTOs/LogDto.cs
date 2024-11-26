namespace WebAPI.DTOs
{
    public class LogDto
    {
        public int LogID { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public int? UserID { get; set; }
    }
}
