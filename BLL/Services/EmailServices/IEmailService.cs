using Contracts.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.EmailServices
{
    public interface IEmailService
    {
        List<EmailMessage> ReceiveEmail(int maxCount = 10);
        Task Send(EmailMessage emailMessage);
    }
}
