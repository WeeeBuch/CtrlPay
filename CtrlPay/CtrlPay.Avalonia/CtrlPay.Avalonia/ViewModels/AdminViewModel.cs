using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Entities;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.ViewModels;

public partial class AdminViewModel : ViewModelBase
{
    [ObservableProperty] private string _title = "System Administration";
    [ObservableProperty] private string? _searchText;
    [ObservableProperty] private string _selectedRole = "All";

    private List<FrontendUserDTO> _allUsers = new();
    public ObservableCollection<FrontendUserDTO> SystemUsers { get; } = new();

    public List<string> Roles { get; } = new() { "All", "Admin", "Accountant", "Employee", "Customer" };

    public AdminViewModel()
    {
        LoadUsers();
    }

    public async void LoadUsers()
    {
        _allUsers = await ToDoRepo.GetAdminUsers();
        ApplyFilter();
    }

    partial void OnSearchTextChanged(string? value) => ApplyFilter();
    partial void OnSelectedRoleChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        var filtered = _allUsers.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(u => u.Username.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        if (SelectedRole != "All")
        {
            filtered = filtered.Where(u => u.Role.ToString() == SelectedRole);
        }

        SystemUsers.Clear();
        foreach (var user in filtered)
        {
            SystemUsers.Add(user);
        }
    }
}
