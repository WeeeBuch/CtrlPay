using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.Translations;
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
    public class RoleFilterItem
    {
        public string Name { get; set; } = string.Empty;
        public Role? Value { get; set; }
        public override string ToString() => Name;
    }

    [ObservableProperty] private string? _searchText;
    [ObservableProperty] private RoleFilterItem? _selectedRoleItem;

    private List<FrontendUserDTO> _allUsers = [];
    public ObservableCollection<UserPieceViewModel> SystemUsers { get; } = [];

    public ObservableCollection<RoleFilterItem> Roles { get; } = [];

    public AdminViewModel()
    {
        InitializeRoles();
        LoadUsers();

        TranslationManager.LanguageChanged.Add(() => 
        {
            var currentRole = SelectedRoleItem?.Value;
            InitializeRoles();
            SelectedRoleItem = Roles.FirstOrDefault(r => r.Value == currentRole) ?? Roles[0];
        });

        // Dynamické aktualizace
        UpdateHandler.UpdatedAdminUsers.Add(() =>
        {
            LoadUsers();
        });
    }

    private void InitializeRoles()
    {
        Roles.Clear();
        Roles.Add(new RoleFilterItem { Name = TranslationManager.GetString("AdminView.Filter.AllRoles"), Value = null });

        foreach (Role role in Enum.GetValues(typeof(Role)))
        {
            Roles.Add(new RoleFilterItem { Name = role.ToString(), Value = role });
        }

        if (SelectedRoleItem == null) SelectedRoleItem = Roles[0];
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
        _allUsers = AdminRepo.GetUsers();
        ApplyFilter();
    }

    partial void OnSearchTextChanged(string? value) => ApplyFilter();
    partial void OnSelectedRoleItemChanged(RoleFilterItem? value) => ApplyFilter();

    private void ApplyFilter()
    {
        var filtered = _allUsers.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(u => u.Username.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        if (SelectedRoleItem?.Value != null)
        {
            filtered = filtered.Where(u => u.Role == SelectedRoleItem.Value);
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
