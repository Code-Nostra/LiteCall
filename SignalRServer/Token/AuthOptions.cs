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

        internal const string Audience = "LiteCallChat"; //потребитель токена

        public static SecurityKey PublicKey;


        public static void SetKey(string _PublicKey)
        {
            var key = RSA.Create();
            key.ImportRSAPublicKey(source: Convert.FromBase64String(_PublicKey), bytesRead: out int _);
            PublicKey = new RsaSecurityKey(key);
        }

    }


}
