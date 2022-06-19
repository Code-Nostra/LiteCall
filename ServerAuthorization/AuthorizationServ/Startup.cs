using AuthorizationServ.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using ServerAuthorization.Logger;
using Microsoft.AspNetCore.Http;
using ServerAuthorization.Infrastructure;
using System.Text.Json;
using System.Net.Http;
using System.Net.Mime;
using System.Text;

namespace AuthorizationServ
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            using var httpClient = new HttpClient(clientHandler);
          

            string IP = configuration["IPmain"];
            if (string.IsNullOrEmpty(IP)) IP = "localhost:5005";
            
            
            

            DB local = new DB();
            var srv = local.Servers.FirstOrDefault();
            srv.Ip = configuration["IPchat"];
            local.SaveChanges();

            string ip = configuration["urls"].Replace("https://", "");
            var ServerMonitor = new { Title =srv.Title, Ip=ip, Ident=srv.Ident.GetSha1() };
            var json = JsonSerializer.Serialize(ServerMonitor, new JsonSerializerOptions { IgnoreNullValues = true });
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            httpClient.DefaultRequestHeaders.Add("ApiKey", "ACbaAS324hnaASD324bzZwq41");
            
            try
            {
                var response = httpClient.PostAsync($"https://{IP}/api/Server/ServerMonitoringAdd", content).Result;
                Console.Write("*");
            }
            catch
            {
                
            }


        }

        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;
            services.AddEntityFrameworkSqlite().AddDbContext<DB>();
            //services.AddLogging(
            //builder =>
            //{
            //    builder.AddFilter("Microsoft", LogLevel.Warning)
            //           .AddFilter("System", LogLevel.Warning)
            //           .AddFilter("NToastNotify", LogLevel.Warning)
            //           .AddConsole();
            //});
            services.AddOptions();

            services.AddControllers();
            services.AddCors();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);//You can set Time   
            });
            //Управление секретами пользователей
            //AuthOptions.SetKey(Configuration.GetSection("PrivateKey").Value);
            try
            {
                var key = JsonNode.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, @"..\files\Key\PrivateKey.json")));
                AuthOptions.SetKey((string)key["Private"]);
            }
            catch 
            { 
                Console.WriteLine("Private key not found "+ (Path.Combine(AppContext.BaseDirectory, @"..\files\Key\PrivateKey.json")));
                Console.WriteLine("Closing after 10 seconds");
                Thread.Sleep(10000);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            
            //Для капчи
            services.AddDistributedMemoryCache();
            #region
            //Added for session state
            //Added for session state


            //services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromMinutes(10);
            //});


            //var paramss = new TokenValidationParameters();
            //AuthOptions auth = new AuthOptions(Configuration.GetSection("PrivateKey").Value);
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.RequireHttpsMetadata = false;

            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuer = true,
            //            ValidIssuer = AuthOptions.Issuer,
            //            RequireAudience = true,
            //            ValidateAudience = true,
            //            ValidAudience = AuthOptions.Audience,
            //            RequireExpirationTime = true,
            //            ValidateLifetime = true,
            //            ValidateIssuerSigningKey = true,
            //            RequireSignedTokens = true,
            //            IssuerSigningKey = auth.PrivateKey
            //        };

            //    });   
            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseHsts();
            app.UseRouting();
            //app.UseMiddleware<PingMiddleware>();
            app.UseCors(policy=> 
            {
                policy
                .SetIsOriginAllowed(origin => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            });
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
