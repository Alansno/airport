using airport.Context;
using airport.Models;
using airport.Models.Dto;
using airport.Models.Querys;
using airport.Services.Interfaces;
using airport.Utils;
using Microsoft.EntityFrameworkCore;

namespace airport.Services
{
    public class AuthService : IAuthService
    {
        private readonly AirportContext _airportContext;
        private readonly Utilities _utilities;
        public AuthService(AirportContext airportContext, Utilities utilities)
        {
            _airportContext = airportContext;
            _utilities = utilities;
        }
        public async Task<UserDataFront> authenticateUser(LoginDto loginDto)
        {
            try
            {
                var authUser = await _airportContext.Users.Where(u => u.Email == loginDto.Email).FirstOrDefaultAsync();
                if (authUser == null) throw new ArgumentException("El usuario no existe");

                var isMatch = _utilities.VerifyPassword(loginDto.Password, authUser.Password, authUser.Salt);
                if (isMatch == false) throw new ArgumentException("Credenciales incorrectas");

                var token = _utilities.GenerateToken(authUser);
                var user = authUser.Name;

                return new UserDataFront { Name = user, Token = token };
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> registerUser(RegisterDto registerDto)
        {
            try
            {
                var userExist = await _airportContext.Users.Where(u => u.Email == registerDto.Email).Select(u => u.Id).ToListAsync();
                if (userExist.Any()) throw new ArgumentException("El correo ya está en uso");

                var user = new User
                {
                    Name = registerDto.Name,
                    Email = registerDto.Email,
                    Password = _utilities.HashPassword(registerDto.Password, out byte[] salt),
                    Role = "Cliente",
                    Salt = salt,
                    CreatedAt = DateTime.UtcNow,
                };

                await _airportContext.Users.AddAsync(user);
                await _airportContext.SaveChangesAsync();
                return true;
            }
            catch(DbUpdateException ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
