using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;

namespace ToDo.Controllers
{
	[Authorize(Roles = "Member")]
	[Route("api/todo")]
    [ApiController]
	public class TodoController : ControllerBase
	{
		private IRepositoryWrapper _repository;
		private IMapper _mapper;

		public TodoController(IRepositoryWrapper repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		[HttpGet]
		public IActionResult GetAllTodos()
		{
			try
			{
				var todos = _repository.Todo.GetAllTodos();
				var todosResult = _mapper.Map<IEnumerable<TodoDto>>(todos);
				return Ok(todos);
			} catch (Exception ex)
			{
				return StatusCode(500, ex);
			}
		}

		[HttpGet("GetMyTodos")]
		public IActionResult GetMyTodos()
		{
			try
			{
                var UserId = Helper.JwtHelper.decodeJwt(Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", ""), "id");
                var todos = _repository.Todo.GetMyTodos(UserId);

				var todoResult = _mapper.Map<IEnumerable<TodoDto>>(todos);

				return Ok(todoResult);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex);
			}
		}

		[HttpGet("{id}", Name = "TodoById")]
		public IActionResult GetTodoById(Guid id)
		{
			try
			{
				var todo = _repository.Todo.GetTodoById(id);
				if(todo == null)
				{
					return NotFound();
				}
				else
				{
					var todoResult = _mapper.Map<TodoDto>(todo);

					return Ok(todoResult);
				}
			} catch(Exception ex)
			{
				return StatusCode(500, ex);
			}
		} 

		[HttpPost]
		public IActionResult CreateTodo([FromBody]TodoForCreationDto todo)
		{
			try
			{
				if(todo == null)
				{
					return NotFound();
				}
				if(!ModelState.IsValid)
				{
					return BadRequest("Invalid model object");
                }
                var id = Helper.JwtHelper.decodeJwt(Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", ""), "id");
                todo.UserId = new Guid(id);
				var todoEntity = _mapper.Map<Todo>(todo);
                _repository.Todo.CreateTodo(todoEntity);
				_repository.Save();

                var createdTodo = _mapper.Map<TodoDto>(todoEntity);
				return CreatedAtRoute("TodoById", new { id = createdTodo.Id }, createdTodo);
			} catch(Exception ex)
			{
				return StatusCode(500, ex);
			}
		}

		[HttpPut("{id}")]
		public IActionResult UpdateTodo(Guid id, [FromBody]TodoForUpdateDto todo)
		{
			try
			{
				if(todo is null)
				{
					return BadRequest("User object is null");
				}

				if(!ModelState.IsValid)
				{
					return BadRequest("Invalid model object");
				}
				var todoEntity = _repository.Todo.GetTodoById(id);
				if(todoEntity == null)
                {
					return NotFound();
				}

                var UserId = Helper.JwtHelper.decodeJwt(Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", ""), "id");
                todo.UserId = new Guid(UserId);
                _mapper.Map(todo, todoEntity);

                _repository.Todo.UpdateTodo(todoEntity);
				_repository.Save();

				return Ok(todoEntity);
			}catch(Exception ex)
			{
				return StatusCode(500, ex);
			}
		}

		[HttpDelete("{id}")]
		public IActionResult DeleteTodo(Guid id)
		{
			try
			{
				var todo = _repository.Todo.GetTodoById(id);
				if(todo == null)
				{
					return NotFound();
				}

				_repository.Todo.DeleteTodo(todo);
				_repository.Save();

				return NoContent();
			}catch(Exception ex)
			{
				return StatusCode(500, ex);
			}
		}
	}
}
