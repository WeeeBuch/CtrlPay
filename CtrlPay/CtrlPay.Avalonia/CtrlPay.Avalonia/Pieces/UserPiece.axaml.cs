using Avalonia.Controls;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia;

public partial class UserPiece : UserControl
{
    public UserPiece()
    {
        InitializeComponent();
    }

    private void OnRoleSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is UserPieceViewModel vm)
        {
            vm.RefreshRoleVisibility();
        }
    }
}
