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

        [HttpPost("ServerSetInfo")]
        public IActionResult ServerSetInfo([FromBody] ServerInfo server)
        {
            var admin = db.Users.FirstOrDefault(x => x.Login == server.Login);

            if (admin == null || admin.Role != "Admin" || admin.Password != server.Password)
                return Unauthorized("Неверный логин или пароль");

            var Server = db.Servers.FirstOrDefault();

            if (Server != null)
            {
                Server.Title = string.IsNullOrEmpty(server.Title) ? Server.Title : server.Title;
                Server.Country = string.IsNullOrEmpty(server.Country) ? Server.Country : server.Country;
                Server.City = string.IsNullOrEmpty(server.City) ? Server.City : server.City;
                Server.Description = string.IsNullOrEmpty(server.Description) ? Server.Description : server.Description;
                Server.Ip = string.IsNullOrEmpty(server.Ip) ? config["IPchat"] : server.Ip;
                db.SaveChanges();
                return Ok(Server);
            }

            return BadRequest();
        }

        [HttpPost("SaveServersUser")]
        public IActionResult SaveServersUser([FromBody] SaveServer authModel)
        {
            var user = db.Users.FirstOrDefault(x => x.Login == authModel.Login);
            
            if (user == null || user.Password != authModel.Password)
                return Unauthorized("Invalid login or password");

            if (!string.IsNullOrEmpty(authModel.SaveServers))
            {
                user.SaveServers = authModel.SaveServers;
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("GetServersUser")]
        public IActionResult GetServersUser([FromBody] SaveServer authModel)
        {
            var user = db.Users.FirstOrDefault(x => x.Login == authModel.Login);
           
            if (user == null || user.Password != authModel.Password)
                return Unauthorized("Invalid login or password");
            
            return Ok(user.SaveServers);
        }

        [HttpGet("ServerGetIP")]
        public IActionResult ServerGetIP()
        {
            return Ok(config["IPchat"]);
        }
    }
}
