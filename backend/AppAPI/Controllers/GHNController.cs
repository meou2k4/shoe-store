using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text;
using System.Text.Json.Nodes;

[Route("api/ghn")]
[ApiController]
public class GHNController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public GHNController(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _httpClient = httpClientFactory.CreateClient("GHN");
    }

    [HttpGet("provinces")]
    public Task<IActionResult> GetProvinces() => Forward("master-data/province", new JsonObject());

    [HttpGet("districts")]
    public Task<IActionResult> GetDistricts([FromQuery] int province_id) =>
        Forward("master-data/district", new JsonObject { ["province_id"] = province_id });

    [HttpGet("wards")]
    public Task<IActionResult> GetWards([FromQuery] int district_id) =>
        Forward("master-data/ward", new JsonObject { ["district_id"] = district_id });

    [HttpPost("master-data/province")]
    public Task<IActionResult> Provinces(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] JsonObject? body = null) =>
        Forward("master-data/province", body ?? new JsonObject());

    [HttpPost("master-data/district")]
    public Task<IActionResult> Districts([FromBody] JsonObject body) => Forward("master-data/district", body);

    [HttpPost("master-data/ward")]
    public Task<IActionResult> Wards([FromBody] JsonObject body) => Forward("master-data/ward", body);

    [HttpPost("v2/shipping-order/available-services")]
    public Task<IActionResult> AvailableServices([FromBody] JsonObject body) =>
        Forward("v2/shipping-order/available-services", body, requiresShop: true);

    [HttpPost("v2/shipping-order/leadtime")]
    public Task<IActionResult> Leadtime([FromBody] JsonObject body) =>
        Forward("v2/shipping-order/leadtime", body, requiresShop: true);

    [HttpPost("v2/shipping-order/fee")]
    public Task<IActionResult> Fee([FromBody] JsonObject body) =>
        Forward("v2/shipping-order/fee", body, requiresShop: true);

    [HttpPost("calculate-delivery")]
    public Task<IActionResult> CalculateDelivery([FromBody] DeliveryRequest request) =>
        Forward("v2/shipping-order/leadtime", new JsonObject
        {
            ["from_district_id"] = 1444,
            ["from_ward_code"] = "20308",
            ["to_district_id"] = request.ToDistrictId,
            ["to_ward_code"] = request.ToWardCode,
            ["service_id"] = 53320
        }, requiresShop: true);

    private async Task<IActionResult> Forward(string path, JsonObject body, bool requiresShop = false)
    {
        var token = _config["GHNConfig:Token"];
        if (string.IsNullOrWhiteSpace(token))
            return StatusCode(503, new { message = "Cần điền GHNConfig:Token trong cấu hình riêng của backend." });

        var shopId = _config["GHNConfig:ShopId"];
        if (requiresShop && (!int.TryParse(shopId, out var parsedShopId) || parsedShopId <= 0))
            return StatusCode(503, new { message = "Cần điền GHNConfig:ShopId hợp lệ trong cấu hình riêng của backend." });

        // Only fixed actions reach GHN. Credentials never come from the browser.
        if (requiresShop && path == "v2/shipping-order/available-services")
            body["shop_id"] = int.Parse(shopId!);

        using var request = new HttpRequestMessage(HttpMethod.Post,
            "https://online-gateway.ghn.vn/shiip/public-api/" + path);
        request.Headers.Add("Token", token);
        if (requiresShop) request.Headers.Add("ShopId", shopId);
        request.Content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json");

        try
        {
            using var response = await _httpClient.SendAsync(request, HttpContext.RequestAborted);
            return new ContentResult
            {
                StatusCode = (int)response.StatusCode,
                ContentType = "application/json",
                Content = await response.Content.ReadAsStringAsync()
            };
        }
        catch (HttpRequestException)
        {
            return StatusCode(502, new { message = "Không thể kết nối GHN." });
        }
        catch (OperationCanceledException) when (!HttpContext.RequestAborted.IsCancellationRequested)
        {
            return StatusCode(504, new { message = "GHN không phản hồi trong thời gian cho phép." });
        }
    }
}

public class DeliveryRequest
{
    public int ToDistrictId { get; set; }
    public string ToWardCode { get; set; } = "";
}
