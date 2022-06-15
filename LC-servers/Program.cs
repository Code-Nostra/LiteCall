using System;
using System.Diagnostics;

namespace LC_servers
{
    class Program
    {
        static void Main(string[] args)
        {
            Process.Start(@"ServerAuthorization\ServerAuthorization\ServerAuthorization.exe");
            Process.Start(@"ServerChat\ServerChat\ServerChat.exe");
        }
    }
}
