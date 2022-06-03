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
        public static async Task<bool> RoomAdd(string Name,string Password)
        {
            using var httpClient = new HttpClient();
            var json = JsonSerializer.Serialize(new { Name, Password });
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            httpClient.DefaultRequestHeaders.Add("ApiKey", "ACbaAS324hnaASD324bzZwq41");
            var response = await httpClient.PostAsync("http://localhost:5000/api/Server/RoomAdd", content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;//Convert.ToBoolean(await response.Content.ReadAsStringAsync());
            }
            else
            {
                return false;
            }
        }
    }

}
