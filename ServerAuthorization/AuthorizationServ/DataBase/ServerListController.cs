using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServ.DataBase
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerListController : ControllerBase
    {
        [HttpPost("ServerAddInfo")]
        public IActionResult ServerAdd([FromBody] InfoServerModel Info)//Авторизация
        {
            UserAuth db = new UserAuth();
            
            var Server = db.ServerDB.FirstOrDefault(x => x.Title.Trim().ToLower() == Info.Title.Trim().ToLower());

            if (Server == null)
            {
                db.ServerDB.Add(new ServerDB { Title = Info.Title.Trim(), Country = Info.Country, City = Info.City });
                    return Ok();
            }
            return BadRequest();
        }
        
        [HttpPost("ServerGetInfo")]
        public IActionResult ServerGetInfo([FromBody] string Title)//Вернуть информацию по конкретному серверу
        {
            UserAuth db = new UserAuth();

            var Server = db.ServerDB.FirstOrDefault(x => x.Title.Trim().ToLower() == Title.Trim().ToLower());

            if (Server != null)
            {
                var a = new ServerDB { Id=Server.Id,Title = Server.Title.Trim(), Country = Server.Country, City = Server.City,Ip=Server.Ip };
                return Ok(a);
            }
            return BadRequest();
        }

        [HttpGet("ServerAll")]
        public IActionResult ServerAll()//Вернуть информацию по конкретному серверу
        {
            UserAuth db = new UserAuth();

            List<ServerDB> Server = db.ServerDB.AsEnumerable().ToList();
            
            if (Server != null)
            {
                return Ok(Server);
            }
            return BadRequest();
        }
        [HttpPost("CheckName")]
        public IActionResult CheckName([FromBody] string Name)//Проверка занятого имени, если имя есть то вернуть Name+Count(Name)
        {
            UserAuth db = new UserAuth();

            var user = db.UsersDB.FirstOrDefault(x => x.Name == Name);

            if (user == null)
                return Ok($"Name");
            
            int temp = db.UsersDB.Where(a => a.Name == Name).Count();
            if (temp >= 1) Name += $"({temp + 1})";
            return Ok(Name);
        }

    }
}
