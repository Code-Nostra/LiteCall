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

        //public static SecurityKey PublicKey=GetPublicKey();    
        public static SecurityKey PrivateKey;//= GetPrivateKey();

        public static void SetKey(string _PrivateKey)
        {
            var key = RSA.Create();
            key.ImportRSAPrivateKey(source: Convert.FromBase64String(_PrivateKey), bytesRead: out int _);
            PrivateKey = new RsaSecurityKey(key);
        }

        #region
        //private static SecurityKey GetPrivateKey(out string PrK)
        //{

        //    //PrK = GenerationKey().Item1;
        //   // key.ImportRSAPrivateKey(source: Convert.FromBase64String(PrK), bytesRead: out int _);

        //    //var json = JsonSerializer.Serialize(new
        //    //{
        //    //    Public = Convert.FromBase64String(PrivateKeyString)

        //    //}, new JsonSerializerOptions { WriteIndented = true });

        //    //File.AppendAllText("rsaKey.json", json);

        //    //return new RsaSecurityKey(key);

        //}

        //private static SecurityKey GetPublicKey(out string PK)
        //{
        //    //key = RSA.Create();
        //    //PK = GenerationKey().Item2;
        //    //key.ImportRSAPublicKey(source: Convert.FromBase64String(PK), bytesRead: out int _);

        //    //var json = JsonSerializer.Serialize(new
        //    //{
        //    //    Public = Convert.FromBase64String(PublicKeyString)

        //    //}, new JsonSerializerOptions { WriteIndented = true });

        //    //File.WriteAllText("rsaKey.json", json);

        //    //return new RsaSecurityKey(key);
        //}


        //private static Tuple<string,string> GenerationKey()
        //{
        //    using var key = RSA.Create();
        //    var privateKey = Convert.ToBase64String(key.ExportRSAPrivateKey());
        //    var publicKey = Convert.ToBase64String(key.ExportRSAPublicKey());
        //    Console.WriteLine("publicKey"+publicKey.ToString());
        //    return Tuple.Create(privateKey, publicKey);
        //}

        //private static SecurityKey GetPrivateKey()
        //{
        //    var key = RSA.Create();
        //    key.ImportRSAPrivateKey(source: Convert.FromBase64String(PrivateKeyString), bytesRead: out int _);
        //    return new RsaSecurityKey(key);
        //}

        //private static SecurityKey GetPublicKey()
        //{ 
        //    var key = RSA.Create();

        //    key.ImportRSAPublicKey(source: Convert.FromBase64String(PublicKeyString), bytesRead: out int _);
        //    return new RsaSecurityKey(key);
        //}
        #endregion
    }


}
