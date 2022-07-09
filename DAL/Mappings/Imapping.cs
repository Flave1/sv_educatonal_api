using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Mappings
{
    public interface IMapService
    {
        void ConfigureMapService(ModelBuilder builder);
    }

   
}
