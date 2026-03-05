# 02 Komunikace s API

Tento soubor popisuje, jak klientská aplikace komunikuje se serverem (API) pomocí protokolu HTTP.

## Třída `HttpWorker.cs`
Hlavním komunikačním uzlem aplikace je třída `HttpWorker`, která se stará o všechny síťové požadavky. Používá statickou instanci `HttpClient`, aby se zabránilo vyčerpání systémových socketů.

### Hlavní metody:
- **`HttpGet(url, auth)`**: Stáhne data (JSON) ze serveru.
- **`HttpPost(url, payload, auth)`**: Odešle data na server (např. při přihlášení nebo vytvoření platby).
- **`HttpDelete(url, payload, auth)`**: Pošle požadavek na smazání dat.

## Autentizace a JWT
Aplikace využívá pro zabezpečení **JWT (JSON Web Token)**. Jakmile se uživatel přihlásí, token se uloží v paměti a `HttpWorker` ho automaticky přidává do každého dalšího požadavku:

```csharp
if (requireAuth)
{
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Credentials.JwtAccessToken);
}
```

##  Data Transfer Objects (DTOs)
Abychom v aplikaci nepracovali s čistým JSONem, využíváme tzv. **DTOs** (Data Transfer Objects). Jsou to jednoduché třídy, které přesně odpovídají struktuře dat, kterou posílá server.

Tyto třídy najdeš ve složce `CtrlPay.Repos/Frontend`. Například:
- `FrontendPaymentDTO.cs`: Obsahuje detaily o platbě (částka, adresa, stav).
- `FrontendUserDTO.cs`: Informace o přihlášeném uživateli.

### Převod dat (Deserializace):
Data přicházející z API se automaticky mění na tyto objekty pomocí knihovny `System.Text.Json`:
```csharp
// Příklad použití (pseudokód):
string jsonResponse = await HttpWorker.HttpGet("/api/payments");
var payments = JsonSerializer.Deserialize<List<FrontendPaymentDTO>>(jsonResponse);
```

## Logování požadavků
Pro snazší hledání chyb je každý HTTP požadavek i odpověď logována pomocí `AppLogger`. Pokud se něco nepovede (např. server vrátí chybu 500), v logu uvidíme přesnou chybu i s detaily.
