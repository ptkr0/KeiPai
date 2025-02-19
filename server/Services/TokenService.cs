using Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services
{
	public class TokenService : ITokenService
	{
		private readonly IConfiguration _config;
		private readonly SymmetricSecurityKey _key;

		public TokenService(IConfiguration config)
		{
			_config = config;
			_key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SigningKey"] ?? throw new InvalidOperationException("JWT Signing Key is not configured.")));
		}

		public string CreateToken(User user, string role)
		{
			var claims = new List<Claim>
									{
										new Claim(JwtRegisteredClaimNames.GivenName, user.UserName ?? string.Empty),
										new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
										new Claim(JwtRegisteredClaimNames.NameId, user.Id ?? string.Empty),
										new Claim(ClaimTypes.Role, role)
									};

			var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddDays(7), // to be changed for shorter value in the future
				SigningCredentials = creds,
				Issuer = _config["Jwt:Issuer"],
				Audience = _config["Jwt:Audience"]
			};

			var tokenHandler = new JwtSecurityTokenHandler();

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}
	}
}
