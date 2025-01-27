namespace mushroomAPI.DTOs
{
    public class LoginResponseDTO
    {
        public string Token { get; set; } = "";
        public UserProfileDTO User { get; set; } = new UserProfileDTO();
    }
}