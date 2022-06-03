﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerAuthorization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServ.DataBase
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class ServerController : ControllerBase
    {
        //[HttpPost("RoomAdd")]
        //public IActionResult RoomAdd([FromBody] StandingRoomModel Info)//Добавление сервера
        //{
        //    DB db = new DB();

        //    var Server = db.Rooms.FirstOrDefault(x => x.Title.Trim().ToLower() == Info.Title.Trim().ToLower());

        //    if (Server == null)
        //    {
        //        db.Rooms.Add(new StandingRoomDB { Title = Info.Title, Password = Info.Password });
        //        db.SaveChanges();
        //        return Ok();
        //    }
        //    return BadRequest();
        //}

        //[HttpPost("ServerGetInfo")]
        //public IActionResult ServerGetInfo([FromBody] string Title)//Вернуть информацию по конкретному серверу
        //{
        //    DB db = new DB();

        //    var Server = db.ServerDB.FirstOrDefault(x => x.Title.Trim().ToLower() == Title.Trim().ToLower());

        //    if (Server != null)
        //    {
        //        return Ok(Server);
        //    }
        //    return BadRequest();
        //}

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