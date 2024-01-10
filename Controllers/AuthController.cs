using dotnet_core_auth_api.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace dotnet_core_auth_api.Controllers
{
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {

        private static User user = new User();

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly DataContext liftxrDB;

        public AuthController(IConfiguration configuration, DataContext db, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            liftxrDB = db;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet, Authorize]
        public async Task<ActionResult<List<User>>> Get()
        {
            return Ok(await liftxrDB.Users.ToListAsync());
        }

        private void SetUser(User importedUser)
        {
            user = importedUser;
        }

        [HttpPost("ReturnUser")]
        public User ReturnUser()
        {
            User returnUser = user;

            if (user == null)
            {
                return null;
            }

            return returnUser;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register(UserDTO request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Email = request.Email;

            //check to see if username is taken
            var existingUser = liftxrDB.Users.Where(x => x.Email == request.Email).FirstOrDefault();

            if (existingUser == null)
            {
                liftxrDB.Users.Add(user);
                await liftxrDB.SaveChangesAsync();
                return Ok(await liftxrDB.Users.ToListAsync());
            }

            return BadRequest("Username is taken.");
        }

        [HttpPost("Login")]

        public async Task<ActionResult<string>> Login([FromBody] UserLoginDTO request)
        {

            User? userLogin;

            if (request == null)
            {
                return BadRequest("User not found.");
            }

            userLogin = liftxrDB.Users.FirstOrDefault(x => x.Email == request.Email);

            if (userLogin == null)
            {
                return BadRequest("User attempting to login with null info.");
            }

            if (userLogin.Email != request.Email)
            {
                return BadRequest("User not found.");
            }

            if (!VerifyPasswordHash(request.Password, userLogin.PasswordHash, userLogin.PasswordSalt))
            {
                return BadRequest("Wrong password.");
            }

            string token = CreateToken(userLogin);
            var refreshToken = CreateRefreshToken();
            await SetRefreshToken(refreshToken, userLogin);

            userLogin.PasswordHash = new byte[0];
            userLogin.PasswordSalt = new byte[0];

            UserLogin user = new UserLogin
            {
                Email = userLogin.Email,
                JWT = token,
                RefreshToken = refreshToken.Token,
            };

            SetUser(userLogin);
            return Ok(user); //return JWT + Refresh Token + Authority Level

        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken([FromBody] string request)
        {

            //var refreshToken = _httpContextAccessor?.HttpContext?.Request.Cookies["refreshToken"];

            string refreshToken = request;

            var user = await liftxrDB.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null)
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            else if (user.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Refresh Token Expired.");
            }

            string token = CreateToken(user);
            var newRefreshToken = CreateRefreshToken();
            await SetRefreshToken(newRefreshToken, user);

            return Ok(newRefreshToken);

        }

        private RefreshToken CreateRefreshToken()
        {

            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;

        }

        private async Task SetRefreshToken(RefreshToken newRefreshToken, User user)
        {

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };

            _httpContextAccessor?.HttpContext?.Response.Cookies.Append(
                "refreshToken", newRefreshToken.Token, cookieOptions);

            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;

            await liftxrDB.SaveChangesAsync();

        }

        private string CreateToken(User user)
        {

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1), //Subject to change, not much risk of someones fitness data being stolen
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

    }
}
