using System;
using System.Collections.Generic;
using System.Text;

namespace App.Helper.Extensions
{
    public static class StaffCode
    {

        public static string Generate(int staffCount)
        {
            string zeros = AppendZero(staffCount);
            return $"ST/{zeros}{staffCount}/{DateTime.Now.Year}";
        }

        public static string AppendZero(int ValueInt)
        {
            double value = ValueInt;
            int sign = 0;
            if (value < 0)
            {
                value = -value;
                sign = 0;
            }
            if (value <= 9)
            {
                return "00000".ToString();
            }
            if (value <= 99)
            {
                return "0000".ToString();
            }
            if (value <= 999)
            {
                return "000".ToString();
            }
            if (value <= 9999)
            {
                return "00".ToString();
            }
            if (value <= 99999)
            {
                return "0".ToString();
            }
            else if (value <= 999999)
            {
                return sign.ToString();
            }
            else
            {
                return sign.ToString();
            }
        }
    }
}
