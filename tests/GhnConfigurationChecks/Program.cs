using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text.Json.Nodes;

static void Check(bool passed, string message)
{
    if (!passed) throw new Exception(message);
}

static GHNController Controller(Dictionary<string, string?> settings, RecordingHandler handler)
{
    return new GHNController(new ConfigurationBuilder().AddInMemoryCollection(settings).Build(),
        new TestClientFactory(handler))
    {
        ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
    };
}

var missingHandler = new RecordingHandler();
var missing = Controller(new(), missingHandler);
Check((await missing.GetProvinces() as ObjectResult)?.StatusCode == 503,
    "Missing token must return 503.");
Check(missingHandler.Calls == 0, "Missing credentials must not make an external request.");

var invalidShopHandler = new RecordingHandler();
var invalidShop = Controller(new() { ["GHNConfig:Token"] = "test-only-token", ["GHNConfig:ShopId"] = "invalid" }, invalidShopHandler);
Check((await invalidShop.Fee(new()) as ObjectResult)?.StatusCode == 503,
    "An invalid shop ID must return 503.");
Check(invalidShopHandler.Calls == 0, "Invalid shop configuration must not reach GHN.");

var handler = new RecordingHandler();
var configured = Controller(new() { ["GHNConfig:Token"] = "test-only-token", ["GHNConfig:ShopId"] = "123" }, handler);
var result = await configured.AvailableServices(new JsonObject
{
    ["shop_id"] = 999, ["from_district"] = 1, ["to_district"] = 2
}) as ContentResult;
Check(handler.Url == "https://online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/available-services",
    "Requests must use the fixed HTTPS GHN destination.");
Check(handler.Token == "test-only-token" && handler.ShopId == "123",
    "Credentials must come from backend configuration.");
Check(JsonNode.Parse(handler.Body!)!["shop_id"]!.GetValue<int>() == 123,
    "The browser must not override the configured shop ID.");
Check(result?.StatusCode == 200 && result.Content == "{\"code\":200,\"data\":[]}",
    "The proxy must preserve the upstream response without adding credentials.");

await configured.Districts(new JsonObject { ["province_id"] = 42 });
Check(JsonNode.Parse(handler.Body!)!["province_id"]!.GetValue<int>() == 42,
    "District request parameters must be preserved.");
handler.ResponseStatus = HttpStatusCode.BadRequest;
Check((await configured.GetWards(7) as ContentResult)?.StatusCode == 400,
    "Upstream failures must retain their HTTP status.");
handler.ThrowNetworkError = true;
Check((await configured.GetProvinces() as ObjectResult)?.StatusCode == 502,
    "Network failures must return a safe 502 response.");
Console.WriteLine("PASS: 11 GHN configuration and forwarding assertions; no external requests.");

sealed class TestClientFactory : IHttpClientFactory
{
    private readonly HttpClient _client;
    public TestClientFactory(HttpMessageHandler handler) => _client = new HttpClient(handler);
    public HttpClient CreateClient(string name) => _client;
}

sealed class RecordingHandler : HttpMessageHandler
{
    public int Calls { get; private set; }
    public string? Url { get; private set; }
    public string? Token { get; private set; }
    public string? ShopId { get; private set; }
    public string? Body { get; private set; }
    public HttpStatusCode ResponseStatus { get; set; } = HttpStatusCode.OK;
    public bool ThrowNetworkError { get; set; }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Calls++;
        if (ThrowNetworkError) throw new HttpRequestException("Test network failure");
        Url = request.RequestUri!.AbsoluteUri;
        Token = request.Headers.GetValues("Token").Single();
        ShopId = request.Headers.TryGetValues("ShopId", out var values) ? values.Single() : null;
        Body = await request.Content!.ReadAsStringAsync();
        return new HttpResponseMessage(ResponseStatus)
        {
            Content = new StringContent("{\"code\":200,\"data\":[]}")
        };
    }
}
