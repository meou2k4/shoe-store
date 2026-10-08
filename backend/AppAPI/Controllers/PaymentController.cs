

using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using AppData.DTO;

namespace AppAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly PayOS _payOS;

    public PaymentController(PayOS payOS)
    {
        _payOS = payOS;
    }

    [HttpPost("payos_transfer_handler")]
    public IActionResult payOSTransferHandler(WebhookType body)
    {
        try
        {
            WebhookData data = _payOS.verifyPaymentWebhookData(body);

            if (data.description == "" || data.description == "VQRIO123")
            {
                return Ok(new Response(0, "Ok", null));
            }
            return Ok(new Response(0, "Ok", null));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return Ok(new Response(-1, "fail", null));
        }
    }
}