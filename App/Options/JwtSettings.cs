using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Options
{
    public class JwtSettings
    {
        public string Secret { get; set; }

        public TimeSpan TokenLifeSpan { get; set; }
    }
}
