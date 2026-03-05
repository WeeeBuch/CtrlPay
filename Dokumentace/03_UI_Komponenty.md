# 03 UI Komponenty a Stylizace

Tento soubor popisuje vizuální část aplikace, strukturu oken a systémy pro motivy a lokalizaci.

## Struktura oken a pohledů
Aplikace rozlišuje mezi **Okny (Windows)**, která mají vlastní rám v operačním systému, a **Pohledy (Views)**, které se dynamicky přepínají uvnitř oken.

### Hlavní okna:
- **OnboardingWindow**: Průvodce prvním spuštěním.
- **LoginWindow**: Přihlašovací obrazovka.
- **MainWindow**: Hlavní kontejner aplikace po přihlášení.
- **DebugWindow**: Speciální okno pro vývojáře (logy, stavy).

### Klíčové pohledy (Views):
- **DashboardView**: Přehled základních statistik.
- **PaymentManagementView**: Správa a vytváření plateb.
- **AccountantDashboardView**: Specializovaný pohled pro účetní s grafy a přehledy.
- **QrCodeView**: Zobrazení platebních údajů ve formě QR kódu.

## Onboarding Flow
Při prvním spuštění (nebo pokud chybí konfigurace) aplikace vynutí průvodce:
1. **APIConnectView**: Nastavení adresy serveru a ověření spojení.
2. **OnboardingView**: Volba jazyka a základní nastavení.
Jakmile je hotovo, `SettingsManager` uloží data do `settings.json` a aplikace se restartuje do přihlašovacího okna.

## Dynamická lokalizace (`TranslationManager.cs`)
Aplikace podporuje změnu jazyka za běhu bez nutnosti restartu. Překlady jsou uloženy v XAML slovnících ve složce `Translations/`.

### Podporované jazyky:
- **Standardní**: Angličtina, Čeština, Slovenština, Němčina, Francouzština, Španělština, Polština, Ruština, Japonština, Mandarínština.
- **Speciální**: Moravština (Moravian), Pirátská mluva (Pirate), Leetspeak.

### Klíčové funkce:
- **Detekce systému**: Metoda `GetCurrentSystemLanguage` automaticky nastaví jazyk podle OS při prvním spuštění.
- **Změna za běhu**: Metoda `Apply(AppLanguage language)` dynamicky vymění zdroje v `Application.Current.Resources`.
- **Překlad v kódu**: Metoda `GetString(key)` slouží k získání lokalizovaného řetězce přímo v C# kódu.

## Systém motivů a barev
Aplikace podporuje dynamickou změnu barev. Všechny barvy jsou definovány v XAML souborech ve složce `Styles/`.

### Seznam dostupných barev:
- **Modrá**: `Blue.axaml`, `LightBlue.axaml`
- **Zelená**: `Green.axaml`, `DarkGreen.axaml`, `Lime.axaml`
- **Oranžová/Žlutá**: `Orange.axaml`, `DarkOrange.axaml`, `LightOrange.axaml`, `Yellow.axaml`
- **Červená/Růžová**: `Red.axaml`, `Pink.axaml`
- **Ostatní**: `Purple.axaml`, `Turquoise.axaml`

### Správa stylů (`ThemeManager.cs`):
Tato třída umožňuje přepínat motivy za běhu aplikace:
```csharp
ThemeManager.Apply(ThemeEnum.DarkBlue);
```

### Speciální styly:
- **StatusBorder / StatusEllipse**: Sjednocené styly pro zobrazení stavu plateb (zelená pro zaplaceno, červená pro chybu atd.).
- **StylePillList**: Styl pro zaoblené seznamy a karty.
