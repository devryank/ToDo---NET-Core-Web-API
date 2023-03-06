using Contracts;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
	public class RepositoryWrapper : IRepositoryWrapper
	{
		private RepositoryContext _repoContext;
		private IUserRepository _user;
		private ITodoRepository _todo;

		public IUserRepository User
		{
			get
			{
				if(_user == null)
				{
					_user = new UserRepository(_repoContext);
				}

				return _user;
			}
		}

		public ITodoRepository Todo
		{
			get
			{
				if(_todo == null)
				{
					_todo = new TodoRepository(_repoContext);
				}

				return _todo;
			}
		}

		public RepositoryWrapper(RepositoryContext repositoryContext)
		{
			_repoContext = repositoryContext;
		}

		public void Save()
		{
			_repoContext.SaveChanges();
		}
	}
}
