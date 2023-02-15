using DAL.EF;
using MainServer.Models;
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

namespace MainServer
{ 
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var db = new MainServerDbContext())
            {
                db.Database.Migrate();
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("MainServer.json", optional: true, reloadOnChange: true)
                            .Build();
            
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    
                    logging.ClearProviders();
                    var filepath = Path.Combine(AppContext.BaseDirectory, "logger.txt");
                    
                    logging.AddFilter("Default", LogLevel.Information);
                    logging.AddFilter("Microsoft", LogLevel.Error);
                    logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Information);
                    logging.AddFilter("Microsoft.AspNetCore.Server.Kestrel", LogLevel.Information);
                    logging.AddFilter("System", LogLevel.Error);
                    logging.AddFilter("Microsoft.AspNetCore.Session.SessionMiddleware", LogLevel.None);
					logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Information);
					logging.AddFilter("Microsoft.EntityFrameworkCore.Sqlite", LogLevel.Information);
					//logging.SetMinimumLevel(LogLevel.Information);
					logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddSimpleConsole(configure =>
                    {

                        config.GetSection("Logging");
                        configure.IncludeScopes = false;
                        configure.TimestampFormat = "yyyy.MM.dd HH:mm ";
                        //configure.SingleLine = true;
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
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseStartup<Startup>();
					webBuilder.UseStartup<Startup>();
                });
        }
    }
}
