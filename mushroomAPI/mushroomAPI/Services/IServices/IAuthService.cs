using mushroomAPI.Entities;

namespace mushroomAPI.Services.IServices
{
    public interface IAuthService
    {
        string CreateToken(User user);
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}
