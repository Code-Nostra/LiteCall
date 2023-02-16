using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MainServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class HomeController : EntityController<MainServerDbContext,User>
	{
		//private readonly IUserRepository _userRepository;

		public HomeController(IBaseRepository<User> repository) : base(repository)
		{
			//_userRepository = userRepository;
		}
		[HttpGet("Get2")]
		public async Task<IEnumerable<User>> Get2()
		{
			return await _repository.GetAll();
		}

	}
}