using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
	[Table("Todo")]
	public class Todo
	{	
		[Column("TodoId")]
		public Guid Id { get; set; }

		[Required(ErrorMessage = "Activity is required")]
		public string Activity { get; set; }
		public DateTime Created_at { get; set; }
		public Guid UserId { get; set; }


		//[ForeignKey("User_UserId")]

		//public Guid UserId { get; set; }
		//public User User { get; set; }
	}
}
