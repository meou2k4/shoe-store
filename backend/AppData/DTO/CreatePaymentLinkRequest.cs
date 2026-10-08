using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.DTO
{
    public record CreatePaymentLinkRequest(
       string ProductName,
        string Description,
        int Price,
        string ReturnUrl,
        string CancelUrl
    );
}
