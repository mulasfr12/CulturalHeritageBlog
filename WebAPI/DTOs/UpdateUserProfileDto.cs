namespace WebAPI.DTOs
{
    public class UpdateUserProfileDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
