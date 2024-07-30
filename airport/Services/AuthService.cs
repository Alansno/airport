using airport.Context;
using airport.Models;
using airport.Models.Dto;
using airport.Models.Querys;
using airport.Services.Interfaces;
using airport.Utils;
using airport.Utils.Results;
using Microsoft.EntityFrameworkCore;

namespace airport.Services
{
    public class AuthService
    {
        private readonly AirportContext _airportContext;
        private readonly Utilities _utilities;
        public AuthService(AirportContext airportContext, Utilities utilities)
        {
            _airportContext = airportContext;
            _utilities = utilities;
        }
        public async Task<Result<UserDataFront>> AuthenticateUser(LoginDto loginDto)
        {
            var result = await FindByEmail(loginDto);
            if (!result.IsSuccess) return Result<UserDataFront>.Failure(result.Error);

            var users = LoadUsersAsync();

            var resultt = result.Bind(user => VerifyPassword(loginDto, user));
            if (!resultt.IsSuccess) return Result<UserDataFront>.Failure(resultt.Error);

            var usersLoading = await users;
            return result.Bind(u => CreateToken(u, usersLoading.Value));
        }

        private async Task<Result<User>> FindByEmail(LoginDto loginDto)
        {
            var authUser = await _airportContext.Users.Where(u => u.Email == loginDto.Email).FirstOrDefaultAsync();
            return authUser != null ? Result<User>.Success(authUser) : Result<User>.Failure("El usuario no existe");
        }

        private async Task<Result<List<User>>> LoadUsersAsync()
        {
            var users = await _airportContext.Users.ToListAsync();
            return Result<List<User>>.Success(users);
        }

        private Result<bool> VerifyPassword(LoginDto loginDto, User user)
        {
            var isMatch = _utilities.VerifyPassword(loginDto.Password, user.Password, user.Salt);
            return isMatch != false ? Result<bool>.Success(isMatch) : Result<bool>.Failure("Credenciales incorrectas");
        }

        private Result<UserDataFront> CreateToken(User user, List<User> users)
        {
            var token = _utilities.GenerateToken(user);
            return Result<UserDataFront>.Success(new UserDataFront { Name = user.Name, Token = token });
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
