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
using ServerAuthorization.DataBase;

namespace AuthorizationServ
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;

            Sync.Synch(configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;
            services.AddEntityFrameworkSqlite().AddDbContext<DB>();
            services.AddOptions();

            services.AddControllers();
            services.AddCors();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);//You can set Time   
            });
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
