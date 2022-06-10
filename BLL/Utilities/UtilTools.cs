using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SMP.BLL.Utilities
{
    public class UtilTools
    {
        private static readonly Regex sWhitespace = new Regex(@"\s+");
        public static string ReplaceWhitespace(string input)
        {
            if (input == null)
                return input;
            return sWhitespace.Replace(input.ToLower(), "");
        }
    }
}
