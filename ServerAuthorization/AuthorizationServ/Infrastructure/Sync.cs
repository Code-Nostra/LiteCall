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
using System.Text.Json;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using ServerAuthorization.Models;

namespace ServerAuthorization.Infrastructure
{
    public static class Sync
    {
        public static void Synch(IConfiguration configuration)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            using var httpClient = new HttpClient(clientHandler);

            string IP = configuration["IPmain"];
            if (string.IsNullOrEmpty(IP)) IP = "localhost:5005";

            ApplicationDbContext local = new ApplicationDbContext();
            var srv = local.Servers.FirstOrDefault();
            srv.Ip = configuration["IPchat"];
            local.SaveChanges();

            string ip = configuration["IPSync"]?.Replace("https://", "");
            var ServerMonitor = new { srv.Title, Ip = ip, Ident = srv.Ident.GetSha1() };
            var json = JsonSerializer.Serialize(ServerMonitor, new JsonSerializerOptions { IgnoreNullValues = true });
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            httpClient.DefaultRequestHeaders.Add("ApiKey", "ACbaAS324hnaASD324bzZwq41");
            try
            {
                var response = httpClient.PostAsync($"https://{IP}/api/Server/ServerMonitoringAdd", content).Result;
                Console.Write("*");
            }
            catch { }
        }

    }
}
