using airport.Models.Dto;
using airport.Models.Querys;

namespace airport.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<bool> registerUser(RegisterDto registerDto);
        public Task<UserDataFront> authenticateUser(LoginDto loginDto);
    }
}
