using CtrlPay.Entities;
using System.ComponentModel;

namespace CtrlPay.Repos.Frontend;

public class FrontendUserDTO : IEditableObject
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public Role Role { get; set; }
    public bool TwoFactorEnabled { get; set; }

    // Pomocná vlastnost pro zobrazení barvy role v UI
    public string RoleColor => Role switch
    {
        Role.Admin => "#FF4444",
        Role.Accountant => "#44FF44",
        _ => "#AAAAAA"
    };

    public FrontendUserDTO() { }

    public FrontendUserDTO(User user)
    {
        Id = user.Id;
        Username = user.Username;
        Role = user.Role;
        TwoFactorEnabled = user.TwoFactorEnabled;
    }

    // --- IEditableObject implementace ---
    private FrontendUserDTO? _oldVersion;

    public void BeginEdit()
    {
        _oldVersion = (FrontendUserDTO)this.MemberwiseClone();
    }

    public void CancelEdit()
    {
        if (_oldVersion == null) return;
        Id = _oldVersion.Id;
        Username = _oldVersion.Username;
        Role = _oldVersion.Role;
        TwoFactorEnabled = _oldVersion.TwoFactorEnabled;
    }

    public void EndEdit()
    {
        _oldVersion = null;
    }
}
