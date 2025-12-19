# Guide d'Authentification ASP.NET Core Identity

## Vue d'ensemble

Ce projet utilise **ASP.NET Core Identity** pour gérer l'authentification et l'autorisation avec trois types d'utilisateurs :

- **Clients** : Accès à l'espace client pour passer des commandes et consulter leur historique
- **Employés** : Accès à l'espace employé avec 3 rôles différents :
  - **Administrateur** : Accès complet à toutes les fonctionnalités
  - **Responsable d'Achat** : Gestion des achats et fournisseurs
  - **Magasinier** : Gestion des stocks et inventaires
- **Visiteurs** : Accès public sans authentification au catalogue et informations générales

## Structure des Fichiers

### Modèles
- `Models/ApplicationUser.cs` : Utilisateur étendu avec support Identity
- `Models/ApplicationDbContext.cs` : Contexte de base de données pour Identity
- `ViewModels/LoginViewModel.cs` : Modèle pour la connexion
- `ViewModels/RegisterViewModel.cs` : Modèle pour l'inscription

### Contrôleurs
- `Controllers/AccountController.cs` : Gestion de l'authentification (login, register, logout)
- `Controllers/ClientController.cs` : Espace client (requiert le rôle Client)
- `Controllers/EmployeeController.cs` : Espace employé (requiert un rôle employé)

### Services
- `Services/DatabaseSeeder.cs` : Initialisation des rôles et compte admin par défaut

### Constantes
- `Constants/RoleConstants.cs` : Définition des rôles et politiques d'autorisation

## Configuration

### 1. Base de données

La chaîne de connexion est définie dans `appsettings.json` :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=ZED;Database=dotnet_project;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

### 2. Migration de la base de données

Pour créer les tables Identity dans votre base de données, exécutez les commandes suivantes dans la Console du Gestionnaire de package ou le terminal :

```bash
# Créer une migration
Add-Migration InitialIdentity -Context ApplicationDbContext

# Appliquer la migration
Update-Database -Context ApplicationDbContext
```

## Compte Administrateur par Défaut

Un compte administrateur est automatiquement créé au démarrage de l'application :

- **Email** : `admin@magasin.com`
- **Mot de passe** : `Admin123!`

?? **IMPORTANT** : Changez ce mot de passe dès la première connexion !

## Rôles et Autorisations

### Rôles disponibles

| Rôle | Description | Accès |
|------|-------------|-------|
| **Client** | Utilisateur client standard | Espace client, commandes, profil |
| **Administrateur** | Administrateur système | Accès complet à toutes les fonctionnalités |
| **ResponsableAchat** | Responsable des achats | Gestion des achats et fournisseurs |
| **Magasinier** | Gestionnaire de stock | Gestion des stocks et inventaires |

### Politiques d'autorisation

Les politiques suivantes sont définies dans `Program.cs` :

- `ClientPolicy` : Requiert le rôle Client
- `EmployePolicy` : Requiert un rôle employé (Admin, ResponsableAchat, ou Magasinier)
- `AdminPolicy` : Requiert le rôle Administrateur
- `ResponsableAchatPolicy` : Requiert le rôle ResponsableAchat
- `MagasinierPolicy` : Requiert le rôle Magasinier

### Utilisation dans les contrôleurs

```csharp
// Requiert un rôle spécifique
[Authorize(Roles = RoleConstants.Administrateur)]
public IActionResult AdminOnly() { }

// Requiert une politique
[Authorize(Policy = RoleConstants.ClientPolicy)]
public IActionResult ClientOnly() { }

// Requiert plusieurs rôles (OR)
[Authorize(Roles = $"{RoleConstants.Administrateur},{RoleConstants.Magasinier}")]
public IActionResult AdminOrMagasinier() { }
```

## Flux d'Authentification

### 1. Inscription (Nouveau Client)

1. L'utilisateur accède à `/Account/Register`
2. Il remplit le formulaire avec ses informations
3. Un compte est créé avec le rôle **Client** par défaut
4. L'utilisateur est automatiquement connecté
5. Redirection vers la page d'accueil

### 2. Connexion

1. L'utilisateur accède à `/Account/Login`
2. Il saisit son email et mot de passe
3. Identity vérifie les credentials
4. Si réussi, création d'un cookie d'authentification
5. Redirection vers la page demandée ou l'accueil

