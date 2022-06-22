using Aspose.Zip;
using Aspose.Zip.Saving;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace KeyGenerator
{
    
    class Programg
    {
        static void Main(string[] args)
        {
            try
            {
                Directory.Delete("Keys", true);
            }
            catch { }

            using var key = RSA.Create();
            var privateKey = Convert.ToBase64String(key.ExportRSAPrivateKey());
            var publicKey = Convert.ToBase64String(key.ExportRSAPublicKey());

            var privateKeyJson = JsonSerializer.Serialize(new
            {
               Private = privateKey
            }, new JsonSerializerOptions { WriteIndented = true });

            var publicKeyJson = JsonSerializer.Serialize(new
            {
               Public = publicKey
            }, new JsonSerializerOptions { WriteIndented = true });

            Directory.CreateDirectory("Keys");
            File.WriteAllText(@"Keys\PublicKey.json", publicKeyJson);
            File.WriteAllText(@"Keys\PrivateKey.json", privateKeyJson);

            restart:
            Console.WriteLine("Зашифровать приватный ключ? Y-Да N-нет");

            switch (Console.ReadLine().ToUpper()) 
            {
                case "Y":
                    Console.Write("Введите пароль:");

                    using (FileStream zipFile = File.Open(@"Keys\PasswordPrivateKey.zip", FileMode.Create))
                    {
                        using (FileStream source1 = File.Open(@"Keys\PrivateKey.json", FileMode.Open, FileAccess.Read))
                        {
                            using (var archive = new Archive(new ArchiveEntrySettings(null, new AesEcryptionSettings(Console.ReadLine(), EncryptionMethod.AES256))))
                            {
                                archive.CreateEntry("PrivateKey.json", source1);
                                archive.Save(zipFile);
                            }
                        }
                    }
                    File.Delete("Keys/PrivateKey.json");
                    Console.WriteLine("Ключи созданы");
                    break;
                case "N":
                    Console.WriteLine("Ключи созданы");
                    break;
                default:
                    Console.WriteLine("Ввод неверный");
                    goto restart;
            }
            Console.ReadLine();
        }
    }
}
