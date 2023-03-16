﻿using DAL;
using Microsoft.Extensions.Options;
using Polly;
using SMP.DAL.Models;
using System;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using DAL.StudentInformation;
using SMP.Contracts.Options;
using SMP.DAL.Models.PortalSettings;
using BLL.LoggerService;

namespace SMP.BLL.Utilities
{
    public class UtilitiesService : IUtilitiesService
    {
        private readonly DataContext context;
        private readonly IConfiguration config;
        private readonly ILoggerService loggerService;
        private readonly string smsClientId;
        public UtilitiesService(DataContext context, IConfiguration config, IHttpContextAccessor accessor, ILoggerService loggerService)
        {
            this.context = context;
            this.config = config;
            this.loggerService = loggerService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
        }
        public string GetStudentRegNumberValue(string regNo, string clientId)
        {
            try
            {
                var schoolSettings = new SchoolSetting();
                if (!string.IsNullOrEmpty(clientId))
                {
                    schoolSettings = context.SchoolSettings.FirstOrDefault(x => x.ClientId == clientId);
                }
                else
                {
                    schoolSettings = context.SchoolSettings.FirstOrDefault(x => x.ClientId == smsClientId);
                }

                var splited = regNo.Split(schoolSettings.SCHOOLSETTINGS_RegNoSeperator);
                if (schoolSettings.SCHOOLSETTINGS_RegNoPosition == 3)
                {
                    return splited[2];
                }
                if (schoolSettings.SCHOOLSETTINGS_RegNoPosition == 2)
                {
                    return splited[1];
                }
                if (schoolSettings.SCHOOLSETTINGS_RegNoPosition == 1)
                {
                    return splited[0];
                }
                return regNo;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Please ensure registeration number is in correct format");
            }
        }
        public async Task<StudentContact> GetStudentContactByRegNo(string studentRegNoValue, string clientId)
        {
            try
            {
                if (!string.IsNullOrEmpty(clientId))
                {
                    var student = await context.StudentContact.FirstOrDefaultAsync(x => x.Deleted == false && x.ClientId == clientId && x.RegistrationNumber == studentRegNoValue);
                    return student;
                }
                else
                {
                    var student = await context.StudentContact.FirstOrDefaultAsync(x => x.Deleted == false && x.ClientId == smsClientId && x.RegistrationNumber == studentRegNoValue);
                    return student;
                }
            }
            catch (Exception ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw new ArgumentException("Please ensure registeration number is in correct format");
            }
        }
        public async Task<IDictionary<string, string>> GenerateStudentRegNo()
        {
            try
            {
                var dictionary = new Dictionary<string, string>();
                var lastRegNumber = context.StudentContact.Where(x => x.ClientId == smsClientId).Max(d => d.RegistrationNumber) ?? "1";
                var newRegNo = (lastRegNumber == "1" ? 1 : long.Parse(lastRegNumber) + 1).ToString();

                var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == smsClientId).SCHOOLSETTINGS_StudentRegNoFormat;
                newRegNo = number(newRegNo);

                var regNo = regNoFormat.Replace("%VALUE%", newRegNo);
                dictionary.Add(newRegNo, regNo);
                return await Task.Run(() => dictionary);
            }
            catch (Exception ex)
            {
                await loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw new ArgumentException("Unable to Generate Registration Number");
            }
        }
        private static string number(string regNo)
        {
            if (regNo.Length == 1)
                return "000000" + regNo;
            if (regNo.Length == 2)
                return "00000" + regNo;
            if (regNo.Length == 3)
                return "0000" + regNo;
            if (regNo.Length == 4)
                return "000" + regNo;
            if (regNo.Length == 5)
                return "00" + regNo;
            if (regNo.Length == 6)
                return "0" + regNo;
            if (regNo.Length == 7)
                return regNo;
            else
                return regNo;

        }
    }
}
