using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace AuthorizationServ
{ 
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var db = new DB())
            {
                db.Database.Migrate();
            }
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                return;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(),@"..\files\ServerAuthorization.json"), optional: true, reloadOnChange: true)
                            .Build();
            if (!string.IsNullOrEmpty(config["urls"]))
            {
                if (!config["urls"].Contains("https"))
                {
                    config["urls"] = ("https://" + config["urls"]);
                }
            }

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseConfiguration(config)
                    .UseKestrel(kestrelOptions =>
                    {
                        kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
                        {
                            httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
                        });
                    })
                    .UseContentRoot(Directory.GetCurrentDirectory());
                    //.UseStartup<Startup>();
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
