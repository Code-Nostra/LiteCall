using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("ServerGetInfo")]
        public IActionResult ServerGetInfo()//Вернуть информацию
        {
            DB db = new DB();
            
            var Server = db.Servers.First();
           
            if (Server != null)
            {
                try
                {
                    var key = JsonNode.Parse(System.IO.File.ReadAllText("ServerAuthorization.json"));
                    Server.Ip = (string)key["IPchat"];
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
        
        [HttpGet("ServerGetIP")]
        public IActionResult ServerGetIP()
        {
            DB db = new DB();

            var Server = db.Servers.First();

            if (Server != null)
            {
                return Ok(Server.Ip);
            }
            else return BadRequest();
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
