using AuthorizationServ.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ServerAuthorization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace AuthorizationServ.DataBase
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
                    db.SaveChanges();
                }
                catch
                {
                    Console.WriteLine("Specify chat IP in ServerAuthorization.json");
                    Server.Ip = "Set the IP of the chat server using IPchat";
                }
                return Ok(new Server {Title=Server.Title,City=Server.City,Country=Server.Country,Description=Server.Description,Ip=Server.Ip });
            }
            else return BadRequest(new Server { Title = "LiteCall" });
        }

        [HttpPost("ServerSetInfo")]
        public IActionResult ServerSetInfo([FromBody] ServerInfo server)
        {
            var admin = db.Users.FirstOrDefault(x => x.Login == server.Login);

            if (admin == null || admin.Role != "Admin"  || admin.Password != server.Password )
                return Unauthorized("\nНеверный логин или пароль");

            var Server = db.Servers.FirstOrDefault();

            if (Server != null)
            {
                Server.Title = string.IsNullOrEmpty(server.Title) ? Server.Title: server.Title;
                Server.Country = string.IsNullOrEmpty(server.Country) ? Server.Country : server.Country;
                Server.City = string.IsNullOrEmpty(server.City) ? Server.City : server.City;
                Server.Description = string.IsNullOrEmpty(server.Description) ? Server.Description : server.Description;
                Server.Ip = string.IsNullOrEmpty(server.Ip) ? config["IPchat"] : server.Ip;
                
                try
                {
                    string path = Path.Combine(AppContext.BaseDirectory, @"..\files\ServerAuthorization.json");
                    string requestBody = System.IO.File.ReadAllText(path);
                    Settings data = JsonSerializer.Deserialize<Settings>(requestBody);
                    data.IPchat = Server.Ip;
                    System.IO.File.WriteAllText(path, JsonSerializer.Serialize<Settings>(data, new JsonSerializerOptions { WriteIndented = true, IgnoreNullValues = true }));

                    path = Path.Combine(AppContext.BaseDirectory, @"..\..\files\ServerChat.json");
                    requestBody = System.IO.File.ReadAllText(path);
                    data = JsonSerializer.Deserialize<Settings>(requestBody);
                    data.IPchat = Server.Ip;
                    System.IO.File.WriteAllText(path, JsonSerializer.Serialize<Settings>(data, new JsonSerializerOptions { WriteIndented = true, IgnoreNullValues = true }));
                }
                catch (Exception e)
                {
                }
                db.SaveChanges();
                return Ok("\nНастройки успешно применены");
            }
           
            return BadRequest();
        }

        [HttpGet("ServerGetIP")]
        public IActionResult ServerGetIP()
        {
            return Ok(config["IPchat"]);
        }
    }
}
