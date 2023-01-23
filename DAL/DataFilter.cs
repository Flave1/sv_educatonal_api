using DAL;
using Microsoft.EntityFrameworkCore;
using SMP.DAL.Models.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMP.DAL
{
    public class Data
    {
        private readonly DataContext context;
        public Data(DataContext context)
        {
            this.context = context;
        }

      
    }

    public static class ReflectiveEnumerator
    {
        public static IEnumerable<T> GetEnumerableOfType<T>(T data)
        {
            var dynamics = (dynamic)data;
            return dynamics.Where(x => x.ClientId == "ClientID");
        }
    }

}
