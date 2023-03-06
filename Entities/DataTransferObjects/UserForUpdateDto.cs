using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
	public class UserForUpdateDto
	{
		[Required(ErrorMessage = "Username is required")]
		[StringLength(45, ErrorMessage = "Username can't be longger than 45 characters")]
		public string? Username { get; set; }

		[Required(ErrorMessage = "Email is required")]
		[StringLength(60, ErrorMessage = "Email can't be longger than 60 characters")]
		public string? Email { get; set; }


		[Required(ErrorMessage = "Password is required")]
		[StringLength(255, ErrorMessage = "Password can't be longger than 255 characters")]
		public string? Password { get; set; }

		[Required(ErrorMessage = "Role is required")]
		[EnumDataType(typeof(RoleDto))]
		public string? Role { get; set; }
	}
}
