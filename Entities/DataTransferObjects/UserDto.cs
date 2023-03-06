using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
	public class UserDto
	{
		public Guid Id { get; set; }
		public string? Username { get; set; }
		public string? Email { get; set; }
		public string? Role { get; set; }

		public IEnumerable<TodoDto>? Todos { get; set; }
	}
}
