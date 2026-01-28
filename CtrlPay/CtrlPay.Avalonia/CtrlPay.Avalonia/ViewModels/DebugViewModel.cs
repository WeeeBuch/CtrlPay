using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CtrlPay.Repos.Frontend;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace CtrlPay.Avalonia.ViewModels;

// ViewModel pro jeden řádek v seznamu
public partial class DebugPropertyViewModel : ObservableObject
{
    private readonly PropertyInfo _info;

    public string Name { get; }

    public bool Value
    {
        get => (bool)_info.GetValue(null)!;
        set
        {
            if ((bool)_info.GetValue(null)! != value)
            {
                _info.SetValue(null, value);
                OnPropertyChanged();

                // Pošleme globální zprávu, že se změnil Debug mód
                WeakReferenceMessenger.Default.Send(new DebugModeChangedMessage(Name, value));
            }
        }
    }

    // Jednoduchý záznam pro zprávu
    public record DebugModeChangedMessage(string PropertyName, bool NewValue);

    public DebugPropertyViewModel(PropertyInfo info)
    {
        _info = info;
        Name = info.Name;
    }
}

// Hlavní ViewModel okna
public partial class DebugWindowViewModel : ObservableObject
{
    public ObservableCollection<DebugPropertyViewModel> DebugItems { get; }

    public DebugWindowViewModel()
    {
        var props = typeof(DebugMode)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(bool) && p.Name != "StartDebug")
            .Select(p => new DebugPropertyViewModel(p));

        DebugItems = new ObservableCollection<DebugPropertyViewModel>(props);
    }
}