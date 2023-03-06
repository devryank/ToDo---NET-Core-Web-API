using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
	public class TodoForUpdateDto
	{
		[Required(ErrorMessage = "Activity is required")]
		[StringLength(255, ErrorMessage = "Activity can't be longger than 255 characters")]
		public string Activity { get; set; }

		public DateTime Created_at { get; set; } = DateTime.Now;

		public Guid UserId { get; set; }
	}
}
