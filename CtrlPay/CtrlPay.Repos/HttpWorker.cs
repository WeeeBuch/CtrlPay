using CtrlPay.Repos.Frontend;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

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
            AppLogger.Info($"Praparing http GET...");
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{Credentials.BaseUri}{url}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Credentials.JwtAccessToken);

            AppLogger.Info($"Calling API...");
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            AppLogger.Info($"Returning API response...");
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            AppLogger.Error($"API Get failed.", ex);
            return null;
        }
    }

    public static async Task<string?> HttpPost(string url, object payload, bool requireAuth = true, CancellationToken cancellationToken = default)
    {
        if (url[0] != '/')
        {
            url = "/" + url;
        }

        try
        {
            AppLogger.Info($"Praparing http POST...");
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{Credentials.BaseUri}{url}");

            // 1. Přidání hlavičky pouze pokud je vyžadována (Login ji nepotřebuje)
            if (requireAuth)
            {
                AppLogger.Info($"Adding authorization...");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Credentials.JwtAccessToken);
            }

            // 2. Serializace dat do JSONu
            AppLogger.Info($"Serializing payload...");
            string jsonPayload = JsonSerializer.Serialize(payload);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // 3. Odeslání
            AppLogger.Info($"Calling API...");
            using var response = await _httpClient.SendAsync(request, cancellationToken);

            // Pokud server vrátí chybu, vyhodí výjimku, kterou chytíme níže
            response.EnsureSuccessStatusCode();

            AppLogger.Info($"Returning API response...");
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            AppLogger.Error($"API Post failed.", ex);
            return null;
        }
    }
}