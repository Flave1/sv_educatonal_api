using System;

namespace Contracts.Options
{

    public class JwtSettings
    {
        public string Secret { get; set; }

        public TimeSpan TokenLifeSpan { get; set; }
    }
}
