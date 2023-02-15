using AutoMapper;
using DAL.Entities;
using DAL.UnitOfWork.MainServer;
using MainServer.Attributes;
using MainServer.Models;
using MainServer.Models.ViewModels;
using MainServer.Token;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace MainServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class ServerController : ControllerBase
    {
        private readonly IConfiguration config;
		private readonly IUnitOfWorkMain _unitOfWork;
		private readonly IMapper mapper;
        public ServerController(IConfiguration configuration, IUnitOfWorkMain unitOfWork, IMapper _mapper)
        {
            config = configuration;
            _unitOfWork = unitOfWork;
            mapper = _mapper;
        }

        [HttpPost("ServerGetIP")]
        public async Task<IActionResult> ServerGetIPAsync([FromBody] string Title)
        {
            var Server = await _unitOfWork.Servers.GetByTitle(Title);

            if (Server != null)
            {
                if (!string.IsNullOrEmpty(Server.Ip))
                {
                    string[] temp = Server.Ip.Split(":");
                    if (temp[0] == "0.0.0.0") return Ok("localhost:" + temp[1]);

                    return Ok(Server.Ip);
                }
            }
            return BadRequest();
        }

        [HttpPost("ServerMonitoringAdd")]
        public async Task<IActionResult> ServerMonitoringAddAsync([FromBody] ServerInformation newServerInfo)
        {
            var _server = await _unitOfWork.Servers.GetByTitle(newServerInfo.Title);

            if (_server != null)
                return Conflict("Сервер с таким именем уже существует.\n" +
                           "Смените имя сервера через LTPanel");

            //await _unitOfWork.Servers.Add(new Server { Title = server.Title, Ip = server.Ip, Ident = server.Ident,  City=server.City,Country=server.Country, Description=server.Description});
            Server newDefaultServer = new Server();
            mapper.Map(newServerInfo, newDefaultServer);
            await _unitOfWork.Servers.Add(newDefaultServer);
            await _unitOfWork.SaveChangesAsync();
            return Ok("Сервер успешно добавлен");
        }

        [HttpPost("ServerChangeMonitor")]
        public async Task<IActionResult> ServerChangeMonitorAsync([FromBody] ServerInformation newServerInfo)
        {
            var oldServerInfo = await _unitOfWork.Servers.GetByIdent(newServerInfo.Ident);

            if (oldServerInfo == null) return BadRequest("Сервер не найден");

            if (oldServerInfo.Ident != newServerInfo.Ident) return BadRequest("Неверный идентификатор");

            mapper.Map(newServerInfo, oldServerInfo);

            await _unitOfWork.SaveChangesAsync();

            return Ok("\nНастройки успешно применены");
        }
    }
}
