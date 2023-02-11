using AutoMapper;
using MainServer.Attributes;
using MainServer.Models;
using MainServer.Models.ViewModels;
using MainServer.Token;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace MainServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class ServerController : ControllerBase
    {
    //    private readonly IConfiguration config;
    //    private readonly ApplicationDbContext db;
    //    private readonly IMapper mapper;
    //    public ServerController(IConfiguration configuration, ApplicationDbContext database, IMapper _mapper)
    //    {
    //        config = configuration;
    //        db = database;
    //        mapper = _mapper;
    //    }

    //    [HttpPost("ServerGetIP")]
    //    public IActionResult ServerGetIP([FromBody] string Title)
    //    {
    //        var Server = db.Servers.FirstOrDefault(x => x.Title == Title);

    //        if (Server != null)
    //        {
    //            if (!string.IsNullOrEmpty(Server.Ip))
    //            {
    //                string[] temp = Server.Ip.Split(":");
    //                if (temp[0] == "0.0.0.0") return Ok("localhost:" + temp[1]);

    //                return Ok(Server.Ip);
    //            }
    //        }
    //        return BadRequest();
    //    }

    //    [HttpPost("ServerMonitoringAdd")]
    //    public IActionResult ServerMonitoringAdd([FromBody] ServerInformation server)
    //    {
    //        var Server = db.Servers.FirstOrDefault(x => x.Title == server.Title);

    //        if (Server != null)
    //            return Conflict("Сервер с таким именем уже существует.\n" +
    //                       "Смените имя сервера через LTPanel");

    //        db.Servers.Add(new ServerDB { Title = server.Title, Ip = server.Ip, Ident = server.Ident });
    //        db.SaveChanges();
    //        return Ok("Сервер успешно добавлен");
    //    }

    //    [HttpPost("ServerChangeMonitor")]
    //    public IActionResult ServerChangeMonitor([FromBody] ServerInformation newServerInfo)
    //    {
    //        var oldServerInfo = db.Servers.FirstOrDefault(x => x.Ident == newServerInfo.Ident);

    //        if (oldServerInfo == null) return BadRequest("Сервер не найден");

    //        if (oldServerInfo.Ident != newServerInfo.Ident) return BadRequest("Неверный идентификатор");

    //        mapper.Map(newServerInfo, oldServerInfo);

    //        db.SaveChanges();
    //        return Ok("\nНастройки успешно применены");
    //    }
    }
}
