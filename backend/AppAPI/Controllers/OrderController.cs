
using Microsoft.AspNetCore.Mvc;
using AppData.DTO;
using Net.payOS;
using Net.payOS.Types;
using AppData.Models;
using System;
using AppData;

namespace AppAPI.Controllers;
[Route("[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly PayOS _payOS;
    public OrderController(PayOS payOS)
    {
        _payOS = payOS;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePaymentLink(CreatePaymentLinkRequest body)
    {
        try
        {
            // Tạo mã đơn hàng
            int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
            ItemData item = new ItemData(body.ProductName, 1, body.Price);
            List<ItemData> items = new List<ItemData> { item };

            // Cấu hình dữ liệu thanh toán
            PaymentData paymentData = new PaymentData(orderCode, body.Price, body.Description, items, body.CancelUrl, body.ReturnUrl);

            // Gọi API để tạo liên kết thanh toán
            CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);


            // Trường hợp tạo liên kết thất bại
            return Ok(new Response(-1, "failed to create payment link", null));
        }
        catch (System.Exception exception)
        {
            Console.WriteLine(exception);
            return Ok(new Response(-1, "fail", null));
        }
    }


    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrder([FromRoute] int orderId)
    {
        try
        {
            PaymentLinkInformation paymentLinkInformation = await _payOS.getPaymentLinkInformation(orderId);
            return Ok(new Response(0, "Ok", paymentLinkInformation));
        }
        catch (Exception exception)
        {

            Console.WriteLine(exception);
            return Ok(new Response(-1, "fail", null));
        }

    }
    [HttpPut("{orderId}")]
    public async Task<IActionResult> CancelOrder([FromRoute] int orderId)
    {
        try
        {
            PaymentLinkInformation paymentLinkInformation = await _payOS.cancelPaymentLink(orderId);
            return Ok(new Response(0, "Ok", paymentLinkInformation));
        }
        catch (System.Exception exception)
        {

            Console.WriteLine(exception);
            return Ok(new Response(-1, "fail", null));
        }

    }
    [HttpPost("confirm-webhook")]
    public async Task<IActionResult> ConfirmWebhook(ConfirmWebhook body)
    {
        try
        {
            await _payOS.confirmWebhook(body.Webhook_url);
            return Ok(new Response(0, "Ok", null));
        }
        catch (System.Exception exception)
        {

            Console.WriteLine(exception);
            return Ok(new Response(-1, "fail", null));
        }

    }
}
