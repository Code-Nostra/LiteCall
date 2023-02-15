using DAL.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServerAuthorization.Logger;
using ServerAuthorization.Models;
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
            using (var db = new ServerAuthDbContext())
            {
                db.Database.Migrate();
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                            .SetBasePath(AppContext.BaseDirectory)
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile(Path.Combine(AppContext.BaseDirectory, @"..\files\ServerAuthorization.json"), optional: true, reloadOnChange: true)
                            .Build();


            if (!string.IsNullOrEmpty(config["urls"]))
            {
                config["IPSync"] = config["urls"];
                string[] temp = config["urls"].Split(":");
                config["urls"] = "0.0.0.0:" + temp[1];
                if (!config["urls"].Contains("https"))
                {
                    config["urls"] = ("https://" + config["urls"]);
                }
            }

            return Host.CreateDefaultBuilder(args)

                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    var filepath = Path.Combine(AppContext.BaseDirectory, "logger.txt");
                    logging.AddFile(filepath);
                    logging.AddFilter("Default", LogLevel.Information);
                    logging.AddFilter("Microsoft", LogLevel.Error);
                    logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Information);
                    logging.AddFilter("Microsoft.AspNetCore.Server.Kestrel", LogLevel.Information);
                    logging.AddFilter("System", LogLevel.Error);
                    logging.AddFilter("Microsoft.AspNetCore.Session.SessionMiddleware", LogLevel.None);
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddSimpleConsole(configure =>
                    {
                        config.GetSection("Logging");
                        configure.IncludeScopes = false;
                        configure.TimestampFormat = "yyyy.MM.dd HH:mm ";
                    });

                })

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
                    .UseContentRoot(AppContext.BaseDirectory);
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
