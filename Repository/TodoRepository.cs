using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
	public class TodoRepository : RepositoryBase<Todo>, ITodoRepository
	{
		public TodoRepository(RepositoryContext repositoryContext) : base(repositoryContext)
		{

		}
		public IEnumerable<Todo> GetAllTodos()
		{
			return FindAll().ToList();
		}

		public IEnumerable<Todo> GetMyTodos(string id)
		{
			return FindByCondition(todo => todo.UserId.Equals(new Guid(id))).ToList();
		}

		public Todo GetTodoById(Guid todoId)
		{
			return FindByCondition(todo => todo.Id.Equals(todoId)).FirstOrDefault();
		}

		public void CreateTodo(Todo todo) => Create(todo);

		public void UpdateTodo(Todo todo) => Update(todo);

		public void DeleteTodo(Todo todo) => Delete(todo);
	}
}
