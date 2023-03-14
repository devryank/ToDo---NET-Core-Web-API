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

		[Authorize(Roles = "Admin")]
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

		[Authorize]
		[HttpGet("{id}/todo")]
		public IActionResult GetUserWithDetails(Guid id)
		{
			try
			{
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

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public IActionResult CreateUser([FromBody]UserForCreationDto user)
		{
			return Ok(user);
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

		[Authorize]
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

        //[Authorize(Roles = "Admin")]
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
