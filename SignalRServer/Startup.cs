using AuthorizationServ.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SignalRServ
{
    public class Startup
    {
        public static string lastToken { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.AddRazorPages();
            
            var paramss = new TokenValidationParameters();

            //”правление секретами пользователей
            //AuthOptions.SetKey(Configuration.GetSection("PublicKey").Value);

            #region ƒл€ анонимных пользователей (без сервера авторизации)
            
            
            bool IsAuthorize = true;
            #endregion
            var key = JsonNode.Parse(File.ReadAllText(@"..\PublicKey\PublicKey.json"));
            AuthOptions.SetKey((string)key["Public"]);
            AuthOptions.SetCertificate();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["token"];


                            if (!string.IsNullOrWhiteSpace(accessToken) &&
                                context.Request.Path.StartsWithSegments("/LiteCall"))
                            {
                                #region ƒл€ анонимных пользователей (без сервера авторизации)
                                dynamic obj = JsonNode.Parse(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(accessToken.ToString().Split('.')[1])));
                                IsAuthorize = (string)obj["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] != "Anonymous" ? true : false;
                                if (!IsAuthorize)
                                {
                                    options.TokenValidationParameters.RequireExpirationTime =  false;
                                    options.TokenValidationParameters.ValidateLifetime = false;
                                    options.TokenValidationParameters.IssuerSigningKey = AuthOptions.Certificate;
                                }
                                #endregion
                                context.Token = accessToken;
                                lastToken = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                    options.RequireHttpsMetadata = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {

                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.Issuer,
                        RequireAudience = true,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.Audience,
                        RequireExpirationTime = IsAuthorize ? true : false,
                        ValidateLifetime = IsAuthorize ? true : false,
                        ValidateIssuerSigningKey = true,
                        RequireSignedTokens = true,
                        IssuerSigningKey = IsAuthorize ? AuthOptions.PublicKey : AuthOptions.Certificate

                        #region  опи€
                        /*
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.Issuer,
                        RequireAudience = true,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.Audience,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        RequireSignedTokens = true,
                        IssuerSigningKey = AuthOptions.PublicKey
                        */
                        #endregion

                    };
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddSignalR(hubOptions =>
            {
                //hubOptions.EnableDetailedErrors = true;
                hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(180);
                hubOptions.ClientTimeoutInterval = TimeSpan.FromSeconds(360);
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<ChatHub>("/LiteCall");
            });
        }
    }
}
