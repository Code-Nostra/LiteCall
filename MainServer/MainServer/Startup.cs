using DAL.EF;
using DAL.Interfaces;
using DAL.Repositories;
using DAL.UnitOfWork.MainServer;
using MainServer.Mappings;
using MainServer.Token;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Threading;

namespace MainServer
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

			services.AddAutoMapper(typeof(MappingProfilesServer));

            //https://localhost:43800/swagger/v1/swagger.json
            services.AddSwaggerGen();

			services.AddControllersWithViews();

			IdentityModelEventSource.ShowPII = true;
            services.AddEntityFrameworkSqlite().AddDbContext<MainServerDbContext>();
			services.AddScoped<IUnitOfWorkMain, UnitOfWorkMain>();

			//services.AddScoped<IUserRepository,UserRepository>();
            //temo
			services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepositoryGeneric<,>));


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
               // var key = JsonNode.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, @"PrivateKey\PrivateKey.json")));
                AuthOptions.SetKey("MIIEpAIBAAKCAQEA519jIw6qsRq8VayaJbgMctpIzZEW8p6UcAvsMbs\u002Biy1w73pCe\u002Bndlyhl15\u002BsG83nwBz3YIb4vdYB6IvY0SGLbuB21Nqe/Eb4J/wfVhTQaIhdjLDPS8m4eBpZZx8ey7RpFDTcstWbV/IIKlqQkPKptYLl/FVkabJbhC8Z9Pnvnel1HYO2WxdYF7MQIO0SQPzrtcmLkQUlZZ5EeZjZVm82ieDL79c3g3d3kkw1Wf6RHzsdhomn8CW7VtMZd\u002Byp8qnbz1Iy5pjMFYLRHswS2CCFT7qEGHVdNE2VpJ3wrZ2pTJr5Sl6wKSzWhfJVy8\u002BZ/n94HT061IkWk5c9NdwOhqg0xQIDAQABAoIBAQC0nanuPAzTpGNRHiRXPY26OtyKXLDlRRXrQeNoDxuL2jVHPNGZmb75uPeHk4XvTpWTSwlxuOuEhgOHXWQsHVuTenZJQ0DI/z3JOO/davImKA2fPocUSqxOVYNlFM4dhwBgXcPLdySFStNE/N3NySDiKQUQLjjsqeb3ES3aJyH\u002Byjw9RddFPNTpAfFKpv\u002Bvo15EzKVANWlaTzO3Mfymmzh1QpQaKZoWmvN6Z8vBgJR0cqtxzs5iMpt9TPVByTYyY9r4DbnyaGptp2xAGRn0QU\u002BneRIbXNjxttF0cZfdXX4ZQX4YqH0VjB6JICOKJWrqbT1qD57rvrqGMj6WeZTa9IQhAoGBAPFgREMen2B7o9hs\u002B4Uj3CyqTTuV1iw29qnS0Ql/BKqdGf7QLbXZp6B6ikIwRwJNiQKB6qz62SuDjhl/JZc53AWCSgYNZ0fDtgqaNsM6EIiWWbvbIUdk3gp1rM87W0Poctwj2BtpvfyLc3seW3oOKv41SpgQzfAiKLvhe0I/veqLAoGBAPVj97s8sDNOmsuOMU8xNVUzitVhkfaAZVJ7rab3cR3L5UxcADFmSz8G99dVsJAI8Xm/6oSzZxL32xq4DgI2AvjJzlHqrN56yFzMNmL3PFzBVzTxO/b\u002BUENuVhFmJKKvdSMk\u002BLi0qJifB9ugeotiLZtLrjeqALFyctg74M0ij1fvAoGBANC\u002BhSpawFBrfM39qiOBWLcT7Mj8EwWX3wgM9pS00W9PgBy7Itzix7HkZ89H\u002BxEFxthxTGeeZNPZ6LkqGXNIKXxmKehEDS6mFbfjmDqL66KEDESHBKe\u002BlrwK\u002BpEZbxsyWqsBILLMZu6SFyPBpDDRMeM4aTNLTR5AHGQnYzZu86pzAoGAfTWlEkTFI2jYU0Uao\u002B3X6MGD9ZzzfXPGP3AcCzX24d1IAs5uysYHXwGPNLDkhpoJnLXwVAW2cldF1TxU6YvluExAmkvEKUE9Oxx4I8dDZZDBjqIIlt7s1XtXL0mPJ/OfMW\u002BL\u002BbM006tRtx8LaFOQMc1L99smv2RGdU8YRZN80MECgYB6RwyD/LaeyIljZtx\u002BNmMQ8LWbgT26bCzTFNiBHwCU51oI9kemp8TDcZh29eMjhiJP7xx0OetDH9BjOi5lO\u002BpNts9L9GL4ByY3tL5Hu0vgfDB67HB\u002Bak0e9E7ePdH/xzkPuXADlvqr3Cqb93hzUV95xCnFMw\u002B7zJ7LBuHX6Vfrqg==");
            }
            catch
            {
                Console.WriteLine("Private key not found " + (Path.Combine(AppContext.BaseDirectory, @"PrivateKey\PrivateKey.json")));
                Console.WriteLine("Closing after 10 seconds");
                Thread.Sleep(10000);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

			//Для капчи
			services.AddDistributedMemoryCache();

		

		}

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
				});
			}
			

			app.UseSession();
            app.UseHttpsRedirection();
            app.UseHsts();
            app.UseRouting();
            //app.UseMiddleware<PingMiddleware>();
            app.UseCors(policy =>
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
