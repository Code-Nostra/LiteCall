using System;
using System.Collections.Generic;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServerAuthorization.Attributes;
using ServerAuthorization.Captcha;

namespace AuthorizationServ.Token
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class AuthController : ControllerBase
    {
        [HttpPost("Authorization")]
        public IActionResult Authorization([FromBody] AuthModel authModel)//Авторизация
        {
            //if (authModel.Captcha != SessionClass.Session[authModel.Guid])
            //{
            //    return BadRequest("Капча неверна ");
            //}g
            DB db = new DB();

            var user = db.Users.FirstOrDefault(x => x.Login == authModel.Login);
            if(user==null && (authModel.Password==null|| authModel.Password == string.Empty)) 
                return Ok(GetJwt(new UserDB { Login = authModel.Login, Role = "Anonymous" }));

            if (user != null && user.Password != authModel.Password)
                return Unauthorized("Invalid login or password");

            if (user != null && user.Password == authModel.Password)
                return Ok(GetJwt(user));

            return Unauthorized("Invalid login or password");
        }
        //return new StatusCodeResult(1);//(int)response.StatusCode==1
        [HttpPost("Registration")]
        public IActionResult Registration([FromBody] AuthModel authModel)//Регистрация
        {
            if (authModel.Guid == null) return NoContent();
            string ServerCaptcha;
            SessionClass.Session.TryGetValue(authModel.Guid, out ServerCaptcha);
            
                try
                {
                    if (authModel.Captcha != ServerCaptcha)
                    {
                        return BadRequest("Captcha was not correct");
                    }
                }
                catch
                {
                    return BadRequest("Captcha was not correct");
                }
            SessionClass.Session.Remove(authModel.Guid);
            DB db = new DB();
            var user = db.Users.SingleOrDefault(a => a.Login.ToLower() == authModel.Login.Trim().ToLower());
            if (user != null) return Conflict(($"User name {0} is already taken",authModel.Login));
            //!!!
            var NewUser = db.Users.Add(new UserDB { Login = authModel.Login.Trim(), 
                Password = authModel.Password, Role = "User" }).Entity;
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

            
            //JsonObject key = JsonNode.Parse(new { F=""});
            try
            {
                var key= JsonNode.Parse(System.IO.File.ReadAllText("ServerAuthorization.json"));
                var claims = new List<Claim>
                {
                new Claim("Name", User.Login),
                new Claim(ClaimTypes.Role,User.Role),
                new Claim("IP",(string)((key["IPchat"]==null|| (string)key["IPchat"]==string.Empty)?"Set the IP of the chat server using IPchat":(string)key["IPchat"]))
                };
                return claims;
            }
            
            catch 
            {
                var claims = new List<Claim>
                {
                new Claim("Name", User.Login),
                new Claim(ClaimTypes.Role,User.Role),
                new Claim("IP","Set the IP of the chat server using IPchat")
                };
                return claims;
            }
            //AuthOptions.SetKey((string)key["IPchat"]);

            
        }
        [HttpPost("CaptchaGenerator")]
        public ActionResult Captcha([FromBody] JsonElement guid)
        {
            string code = new Random(DateTime.Now.Millisecond).Next(1111, 9999).ToString();

            SessionClass.Session.TryAdd(guid.GetProperty("guid").GetString(), code);
            //SessionClass.Session[guid.ToString()] = code;
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
            var json = JsonSerializer.Serialize<ImagePacket>(packet);
            image.Dispose();
            return Ok(json);
        }

    }
}

