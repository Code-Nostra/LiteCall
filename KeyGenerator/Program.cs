using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace KeyGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            using var key = RSA.Create();
            var privateKey = Convert.ToBase64String(key.ExportRSAPrivateKey());
            var publicKey = Convert.ToBase64String(key.ExportRSAPublicKey());

            var json = JsonSerializer.Serialize(new
            {
                Public = publicKey,
                Private = privateKey
            }, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText("rsaKey.json", json);
        }
    }
}
