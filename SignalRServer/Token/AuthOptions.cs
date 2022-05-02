using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using SignalRServ;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthorizationServ.Token
{
    public class AuthOptions
    {
        internal const int Lifetime = 10; // время жизни токена - 1 минута
        internal const string Issuer = "LiteCall"; //издатель токена
        internal const string Audience = "ClientLiteCall"; //потребитель токена



        public static SecurityKey PublicKey;
        public static SecurityKey Certificate;

        public static void SetKey(string _PublicKey)
        {
            var key = RSA.Create();
            key.ImportRSAPublicKey(source: Convert.FromBase64String(_PublicKey), bytesRead: out int _);
            PublicKey = new RsaSecurityKey(key);
        }
        public static void SetCertificate(string _Certificate= "MIIBCgKCAQEA7LmG6YxF3TqGjdzqSAg16EcebRHvRpQ77MtMkK0Q9h8gWG/vHcsDV4\u002Bkja2P8WHFCyqjIW2teYKr1e6RsUQox/kdgbwcPXyV3bkIWXNNfH5Ky2uyXjEMSWFYL7PhEcC5zqAascyuIrVQVuF3Vh/YGt3etCrxG8WnrKYQZlO8aS36bcMnv9Es5cMSOPaaOVd3nVwoNp33YIW1jkB2QMf5wR/5Z3iaiLLcyNV\u002B\u002B1c4stvyvgLaXMNRTrUr0a\u002Bp9vXp1812m2zdoN0Jay\u002B52Xj5lG4L5r33Rmn3hDrVanYSkdxhI8WEaXU4qKEXoamIUzx2C03W/1gJLf\u002B/ybsw9TA2kQIDAQAB")
        {
            var key = RSA.Create();
            key.ImportRSAPublicKey(source: Convert.FromBase64String(_Certificate), bytesRead: out int _);
            Certificate = new RsaSecurityKey(key);
        }
    }


}
