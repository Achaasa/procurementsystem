using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace procurementsystem.utils
{
    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string FromEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
    }

}