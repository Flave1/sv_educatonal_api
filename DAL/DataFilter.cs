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

        public static TEntity Filter<TEntity>(string entityId) where TEntity : class
        {
            using (DataContext rpContext = new DataContext())
            {
                foreach (Type type in Assembly.GetAssembly(typeof(CommonEntity)).GetTypes()
               .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(CommonEntity))))
                {
                    //objects.Add((T)Activator.CreateInstance(type, constructorArgs));
                }
                return 
                //return (from e in rpContext.Set<TEntity>()
                //        where e["ClientId"] == entityId
                //        select e).FirstOrDefault();
            }
        }
    }

    public static class ReflectiveEnumerator
    {
        static ReflectiveEnumerator() { }

        public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class, IComparable<T>
        {
            List<T> objects = new List<T>();
            foreach (Type type in Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                //objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }
            objects.Sort();
            return objects;
        }
    }

}
