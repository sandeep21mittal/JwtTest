using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace jwtAuthapi
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _key = "ThisIsASecretKeyForJwtTokenDontShare";


        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model.Username == "admin" && model.Password == "password")
            {
                var token = GenrateAuthTicket(model.Username);
                return Ok(new { token });
            }
            return Unauthorized();

            //var key = Encoding.UTF8.GetBytes(_key);
            //if (model.Username != "admin" && model.Password != "password")
            //{
            //    return Unauthorized();
            //}
            //var tokenHandler = new JwtSecurityTokenHandler();
            ////var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            //var tokenDescription = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(new[]
            //    {
            //       new Claim(ClaimTypes.Name,model.Username),
            //       new Claim(ClaimTypes.Role,"Admin")
            //    }),
            //    Expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])),
            //    Issuer = _configuration["Jwt:Issuer"],
            //    Audience = _configuration["Jwt:Audience"],
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            //};
            //var token = tokenHandler.CreateToken(tokenDescription);
            //return Ok(new { token = tokenHandler.WriteToken(token) });
        }

        private string GenrateAuthTicket(string UserName)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsASecretKeyForJwtTokenDontShare"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, UserName),
                new Claim(ClaimTypes.Role, "Admin") // Example role claim
            };

            var token = new JwtSecurityToken(
                issuer: "JwtAuthDemo1",
                audience: "JwtAuthDemoUser",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [Authorize]
        [HttpGet("data")]
        public IActionResult GetSecureData()
        {
            var username = User.Identity.Name;
            return Ok($"Hello {username}, you have accessed a secure endpoint.");
        }
    }
}
