# Configuration de l'authentification Identity - Solution_Magasin

## Vue d'ensemble

Ce projet utilise ASP.NET Core Identity pour gérer l'authentification et l'autorisation avec trois types d'utilisateurs :
- **Clients** : Accès à l'espace client
- **Employés** : Accès à l'espace employé avec 3 rôles (Administrateur, ResponsableAchat, Magasinier)
- **Visiteurs** : Accès public sans authentification

## Configuration

### 1. Configuration d'Identity (Program.cs)

Identity est configuré avec :
- Clé primaire de type `int` pour AspNetUser et AspNetRole
- Validation de mot de passe (6 caractères minimum, majuscules, minuscules, chiffres)
- Verrouillage automatique après 5 tentatives échouées
- Email obligatoire et unique

### 2. Politiques d'autorisation

Les politiques suivantes sont définies :

| Politique | Description | Rôles autorisés |
|-----------|-------------|-----------------|
| `ClientOnly` | Accès espace client | Client |
| `EmployeOnly` | Accès espace employé | Administrateur, ResponsableAchat, Magasinier |
| `AdminOnly` | Accès administrateur uniquement | Administrateur |
| `ResponsableAchatOnly` | Gestion des achats | Administrateur, ResponsableAchat |
| `MagasinierOnly` | Gestion du stock | Administrateur, Magasinier |

### 3. Rôles disponibles

- **Client** : Client du magasin
- **Administrateur** : Accès complet à toutes les fonctionnalités
- **ResponsableAchat** : Gestion des achats et des fournisseurs
- **Magasinier** : Gestion du stock

## Comptes de test

En mode développement, les comptes suivants sont créés automatiquement :

| Email | Mot de passe | Rôle |
|-------|--------------|------|
| admin@magasin.com | Admin@123 | Administrateur |
| client@test.com | Client@123 | Client |
| achat@magasin.com | Achat@123 | ResponsableAchat |
| magasin@magasin.com | Magasin@123 | Magasinier |

## Structure des contrôleurs

### AccountController
Gestion de l'authentification (accessible à tous) :
- `/Account/Login` - Connexion
- `/Account/Register` - Inscription (crée automatiquement un compte Client)
- `/Account/Logout` - Déconnexion
- `/Account/AccessDenied` - Page d'accès refusé
- `/Account/Lockout` - Page de compte verrouillé

### ClientController
Espace client (`[Authorize(Policy = "ClientOnly")]`) :
- `/Client/Index` - Tableau de bord client
- `/Client/MyOrders` - Mes commandes
- `/Client/Profile` - Mon profil
- `/Client/Reviews` - Mes avis

### EmployeeController
Espace employé avec autorisation par rôle :

**Accessible à tous les employés :**
- `/Employee/Index` - Tableau de bord
- `/Employee/Sales` - Gestion des ventes
- `/Employee/Presence` - Pointage

**Accessible aux Admins et Responsables Achat :**
- `/Employee/Purchases` - Gestion des achats

**Accessible aux Admins et Magasiniers :**
- `/Employee/Stock` - Gestion du stock

**Accessible aux Admins uniquement :**
- `/Employee/Users` - Gestion des utilisateurs
- `/Employee/Reports` - Rapports et statistiques

## Utilisation dans les vues

### Vérifier si l'utilisateur est connecté
```cshtml
@if (User.Identity?.IsAuthenticated == true)
{
    <p>Utilisateur connecté : @User.Identity.Name</p>
}
```

### Vérifier le rôle de l'utilisateur
```cshtml
@if (User.IsInRole("Client"))
{
    <a asp-controller="Client" asp-action="Index">Mon espace</a>
}

@if (User.IsInRole("Administrateur"))
{
    <a asp-controller="Employee" asp-action="Users">Gérer les utilisateurs</a>
}
```

### Avec injection de UserManager
```cshtml
@using Microsoft.AspNetCore.Identity
@using Solution_Magasin.Models
@inject UserManager<AspNetUser> UserManager

@{
    var user = await UserManager.GetUserAsync(User);
    var roles = await UserManager.GetRolesAsync(user);
}

<p>Rôle(s) : @string.Join(", ", roles)</p>
```

## Utilisation dans les contrôleurs

### Attributs d'autorisation

**Sur toute la classe :**
```csharp
[Authorize(Policy = RoleConstants.ClientPolicy)]
public class ClientController : Controller
{
    // Toutes les actions nécessitent le rôle Client
}
```

