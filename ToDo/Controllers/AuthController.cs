using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ToDo.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IRepositoryWrapper _repository;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;
        public AuthController(IRepositoryWrapper repository, IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            if (request == null)
            {
                return NotFound();
            }
            var user = _repository.User.GetUserByEmail(request.Email);
            if (user is null)
            {
                return StatusCode(404, new { status = 404, message = "User tidak ditemukan" });
            }
            var checkPassword = BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.Password);
            if (checkPassword)
            {
                string token = CreateToken(user);
                var userResult = _mapper.Map<UserDto>(user);
                return Ok(new { user = userResult, token });
            }
            else
            {
                return StatusCode(401, new { status = 401, message = "Password salah" });
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserForCreationDto user)
        {
            try
            {
                if (user is null)
                {
                    return BadRequest("Owner object is null");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model object");
                }
                var hash = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password);
                user.Password = hash;

                var userEntity = _mapper.Map<User>(user);
                _repository.User.CreateUser(userEntity);
                _repository.Save();

                var createdUser = _mapper.Map<UserDto>(userEntity);

                return CreatedAtRoute("UserById", new { id = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "error");
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
