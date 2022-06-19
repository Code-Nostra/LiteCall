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
        readonly static byte[] entropy = { 1, 2, 3, 4, 5, 6 };
        private static string Encrypt(string text)
        {
            // first, convert the text to byte array 
            byte[] originalText = Encoding.Unicode.GetBytes(text);

            // then use Protect() to encrypt your data 
            byte[] encryptedText = ProtectedData.Protect(originalText, entropy, DataProtectionScope.CurrentUser);

            //and return the encrypted message 
            return Convert.ToBase64String(encryptedText);
        }
        private static string Decrypt(string text)
        {
            // the encrypted text, converted to byte array 
            byte[] encryptedText = Convert.FromBase64String(text);

            // calling Unprotect() that returns the original text 
            byte[] originalText = ProtectedData.Unprotect(encryptedText, entropy, DataProtectionScope.CurrentUser);

            // finally, returning the result 
            return Encoding.Unicode.GetString(originalText);
        }
        static void Main(string[] args)
        {
            Process.Start(@"C:\Program Files\Google\Chrome\Application\chrome");


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
