# 04 Debug a Logování

Tento soubor popisuje nástroje pro diagnostiku chyb a sledování běhu aplikace.

## AppLogger (Systémový log)
Aplikace používá vlastní asynchronní logger `AppLogger`, který běží na pozadí a neblokuje uživatelské rozhraní.

### Kde najdu logy?
Logy se ukládají do složky uživatelských dat (AppData). Soubor se dá nejít zde:
- **Windows**: `%AppData%\Roaming\CtrlPay\app_logs.txt`

### Úrovně logování:
- `INFO`: Běžné informace o běhu (start aplikace, přepnutí okna).
- `WARNING`: Potenciální problémy, které nezpůsobily pád.
- `ERROR`: Kritické chyby, výjimky (Exception) nebo neúspěšná volání API.

### Technické řešení:
Logger využívá `System.Threading.Channels`, což zajišťuje, že zápis na disk probíhá v samostatném vlákně a nezpomaluje aplikaci.

## Debug Mode
Aplikace obsahuje skrytý režim pro vývojáře. Pokud je aktivován, zobrazí se **DebugWindow**.

### Co DebugWindow umí:
- Možnost simulovat určité stavy pro testování UI.

