using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System.Collections.ObjectModel;

namespace CtrlPay.Avalonia.ViewModels;

public partial class AdminViewModel : ViewModelBase
{
    [ObservableProperty] private string _title = "System Administration";
    public ObservableCollection<FrontendUserDTO> SystemUsers { get; } = new();
    
    public AdminViewModel()
    {
        LoadUsers();
    }

    public async void LoadUsers()
    {
        var users = await ToDoRepo.GetAdminUsers();
        SystemUsers.Clear();
        foreach (var user in users)
        {
            SystemUsers.Add(user);
        }
    }
}