**Sur une action spécifique :**
```csharp
[Authorize(Policy = RoleConstants.AdminPolicy)]
public IActionResult AdminOnlyAction()
{
    // Uniquement pour les administrateurs
}
```

**Autoriser l'accès anonyme :**
```csharp
[AllowAnonymous]
public IActionResult PublicAction()
{
    // Accessible sans authentification
}
```

### Vérifier les rôles dans le code

```csharp
public async Task<IActionResult> MyAction()
{
    var user = await _userManager.GetUserAsync(User);
    var roles = await _userManager.GetRolesAsync(user);
    
    if (roles.Contains(RoleConstants.Administrateur))
    {
        // Code spécifique aux admins
    }
    
    return View();
}
```

## Création manuelle d'utilisateurs

### Créer un client
```csharp
var client = new AspNetUser
{
    UserName = "client@example.com",
    Email = "client@example.com",
    Prenom = "Jean",
    Nom = "Dupont",
    Cin = "AB123456",
    PhoneNumber = "0612345678",
    EmailConfirmed = true
};

var result = await _userManager.CreateAsync(client, "MotDePasse@123");
if (result.Succeeded)
{
    await _userManager.AddToRoleAsync(client, RoleConstants.Client);
}
```

### Créer un employé
```csharp
var employe = new AspNetUser
{
    UserName = "employe@magasin.com",
    Email = "employe@magasin.com",
    Prenom = "Marie",
    Nom = "Martin",
    Cin = "CD789012",
    PhoneNumber = "0698765432",
    EmailConfirmed = true
};

var result = await _userManager.CreateAsync(employe, "MotDePasse@123");
if (result.Succeeded)
{
    // Assigner un ou plusieurs rôles
    await _userManager.AddToRoleAsync(employe, RoleConstants.Magasinier);
}
```

## Fichiers importants

### Classes de configuration
- `Constants/RoleConstants.cs` - Constantes pour les rôles et politiques
- `Services/DbInitializer.cs` - Initialisation des rôles et utilisateurs de test

### ViewModels
- `ViewModels/LoginViewModel.cs` - Modèle pour la connexion
- `ViewModels/RegisterViewModel.cs` - Modèle pour l'inscription

### Contrôleurs
- `Controllers/AccountController.cs` - Gestion de l'authentification
- `Controllers/ClientController.cs` - Espace client
- `Controllers/EmployeeController.cs` - Espace employé

### Vues
- `Views/Account/Login.cshtml` - Page de connexion
- `Views/Account/Register.cshtml` - Page d'inscription
- `Views/Account/AccessDenied.cshtml` - Accès refusé
- `Views/Account/Lockout.cshtml` - Compte verrouillé
- `Views/Client/Index.cshtml` - Tableau de bord client
- `Views/Employee/Index.cshtml` - Tableau de bord employé

## Notes importantes

1. **Clé primaire int** : Ce projet utilise `int` comme clé primaire pour AspNetUser au lieu de `string` (défaut d'Identity). Identity est configuré pour supporter cette personnalisation.

2. **Initialisation automatique** : Les rôles et le compte administrateur sont créés automatiquement au démarrage de l'application.

3. **Utilisateurs de test** : Activés uniquement en mode développement. Pour la production, désactivez cette fonctionnalité.

4. **Connexion string** : La connexion à la base de données doit être configurée dans `appsettings.json` sous `ConnectionStrings:DefaultConnection`.

5. **Migration de la base de données** : Assurez-vous que les tables Identity existent dans votre base de données avant de lancer l'application.

## Prochaines étapes recommandées

1. **Gestion du profil utilisateur** : Ajouter des pages pour modifier le profil (mot de passe, informations personnelles)
2. **Réinitialisation de mot de passe** : Implémenter la fonctionnalité "mot de passe oublié"
3. **Confirmation par email** : Activer la confirmation d'email lors de l'inscription
4. **Double authentification** : Ajouter 2FA pour plus de sécurité
5. **Logs d'activité** : Tracer les actions importantes des utilisateurs
6. **Gestion avancée des employés** : Interface admin pour créer/modifier/supprimer les employés et leurs rôles

## Support

Pour plus d'informations sur ASP.NET Core Identity :
- [Documentation officielle](https://docs.microsoft.com/aspnet/core/security/authentication/identity)
- [Autorisation basée sur les rôles](https://docs.microsoft.com/aspnet/core/security/authorization/roles)
- [Autorisation basée sur les politiques](https://docs.microsoft.com/aspnet/core/security/authorization/policies)
