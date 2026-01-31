using System.Net.Http.Headers;

namespace CtrlPay.Repos;

public class HttpWorker
{
    // Statická instance, aby nedocházelo k vyčerpání socketů
    private static readonly HttpClient _httpClient = new(new HttpClientHandler { UseProxy = false });

    public static async Task<string?> HttpGet(string url, CancellationToken cancellationToken = default)
    {
        if (url[0] != '/')
        {
            url = "/" + url;
        }

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{Credentials.BaseUri}{url}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Credentials.JwtAccessToken);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Tady můžeš logovat chyby, pokud máš logger
            Console.WriteLine($"[HttpGetter] Chyba při volání {url}: {ex.Message}");
            return null;
        }
    }

    public static async Task<string?> HttpPost(string url, CancellationToken cancellationToken = default)
    {
        if (url[0] != '/')
        {
            url = "/" + url;
        }

        try
        {


        }
        catch (Exception ex)
        {
            Console.WriteLine($"[HttpPostter] Chyba při volání {url}: {ex.Message}");
            return null;
        }
    }
}