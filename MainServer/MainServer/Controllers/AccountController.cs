using DAL.Entities;
using MainServer.Attributes;
using MainServer.DAL.Interfaces;
using MainServer.Models.Captcha;
using MainServer.Models.ViewModels;
using MainServer.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MainServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[ApiKey]
	public class AccountController : ControllerBase
	{
		private readonly ILogger<AccountController> _logger;
		private readonly IUserRepository _userRepository;
		private readonly IConfiguration _config;
		public AccountController(ILogger<AccountController> logger, IConfiguration configuration, IUserRepository userRepository)
		{
			_logger = logger;
			_config = configuration;
			_userRepository = userRepository;
		}

		[HttpGet("Get")]
		public async Task<IEnumerable<User>> Get()
		{
			return await _userRepository.GetAll();
		}


		[HttpPost("Authorization")]
		public async Task<IActionResult> AuthorizationAsync([FromBody] LoginViewModel authModel)//Авторизация
		{
			var user = await _userRepository.FindByName(authModel.Login);

			if (string.IsNullOrEmpty(authModel.Password))
				return Ok(GetJwt(new User { Login = authModel.Login, Role = "Anonymous" }));

			if (user == null || user.Password != authModel.Password)
				return Unauthorized("Invalid login or password");

			if (user != null && user.Password == authModel.Password)
				return Ok(GetJwt(user));

			return BadRequest("Account not found");
		}
		//return new StatusCodeResult(1);//(int)response.StatusCode==1
		[HttpPost("Registration")]
		public async Task<IActionResult> RegistrationAsync([FromBody] RegisterViewModel RegModel)
		{
			string ServerCaptcha = HttpContext.Session.GetString("code");

			if (RegModel.Captcha != ServerCaptcha)
				return BadRequest("Captcha was not correct");

			var user = await _userRepository.FindByName(RegModel.Login);

			if (user != null) return Conflict($"User name {RegModel.Login} is already taken");

			var questions = await _userRepository.GetSequrityQuestionById(RegModel.QuestionsId);

			if (questions == null) return BadRequest("Secret question not found");

			var newUser = new User
			{
				Login = RegModel.Login.Trim(),
				Password = RegModel.Password,
				Role = "User",
				SecurityQuestionId = questions.id,
				AnswerSecurityQ = RegModel.AnswersecurityQ
			};
			var result = await _userRepository.AddNewUser(newUser);

			return Ok(GetJwt(newUser));
		}
		[HttpPost("СhangePasswordbySecurityQuestions")]
		public async Task<IActionResult> СhangePasswordbySecurityQuestionsAsync([FromBody] ChangePassword ChangModel)
		{
			var user = await _userRepository.FindByName(ChangModel.Login);
			if (user == null) return BadRequest("Account not found"); ;

			if (user.SecurityQuestionId == ChangModel.QuestionsId && user.AnswerSecurityQ.ToLower() == ChangModel.AnswersecurityQ.ToLower())
			{
				user.Password = ChangModel.newPassword;

				await _userRepository.Update(user);

				return Ok("Your password has been successfully changed");
			}
			return Unauthorized("Wrong answer");
		}

		[HttpPost("AddRole")]
		public async Task<IActionResult> AddRoleAsync([FromBody] EditRoles addRole)
		{
			var opUser = await _userRepository.FindByName(addRole.roleTaker);

			if (opUser == null) return BadRequest("Пользователь с данным именем не найден");

			var admin =await _userRepository.FindByName(addRole.Login);

			if (admin == null || admin.Password != addRole.Password || admin.Role != "Admin")
				return Unauthorized("Неверный логин или пароль");

			if (addRole.roleTaker != "Admin" && (addRole.Role == "User" || addRole.Role == "Moderator"))
			{
				opUser.Role = addRole.Role;
				await _userRepository.Update(opUser);
				return Ok("Роль успешно установлена");
			}
			return BadRequest("Роль может быть только User или Moderator");
		}
		
		//[HttpGet("SecurityQuestions")]
		//public IActionResult SecurityQuestions()
		//{
		//	//return Ok(db.SecurityQuestions.Select(x => new { x.id, x.Questions }).ToList());
		//}


		[HttpPost("CaptchaGenerator")]
		public ActionResult Captcha([FromBody] string guid)
		{
			if (string.IsNullOrEmpty(guid)) return BadRequest();
			string code = new Random(DateTime.Now.Millisecond).Next(1000, 9999).ToString();

			HttpContext.Session.SetString("code", code);

			this.Response.Clear();
			return Ok(ImagePacket.GetImage(code, 60, 30));
		}
		private string GetJwt(User _User)
		{
			var now = DateTime.UtcNow;
			var jwt = new JwtSecurityToken(
				issuer: AuthOptions.Issuer,
				audience: AuthOptions.Audience,
				notBefore: now,
				claims: GetClaims(_User),
				expires: now.AddMinutes(AuthOptions.Lifetime),
				signingCredentials: new SigningCredentials(AuthOptions.PrivateKey, SecurityAlgorithms.RsaSha256)
				);
			var tokenString = new JwtSecurityTokenHandler().WriteToken(jwt);

			return tokenString;
		}

		private IEnumerable<Claim> GetClaims(User _User)
		{
			var claims = new List<Claim>
				 {
				 new Claim("Name", _User.Login),
				 new Claim(ClaimTypes.Role,_User.Role)
				 };
			return claims;
		}
	}
}

