using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServerAuthorization.Logger;
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
            catch
            {
                Console.WriteLine($"Используемые вами порты заняты") ;
                Console.ReadLine();
                return;
            }
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
                    //logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddSimpleConsole(configure =>
                    {
                        
                        config.GetSection("Logging");
                        configure.IncludeScopes = false;
                        configure.TimestampFormat = "yyyy.MM.dd HH:mm ";
                        configure.SingleLine = true;
                    });

                })
                //.ConfigureAppConfiguration((hostingContext, _config) =>
                //{
                //    _config.AddConfiguration(config);
                //})
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
                    //.UseStartup<Startup>();
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
