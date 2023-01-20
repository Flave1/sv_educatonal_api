using DAL;
using Microsoft.EntityFrameworkCore;
using SMP.DAL.Models.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
      
        public TEntity Filter<TEntity>(string entityId) where TEntity : class
        {
            using (DataContext rpContext = new DataContext())
            {

                return (from e in rpContext.Set<TEntity>()
                        where e.["ClientId"] == entityId
                        select e).FirstOrDefault();
            }
        }
    }
   
}
