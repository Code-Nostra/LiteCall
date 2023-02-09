using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MainServer.Token
{
    public static class AuthOptions
    {
        internal const int Lifetime = 10; // время жизни токена - 10 минут
        internal const string Issuer = "LiteCall"; //издатель токена
        internal const string Audience = "ClientLiteCall"; //потребитель токена

        public static SecurityKey PrivateKey;

        public static void SetKey(string _PrivateKey)
        {
            var key = RSA.Create();
            key.ImportRSAPrivateKey(source: Convert.FromBase64String(_PrivateKey), bytesRead: out int _);
            PrivateKey = new RsaSecurityKey(key);
        }
    }


}
