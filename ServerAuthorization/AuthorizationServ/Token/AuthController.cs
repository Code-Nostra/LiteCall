using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace AuthorizationServ.Token
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("token")]
        public IActionResult Token([FromBody] AuthModel authModel,bool isReg)//Авторизация
        {

            UserAuth db = new UserAuth();

            var user = db.UsersDB.FirstOrDefault(x => x.Name == authModel.Login);

            if (user == null)
                return BadRequest();

            if (user.Password != authModel.Password)
                return BadRequest();
            
            var token = GetJwt(user);
            
            return Ok(token);
        }
        [HttpPost("Registration")]
        public IActionResult Registration([FromBody] AuthModel authModel)//Регистрация
        {
            UserAuth db = new UserAuth();
            var user = db.UsersDB.SingleOrDefault(a => a.Name.ToLower() == authModel.Login.Trim().ToLower());
            if (user != null) return BadRequest();
            
            var NewUser=db.UsersDB.Add(new UserDB { Name = authModel.Login.Trim(), Password = authModel.Password, Role = "User" });
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
               // new Claim(ClaimTypes.Hash, AuthOptions.PrK),
            };
            return claims;
        }
    }
}

