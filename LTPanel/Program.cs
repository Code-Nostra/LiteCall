using Aspose.Zip;
using Aspose.Zip.Saving;
using CommandDotNet;
using CommandDotNet.Diagnostics;
using CommandDotNet.Help;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
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
        string keysAuth = Path.Combine(AppContext.BaseDirectory, @"ServerAuthorization\files\Key");
        string keyChat = Path.Combine(AppContext.BaseDirectory, @"ServerChat\files\Key");
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
        }

        [Command("-port",
            Description="Смена порта" ,
            UsageLines = new[]
            {
            "-port 43891 ServerAuthorization",
            "%AppName% %CmdPath% 43891 ServerAuthorization"
            },
            ExtendedHelpText = "")]

        public void ChangePort([Operand(Description = "Новый порт")] int port, [Operand(Description = "Имя сервера")] string server )
        {
            //Console.WriteLine("Лучше использовать порты между 1024—49151");
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
            //if(server== "ServerAuthorization")
            //{
            //    Console.WriteLine("Смена IP доступна только для ServerChat");
            //    return;
            //}
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
            Server srv = new Server(server,true);
            //if (!srv.Valid |) return;
            srv.SetIP(ip);
            if (server != "ServerChat") srv.Save();
        }
        [Command("DefaultSettings",
            Description = "Применение стандартных настройки",
            UsageLines = new[]
            {
                    "DefaultSettings {имя сервера}",
                    "без указания имени сервера применяются стандартные насройки для всех серверов",
                    "%AppName% %CmdPath% ServerAuthorization"
            },
            ExtendedHelpText = "")]
        public void DefaultSettings( [Operand(Description = "Имя сервера")] string? server)
        {
            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, @"ServerAuthorization\files"));
            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, @"ServerChat\files"));
            CreateKeys();
            if (server== "ServerAuthorization" || string.IsNullOrEmpty(server))
            {
                try
                {
                    Settings temp = new Settings { urls = "0.0.0.0:43891", IPchat = "localhost:43893" };
                    string path = Path.Combine(AppContext.BaseDirectory, $@"ServerAuthorization\files\ServerAuthorization.json");
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
                    string path = Path.Combine(AppContext.BaseDirectory, $@"ServerChat\files\ServerChat.json");
                    File.WriteAllText(path, JsonSerializer.Serialize<Settings>(temp, new JsonSerializerOptions { WriteIndented = true, IgnoreNullValues = true }));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        [Command("InfoSettings",
           Description = "Вывод локальной информации о серверах",
           UsageLines = new[]
           {
                "info {имя сервера}",
                "без указания имени сервера выводится информация для всех серверов",
                "%AppName% %CmdPath% ServerAuthorization"
           },
           ExtendedHelpText = "")]
        public void InfoSettings([Operand(Description = "Имя сервера")] string? server)
        {
            if (string.IsNullOrEmpty(server))
            {
                Server tempServer = new Server("ServerAuthorization");
                if(tempServer.Valid) tempServer.Show();
                tempServer = new Server("ServerChat");
                Console.WriteLine();
                if (tempServer.Valid) tempServer.Show();
                return;
            }
            Server curren = new Server(server);
            if (curren.Valid) curren.Show();
            Console.WriteLine();
        }


        public class ServerInfo
        {
            public int id { get; set; }
            public string title { get; set; }
            public string country { get; set; }
            public string city { get; set; }
            public string ip { get; set; }
            public string description { get; set; }
        }
        [Command("Info",
            Description = "Вывод информации о серверах",
            UsageLines = new[]
            {
                        "info",
                        "%AppName% %CmdPath% 165.123.12.2:43893"
            },
            ExtendedHelpText = "")]
        public void Info([Operand(Description = "IP-адрес")] string IP)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            using var httpClient = new HttpClient(clientHandler);
           
            httpClient.DefaultRequestHeaders.Add("ApiKey", "ACbaAS324hnaASD324bzZwq41");
            try
            {
                var response = httpClient.GetAsync($"https://{IP}/api/Server/ServerGetInfo").Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ServerInfo srv= JsonSerializer.Deserialize<ServerInfo>(response.Content.ReadAsStringAsync().Result);
                    Console.Write("IP адрес сервера чата:"+srv.ip) ;

                    Console.Write("Название сервера чата:" + srv.title);

                    Console.Write("Страна расположения сервера чата:" + srv.country);
                    
                    Console.Write("Город расположения сервера чата:" + srv.city);
;
                    Console.Write("Описание сервера чата:" + srv.description);
                }
                else
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);


            }
            catch
            {
                Console.WriteLine("\nСервер недоступен");
            }
        }


        [Command("ResetAdminPassword",
           Description = "Сброс пароля администратора",
           UsageLines = new[]
           {
                "ResetAdminPassword {новый пароль}",
                "У вас должен быть доступ к локальной базе данных",
                "%AppName% %CmdPath% NewPassword"
           },
           ExtendedHelpText = "")]
        public void ResetAdminPassword([Operand(Description = "Имя сервера")] string password)
        {
            if (password.Length < 6)
            {
                Console.WriteLine("Длина пароля не может быть меньше 6 символов");
                return;
            }
            DB db = new DB();
            try
            {
                db.Users.FirstOrDefault(x => x.Role == "Admin").Password = GetHashSha1(GetHashSha1(password));
            }
            catch (Exception ex)
            {
                Console.WriteLine("База данных не найдена");
                
            }
            db.SaveChanges();
        }

        [Command("pex",
           Description = "Выдача привилегий",
           UsageLines = new[]
           {
                "pex {адресс сервера:порт} {имя зарегестрированного пользователя} {привилегия} ",
                "%AppName% %CmdPath% 192.156.32.12:43891 User Moderator"
           },
           ExtendedHelpText = "")]
        public void pex([Operand(Description = "IP-адрес")] string IP, [Operand(Description = "Имя того, кому устанавливаем роль")]  string Login, [Operand(Description = "Роль (Moderator)")] string Role)
        {
            string ipPatter = @"\blocalhost:\d{1,5}\b|(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?):\d{1,5}\b";
            Regex check = new Regex(ipPatter);
            if (!check.IsMatch(IP, 0))
            {
                Console.WriteLine($"IP адресс неккоректен");
                return;
            }

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            using var httpClient = new HttpClient(clientHandler);
            httpClient.Timeout = TimeSpan.FromSeconds(250);
            Console.Write("Введите логин администратора:");
            string loginAdmin = Console.ReadLine();
            if(string.IsNullOrEmpty(loginAdmin))
            {
                Console.WriteLine("Логин администратора не введен");
                return;
            }
            Console.Write("Введите пароль администратора:");

            var passwordAdmin = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && passwordAdmin.Length > 0)
                {
                    Console.Write("\b \b");
                    passwordAdmin = passwordAdmin[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    passwordAdmin += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);

            if (string.IsNullOrEmpty(passwordAdmin))
            {
                Console.WriteLine("Пароль администратора не введен");
                return;
            }

            var authModel = new { Login = loginAdmin, Password = GetHashSha1(passwordAdmin),Role=Role, OpLogin=Login };
            var json = JsonSerializer.Serialize(authModel);
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            httpClient.DefaultRequestHeaders.Add("ApiKey", "ACbaAS324hnaASD324bzZwq41");
            httpClient.Timeout = TimeSpan.FromSeconds(250);
            try
            {
                var response = httpClient.PostAsync($"https://{IP}/api/auth/AddRole", content).Result;
                Console.WriteLine();
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }
            catch
            {
                Console.WriteLine("\nСервер недоступен");
            }
        }

        [Command("ChangeInfo",
           Description = "Изменение публичной информации",
           UsageLines = new[]
           {
            "pex {адресс сервера:порт} ",
            "%AppName% %CmdPath% 165.123.12.2:43893"
           },
           ExtendedHelpText = "")]
        public void ChangeInfo([Operand(Description = "IP-адрес")] string IP)
        {
            string ipPatter = @"\blocalhost:\d{1,5}\b|(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?):\d{1,5}\b";
            Regex check = new Regex(ipPatter);

            if (!check.IsMatch(IP, 0))
            {
                Console.WriteLine($"IP адресс сервера некорректен");
                return;
            }

            Console.Write("Введите ip адрес сервера чата:");
            string Ip= Console.ReadLine();

            if (!string.IsNullOrEmpty(Ip))
            {
                if (!check.IsMatch(Ip, 0))
                {
                    Console.WriteLine($"Вводимые IP адресс сервера чата неккоректен");
                    return;
                }
            }
            Console.Write("Введите название сервера чата:");
            string Title = Console.ReadLine();
            Console.Write("Введите страну расположения сервера чата:");
            string Country = Console.ReadLine();
            Console.Write("Введите город расположения сервера чата:");
            string City = Console.ReadLine();
            Console.Write("Введите описание сервера чата:");
            string Description = Console.ReadLine();

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            using var httpClient = new HttpClient(clientHandler);
            Console.Write("Введите логин администратора:");
            string loginAdmin = Console.ReadLine();
            if (string.IsNullOrEmpty(loginAdmin))
            {
                Console.WriteLine("Логин администратора не введён");
                return;
            }
            Console.Write("Введите пароль администратора:");

            var passwordAdmin = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && passwordAdmin.Length > 0)
                {
                    Console.Write("\b \b");
                    passwordAdmin = passwordAdmin[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    passwordAdmin += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
            if (string.IsNullOrEmpty(passwordAdmin))
            {
                Console.WriteLine("Пароль администратора не введён");
                return;
            }

            var authModel = new { Login = loginAdmin, Password = GetHashSha1(passwordAdmin), Title,Country,City,Ip,Description };
            var json = JsonSerializer.Serialize(authModel, new JsonSerializerOptions { IgnoreNullValues = true });
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            httpClient.DefaultRequestHeaders.Add("ApiKey", "ACbaAS324hnaASD324bzZwq41");
            try
            {
                var response = httpClient.PostAsync($"https://{IP}/api/Server/ServerSetInfo", content).Result;

                Console.WriteLine("\n"+response.Content.ReadAsStringAsync().Result);
            }
            catch
            {
                Console.WriteLine("\nСервер недоступен");
            }
        }




        public static string GetHashSha1(string content)
        {
            if (string.IsNullOrEmpty(content)) return null;
            using var sha1 = new SHA1Managed();

            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(content));

            return string.Concat(hash.Select(b => b.ToString("x2")));
        }

    }


    public class Settings
    {
        public string urls { get; set; }
        public string IPchat { get; set; }
    }

    class Server
    {
        public string authIP;
        public string authPort;
        public string IP
        { 
            get
            {
                return !(string.IsNullOrEmpty(authIP)|| string.IsNullOrEmpty(authPort)) ? authIP + ":" + authPort:null;
            }
        }
        public string chatIP;
        public string chatPort;
        public string CIP 
        { 
            get 
            {
                return !(string.IsNullOrEmpty(chatIP) || string.IsNullOrEmpty(chatPort)) ? chatIP + ":" + chatPort : null;
            } 
        }
        private Settings data;
        private string path;
        private const string ServerAuthorization = "ServerAuthorization";
        private const string ServerChat = "ServerChat";
        private string serverName;
        public string anotherName;
        public bool Valid
        {
            get
            {
                return !(string.IsNullOrEmpty(authIP) && string.IsNullOrEmpty(authPort));
            }
        }

        public Server(string _serverName,bool flag=false)
        {
            serverName = _serverName;
            anotherName = (serverName == ServerAuthorization ? ServerChat : ServerAuthorization);
            
            if (serverName != ServerAuthorization && serverName != ServerChat)
            {
                Console.WriteLine($"Сервер {serverName} не найден" +
                    $"\n Доступные сервера:" +
                    $"\n 1.ServerAuthorization " +
                    $"\n 2.ServerChat");
                return;
            }

            string requestBody;
            try
            {
                if (flag && serverName == ServerChat) goto loop;
                    path = Path.Combine(AppContext.BaseDirectory, $@"{serverName}\files\{serverName}.json");
                requestBody = File.ReadAllText(path);
                data = JsonSerializer.Deserialize<Settings>(requestBody);
                data.urls = data.urls?.Replace(";", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            
            if (string.IsNullOrEmpty(data.urls)|| !data.urls.Contains(":") || data.urls.Length < 9)
            {
                Console.WriteLine("IP адресс сервера не обнаружен");
                return;
            }
            if (serverName == ServerAuthorization)
            {
                if (string.IsNullOrEmpty(data.IPchat) || !data.IPchat.Contains(":") && data.IPchat.Length < 9)
                {
                    Console.WriteLine("IP адресс чата не обнаружен");
                    return;
                }
            }
            string[] temp;
            temp = data.urls.Split(":");
            authIP = temp[0];
            authPort = temp[1];
        loop:
            if (serverName == ServerAuthorization)
            {
                temp = data.IPchat?.Split(":");
                chatIP = temp[0];
                chatPort = temp[1];
            }
        
        }

        public void SetPort(int _port)
        {
            if (_port < 1024 || _port > 49151)
            {
                Console.WriteLine("Вводимый порт должен лежать в диапазоне от 1024 до 49151");
                return;
            }
            if (CheckFile(anotherName))
            {
                Server anotherServer = new Server(anotherName);
                if (anotherServer.IP == authIP + ":" + _port)
                {
                    Console.WriteLine($"IP сервера {serverName} не может быть таким-же как и у {anotherName}");
                    return;
                }
                if (serverName == ServerChat)
                {
                    anotherServer.chatPort= Convert.ToString(_port);
                    anotherServer.Save();
                }
            }
            authPort = Convert.ToString(_port);
        }
        public void SetIP(string _ip)
        {
            if (!CheckIP(_ip)) return;
            
            if (CheckFile(anotherName))
            {
                Server anotherServer = new Server(anotherName);
                if (anotherServer.IP == _ip + ":" + authPort)
                {
                    Console.WriteLine($"IP сервера {serverName} не может быть таким-же как и у {anotherName}");
                    return;
                }
                if (serverName == ServerChat)
                {
                    anotherServer.chatIP = _ip;
                    anotherServer.Save();
                }
            }
            authIP = _ip;
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
            return File.Exists(Path.Combine(AppContext.BaseDirectory, $@"{nameFile}\files\{nameFile}.json"));
        }

        public override string ToString()
        {
            return (authIP + ":" + authPort);
        }
        public void Save()
        {
            data.urls = IP;
            data.IPchat = CIP;
            File.WriteAllText(path, JsonSerializer.Serialize<Settings>(data, new JsonSerializerOptions { WriteIndented = true, IgnoreNullValues = true }));
        }
        public void Show()
        {
            if (serverName == ServerAuthorization)
            {
                Console.WriteLine(ServerAuthorization+":");
                Console.WriteLine($"IP:{IP}\n" +
                                  $"IPchat:{chatIP}");
            }
            if (serverName == ServerChat)
            {
                Console.WriteLine(ServerChat+":");
                Console.WriteLine($"IPchat:{IP}");
            }
        }
    }

}
