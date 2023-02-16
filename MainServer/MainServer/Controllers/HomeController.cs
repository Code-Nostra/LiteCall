using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MainServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class HomeController : EntityController<MainServerDbContext,User>
	{
		private readonly IBaseRepository<MainServerDbContext, User> _repository;
		
		public HomeController(IBaseRepository<MainServerDbContext,User> repository) : base(repository)
		{
			_repository = repository;
		}
		[HttpGet("Get2")]
		public async Task<IEnumerable<User>> Get2()
		{
			return await _repository.GetAll();
		}

	}
}