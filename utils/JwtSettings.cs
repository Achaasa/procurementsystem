using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace procurementsystem.utils
{
   public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; }
}

}