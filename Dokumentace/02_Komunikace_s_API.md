# 02 Komunikace s API

Tento soubor popisuje, jak klientská aplikace komunikuje se serverem (API) pomocí protokolu HTTP.

## Třída `HttpWorker.cs`
Hlavním komunikačním uzlem aplikace je třída `HttpWorker` (v `CtrlPay.Repos`), která se stará o všechny síťové požadavky. Používá statickou instanci `HttpClient`, aby se zabránilo vyčerpání systémových socketů.

### Hlavní metody:
- **`HttpGet(url, auth)`**: Stáhne data (JSON) ze serveru.
- **`HttpPost(url, payload, auth)`**: Odešle data na server (např. při přihlášení nebo vytvoření platby).
- **`HttpDelete(url, payload, auth)`**: Pošle požadavek na smazání dat.

Metody automaticky doplňují základní URL z `Credentials.BaseUri` a v případě potřeby přidávají Authorization hlavičku.

## Autentizace a JWT
Aplikace využívá pro zabezpečení **JWT (JSON Web Token)**. Jakmile se uživatel přihlásí přes `AuthRepo.Login`, tokeny a role se uloží do statické třídy `Credentials`. `HttpWorker` tyto údaje automaticky používá pro autorizované požadavky:

```csharp
if (requireAuth)
{
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Credentials.JwtAccessToken);
}
```

##  Data Transfer Objects (DTOs)
Abychom v aplikaci nepracovali s čistým JSONem, využíváme tzv. **DTOs** (Data Transfer Objects). Jsou to jednoduché třídy, které přesně odpovídají struktuře dat, kterou posílá server.

Tyto třídy najdeš ve složce `CtrlPay.Repos/Frontend`. Například:
- **`FrontendPaymentDTO`**: Detaily o platbě (částka, adresa, stav).
- **`FrontendCustomerDTO`**: Informace o zákazníkovi.
- **`FrontendTransactionDTO`**: Detaily o blockchainové transakci.

### Převod dat (Deserializace):
Data přicházející z API se automaticky mění na tyto objekty pomocí knihovny `System.Text.Json` (s nastavením `PropertyNameCaseInsensitive = true`).

## Logování a Error Handling
Pro snazší hledání chyb je každý HTTP požadavek i odpověď logována pomocí `AppLogger`. 
- `HttpWorker` zachytává výjimky a v případě chyby vrací `null`.
- Volající repozitáře (např. `AuthRepo`, `PaymentRepo`) pak na základě toho vrací příslušný `ReturnModel` s chybovým kódem pro uživatele.
