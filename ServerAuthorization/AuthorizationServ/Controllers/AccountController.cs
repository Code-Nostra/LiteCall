using AuthorizationServ.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ServerAuthorization.Attributes;
using ServerAuthorization.Models;
using ServerAuthorization.Models.Captcha;
using ServerAuthorization.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace ServerAuthorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext db;
        private readonly IConfiguration config;
        public AccountController(ILogger<AccountController> logger, IConfiguration configuration, ApplicationDbContext database)
        {
            _logger = logger;
            config = configuration;
            db = database;
        }

        [HttpPost("Authorization")]
        public IActionResult Authorization([FromBody] LoginViewModel authModel)//Авторизация
        {
            var user = db.Users.FirstOrDefault(x => x.Login == authModel.Login);
            if (string.IsNullOrEmpty(authModel.Password) || authModel.Password?.ToString() == "null")
                return Ok(GetJwt(new UserDB { Login = authModel.Login, Role = "Anonymous" }));
            
            if (user == null || user.Password != authModel.Password)
                return Unauthorized("Invalid login or password");

            if (user != null && user.Password == authModel.Password)
                return Ok(GetJwt(user));

            return BadRequest("Account not found");
        }

        [HttpPost("Registration")]
        public IActionResult Registration([FromBody] RegisterViewModel RegModel)
        {
            string ServerCaptcha = HttpContext.Session.GetString("code");

            if (RegModel.Captcha != ServerCaptcha)
                return BadRequest("Captcha was not correct");
            var user = db.Users.FirstOrDefault(a => a.Login.ToLower().ToLower() == RegModel.Login.Trim().ToLower());
            if (user != null) return Conflict($"User name " + RegModel.Login + " is already taken");

            var questions = db.SecurityQuestions.ToList().FirstOrDefault(x => x.id == RegModel.QuestionsId);
            if (questions == null) return BadRequest("Secret question not found");


            var NewUser = db.Users.Add(new UserDB
            {
                Login = RegModel.Login.Trim(),
                Password = RegModel.Password,
                Role = "User",
                SecurityQuestionId = questions.id,
                AnswerSecurityQ = RegModel.AnswersecurityQ
            }).Entity;

            db.SaveChanges();

            return Ok(GetJwt(NewUser));
        }
        [HttpPost("СhangePasswordbySecurityQuestions")]
        public IActionResult СhangePasswordbySecurityQuestions([FromBody] ChangePassword ChangModel)
        {
            var user = db.Users.SingleOrDefault(a => a.Login.ToLower() == ChangModel.Login.Trim().ToLower());
            if (user == null) return BadRequest("Account not found"); ;

            if (user.SecurityQuestionId == ChangModel.QuestionsId && user.AnswerSecurityQ.ToLower() == ChangModel.AnswersecurityQ.ToLower())
            {
                user.Password = ChangModel.newPassword;
                db.SaveChanges();
                return Ok("Your password has been successfully changed");
            }
            return Unauthorized("Wrong answer");
        }

        [HttpPost("AddRole")]
        public IActionResult AddRole([FromBody] EditRoles addRole)
        {
            var opUser = db.Users.FirstOrDefault(x => x.Login == addRole.roleTaker);

            if (opUser == null) return BadRequest("Пользователь с данным именем не найден");

            var admin = db.Users.FirstOrDefault(x => x.Login == addRole.Login);

            if (admin == null || admin.Password != addRole.Password || admin.Role != "Admin")
                return Unauthorized("Неверный логин или пароль");

            if (addRole.roleTaker != "Admin" && (addRole.Role == "User" || addRole.Role == "Moderator"))
            {
                opUser.Role = addRole.Role;
                db.SaveChanges();
                return Ok("Роль успешно установлена");
            }
            return BadRequest("Роль может быть только User или Moderator");
        }
        [HttpGet("SecurityQuestions")]
        public IActionResult SecurityQuestions()
        {
            return Ok(db.SecurityQuestions.Select(x => new { x.id, x.Questions }).ToList());
        }
        #region Сессия
        #endregion
        [HttpPost("CaptchaGenerator")]
        public ActionResult Captcha([FromBody] string guid)
        {
			if (string.IsNullOrEmpty(guid)) return BadRequest();
			string code = new Random(DateTime.Now.Millisecond).Next(1000, 9999).ToString();

			HttpContext.Session.SetString("code", code);

			this.Response.Clear();
			return Ok(ImagePacket.GetImage(code, 60, 30));
		}

        private string GetJwt(UserDB User)
        {
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: AuthOptions.Audience,
                notBefore: now,
                claims: GetClaims(User),
                expires: now.AddMinutes(AuthOptions.Lifetime),
                signingCredentials: new SigningCredentials(AuthOptions.PrivateKey, SecurityAlgorithms.RsaSha256)
                );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwt);

            return tokenString;
        }
        private IEnumerable<Claim> GetClaims(UserDB User)
        {
            var claims = new List<Claim>
            {
            new Claim("Name", User.Login),
            new Claim(ClaimTypes.Role,User.Role),
            new Claim("IP",config["IPchat"])
            };
            return claims;
        }
    }
}

