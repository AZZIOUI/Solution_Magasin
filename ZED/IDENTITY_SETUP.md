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
- `ViewModels/RegisterViewModel.cs` : Modèle pour l'inscription (avec adresse et téléphone)
- `ViewModels/ClientProfileViewModel.cs` : Modèle pour le profil client

### Contrôleurs
- `Controllers/AccountController.cs` : Gestion de l'authentification (login, register, logout)
- `Controllers/ClientController.cs` : Espace client (requiert le rôle Client)
- `Controllers/EmployeeController.cs` : Espace employé (requiert un rôle employé)
- `Controllers/AdminController.cs` : Administration (création employés, gestion utilisateurs)

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

Pour créer les tables Identity dans votre base de données, exécutez les commandes suivantes :

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

## Flux d'Authentification

### 1. Inscription (Nouveau Client)

**Informations collectées** :
- Prénom et Nom
- Email
- Téléphone
- Adresse complète
- Mot de passe

**Processus** :
1. L'utilisateur accède à `/Account/Register`
2. Il remplit le formulaire complet avec toutes ses informations
3. Un enregistrement **Client** est créé dans la table `Client` avec:
   - `PrenomClient`, `NomClient`
   - `MailClient`
   - `TelClient`
   - `AdresseClient`
4. Un compte **ApplicationUser** est créé avec le rôle **Client** et lié au Client via `ClientId`
5. L'utilisateur est automatiquement connecté
6. Redirection vers la page d'accueil

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

## Espaces Utilisateur

### Espace Client (`/Client`)

Accessible uniquement aux utilisateurs avec le rôle **Client**.

**Fonctionnalités** :
- **Dashboard** (`/Client/Index`) : Vue d'ensemble
- **Mes Commandes** (`/Client/MyOrders`) : Historique complet des commandes avec détails
- **Mon Profil** (`/Client/Profile`) : Consultation et modification des informations personnelles
  - Prénom, Nom
  - Email, Téléphone
  - Adresse
  - Date de création du compte

### Espace Employé (`/Employee`)

Accessible aux utilisateurs avec un rôle employé.

**Fonctionnalités selon le rôle** :
- **Administrateur** : Accès à tout (stocks, achats, administration)
- **ResponsableAchat** : Gestion des achats et fournisseurs
- **Magasinier** : Gestion des stocks et inventaires

### Espace Administration (`/Admin`)

Accessible uniquement aux **Administrateurs**.

**Fonctionnalités** :
- **Créer un Employé** (`/Admin/CreateEmployee`) : Création de comptes employés avec attribution de rôle
- **Gérer les Utilisateurs** (`/Admin/Users`) : Liste et gestion de tous les utilisateurs

## Connexion Client ? Identity

### Structure de données

```
Client (Table business)                AspNetUsers (Table Identity)
???????????????????????               ????????????????????????
? IdClient (PK)       ????????????????? ClientId (FK)        ?
? PrenomClient        ?               ? UserName             ?
? NomClient           ?               ? Email                ?
? MailClient          ?               ? PasswordHash         ?
? TelClient           ?               ? FirstName            ?
? AdresseClient       ?               ? LastName             ?
???????????????????????               ? PhoneNumber          ?
                                      ? UserType = "Client"  ?
                                      ? IsActive             ?
                                      ????????????????????????
```

**Lors de l'inscription** :
1. Création du `Client` avec toutes les données
2. Création de l'`ApplicationUser` lié via `ClientId`
3. Les données sont synchronisées entre les deux tables

**Lors de la modification du profil** :
- Les deux tables sont mises à jour simultanément
- Maintien de la cohérence des données

## Sécurité

### Configuration des mots de passe

Les mots de passe doivent respecter les critères suivants :

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

## Prochaines Étapes

1. ? Créer la migration Identity
2. ? Appliquer la migration à la base de données
3. ? Tester l'inscription avec toutes les informations client
4. ? Tester la modification du profil
5. ? Implémenter la récupération de mot de passe
6. ? Ajouter la confirmation d'email (optionnel)
7. ? Implémenter l'authentification à deux facteurs (optionnel)

## Support

Pour toute question ou problème, consultez la documentation officielle :
- [ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity)
- [Authorization in ASP.NET Core](https://docs.microsoft.com/aspnet/core/security/authorization/introduction)
