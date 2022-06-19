using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SignalRServ.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace SignalRServ
{ 
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(Path.Combine(AppContext.BaseDirectory,@"..\files\ServerChat.json"), optional: true, reloadOnChange: true)
                .Build();
            
            if (!string.IsNullOrEmpty(config["urls"]))
                {
                if (!config["urls"].Contains("https"))
                {
                    config["urls"] = ("https://" + config["urls"]);
                }
            }
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    //Console.WriteLine(hostingContext.Configuration.GetSection("Logging:LogLevel:Microsoft").Value);
                    logging.ClearProviders();
                    var filepath = Path.Combine(AppContext.BaseDirectory, "logger.txt");
                    logging.AddFile(filepath);
                    logging.AddFilter("Default", LogLevel.Information);
                    logging.AddFilter("Microsoft", LogLevel.Warning);
                    logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Information);
                    logging.AddFilter("Microsoft.AspNetCore.Server.Kestrel", LogLevel.Information);
                    logging.AddFilter("System", LogLevel.Warning);
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
                .ConfigureAppConfiguration((hostingContext, _config) =>
                {
                    _config.AddConfiguration(config);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseConfiguration(config);
                    webBuilder.UseKestrel(kestrelOptions =>
                    {
                        kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
                        {
                            httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
                        });
                    })
                    .UseContentRoot(AppContext.BaseDirectory);
                    //.UseStartup<Startup>();
                    webBuilder.UseStartup<Startup>();
                });

        }
    }
}
