using System;
using System.Collections.Generic;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServerAuthorization.Captcha;

namespace AuthorizationServ.Token
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("token")]
        public IActionResult Token([FromBody] AuthModel authModel)//Авторизация
        {
            if (authModel.Captcha != SessionClass.Session[authModel.Guid])
            {
                return BadRequest("Капча неверна");
            }

            UserAuth db = new UserAuth();

            var user = db.UsersDB.FirstOrDefault(x => x.Name == authModel.Login);

            if (user != null && user.Password != authModel.Password && authModel.Password != "X")
                return BadRequest();

            if (authModel.Password == "X")
                return Ok(GetJwt(new UserDB { Name = "Anonymous", Role = "Anonymous" }));

            if (user != null && user.Password == authModel.Password)
                return Ok(GetJwt(user));

            return BadRequest();
        }
        [HttpPost("Registration")]
        public IActionResult Registration([FromBody] AuthModel authModel)//Регистрация
        {
            if (authModel.Captcha != SessionClass.Session[authModel.Guid])
            {
                return BadRequest("Капча неверна");
            }

            UserAuth db = new UserAuth();
            var user = db.UsersDB.SingleOrDefault(a => a.Name.ToLower() == authModel.Login.Trim().ToLower());
            if (user != null) return BadRequest();

            var NewUser = db.UsersDB.Add(new UserDB { Name = authModel.Login.Trim(), Password = authModel.Password, Role = "User" });
            db.SaveChanges();
            return Ok(GetJwt(NewUser));
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
                new Claim("Name", User.Name),
                new Claim(ClaimTypes.Role,User.Role)
            };
            return claims;
        }
        [HttpPost("CaptchaGenerator")]
        public ActionResult Captcha([FromBody] string guid)
        {
            string code = new Random(DateTime.Now.Millisecond).Next(1111, 9999).ToString();

            SessionClass.Session.TryAdd(guid, code);
            SessionClass.Session[guid] = code;
            #region Сессия
            // HttpContext.Session.SetString(Guid.NewGuid().ToString(), code);
            // HttpContext.Session.SetString(guid.ToString(), code);
            #endregion

            CaptchaImage captcha = new CaptchaImage(code, 60, 30);

            this.Response.Clear();

            Image image = captcha.Image;
            // conver image to bytes
            byte[] img_byte_arr = ImageMethod.ImageToBytes(image);
            // creat packet
            ImagePacket packet = new ImagePacket(img_byte_arr);

            var json = JsonSerializer.Serialize(packet);

            var json2 = JsonSerializer.Serialize<ImagePacket>(packet);

            return Ok(json2);
            captcha.Dispose();

        }

    }
}

