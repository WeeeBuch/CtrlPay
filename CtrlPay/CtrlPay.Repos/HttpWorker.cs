using CtrlPay.Repos.Frontend;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CtrlPay.Repos;

public class HttpWorker
{
    // Statická instance, aby nedocházelo k vyčerpání socketů
    private static readonly HttpClient _httpClient = new(new HttpClientHandler { UseProxy = false });

    private static string BuildUrl(string path)
    {
        var baseUri = (Credentials.BaseUri ?? "").TrimEnd('/');
        if (path.Length > 0 && path[0] != '/') path = "/" + path;
        return baseUri + path;
    }

    public static async Task<string?> HttpGet(string url, bool requireAuth = true, CancellationToken cancellationToken = default)
    {
        if (url.Length > 0 && url[0] != '/')
            url = "/" + url;

        try
        {
            AppLogger.Info($"Praparing http GET...");
            using var request = new HttpRequestMessage(HttpMethod.Get, BuildUrl(url));

            if (requireAuth)
            {
                AppLogger.Info($"Adding authorization...");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Credentials.JwtAccessToken);
            }

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
        if (url.Length > 0 && url[0] != '/')
            url = "/" + url;

        try
        {
            AppLogger.Info($"Praparing http POST...");
            using var request = new HttpRequestMessage(HttpMethod.Post, BuildUrl(url));

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

    public static async Task<string?> HttpDelete(string url, object payload, bool requireAuth = true, CancellationToken cancellationToken = default)
    {
        if (url.Length > 0 && url[0] != '/')
            url = "/" + url;

        try
        {
            AppLogger.Info($"Praparing http POST...");
            using var request = new HttpRequestMessage(HttpMethod.Delete, BuildUrl(url));

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