using Aspose.Zip;
using Aspose.Zip.Saving;
using CommandDotNet;
using CommandDotNet.Diagnostics;
using CommandDotNet.Help;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace LTPanel
{

    [Command(
    Description = "Программа для настрок серверов")]
    public class Program
    {

        static int Main(string[] args)
        {
            Debugger.AttachIfDebugDirective(args);
            return new AppRunner<Program>(new AppSettings { Help = { TextStyle = HelpTextStyle.Basic } }).Run(args);
        }
        string currentDirectory = Directory.GetCurrentDirectory();
        string keysAuth = Path.Combine(Directory.GetCurrentDirectory(), @"..\ServerAuthorization\files\Key");
        string keyChat = Path.Combine(Directory.GetCurrentDirectory(), @"..\ServerChat\files\Key");
        const string privKey = @"\PrivateKey.json";
        const string pubKey = @"\PublicKey.json";

        [Command("keys",
            Description = "Создание ключей шифрования",
            ExtendedHelpText = "Создание приватного ключа для ServerAuthorization и публичного для ServerChat")]
        public void CreateKeys()
        {
            try
            {
                File.Delete(keysAuth);
                File.Delete(keyChat);
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

            Directory.CreateDirectory(keysAuth);
            Directory.CreateDirectory(keyChat);
            try
            {
                File.WriteAllText(keysAuth+privKey, privateKeyJson);
                File.WriteAllText(keyChat+pubKey, publicKeyJson);
            }
            catch { }
        //restart:
        //    Console.WriteLine("Зашифровать приватный ключ для переноса на удалённый сервер?");
        //    Console.Write("Yes/No? [Y/n] ");
        //    switch (Console.ReadLine().ToUpper())
        //    {
        //        case "Y":
        //            Console.Write("Введите пароль:");

        //            using (FileStream zipFile = File.Open(@"Keys\PasswordPrivateKey.zip", FileMode.Create))
        //            {
        //                using (FileStream source1 = File.Open(@"Keys\PrivateKey.json", FileMode.Open, FileAccess.Read))
        //                {
        //                    using (var archive = new Archive(new ArchiveEntrySettings(null, new AesEcryptionSettings(Console.ReadLine(), EncryptionMethod.AES256))))
        //                    {
        //                        archive.CreateEntry("PrivateKey.json", source1);
        //                        archive.Save(zipFile);
        //                    }
        //                }
        //            }
        //            File.Delete(Path.Combine(currentDirectory, @"..\ServerAuthorization\files\Key\PublicKey.json"));
        //            Console.WriteLine("Ключи созданы");
        //            break;
        //        case "N":
        //            Console.WriteLine("Ключи созданы");
        //            break;
        //        default:
        //            Console.WriteLine("Ввод неверный");
        //            goto restart;
        //    }
        }

        [Command("-port",
            DescriptionLines = new[] { " ", "Смена порта" },
            UsageLines = new[]
            {
            "-p 43891 ServerAuthorization",
            "%AppName% %CmdPath% -port 43891 ServerAuthorization"
            },
            ExtendedHelpText = "")]

        public void ChangePort([Operand(Description = "Новый порт")] int port, [Operand(Description = "Имя сервера")] string server )
        {
            Server srv = new Server(server);
            if (!srv.Valid) return;
            srv.SetPort(port);
            srv.Save();
        }

        [Command("-ip",
            DescriptionLines = new[] { " ", "Смена ip" },
            UsageLines = new[]
            {
            "-p 43891 ServerAuthorization",
            "%AppName% %CmdPath% -ip 12.23.12.3 ServerAuthorization"
            },
            ExtendedHelpText = "")]
        public void ChangeIp([Operand(Description = "Новый ip")] string ip, [Operand(Description = "Имя сервера")] string server)
        {
            Server srv = new Server(server);
            if (!srv.Valid) return;
            srv.SetIP(ip);
            srv.Save();
        }

    }


    public class Settings
    {
        public string urls { get; set; }
        public string IPchat { get; set; }
    }

    class Server
    {
        public string ip;
        public string port;
        public string Cip;
        public string Cport;
        private Settings data;
        private string path;
        private const string ServerAuthorization = "ServerAuthorization";
        private const string ServerChat = "ServerChat";
        private string serverName;
        public bool Valid
        {
            get
            {
                return !(string.IsNullOrEmpty(ip) && string.IsNullOrEmpty(port));
            }
        }

        public Server(string serverName)
        {
            if (serverName != ServerAuthorization && serverName != ServerChat)
            {
                Console.WriteLine($"Сервер {serverName} не найден" +
                    $"\n Доступные сервера:\n 1.ServerAuthorization \n 2.ServerChat");
                return;
            }
            this.serverName = serverName;
            string requestBody = string.Empty;
            try
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), $@"..\{serverName}\files\{serverName}.json");
                requestBody = File.ReadAllText(path);
                data = JsonSerializer.Deserialize<Settings>(requestBody);
                data.urls = data.urls?.Replace(";", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            if (string.IsNullOrEmpty(data.urls)|| !data.urls.Contains(":") || data.urls.Length < 3)
            {
                Console.WriteLine("IP адресс не обнаружен");
                return;
            }
            else if (serverName == ServerAuthorization)
            {
                if (string.IsNullOrEmpty(data.IPchat) || !data.IPchat.Contains(":") && data.IPchat.Length < 3)
                {
                    Console.WriteLine("IP адресс чата не обнаружен");
                    return;
                }
            }


            string[] temp;
            //string IPPortPatter = @"\blocalhost:\d{1,5}\b|(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?):\d{1,5}\b";
            //Regex check = new Regex(IPPortPatter);
            //if (!check.IsMatch(data.urls, 0))
            //{
            //    Console.WriteLine($"IP адресс сервера {serverName}  не верен");
            //    return;
            //}

            temp = data.urls.Split(":");
            ip = temp[0];
            port = temp[1];
            if (serverName == ServerAuthorization)
            {
                temp = data.IPchat?.Split(":");
                Cip = temp[0];
                Cport = temp[1];

                //if (!check.IsMatch(data.IPchat, 0))
                //{
                //    Console.WriteLine($"IP адресс сервера {serverName}  не верен");
                //    return;
                //}
            }

        }

        public void SetPort(int _port)
        {
            string anotherName = serverName == ServerAuthorization ? ServerChat : ServerAuthorization;
            if (CheckFile(anotherName))
            {
                Server anotherServer = new Server(anotherName);
                if (anotherServer.ToString() == ip + ":" + _port)
                {
                    Console.WriteLine($"IP сервера {serverName} не может быть таким-же как и у {anotherName}");
                    return;
                }
                data.urls = ip + ":" + _port;
                if (serverName == ServerChat)
                {
                    anotherServer.data.IPchat = data.urls;
                    anotherServer.Save();
                }
            }
            data.urls = ip + ":" + _port;

        }
        public void SetIP(string _ip)
        {
            if (!CheckIP(_ip)) return;
            string anotherName = serverName == ServerAuthorization ? ServerChat : ServerAuthorization;
            if (CheckFile(anotherName))
            {
                Server anotherServer = new Server(anotherName);
                if (anotherServer.ToString() == _ip+":"+port)
                {
                    Console.WriteLine($"IP сервера {serverName} не может быть таким-же как и у {anotherName}");
                    return;
                }
                data.urls = _ip + ":" + port;
                if (serverName == ServerChat)
                {
                    anotherServer.data.IPchat = data.urls;
                    anotherServer.Save();
                }
            }
            data.urls = _ip + ":" + port;

        }
        public bool CheckIP(string ip)
        {
            string ipPatter = @"^\blocalhost\b$|(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b$";
            Regex check = new Regex(ipPatter);
            if (!check.IsMatch(ip, 0))
            {
                Console.WriteLine($"IP адресс неккоректен");
                return false;
            }
            return true;
        }
        public bool CheckFile(string nameFile)
        {
            return File.Exists(Path.Combine(Directory.GetCurrentDirectory(), $@"..\{nameFile}\files\{nameFile}.json"));
        }

        public override string ToString()
        {
            return (ip + ":" + port);
        }
        public void Save()
        {
            File.WriteAllText(path, JsonSerializer.Serialize<Settings>(data, new JsonSerializerOptions { WriteIndented = true, IgnoreNullValues = true }));
        }
    }

}
