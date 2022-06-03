
using CommandDotNet;
using CommandDotNet.Help;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTPanel
{
    class Settings : HelpTextProvider
    {
        public Settings(AppSettings appSettings, string appName = null) : base(appSettings, appName)
        {
            appSettings.Help.TextStyle = HelpTextStyle.Basic;
            
        }
    }
}
