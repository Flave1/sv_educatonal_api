using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.ErrorHandler
{
    public static class ErrorID
    {
        public static string Generate(int length)
        {
            string value = "";
            if (length > 0)
            {
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                var random = new Random();
                var result = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
                value = result;
            }
            return value;
        }
    }
}
