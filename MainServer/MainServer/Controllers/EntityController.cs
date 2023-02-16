using DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MainServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EntityController<TContext,T> : ControllerBase
		where TContext : Microsoft.EntityFrameworkCore.DbContext
		where T : class
	{

		private readonly ILogger<HomeController> _logger;
		private IBaseRepository<TContext, T> _repository { get; set; }
		private readonly IConfiguration _config;
		public EntityController(IBaseRepository<TContext, T> repository)
		{
			_repository = repository;

		}


		[HttpGet("Get")]
		public async Task<IEnumerable<T>> Get()
		{
			return await _repository.GetAll();
		}
	}
}
