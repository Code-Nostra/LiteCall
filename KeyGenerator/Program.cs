using Aspose.Zip;
using Aspose.Zip.Saving;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace KeyGenerator
{
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
        public bool valid
        {
            get
            {
                return !string.IsNullOrEmpty(ip) && !string.IsNullOrEmpty(port);
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
                data.IPchat=data.IPchat?.Replace(";", "");
                data.urls = data.urls?.Replace(";", "");
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("Неверный формат");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            

            
            
            string[] temp;
            string Pattern = @"\blocalhost:\d{1,5}\b|(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?):\d{1,5}\b";
            Regex check = new Regex(Pattern);
            if (!check.IsMatch(data.urls, 0) )
            {
                Console.WriteLine($"IP адресс сервера {serverName}  не верен");
                return;
            }
            if (serverName != ServerChat)
            {
                temp = data.IPchat?.Split(":");
                Cip = temp[0];
                Cport = temp[1];
                if (!check.IsMatch(data.IPchat, 0))
                {
                    Console.WriteLine($"IP адресс сервера {serverName}  не верен");
                    return;
                }
            }
            temp = data.urls?.Split(":");
            ip = temp[0];
            port = temp[1];
        }

        public void SetPort(int port)
        {
            data.urls = ip + ":" + port;
            if (serverName == ServerChat)
            {
                Server local = new Server(ServerAuthorization);
                local.data.IPchat = data.urls;
                local.Save();
            }
        }


        public override string ToString()
        {
            return (ip + ":" + port);
        }
        public void Save()
        {
            File.WriteAllText(path, JsonSerializer.Serialize<Settings>(data,new JsonSerializerOptions { WriteIndented = true,IgnoreNullValues=true }));
        }
    }
    class Program
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
            //string a = "Привет";
            //string encrypted = Encrypt(a);
            //Console.WriteLine(encrypted);
            string decrypted = Decrypt("AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAvpf/bTQm1EWSdSDJ3Fjt+QAAAAACAAAAAAAQZgAAAAEAACAAAAAN5Ad7ag7JqyMkvml7NvfVyO2wcNI7/q91Hnoac2m0ZgAAAAAOgAAAAAIAACAAAACD75/URVrI9AEqCcvlXcYplDIgNW61ElfDoDGDQNPsdBAAAACqKmSEW8CqnE0Xmtpf/DfpQAAAAK9GEPK7gs6LFAwew553+Ou5Q87XqbW1vVvlyCBKwvTDT9rM53/1l1vzmhuNFtR4yFSCLTQP0joSXBly6X62vY8=");
            Console.WriteLine(decrypted);


            string server = Console.ReadLine();
            int port = Convert.ToInt32(Console.ReadLine());
            Server srv = new Server(server);
            if (!srv.valid) return;
            switch (server)
            {
                case "ServerAuthorization":
                    srv.SetPort(port);
                    break;
                case "ServerChat":
                    srv.SetPort(port);
                    break;
            }
            srv.Save();

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
