using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutomateRobotService.Tools
{
    internal class AdditionalFunctions
    {

        public static bool IsTextAllowed(string text)
        {
         Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text

            return !_regex.IsMatch(text);
        }


    }
}