### 3. Déconnexion

1. L'utilisateur clique sur "Déconnexion"
2. Soumission d'un formulaire POST vers `/Account/Logout`
3. Suppression du cookie d'authentification
4. Redirection vers la page d'accueil

## Sécurité

### Configuration des mots de passe

Les mots de passe doivent respecter les critères suivants (configurés dans `Program.cs`) :

- Au moins 6 caractères
- Au moins une majuscule
- Au moins une minuscule
- Au moins un chiffre
- Caractères spéciaux optionnels

### Verrouillage de compte

Protection contre les attaques par force brute :

- Maximum 5 tentatives échouées
- Verrouillage de 5 minutes après dépassement
- Applicable aux nouveaux utilisateurs

### Cookies

- HttpOnly : Oui (protection XSS)
- Secure : Oui (HTTPS uniquement)
- SameSite : Lax (protection CSRF)
- Durée : 24 heures avec renouvellement automatique

## Navigation

### Menu de navigation dynamique

Le menu dans `_Layout.cshtml` s'adapte selon l'état de connexion et les rôles :

**Non connecté :**
- Connexion
- Inscription

**Connecté en tant que Client :**
- Espace Client
- Déconnexion

**Connecté en tant qu'Employé :**
- Espace Employé
- Déconnexion

## Espaces Utilisateur

### Espace Client (`/Client`)

Accessible uniquement aux utilisateurs avec le rôle **Client**.

Fonctionnalités :
- Consultation des commandes
- Gestion du profil
- Navigation du catalogue

### Espace Employé (`/Employee`)

Accessible aux utilisateurs avec un rôle employé.

Fonctionnalités selon le rôle :
- **Administrateur** : Accès à tout (stocks, achats, administration)
- **ResponsableAchat** : Gestion des achats et fournisseurs
- **Magasinier** : Gestion des stocks et inventaires

## Création d'Employés

Les employés doivent être créés par un administrateur via l'interface d'administration. Contrairement aux clients qui peuvent s'inscrire eux-mêmes, les comptes employés sont créés manuellement et assignés à un rôle spécifique.

## Commandes Utiles

### Migration de base de données

```bash
# Créer une nouvelle migration
Add-Migration <NomMigration> -Context ApplicationDbContext

# Appliquer les migrations
Update-Database -Context ApplicationDbContext

# Supprimer la dernière migration
Remove-Migration -Context ApplicationDbContext

# Lister les migrations
Get-Migration -Context ApplicationDbContext
```

### Gestion des utilisateurs (via code)

```csharp
// Créer un utilisateur
var user = new ApplicationUser { /* ... */ };
await _userManager.CreateAsync(user, password);

// Assigner un rôle
await _userManager.AddToRoleAsync(user, RoleConstants.Client);

// Vérifier un rôle
bool isAdmin = await _userManager.IsInRoleAsync(user, RoleConstants.Administrateur);

// Obtenir les rôles d'un utilisateur
var roles = await _userManager.GetRolesAsync(user);
```

## Prochaines Étapes

1. ? Créer la migration Identity
2. ? Appliquer la migration à la base de données
3. ? Tester la connexion avec le compte admin
4. ? Créer l'interface d'administration pour gérer les utilisateurs
5. ? Lier les comptes aux tables Client et Employe existantes
6. ? Implémenter la récupération de mot de passe
7. ? Ajouter la confirmation d'email (optionnel)
8. ? Implémenter l'authentification à deux facteurs (optionnel)

## Dépannage

### Problème : Les migrations ne se créent pas

**Solution** : Assurez-vous que le package `Microsoft.EntityFrameworkCore.Tools` est installé.

### Problème : Erreur de connexion à la base de données

**Solution** : Vérifiez la chaîne de connexion dans `appsettings.json`.

### Problème : L'utilisateur ne peut pas accéder à une page

**Solution** : Vérifiez que l'utilisateur a le bon rôle assigné.

### Problème : Le compte est verrouillé

**Solution** : Attendez 5 minutes ou déverrouillez via code :

```csharp
await _userManager.SetLockoutEndDateAsync(user, null);
```

## Support

Pour toute question ou problème, consultez la documentation officielle :
- [ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity)
- [Authorization in ASP.NET Core](https://docs.microsoft.com/aspnet/core/security/authorization/introduction)
