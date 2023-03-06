using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ToDo.Controllers
{
	[Route("api/user")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private IRepositoryWrapper _repository;
		private IMapper _mapper;
		public UserController(IRepositoryWrapper repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}
		[HttpGet]
		public IActionResult GetAllUsers()
		{
			try
			{
				var users = _repository.User.GetAllUsers();
				var usersResult = _mapper.Map<IEnumerable<UserDto>>(users);
				return Ok(usersResult);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Internal server error");
			}
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

				var userEntity = _mapper.Map<User>(user);
				_repository.User.CreateUser(userEntity);
				_repository.Save();
					
				var createdUser = _mapper.Map<UserDto>(userEntity);

				return CreatedAtRoute("UserById", new { id = createdUser.Id }, createdUser);
			}catch(Exception ex)
			{
				return StatusCode(500, "Internal server error");	
			}
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


	}
}
