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
        string currentDirectory = AppContext.BaseDirectory;
        string keysAuth = Path.Combine(AppContext.BaseDirectory, @"..\ServerAuthorization\files\Key");
        string keyChat = Path.Combine(AppContext.BaseDirectory, @"..\ServerChat\files\Key");
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
            Description="Смена порта" ,
            UsageLines = new[]
            {
            "-p 43891 ServerAuthorization",
            "%AppName% %CmdPath% 43891 ServerAuthorization"
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
            Description="Смена ip" ,
            UsageLines = new[]
            {
            "-p 43891 ServerChat",
            "%AppName% %CmdPath% 186.23.12.52 ServerChat"
            },
            ExtendedHelpText = "")]
        public void ChangeIp([Operand(Description = "Новый ip")] string ip, [Operand(Description = "Имя сервера")] string server)
        {
            if(server== "ServerAuthorization")
            {
                Console.WriteLine("Смена IP доступна только для ServerChat");
                return;
            }
            //Console.WriteLine("ServerAutorization располагается на этом компьютере?");
            //switch (Console.ReadLine().ToUpper())
            //{
            //    case "Y":
            //        ip = "localhost";
            //        break;
            //    case "N":
            //        Console.WriteLine("Ключи созданы");
            //        break;
            //    default:
            //        Console.WriteLine("Ввод неверный");
            //        break;
            //}
            Server srv = new Server(server);
            if (!srv.Valid) return;
            srv.SetIP(ip);
            srv.Save();
        }
        [Command("DefaultSettings",
            Description = "Применить стандартные настройки",
            UsageLines = new[]
            {
                    "DefaultSettings {имя сервера}",
                    "без указания имени сервера применяются стандартные насройки для всех серверов",
                    "%AppName% %CmdPath% ServerAuthorization"
            },
            ExtendedHelpText = "")]
        public void DefaultSettings( [Operand(Description = "Имя сервера")] string? server)
        {
            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, @"..\ServerAuthorization\files"));
            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, @"..\ServerChat\files"));
            CreateKeys();
            if (server== "ServerAuthorization" || string.IsNullOrEmpty(server))
            {
                try
                {
                    Settings temp = new Settings { urls = "0.0.0.0:43891", IPchat = "localhost:43893" };
                    string path = Path.Combine(AppContext.BaseDirectory, $@"..\ServerAuthorization\files\ServerAuthorization.json");
                    File.WriteAllText(path, JsonSerializer.Serialize<Settings>(temp, new JsonSerializerOptions { WriteIndented = true, IgnoreNullValues = true }));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            if (server == "ServerChat" || string.IsNullOrEmpty(server))
            {
                try
                {
                    Settings temp = new Settings { urls = "0.0.0.0:43893" };
                    string path = Path.Combine(AppContext.BaseDirectory, $@"..\ServerChat\files\ServerChat.json");
                    File.WriteAllText(path, JsonSerializer.Serialize<Settings>(temp, new JsonSerializerOptions { WriteIndented = true, IgnoreNullValues = true }));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            

        }

        [Command("info",
           Description = "Вывод информации о серверах",
           UsageLines = new[]
           {
                    "info {имя сервера}",
                    "без указания имени сервера выводится информация для всех серверов",
                    "%AppName% %CmdPath% ServerAuthorization"
           },
           ExtendedHelpText = "")]
        public void Info([Operand(Description = "Имя сервера")] string? server)
        {

            if (string.IsNullOrEmpty(server))
            {
                Server server1 = new Server("ServerAuthorization");
                server1.Show();
                server1 = new Server("ServerChat");
                Console.WriteLine();
                server1.Show();
                return;
            }
            Server curren = new Server(server);
            curren.Show();
            Console.WriteLine();


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
                path = Path.Combine(AppContext.BaseDirectory, $@"..\{serverName}\files\{serverName}.json");
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
                    anotherServer.data.IPchat = (ip== "0.0.0.0"?"localhost:"+ _port : data.urls);
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
                //data.urls = _ip + ":" + port;
                if (serverName == ServerChat)
                {
                    if (_ip == "0.0.0.0") _ip = "localhost";
                    anotherServer.data.IPchat = _ip + ":" + port;
                    anotherServer.Save();
                }
            }
            //data.urls = _ip + ":" + port;

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
            return File.Exists(Path.Combine(AppContext.BaseDirectory, $@"..\{nameFile}\files\{nameFile}.json"));
        }

        public override string ToString()
        {
            return (ip + ":" + port);
        }
        public void Save()
        {
            File.WriteAllText(path, JsonSerializer.Serialize<Settings>(data, new JsonSerializerOptions { WriteIndented = true, IgnoreNullValues = true }));
        }
        public void Show()
        {
            if (serverName == ServerAuthorization)
            {
                Console.WriteLine(ServerAuthorization+":");
                Console.WriteLine($"IP:{this}\n" +
                    $"IPchat:{Cip}:{Cport}");
            }
            if (serverName == ServerChat)
            {
                Console.WriteLine(ServerChat+":");
                Console.WriteLine($"IP:{this}\n");
            }
        }
    }

}
