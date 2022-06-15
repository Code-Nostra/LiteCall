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

namespace AuthorizationServ.DataBase
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class ServerController : ControllerBase
    {
        private IConfiguration _configuration;
        public ServerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("ServerGetInfo")]
        public IActionResult ServerGetInfo()//Вернуть информацию
        {
            DB db = new DB();

            var Server = db.Servers.FirstOrDefault();
           
            if (Server != null)
            {
                try
                {
                    if (string.IsNullOrEmpty(Server.Ip)) Server.Ip = _configuration["IPchat"];
                }
                catch
                {
                    Console.WriteLine("Specify chat IP in ServerAuthorization.json");
                    Server.Ip = "Set the IP of the chat server using IPchat";
                }
                return Ok(Server);
            }
            else return Ok(new Server { Title = "LiteCall" });
        }

        [HttpPost("ServerSetInfo")]
        public IActionResult ServerSetInfo([FromBody] ServerInfo server)//Вернуть информацию
        {
            DB db = new DB();

            var admin = db.Users.FirstOrDefault(x => x.Login == server.Login);

            if (admin == null || admin.Password != server.Password)
                return Unauthorized("Invalid login or password");


            if (admin.Role == "Admin")
            {
                var Server = db.Servers.FirstOrDefault();

                if (Server != null)
                {
                    try
                    {
                        Server.Title = string.IsNullOrEmpty(server.Title) ? Server.Title: server.Title;
                        Server.Country = string.IsNullOrEmpty(server.Country) ? Server.Country : server.Country;
                        Server.City = string.IsNullOrEmpty(server.City) ? Server.City : server.City;
                        Server.Description = string.IsNullOrEmpty(server.Description) ? Server.Description : server.Description;
                        Server.Ip = server.Ip;
                        if (string.IsNullOrEmpty(Server.Ip)) Server.Ip = _configuration["IPchat"];
                        db.SaveChanges();
                    }
                    catch
                    {
                        Console.WriteLine("Specify chat IP in ServerAuthorization.json");
                        Server.Ip = "Set the IP of the chat server using IPchat";
                    }
                    return Ok(Server);
                }
            }
            return BadRequest();
        }

        [HttpGet("ServerGetIP")]
        public IActionResult ServerGetIP()
        {
            return Ok(_configuration["IPchat"]);
        }


        // [HttpGet("ServerAll")]
        //public IActionResult ServerAll()//Вернуть информацию по всем серверам
        //{
        //DB db = new DB();

        //List<ServerDB> Server = db.ServerDB.AsEnumerable().ToList();

        //if (Server != null)
        //{
        //    return Ok(Server);
        //}
        //return BadRequest();
        //}
        //[HttpPost("CheckName")]
        //public IActionResult CheckName([FromBody] string Name)//Проверка занятого имени, если имя есть то вернуть Name+Count(Name)
        //{
        //    UserAuth db = new UserAuth();

        //    var user = db.UsersDB.FirstOrDefault(x => x.Name == Name);

        //    if (user == null)
        //        return Ok(false);

        //    int temp = db.UsersDB.Where(a => a.Name == Name).Count();

        //    if (temp >= 1) 
        //        return Ok(true);
        //    return Ok(false);
        //}


    }
}
