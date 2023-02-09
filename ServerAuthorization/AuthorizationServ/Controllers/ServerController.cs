using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ServerAuthorization.Attributes;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using ServerAuthorization.Models.ViewModels;
using ServerAuthorization.Models;

namespace ServerAuthorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class ServerController : ControllerBase
    {
        private readonly IConfiguration config;
        private readonly ApplicationDbContext db;
        public ServerController(IConfiguration configuration, ApplicationDbContext database)
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
                return Ok(Server);
            }
            else return BadRequest();
        }

        [HttpPost("ServerSetInfo")]
        public IActionResult ServerSetInfo([FromBody] ServerInformation server)
        {
            var admin = db.Users.FirstOrDefault(x => x.Login == server.Login);

            if (admin == null || admin.Role != "Admin" || admin.Password != server.Password)
                return Unauthorized("\nНеверный логин или пароль");

            var Server = db.Servers.FirstOrDefault();

            if (Server != null)
            {
                Server.Title = string.IsNullOrEmpty(server.Title) ? Server.Title : server.Title;
                Server.Country = string.IsNullOrEmpty(server.Country) ? Server.Country : server.Country;
                Server.City = string.IsNullOrEmpty(server.City) ? Server.City : server.City;
                Server.Description = string.IsNullOrEmpty(server.Description) ? Server.Description : server.Description;
                Server.Ip = string.IsNullOrEmpty(server.Ip) ? config["IPchat"] : server.Ip;

                try
                {
                    string path = Path.Combine(AppContext.BaseDirectory, @"..\files\ServerAuthorization.json");
                    string requestBody = System.IO.File.ReadAllText(path);
                    ConfigSettings data = JsonSerializer.Deserialize<ConfigSettings>(requestBody);
                    data.IPchat = Server.Ip;
                    System.IO.File.WriteAllText(path, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true, IgnoreNullValues = true }));

                    path = Path.Combine(AppContext.BaseDirectory, @"..\..\files\ServerChat.json");
                    requestBody = System.IO.File.ReadAllText(path);
                    data = JsonSerializer.Deserialize<ConfigSettings>(requestBody);
                    data.IPchat = Server.Ip;
                    System.IO.File.WriteAllText(path, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true, IgnoreNullValues = true }));
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
