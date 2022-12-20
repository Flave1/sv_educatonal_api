using DAL;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SMP.BLL.Utilities
{
    public class UtilitiesService : IUtilitiesService
    {
        private readonly RegNumber regNoOptions;

        public UtilitiesService(IOptions<RegNumber> regNoOptions)
        {
            this.regNoOptions = regNoOptions.Value;
        }
        public string GetStudentRealRegNumber(string regNo)
        {
            try
            {
                var splited = regNo.Split('/');
                if (regNoOptions.StudentRegNoPosition == 3)
                {
                    return splited[2];
                }
                if (regNoOptions.StudentRegNoPosition == 2)
                {
                    return splited[1];
                }
                if (regNoOptions.StudentRegNoPosition == 1)
                {
                    return splited[0];
                }
                return regNo;
            }
            catch (Exception)
            {

                throw new ArgumentException("Please ensure registeration number is in correct format");
            }
        }
    }
}
