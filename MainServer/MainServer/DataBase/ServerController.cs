using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MainServer.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MainServer.Token;

namespace MainServer.DataBase
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class ServerController : ControllerBase
    {
        private readonly IConfiguration config;
        private readonly DB db;
        public ServerController(IConfiguration configuration, DB database)
        {
            config = configuration;
            db = database;
        }

        [HttpGet("ServerGetInfo")]
        public IActionResult ServerGetInfo()//Вернуть информацию
        {
            var Server = db.Servers.FirstOrDefault();

            if (Server != null)
            {
                try
                {
                    if (string.IsNullOrEmpty(Server.Ip)) Server.Ip = config["IPchat"];
                }
                catch
                {
                    Console.WriteLine("Specify chat IP in ServerAuthorization.json");
                    Server.Ip = "Set the IP of the chat server using IPchat";
                }
                return Ok(Server);
            }
            else return BadRequest(new Server { Title = "LiteCall" });
        }

        [HttpPost("ServerGetIP")]
        public IActionResult ServerGetIP([FromBody] string Title)
        {
            DB db = new DB();

            var Server = db.Servers.FirstOrDefault(x => x.Title == Title);

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
        public IActionResult ServerMonitoringAdd([FromBody] ServerMonitor server)
        {
            var Server = db.Servers.FirstOrDefault(x => x.Title == server.Title);

            if (Server != null)
            {
                if (Server.Ident == server.Ident)
                {
                    var tempServ = db.Servers.FirstOrDefault(x => x.Title == server.Title);
                    if (tempServ != null && tempServ.Title!=server.Title) return Conflict("Сервер с таким именем уже существует.\n" +
                           "Смени имя сервера через LTPanel");
                    Server.Title = string.IsNullOrEmpty(server.Title) ? Server.Title : server.Title;
                    Server.Ip = string.IsNullOrEmpty(server.Ip) ? Server.Ip : server.Ip;
                    db.SaveChanges();
                    return Ok("\nНастройки изменены");
                }
            }
            else
            {
                var tempServ2 = db.Servers.FirstOrDefault(x => x.Title == server.Title);
                if (tempServ2 != null && tempServ2.Title != server.Title) return Conflict("Сервер с таким именем уже существует.\n" +
                         "Смени имя сервера через LTPanel");
                ServerDB newSer = new ServerDB();
                newSer.Title = string.IsNullOrEmpty(server.Title) ? Server?.Title : server.Title;
                newSer.Ip = string.IsNullOrEmpty(server.Ip) ? Server?.Ip : server.Ip;
                newSer.Ident = string.IsNullOrEmpty(server.Ident) ? Server?.Ident : server.Ident;
                db.Servers.Add(newSer);
                db.SaveChanges();
                return Ok("\nСервер успешно добавлен");
            }
            return BadRequest();
            
            
        }


        [HttpPost("ServerChangeMonitor")]
        public IActionResult ServerChangeMonitor([FromBody] ServerMonitor server)
        {
            var Server = db.Servers.FirstOrDefault(x => x.Title == server.Title);
            if (Server == null) BadRequest("Сервер не найден");
            if(Server.Ident!=server.Ident) BadRequest("Неверный идентификатор");
            Server.Title = string.IsNullOrEmpty(server.Title) ? Server.Title : server.Title;
            Server.Ip = string.IsNullOrEmpty(server.Ip) ? Server.Ip : server.Ip;
            Server.Ident = string.IsNullOrEmpty(server.Ident) ? Server.Ident : server.Ident;
            db.SaveChanges();
            return Ok("\nНастройки успешно применены");
        }



        [HttpPost("SaveServersUser")]
        public IActionResult SaveServersUser([FromBody] SaveServer authModel)
        {
            var user = db.Users.FirstOrDefault(x => x.Login == authModel.Login);


            if (user.DateSynch > authModel.DateSynch)
                return BadRequest();
            
            if (user == null || user.Password != authModel.Password)
                return Unauthorized("Invalid login or password");

            
            if (!string.IsNullOrEmpty(authModel.SaveServers))
            {
                user.SaveServers = authModel.SaveServers;
                user.DateSynch = DateTime.Now;
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("GetServersUser")]
        public IActionResult GetServersUser([FromBody] SaveServer authModel)
        {
            var user = db.Users.FirstOrDefault(x => x.Login == authModel.Login);

            if (user.DateSynch < authModel.DateSynch)
                return BadRequest();

            if (user == null || user.Password != authModel.Password)
                return Unauthorized("Invalid login or password");


            return Ok( user.SaveServers);
        }
    }
}
