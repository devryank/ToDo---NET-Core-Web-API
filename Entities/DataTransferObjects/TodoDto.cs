using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
	public class TodoDto
	{
		public Guid Id { get; set; }
		public string Activity { get; set; }
		public DateTime Created_at { get; set; }
	}
}
