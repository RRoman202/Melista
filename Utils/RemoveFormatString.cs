using IWshRuntimeLibrary;
using LibVLCSharp.Shared;
using Melista.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melista.Utils
{
    public class RemoveFormatString
    {
        public static string RemoveFormat(string stringForRemove)
        {
            if (stringForRemove.Contains('\\'))
            {
                string[] strings = stringForRemove.Split('\\');
                stringForRemove = strings[strings.Length - 1];
            }
            string[] strings_1 = stringForRemove.Split('.');
            int ubrat = strings_1[strings_1.Length - 1].Length + 1;
            stringForRemove = stringForRemove.Substring(0, stringForRemove.Length - ubrat);
            return stringForRemove;
        }
        
    }
}
