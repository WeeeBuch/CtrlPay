using CtrlPay.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace CtrlPay.Repos.Frontend;

public class FrontendUserDTO : IEditableObject
{
    public int Id { get; set; }
    public Role Role { get; set; }
    public int? LoyalCustomerId { get; set; }
    public int? AccountId { get; set; }
    public int? CustomerId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; } = string.Empty;
    public bool TwoFactorEnabled { get; set; }
    private string? TwoFactorRecoveryCodesJson { get; set; } = string.Empty;

    [NotMapped]
    public string[] TwoFactorRecoveryCodes
    {
        get => string.IsNullOrWhiteSpace(TwoFactorRecoveryCodesJson)
               ? Array.Empty<string>()
               : JsonSerializer.Deserialize<string[]>(TwoFactorRecoveryCodesJson)!;
        set => TwoFactorRecoveryCodesJson = JsonSerializer.Serialize(value);
    }

    // Pomocná vlastnost pro zobrazení barvy role v UI
    public string RoleColor => Role switch
    {
        Role.Admin => "#FF4444",
        Role.Accountant => "#44FF44",
        _ => "#AAAAAA"
    };

    public FrontendUserDTO() { }

    public FrontendUserDTO(UserApiDTO user)
    {
        Id = user.Id;
        Role = user.Role;
        LoyalCustomerId = user.LoyalCustomerId;
        AccountId = user.AccountId;
        CustomerId = user.CustomerId;
        Username = user.Username;
        Password = user.Password;
        TwoFactorEnabled = user.TwoFactorEnabled;
        TwoFactorRecoveryCodes = user.TwoFactorRecoveryCodes;
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

    public UserApiDTO ToApiDTO()
    {
        return new UserApiDTO
        {
            Id = this.Id,
            Role = this.Role,
            LoyalCustomerId = this.LoyalCustomerId,
            AccountId = this.AccountId,
            CustomerId = this.CustomerId,
            Username = this.Username,
            Password = this.Password,
            TwoFactorEnabled = this.TwoFactorEnabled,
            TwoFactorRecoveryCodes = this.TwoFactorRecoveryCodes
        };
    }

    public string? GetCustFullName()
    {
        if (CustomerId == null) return null;
        var customer = CustomerRepo.GetCustomerById(CustomerId.Value);
        return customer != null ? customer.FullName : null;
    }
}
