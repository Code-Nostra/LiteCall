using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ServerAuthorization.Attributes;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using ServerAuthorization.Models.ViewModels;
using ServerAuthorization.Models;
using DAL.UnitOfWork.ServerAuthorization;
using System.Threading.Tasks;
using AutoMapper;

namespace ServerAuthorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class ServerController : ControllerBase
    {
        private readonly IConfiguration config;
        private readonly IUnitOfWorkAuth _unitOfWork;
		private readonly IMapper mapper;
		public ServerController(IConfiguration configuration, IUnitOfWorkAuth unitOfWork, IMapper _mapper)
        {
            config = configuration;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("ServerGetInfo")]
        public async Task<IActionResult> ServerGetInfoAsync()//Вернуть информацию
        {
            var Server = await _unitOfWork.Servers.GetFirstDefault();

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
				await _unitOfWork.SaveChangesAsync();
				return Ok(Server);
            }
            else return BadRequest();
        }

        [HttpPost("ServerSetInfo")]
        public async Task<IActionResult> ServerSetInfo([FromBody] ServerInformation newServerInfo)
        {
            var admin = await _unitOfWork.Users.FindByName(newServerInfo.Login);

            if (admin == null || admin.Role != "Admin" || admin.Password != newServerInfo.Password)
                return Unauthorized("\nНеверный логин или пароль");

            var oldServerInfo = await _unitOfWork.Servers.GetFirstDefault();

            if (oldServerInfo != null)
            {
				mapper.Map(newServerInfo, oldServerInfo);

				try
                {
                    string path = Path.Combine(AppContext.BaseDirectory, @"..\files\ServerAuthorization.json");
                    string requestBody = System.IO.File.ReadAllText(path);
                    ConfigSettings data = JsonSerializer.Deserialize<ConfigSettings>(requestBody);
                    data.IPchat = oldServerInfo.Ip;
                    System.IO.File.WriteAllText(path, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true, IgnoreNullValues = true }));

                    path = Path.Combine(AppContext.BaseDirectory, @"..\..\files\ServerChat.json");
                    requestBody = System.IO.File.ReadAllText(path);
                    data = JsonSerializer.Deserialize<ConfigSettings>(requestBody);
                    data.IPchat = oldServerInfo.Ip;
                    System.IO.File.WriteAllText(path, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true, IgnoreNullValues = true }));
                }
                catch (Exception e)
                {
                }
                await _unitOfWork.SaveChangesAsync();
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
