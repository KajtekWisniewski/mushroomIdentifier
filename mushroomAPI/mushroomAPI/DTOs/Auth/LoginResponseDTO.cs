using mushroomAPI.DTOs.User;

namespace mushroomAPI.DTOs.Auth
{
    public class LoginResponseDTO
    {
        public string Token { get; set; } = "";
        public UserProfileDTO User { get; set; } = new UserProfileDTO();
    }
}