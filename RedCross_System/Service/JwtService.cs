using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RedCross_System.Models.Domain;
using RedCross_System.Data;

namespace RedCross_System.Services
{
	public class JwtService
	{
		private readonly IConfiguration _configuration;
		private readonly ApplicationDbContext _context;

		public JwtService(IConfiguration configuration, ApplicationDbContext applicationDb)
		{
			_configuration = configuration;
			_context = applicationDb;
		}

		public string GenerateJwtToken(User user)
		{
			var claims = new[]
			{
								new Claim(ClaimTypes.Name, user.Name),
								new Claim(ClaimTypes.Role, user.Role.Name),
								new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Adding user ID claim
                // Add other claims as needed
            };

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinute"]));

			var token = new JwtSecurityToken(
					issuer: _configuration["Jwt:Issuer"],
					audience: _configuration["Jwt:Audience"],
					claims: claims,
					expires: expiration,
					signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
