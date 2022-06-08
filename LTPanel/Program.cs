using Aspose.Zip;
using Aspose.Zip.Saving;
using CommandDotNet;
using CommandDotNet.Help;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace LTPanel
{
    [Command(
    Description = "Performs mathematical calculations",
    ExtendedHelpTextLines = new[]
    {
        "Include multiple lines of text",
        "Extended help of the root command is a good place to describe directives for the app"
    })]
    public class Program
    {
        static int Main(string[] args) =>
            new AppRunner<Program>(new AppSettings { Help = { TextStyle = HelpTextStyle.Basic } }).Run(args);

        [Command(
            Description = "Creating Encryption Keys",
        //UsageLines = new[]
        //    {
        //    "Add 1 2",
        //    "%AppName% %CmdPath% 1 2"
        //    },
            ExtendedHelpText = "Создание приватного ключа для ServerAuthorization и публичного для ServerChat")]
        public void CreateKeys()

            //[Operand(Description = "first value"),] int x,
            //[Operand(Description = "second value"), AppSetting("Add")] int y)
            {
            try
            {
                File.Delete(Path.Combine(Directory.GetCurrentDirectory(), @"..\ServerAuthorization\files\Key\PrivateKey.json"));

                File.Delete(Path.Combine(Directory.GetCurrentDirectory(), @"..\ServerAuthorization\files\Key\PublicKey.json"));
            }
            catch{ }

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

            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),@"..\ServerAuthorization\files\Key"));
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),@"..\ServerChat\files\Key"));
            try
            {
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), @"..\ServerAuthorization\files\Key\PrivateKey.json"), privateKeyJson );
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), @"..\ServerChat\files\Key\PublicKey.json"), publicKeyJson);
            }
            catch { }


        restart:
            Console.WriteLine("Зашифровать приватный ключ для переноса на удалённый сервер?");
            Console.Write("Yes/No? [Y/n] ");
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
                    File.Delete(Path.Combine(Directory.GetCurrentDirectory(), @"..\ServerAuthorization\files\Key\PublicKey.json"));
                    Console.WriteLine("Ключи созданы");
                    break;
                case "N":
                    Console.WriteLine("Ключи созданы");
                    break;
                default:
                    Console.WriteLine("Ввод неверный");
                    goto restart;
            }
        }
        
        [Command(Description = "Subtracts two numbers")]
        public void Subtract(int x, int y) => Console.WriteLine(x - y);

    }
}
