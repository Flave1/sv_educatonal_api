using DAL;
using Microsoft.Extensions.Options;
using SMP.Contracts.Options;
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
        public record ClassRecord { public decimal Average; public string Position; }
        private static readonly Regex sWhitespace = new Regex(@"\s+");
        public static string ReplaceWhitespace(string input)
        {
            if (input == null)
                return input;
            return sWhitespace.Replace(input.ToLower(), "");
        }

        public static List<ClassRecord> GetStudentPositions(IEnumerable<decimal> averages)
        {
            var aves = new List<ClassRecord>();
            var distinctAverages = averages.OrderByDescending(s => s).Distinct().ToList();
            for (var i = 0; i < distinctAverages.Count; i++)
            {
                aves.Add(new ClassRecord
                {
                    Average = distinctAverages[i],
                    Position = OrdinalSuffixOf(i + 1),
                });
            }
            return aves;
        }

        public static List<ClassRecord> GetStudentPositions(IEnumerable<double> averages)
        {
            var aves = new List<ClassRecord>();
            var distinctAverages = averages.OrderByDescending(s => s).Distinct().ToList();
            for (var i = 0; i < distinctAverages.Count; i++)
            {
                aves.Add(new ClassRecord
                {
                    Average = (decimal)distinctAverages[i],
                    Position = OrdinalSuffixOf(i + 1),
                });
            }
            return aves;
        }

        public static bool IsInString(string commaDelimetedString, string value)
        {
            try
            {
                if (string.IsNullOrEmpty(commaDelimetedString))
                    return false;
                return commaDelimetedString.Split(',').AsEnumerable().Contains(value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string OrdinalSuffixOf(int i)
        {
            var j = i % 10;
            var k = i % 100;
            if (j == 1 && k != 11)
            {
                return i + "st";
            }
            if (j == 2 && k != 12)
            {
                return i + "nd";
            }
            if (j == 3 && k != 13)
            {
                return i + "rd";
            }
            return i + "th";
        }

       
    }
}
