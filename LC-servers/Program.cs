using System;
using System.Diagnostics;

namespace LC_servers
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                Process.Start(@"ServerAuthorization\ServerAuthorization\ServerAuthorization.exe");
                Process.Start(@"ServerChat\ServerChat\ServerChat.exe");
            }
            catch
            {
                try
                {
                    Process.Start(@"ServerChat\ServerChat\ServerChat.exe");
                    Process.Start(@"ServerAuthorization\ServerAuthorization\ServerAuthorization.exe");
                }
                catch
                {

                }
            }
        }
    }
}
