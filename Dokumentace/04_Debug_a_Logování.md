# 04 Debug a Logování

Tento soubor popisuje nástroje pro diagnostiku chyb a sledování běhu aplikace.

## AppLogger (Systémový log)
Aplikace používá asynchronní logger `AppLogger` (v `CtrlPay.Repos.Frontend`), který běží na pozadí pomocí `System.Threading.Channels`. To zajišťuje, že zápis na disk nebrzdí uživatelské rozhraní.

### Kde najdu logy?
Logy se ukládají do standardní složky AppData:
- **Windows**: `%AppData%\Roaming\CtrlPay\app_logs.txt`

### Úrovně logování:
- **`INFO`**: Běžné události (start aplikace, přepnutí okna, volání API).
- **`WARNING`**: Varování, která nevyžadují pád aplikace.
- **`ERROR`**: Kritické chyby, výjimky (Exception) nebo neúspěšná volání API.

### Technické detaily:
Logger běží v samostatném vlákně (`LongRunning task`) a zprávy zpracovává sekvenčně z fronty, aby nedocházelo ke kolizím při zápisu do souboru. Každý záznam obsahuje časovou značku a ID procesu (PID).

## Debug Mode (`DebugMode.cs`)
Aplikace obsahuje integrovaný režim pro vývojáře, který umožňuje simulovat různé stavy bez nutnosti reálného připojení k API nebo blockchainu. Aktivuje se příznakem `StartDebug`.

### Co Debug Mode umožňuje:
- **OverrideRole**: Možnost změnit roli přihlášeného uživatele (Customer, Accountant atd.) pro testování různých částí UI.
- **SkipAuth**: Přeskočení přihlašovacího a registračního procesu (`SkipAuthLogin`, `SkipAuthReg`).
- **Mockování dat**: Simulace dat pro platby, transakce a zákazníky bez reálného volání API (`MockPayments`, `MockTransactions`, `MockCustomers`).
- **Simulace chyb**: Možnost testovat chování aplikace při selhání API nebo generování adres.

### DebugWindow
Při spuštění v debug režimu se otevře samostatné okno, které slouží jako ovládací panel pro vývojáře. 

**Klíčové vlastnosti:**
- **Dynamické UI (Reflexe)**: Okno nevyužívá pevně definovaný XAML pro každé nastavení. Místo toho pomocí reflexe projde všechny vlastnosti třídy `DebugMode.cs` a automaticky pro ně vygeneruje příslušné ovládací prvky (CheckBoxy pro bool, TextBoxy pro string atd.). To umožňuje přidávat nová ladicí nastavení bez úpravy UI okna.
- **Okamžitá odezva**: Změna jakékoli hodnoty v tomto okně okamžitě aktualizuje globální stav aplikace. Pro komunikaci se využívá `WeakReferenceMessenger`, aby nedocházelo k memory leakům při častém otevírání/zavírání oken.

