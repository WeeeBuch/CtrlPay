using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace CtrlPay.Avalonia.ViewModels;

// ViewModel pro jeden řádek v seznamu
public partial class DebugPropertyViewModel : ObservableObject
{
    private readonly PropertyInfo _info;

    public string Name { get; }

    // Pomocné vlastnosti pro XAML, aby věděl, co zobrazit
    public bool IsBool => _info.PropertyType == typeof(bool);
    public bool IsEnum => _info.PropertyType.IsEnum;

    // Seznam hodnot pro Dropdown (pokud je to Enum)
    public System.Collections.IEnumerable? EnumValues =>
        IsEnum ? Enum.GetValues(_info.PropertyType) : null;

    // Hodnota musí být 'object', aby zvládla bool i Role
    public object Value
    {
        get => _info.GetValue(null)!;
        set
        {
            var current = _info.GetValue(null);
            if (value != null && !Equals(current, value))
            {
                if (_info.PropertyType.IsInstanceOfType(value) ||
                    (_info.PropertyType == typeof(bool) && value is bool))
                {
                    _info.SetValue(null, value);
                    OnPropertyChanged();
                    WeakReferenceMessenger.Default.Send(new DebugModeChangedMessage(Name, value));
                }
            }
        }
    }

    public record DebugModeChangedMessage(string PropertyName, object NewValue);

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
            .Where(p => (p.PropertyType == typeof(bool) || p.PropertyType.IsEnum)
                        && p.Name != "StartDebug")
            .Select(p => new DebugPropertyViewModel(p));

        DebugItems = new ObservableCollection<DebugPropertyViewModel>(props);
    }
}