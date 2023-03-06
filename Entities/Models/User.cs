using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
	[Table("User")]
	public class User
	{
		[Column("UserId")]
		public Guid Id { get; set; }

		[Required(ErrorMessage = "Username is required")]
		[StringLength(45, ErrorMessage = "Username can't be longer than 45 characters")]
		public string? Username { get; set; }

		[Required(ErrorMessage = "Email is required")]
		[StringLength(60, ErrorMessage = "Email can't be longer than 60 characters")]
		public string? Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[StringLength(255, ErrorMessage = "Password can't be longer than 255 characters")]
		public string? Password { get; set; }

		[Required(ErrorMessage = "Role is required")]
		[EnumDataType(typeof(Role))]
		public string? Role { get; set; }

		//public ICollection<Todo> Todos { get; set; }
		public List<Todo> Todos { get; set; }
	}
}
