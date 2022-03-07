using BLL.Constants;
using DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Utilities
{
   public static class RegistrationNumber
    {
        public static IConfiguration config;
        public static void Initialize(IConfiguration Configuration)
        {
            config = Configuration;
        }
        public static IDictionary<string, string> GenerateForStudents()
        {
            try
            {
                var dictionary = new Dictionary<string, string>();
                DataContext context = new DataContext();
                var lastRegNumber = context.StudentContact.Max(d => d.RegistrationNumber) ?? "1";
                var newRegNo = (lastRegNumber == "1" ? 1 : long.Parse(lastRegNumber) + 1).ToString();

                var regNoFormat = config.GetSection("RegNumber:Student").Value;
                newRegNo = number(newRegNo);

                var regNo = regNoFormat.Replace("%VALUE%", newRegNo);
                dictionary.Add(newRegNo, regNo);
                return dictionary;
            }
            catch (Exception)
            { 
                throw new ArgumentException("Unable to Generate Registration Number");
            }
        }

        private static string number(string regNo)
        {
            if (regNo.Length == 1)
                return "000000"  + regNo;
            if (regNo.Length == 2)
                return "00000" + regNo;
            if (regNo.Length == 3)
                return "0000" + regNo;
            if (regNo.Length == 4)
                return "000" + regNo;
            if (regNo.Length == 5)
                return "00"  + regNo;
            if (regNo.Length == 6)
                return "0"  + regNo;
            if (regNo.Length == 7)
                return regNo;
            else
                return regNo;

        }
    }
}
