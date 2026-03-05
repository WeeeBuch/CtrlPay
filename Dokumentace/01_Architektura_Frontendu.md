# 01 Architektura Frontendu

  Tento soubor popisuje základní stavební kameny klientské aplikace `CtrlPay.Avalonia`. Architektura je navržena jako modulární a podporuje více platforem (Desktop, Android, Browser). V tomto souboru se zaměříme na Desktop, konkrétněji Windows.
  
## Hlavní technologie
   - **Avalonia UI**: Cross-platformní UI framework pro .NET.
   - **CommunityToolkit.Mvvm**: Knihovna pro implementaci vzoru MVVM (Model-View-ViewModel).
   - **ReactiveUI / Messaging**: Pro komunikaci mezi komponentami bez pevné vazby (využívá se `WeakReferenceMessenger`).


## Vzor MVVM (Model-View-ViewModel)
  
  Aplikace striktně odděluje vzhled od logiky:

1. **Views** (`Views/` a `Windows/`): Definují vzhled pomocí XAML. Obsahují pouze kód nezbytný pro UI (tzv. Code-behind).
2. **ViewModels** (`ViewModels/`): Obsahují logiku. Dědí z `ViewModelBase` (který využívá `ObservableObject`). Zde se zpracovávají příkazy (`RelayCommand`) a drží se data pro zobrazení.
3. **Models / DTOs**: Data přicházející z `CtrlPay.Repos`.

### Diagram toku dat:

``` mermaid
graph
View -- "Binding / Commands" --> ViewModel
ViewModel -- "Calls" --> Repository
Repository -- "HTTP Request" --> API
API -- "JSON Response" --> Repository
Repository -- "DTOs" --> ViewModel
ViewModel -- "NotifyPropertyChanged" --> View
```

## Inicializace a Start Aplikace (`App.axaml.cs`)
  Při spuštění aplikace probíhá rozhodovací proces, který určuje, jaké okno se uživateli zobrazí. Tento proces závisí na souboru `settings.json`.

``` csharp
public override void OnFrameworkInitializationCompleted()
{
	if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
	{
		if (IsConfigured) // Máme nastavené API a jazyk?
		{
			desktop.MainWindow = new LoginWindow();
		}
		else
		{
			// Pokud ne, spustíme průvodce nastavením
			desktop.MainWindow = new OnboardingWindow { DataContext = new OnboardingViewModel() };
		}
	}
	// ... podpora pro Mobile/Web SingleView
}
```

## Navigace a přepínání pohledů
  Navigace v rámci hlavního okna není řešena otevíráním nových oken, ale dynamickou výměnou obsahu (`ContentControl`) pomocí třídy Navigator.cs.

   - `ViewLocator`: Automaticky mapuje ViewModel na odpovídající View na základě názvu (např. MainViewModel -> MainView).
   - `Navigator`: Umožňuje z ViewModelu zavolat změnu stránky, aniž by ViewModel věděl o existenci UI komponent.


## Klíčové komponenty
   - `SettingsManager`: Stará se o ukládání a načítání lokální konfigurace aplikace.
   - `TranslationManager`: Zajišťuje lokalizaci textů (vícejazyčnost) za běhu aplikace.
   - `ThemeManager`: Přepíná mezi světlým a tmavým režimem úpravou zdrojů (Styles).