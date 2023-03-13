using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;

namespace ToDo
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<User, UserDto>();

			CreateMap<UserForCreationDto, User>();

			CreateMap<UserForUpdateDto, User>();

            CreateMap<LoginRequestDto, User>();

            CreateMap<Todo, TodoDto>();

			CreateMap<TodoForCreationDto, Todo>();

			CreateMap<TodoForUpdateDto, Todo>();
		}
	}
}
