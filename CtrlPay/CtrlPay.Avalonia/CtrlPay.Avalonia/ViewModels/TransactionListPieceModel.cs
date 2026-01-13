using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.ViewModels;

// Společný základ pro vše, co může být v seznamu
public abstract class DashboardListItem : ObservableObject { }

// Model pro ten tvůj oddělovač
public class SeparatorItemViewModel : DashboardListItem
{
    public string Label { get; set; } = "";
}
public partial class TransactionItemViewModel : DashboardListItem
{
    public string Title { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }

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
                displayList.Add(new SeparatorItemViewModel { Label = groupName });
                currentGroup = groupName;
            }
            displayList.Add(tx);
        }

        Transactions = new ObservableCollection<DashboardListItem>(displayList);
    }

    private string GetGroupName(DateTime date, DateTime now)
    {
        var diff = (now - date.Date).Days;
        if (diff == 0) return "Dnes";
        if (diff == 1) return "Včera";
        if (diff <= 7) return "Před týdnem";
        if (diff <= 14) return "Před 2 týdny";
        return "Starší";
    }
}

