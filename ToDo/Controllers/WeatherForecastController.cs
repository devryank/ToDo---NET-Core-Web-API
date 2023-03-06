using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ToDo.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		private IRepositoryWrapper _repository;

		public WeatherForecastController(IRepositoryWrapper repository)
		{
			_repository = repository;
		}

		[HttpGet]
		public IEnumerable<string> Get()
		{
			var users = _repository.User.FindAll();
			return new string[] { "Value 1", "Value 2" };
		}
	}
}