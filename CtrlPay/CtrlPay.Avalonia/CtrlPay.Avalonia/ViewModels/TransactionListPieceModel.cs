using CommunityToolkit.Mvvm.ComponentModel;
using CtrlPay.Avalonia.Translations;
using CtrlPay.Entities;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CtrlPay.Avalonia.ViewModels;

// Společný základ pro vše, co může být v seznamu
public abstract class DashboardListItem : ObservableObject { }

// Model pro ten tvůj oddělovač
public partial class SeparatorItemViewModel : DashboardListItem
{
    [ObservableProperty]
    private string? label;

    public string? LabelKey;

    public SeparatorItemViewModel()
    {
        TranslationManager.LanguageChanged.Add(UpdateLabel);
    }

    private void UpdateLabel()
    {
        if (!string.IsNullOrEmpty(LabelKey))
        {
            Label = TranslationManager.GetString(LabelKey);
        }
    }

    public void GiveLabelKey(string key)
    {
        LabelKey = key;
        UpdateLabel();
    }
}
public partial class TransactionItemViewModel : DashboardListItem
{
    public string Title { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public StatusEnum Status { get; set; }

    public string StatusText
    {
        get
        {
            return Status switch
            {
                StatusEnum.Completed => TranslationManager.GetString("Transaction.Status.Completed"),
                StatusEnum.Pending => TranslationManager.GetString("Transaction.Status.Pending"),
                StatusEnum.Failed => TranslationManager.GetString("Transaction.Status.Failed"),
                StatusEnum.Confirmed => TranslationManager.GetString("Transaction.Status.Confirmed"),
                StatusEnum.Created => TranslationManager.GetString("Transaction.Status.Created"),
                StatusEnum.WaitingForPayment => TranslationManager.GetString("Transaction.Status.WaitingForPayment"),
                StatusEnum.PartiallyPaid => TranslationManager.GetString("Transaction.Status.PartiallyPaid"),
                StatusEnum.Paid => TranslationManager.GetString("Transaction.Status.Paid"),
                StatusEnum.Overpaid => TranslationManager.GetString("Transaction.Status.Overpaid"),
                StatusEnum.Expired => TranslationManager.GetString("Transaction.Status.Expired"),
                StatusEnum.Cancelled => TranslationManager.GetString("Transaction.Status.Cancelled"),
                StatusEnum.Error => TranslationManager.GetString("Transaction.Status.Error"),
                _ => "Nah state not implemented WTF"
            };
        }
    }

    // Pomocná vlastnost, pokud chceme zobrazit oddělovač nad touto položkou
    [ObservableProperty]
    private string? _dateSeparator;
}

public partial class TransactionListPieceModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<DashboardListItem> _transactions = new();

    public void RefreshTransactions(List<TransactionItemViewModel> rawData)
    {
        AppLogger.Info($"Refreshing transactions...");
        var displayList = new List<DashboardListItem>();
        var now = DateTime.Now.Date;

        // Seřadíme od nejnovějších
        var sorted = rawData.OrderByDescending(x => x.Date).ToList();

        string currentGroup = "";

        foreach (var tx in sorted)
        {
            string groupName = GetGroupName(tx.Date, now);

            // Pokud se skupina změnila (např. z Dnes na Včera), vložíme oddělovač
            if (groupName != currentGroup)
            {
                SeparatorItemViewModel separator = new();
                separator.GiveLabelKey(groupName);
                displayList.Add(separator);
                currentGroup = groupName;
            }
            displayList.Add(tx);
        }

        Transactions = new ObservableCollection<DashboardListItem>(displayList);
        AppLogger.Info($"Transactions refreshed succesfully.");
    }

    private string GetGroupName(DateTime date, DateTime now)
    {
        var diff = (now - date.Date).Days;
        if (diff == 0) return "Date.Today";
        if (diff == 1) return "Date.Yesterday";
        if (diff <= 7) return "Date.LastWeek";
        if (diff <= 14) return "Date.Last2Weeks";
        return "Date.Older";
    }
}

