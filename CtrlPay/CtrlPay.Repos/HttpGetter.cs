using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos;

public class HttpGetter
{
    public static async Task<string> HttpGet(string url)
    {
        var handler = new HttpClientHandler
        {
            UseProxy = false
        };

        using var httpClient = new HttpClient(handler);

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", Credentials.JwtAccessToken);
        string uri = $"{Credentials.BaseUri}{url}";
        // volání chráněného endpointu
        HttpResponseMessage response;

        try
        {
            response = await httpClient.GetAsync(uri);
        }
        catch (Exception ex)
        {
            // TODO: Karele tohle dodelej
            return "NaN";
        }
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
