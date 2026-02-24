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

    private List<FrontendUserDTO> _allUsers = [];
    public ObservableCollection<UserPieceViewModel> SystemUsers { get; } = [];

    public List<string> Roles { get; }

    public AdminViewModel()
    {
        // Dynamické načtení rolí
        Roles = ["All", .. Enum.GetNames(typeof(Role))];

        LoadUsers();

        // Dynamické aktualizace
        UpdateHandler.UpdatedAdminUsers.Add(() =>
        {
            LoadUsers();
        });
    }

    [RelayCommand]
    public void AddUser()
    {
        if (SystemUsers.Any(u => u.Editing && u.Model.Id == 0))
            return;

        var newDto = new FrontendUserDTO { Id = 0, Username = "", Role = Role.Employee };
        var newVm = new UserPieceViewModel(newDto) { Editing = true };
        
        SystemUsers.Insert(0, newVm);
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

        SyncCollections(filtered.ToList());
    }

    private void SyncCollections(List<FrontendUserDTO> data)
    {
        // 1. Připravíme si seznam výsledků, začneme těmi, co jsou noví (ID 0) a právě se editují
        var resultList = SystemUsers.Where(u => u.Model.Id == 0 && u.Editing).ToList();

        // 2. Projdeme data z databáze v pořadí, v jakém přišla
        foreach (var dto in data)
        {
            var existingVm = SystemUsers.FirstOrDefault(vm => vm.Model.Id == dto.Id);

            if (existingVm != null)
            {
                // Pokud už existuje a needitujeme ho, aktualizujeme mu data
                if (!existingVm.Editing)
                {
                    existingVm.Model = dto;
                }

                if (!resultList.Contains(existingVm))
                {
                    resultList.Add(existingVm);
                }
            }
            else
            {
                // Zcela nový uživatel z DB
                resultList.Add(new UserPieceViewModel(dto));
            }
        }

        // 3. Odstraníme ty, co už v datech nejsou (a nejsou to ti nově rozdělaní s ID 0)
        var toRemove = SystemUsers.Where(u => !resultList.Contains(u)).ToList();
        foreach (var item in toRemove) SystemUsers.Remove(item);

        // 4. Seřazení a synchronizace kolekce
        for (int i = 0; i < resultList.Count; i++)
        {
            var targetVm = resultList[i];
            if (i >= SystemUsers.Count)
            {
                SystemUsers.Add(targetVm);
            }
            else if (SystemUsers[i] != targetVm)
            {
                int oldIndex = SystemUsers.IndexOf(targetVm);
                if (oldIndex != -1) SystemUsers.Move(oldIndex, i);
                else SystemUsers.Insert(i, targetVm);
            }
        }
    }
}
