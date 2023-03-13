using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
//using System.Security.Cryptography;
using System.Text;

namespace ToDo.Controllers
{
	[Route("api/user")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private IRepositoryWrapper _repository;
		private IMapper _mapper;
		private readonly IConfiguration _configuration;
		public UserController(IRepositoryWrapper repository, IMapper mapper, IConfiguration configuration)
		{
			_repository = repository;
			_mapper = mapper;
			_configuration = configuration;
		}

		[HttpGet]
		public IActionResult GetUsers([FromQuery] UserParameters userParameters)
		{
			var users = _repository.User.GetUsers(userParameters);

			return Ok(users);
		}

		[HttpGet("{id}", Name = "UserById")]
		public IActionResult GetUserById(Guid id)
		{
			try
			{
				var user = _repository.User.GetUserById(id);
				if(user == null)
				{
					return NotFound();
				} else
				{
					var userResult = _mapper.Map<UserDto>(user);
					return Ok(userResult);
				}
			} catch(Exception ex)
			{
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("{id}/todo")]
		public IActionResult GetUserWithDetails(Guid id)
		{
			try
			{
				return Ok(_repository.User.GetUserWithDetails(id));
				var user = _repository.User.GetUserWithDetails(id);

				if(user == null)
				{
					return NotFound();
				}
				else
				{
					var userResult = _mapper.Map<UserDto>(user);
					return Ok(userResult);
				}
			}
			catch(Exception ex)
			{
				return StatusCode(500, ex);
			}
		}

		[Authorize]
		[HttpPost]
		public IActionResult CreateUser([FromBody]UserForCreationDto user)
		{
			try
			{
				if(user is null)
				{
					return BadRequest("Owner object is null");
				}

				if(!ModelState.IsValid)
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
			}catch(Exception ex)
			{
				return StatusCode(500, "error");
			}
		}

		[HttpPost("login")]
		public IActionResult Login([FromBody]LoginRequestDto request)
		{
			if(request == null)
			{
				return NotFound();
			}
			var user = _repository.User.GetUserByEmail(request.Email);
			if(user is null)
			{
				return StatusCode(404, new { status = 404, message = "User tidak ditemukan" });
			}
			var checkPassword = BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.Password);
            if(checkPassword)
			{
				string token = CreateToken(user);
                var userResult = _mapper.Map<UserDto>(user);
				var claims = decodeJwt(token);
				//var name = claims.FirstOrDefault(u => u.Type == "name")?.Value;
				return Ok(new { user = userResult, token });
			} else
			{
				return StatusCode(401, new { status = 401, message = "Password salah" });
            }
		}

        private string CreateToken(User user)
        {
			List<Claim> claims = new List<Claim>
			{

				new Claim("name", user.Username)
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

		private IEnumerable<Claim> decodeJwt(string token)
		{
			var tokenHandler = new JwtSecurityTokenHandler();

			var jwtDecoded = tokenHandler.ReadJwtToken(token);
			var claims = jwtDecoded.Claims;

			
			//var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
			//{
			//	ValidateIssuerSigningKey = true,
			//	ValidateIssuer = true,
			//	ValidateAudience = true,
			//	ValidIssuer = "your_issuer_here",
			//	ValidAudience = "your_audience_here",
			//	IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value))
			//}, out var validationToken);

			//var claims = claimsPrincipal.Claims;
			return claims;
		}

        [HttpPut("{id}")]
		public IActionResult UpdateUser(Guid id, [FromBody]UserForUpdateDto user)
		{
			try
			{
				if (user is null)
				{
					return BadRequest("User object is null");
				}

				if (!ModelState.IsValid)
				{
					return BadRequest("Invalid model object");
				}

				var userEntity = _repository.User.GetUserById(id);
				if (userEntity is null)
				{
					return NotFound();
				}

				_mapper.Map(user, userEntity);

				_repository.User.UpdateUser(userEntity);
				_repository.Save();

				return Ok(userEntity);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex);
			}
		}

		[HttpDelete("{id}")]
		public IActionResult DeleteUser(Guid id)
		{
			try
			{
				var user = _repository.User.GetUserById(id);
				if(user == null)
				{
					return NotFound();
				}

				_repository.User.DeleteUser(user);
				_repository.Save();

				return NoContent();
			}
			catch(Exception ex)
			{
				return StatusCode(500, ex);
			}
		}

        //const int keySize = 64;
        //const int iterations = 350000;
        //HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
        //string HashPassword(string password, out byte[] salt)
        //{
        //    salt = RandomNumberGenerator.GetBytes(keySize);
        //    var hash = Rfc2898DeriveBytes.Pbkdf2(
        //        Encoding.UTF8.GetBytes(password),
        //        salt,
        //        iterations,
        //        hashAlgorithm,
        //        keySize);
        //    return Convert.ToHexString(hash);
        //}
        //bool VerifyPassword(string password, string hash, byte[] salt)
        //{
        //    var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);
        //    return hashToCompare.SequenceEqual(Convert.FromHexString(hash));
        //}
    }
}
