using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SignalRServ
{
    internal static class Extensions
    {

        internal static string PrettifyJsonString(this string json)
        {
            var jsonObject = JsonSerializer.Deserialize<dynamic>(json);
            return JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
