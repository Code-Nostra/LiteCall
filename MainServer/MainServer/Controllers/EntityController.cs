using DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MainServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EntityController<T> : ControllerBase where T : class
	{
		public EntityController(IBaseRepository<T> repository)
		{
			_repository = repository;
		}

		public IBaseRepository<T> _repository { get; set; }

		[HttpGet("Get")]
		public async Task<IEnumerable<T>> Get()
		{
			return await _repository.GetAll();
		}
	}
}
