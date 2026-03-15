# 03 UI Komponenty a Stylizace

Tento soubor popisuje vizuální část aplikace, strukturu oken a systémy pro motivy a lokalizaci.

## Struktura oken a pohledů
Aplikace rozlišuje mezi **Okny (Windows)**, která mají vlastní rám v operačním systému, a **Pohledy (Views)**, které se dynamicky přepínají uvnitř oken.

### Hlavní okna:
- **OnboardingWindow**: Průvodce prvním spuštěním.
- **LoginWindow**: Přihlašovací obrazovka.
- **MainWindow**: Hlavní kontejner aplikace po přihlášení.
- **DebugWindow**: Speciální okno pro vývojáře (logy, stavy).
- **QrCodeWindow**: Samostatné okno pro zobrazení QR kódu platby.

### Klíčové pohledy (Views):
- **DashboardView**: Přehled základních statistik pro zákazníka.
- **DebtView**: Přehled dluhů/pohledávek.
- **TransactionView**: Historie transakcí.
- **CustomersListView**: Správa zákazníků (pro roli Accountant).
- **PaymentManagementView**: Správa plateb (pro roli Accountant).
- **SettingsView**: Nastavení aplikace (jazyk, téma).

## Onboarding Flow
Při prvním spuštění (nebo pokud chybí konfigurace) aplikace vynutí průvodce:
1. **OnboardingView**: Volba jazyka a základní nastavení.
2. **APIConnectView**: Nastavení adresy serveru a ověření spojení.
Jakmile je hotovo, `SettingsManager` uloží data do `settings.json` a aplikace se restartuje do přihlašovacího okna.

## Zdroje a Stylování
V celém projektu důsledně využíváme **`DynamicResource`** místo `StaticResource`. Důvodem je podpora dynamických změn bez nutnosti restartu aplikace. 

Při změně tématu nebo jazyka se aktualizují prostředky v `Application.Current.Resources` a všechny prvky s `DynamicResource` se na tuto změnu automaticky překreslí.

## Dynamická lokalizace (`TranslationManager.cs`)
Aplikace podporuje změnu jazyka za běhu bez nutnosti restartu. Překlady jsou uloženy v XAML slovnících ve složce `Translations/`.

### Podporované jazyky:
- **Standardní**: Angličtina, Čeština, Slovenština, Němčina, Francouzština, Španělština, Polština, Ruština, Japonština, Mandarínština.
- **Speciální**: Moravština (Moravian), Pirátská mluva (Pirate), Leetspeak.

### Klíčové funkce:
- **Detekce systému**: Metoda `GetCurrentSystemLanguage` automaticky nastaví jazyk podle OS při prvním spuštění.
- **Změna za běhu**: Metoda `Apply(AppLanguage language)` dynamicky vymění zdroje v `Application.Current.Resources` a upozorní odebírající komponenty (přes `LanguageChanged` event).
- **Překlad v kódu**: Metoda `GetString(key)` slouží k získání lokalizovaného řetězce přímo v C# kódu.

## Systém barevných témat (`ThemeManager.cs`)
Aplikace nepoužívá jen jednoduchý světlý/tmavý režim, ale umožňuje výběr z celé řady barevných schémat. Každé téma je definováno jako samostatný `.axaml` soubor ve složce `Styles/`.

### Dostupná témata (AppTheme):
- Lime, Blue, Green, Orange, Pink, Purple, Red, Yellow, Turquoise.
- Tmavé varianty: DarkGreen, DarkOrange.
- Světlé varianty: LightBlue, LightOrange.

### Aplikace tématu:
Téma se aplikuje metodou `ThemeManager.Apply(AppTheme theme)`, která dynamicky odstraní předchozí téma z `MergedDictionaries` a přidá nové.

## Vlastní komponenty (Pieces)
V projektu využíváme vlastní UI prvky pro sjednocení vzhledu a modularitu:

### Kompozice a ViewModely
Každý "Piece" je zapouzdřená komponenta, která má svůj vlastní ViewModel (pokud je to potřeba). 
- **Nezávislost**: `TransactionListPiece` může být použit v `DashboardView` i v `TransactionView`.
- **Komunikace**: Pokud spolu "Pieces" potřebují mluvit, využívají k tomu buď `WeakReferenceMessenger` (CommunityToolkit.Mvvm), nebo sdílené události v `UpdateHandleru`.

Dostupné Pieces:
- **CounterPiece**: Zobrazuje číselné statistiky (např. celkový dluh).
- **CustomerPiece**: Karta s informacemi o zákazníkovi.
- **TransactionListPiece**: Komponenta pro zobrazení seznamu transakcí.
- **NavbarView**: Boční navigační panel.
