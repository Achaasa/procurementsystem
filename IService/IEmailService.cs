using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace procurementsystem.IService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
    }

}