namespace Solution_Magasin.ViewModels;

/// <summary>
/// Modèle de vue pour afficher la liste des utilisateurs
/// </summary>
public class UserListViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Roles { get; set; } = string.Empty;
}
