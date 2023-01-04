using DAL;
using Microsoft.Extensions.Options;
using System;

namespace SMP.BLL.Utilities
{
    public class UtilitiesService : IUtilitiesService
    {
        private readonly RegNumber regNoOptions;

        public UtilitiesService(IOptions<RegNumber> regNoOptions)
        {
            this.regNoOptions = regNoOptions.Value;
        }
        public string GetStudentRegNumberValue(string regNo)
        {
            try
            {
                var splited = regNo.Split(regNoOptions.Separator);
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
