using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL.Models.Logger
{
    public class Log: CommonEntity
    {
        [Key]
        public Guid Id { get; set; }
        public int LogType { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string InnerException { get; set; }
        public string InnerExceptionMessage { get; set; }
    }
}
