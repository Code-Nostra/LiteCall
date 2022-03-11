using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServerSignalR.ServerCheckMethods
{
    public static class ServerCheckMethods
    {
        public static async Task<string> CheckName(string Name)
        {
            using var httpClient = new HttpClient();
            var json = JsonSerializer.Serialize(Name);
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await httpClient.PostAsync("http://localhost:57785/api/ServerList/CheckName", content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return "";
            }
        }
    }

}
