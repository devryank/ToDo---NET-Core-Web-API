using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
	public interface ITodoRepository : IRepositoryBase<Todo>
	{
		IEnumerable<Todo> GetAllTodos();
		Todo GetTodoById(Guid todoId);
		void CreateTodo(Todo todo);
		void UpdateTodo(Todo todo);
		void DeleteTodo(Todo todo);
	}
}
