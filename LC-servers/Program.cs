using System;
using System.Diagnostics;

namespace LC_servers
{
    class Program
    {
        static void Main(string[] args)
        {
            //System.Diagnostics.Process.Start("CMD.exe", strCmdText);

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd";
            p.Start();
            Process.Start(@"C:\Users\PC\source\repos\ServerSignalR\LTPanel\bin\Debug\net5.0\LTPanel.exe");
        }
    }
}
