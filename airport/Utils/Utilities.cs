using Microsoft.IdentityModel.Tokens;
using airport.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using airport.Models.Dto;
using System;


namespace airport.Utils
{
    public class Utilities
    {
        private readonly IConfiguration _configuration;
        const int keySize = 64;
        const int iterations = 350000;
        public HashAlgorithmName HashAlgorithmName { get; private set; }
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA256;

        public Utilities(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var jwtConfig = new JwtSecurityToken
                (
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(40),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
        }

        public string HashPassword(string password, out byte[] salt)
        {

            salt = RandomNumberGenerator.GetBytes(keySize);

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize
                );

            return Convert.ToHexString(hash);
        }

        public bool VerifyPassword(string password, string hash, byte[] salt)
        {

            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }

        public static DateTime convertToUtc(DateTime date, string timezoneId)
        {
            // Obtener la zona horaria del usuario proporcionada por el frontend
            TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);

            // Convertir la fecha y hora a la zona horaria local del usuario
            DateTime localDateTime = TimeZoneInfo.ConvertTime(date, userTimeZone);

            // Convertir la fecha y hora local a UTC
            DateTime utcDateTime = localDateTime.ToUniversalTime();

            return utcDateTime;
        }

        public static int findUserId(HttpContext httpContext)
        {
            ClaimsPrincipal principal = httpContext.User;
            var id = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (id != null) { 
            
                int userId = int.Parse(id);
                return userId;
            }

            return 0;
            
        }
    }
}
