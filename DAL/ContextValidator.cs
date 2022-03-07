using DAL.ClassEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ContextValidator
    {
        //public void AlreadExist<T>(string field, string fieldValue)
        //{
        //    return DbSet<T>.Any(x => x.field == fieldValue)
        //}

        //bool AlreadExist<T>(Func<T, bool> checkFunction)
        //{
        //    DataContext sa = new DataContext();
        //    var alreadyExists = sa.Set<T>.lo.Any(checkFunction);
        //    return !alreadyExists;
        //}
      
        //bool IsFieldValueUnique<T>(Func<T, bool> checkFunction)
        //{
        //    var alreadyExists = DataContext<fdsd>.Any(checkFunction);
        //    return !alreadyExists;
        //    //Other ways to do this, but this is just for clarity.
        //}
    }
}
