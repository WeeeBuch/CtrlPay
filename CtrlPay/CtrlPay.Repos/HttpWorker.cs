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

    public static async Task<string?> HttpPost(string url, object payload, bool requireAuth = true, CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{Credentials.BaseUri}{url}");

            // 1. Přidání hlavičky pouze pokud je vyžadována (Login ji nepotřebuje)
            if (requireAuth)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Credentials.JwtAccessToken);
            }

            // 2. Serializace dat do JSONu
            string jsonPayload = JsonSerializer.Serialize(payload);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // 3. Odeslání
            using var response = await _httpClient.SendAsync(request, cancellationToken);

            // Pokud server vrátí chybu, vyhodí výjimku, kterou chytíme níže
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[HttpPost] Chyba při volání {url}: {ex.Message}");
            // Můžeš sem přidat logování konkrétní chyby serveru, pokud je to třeba
            return null;
        }
    }
}