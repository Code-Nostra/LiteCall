using CommandDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        [Command(
            Description = "Performs mathematical calculations",
            ExtendedHelpTextLines = new[]
            {
        "Include multiple lines of text",
        "Extended help of the root command is a good place to describe directives for the app"
            })]
        public class Program
        {
            static int Main(string[] args) =>
                new AppRunner<Program>().Configure(x => x.CustomHelpProvider = new Settings(x.AppSettings)).Run(args);

            [Command(
                Description = "Adds two numbers",
                UsageLines = new[]
                {
            "Add 1 2",
            "%AppName% %CmdPath% 1 2"
                },
                ExtendedHelpText = "single line of extended help here")]
            public void Add(

                [Operand(Description = "first value"),] int x,
                [Operand(Description = "second value"), AppSetting("Add")] int y) => Console.WriteLine(x + y);

            [Command(Description = "Subtracts two numbers")]
            public void Subtract(int x, int y) => Console.WriteLine(x - y);

        }
    }
}
