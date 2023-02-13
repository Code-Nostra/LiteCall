using DAL.Entities;
using MainServer.DAL.Interfaces;
using MainServer.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MainServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class HomeController : EntityController<User>
	{
		private readonly IUserRepository _userRepository;

		public HomeController(IUserRepository userRepository,IBaseRepository<User> repository) : base(repository)
		{
			_userRepository = userRepository;
		}
		[HttpGet("Get2")]
		public async Task<IEnumerable<User>> Get2()
		{
			return await _repository.GetAll();
		}

	}
}
