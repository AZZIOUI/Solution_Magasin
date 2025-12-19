# ? Administration Complète - Guide de l'Administrateur

## ?? Vue d'ensemble

Un compte **Administrateur** avec le rôle "Administrateur" est automatiquement créé au démarrage de l'application. Cet administrateur peut créer d'autres employés avec différents rôles.

---

## ?? Compte Administrateur par Défaut

### Informations de connexion
```
?? Email: admin@magasin.com
?? Mot de passe: Admin123!
```

?? **IMPORTANT** : Changez ce mot de passe après la première connexion !

### Ce qui est créé automatiquement

Au premier démarrage de l'application, le système crée :

1. **Tous les rôles** dans `AspNetRoles` :
   - Client
   - Administrateur
   - ResponsableAchat
   - Magasinier

2. **Un enregistrement Employe** dans la table `Employe` :
   - CIN: "ADMIN000"
   - Prénom: "Admin"
   - Nom: "Système"
   - Date d'embauche: Date du jour

3. **Un compte Identity** dans `AspNetUsers` :
   - Email: admin@magasin.com
   - Rôle: Administrateur
   - Lié à l'enregistrement Employe via `EmployeId`
   - Actif par défaut

---

## ?? Espace Administration

### Accès

1. **Connexion** : `/Account/Login`
   - Email: `admin@magasin.com`
   - Mot de passe: `Admin123!`

2. **Navigation** :
   - Depuis l'espace employé ? Cliquer sur "Administration"
   - OU directement : `/Admin/Index`

### Tableau de Bord (`/Admin/Index`)

Le tableau de bord affiche :

#### ?? Statistiques Rapides
- **Utilisateurs totaux** : Nombre total de comptes
- **Clients** : Nombre de clients enregistrés
- **Employés** : Nombre d'employés actifs

#### ?? Actions Rapides
- **Créer un Employé** : Formulaire de création
- **Gérer les Utilisateurs** : Liste complète des utilisateurs

---

## ?? Créer un Employé

### Accès
`/Admin/CreateEmployee`

### Formulaire

| Champ | Type | Obligatoire | Description |
|-------|------|-------------|-------------|
| **CIN** | Texte | ? Oui | Carte d'identité nationale |
| **Rôle** | Liste déroulante | ? Oui | Administrateur, ResponsableAchat, ou Magasinier |
| **Prénom** | Texte | ? Oui | Prénom de l'employé |
| **Nom** | Texte | ? Oui | Nom de l'employé |
| **Email** | Email | ? Oui | Adresse email professionnelle |
| **Mot de passe** | Mot de passe | ? Oui | Minimum 6 caractères |
| **Confirmation** | Mot de passe | ? Oui | Doit correspondre au mot de passe |

### Processus de Création

```
1. Administrateur remplit le formulaire
        ?
2. Création d'un enregistrement Employe
   - Stocke CIN, Prénom, Nom
   - Date d'embauche = Date actuelle
        ?
3. Création du compte Identity (AspNetUsers)
   - Email comme nom d'utilisateur
   - Hash du mot de passe
   - Lien vers Employe via EmployeId
   - UserType = "Employe"
        ?
4. Attribution du rôle sélectionné
   - Administrateur
   - ResponsableAchat
   - Magasinier
        ?
5. Confirmation et redirection
```

### Exemple de Création

**Formulaire rempli** :
- CIN: AB123456
- Rôle: Magasinier
- Prénom: Mohamed
- Nom: Alami
- Email: m.alami@magasin.com
- Mot de passe: Magasin123!

**Résultat** :
```sql
-- Table Employe
INSERT INTO Employe (CIN, prenom_emp, nom_emp, dateEmbauche)
VALUES ('AB123456', 'Mohamed', 'Alami', '2025-01-19');

-- Table AspNetUsers (simplifié)
INSERT INTO AspNetUsers (Email, UserName, EmployeId, UserType, ...)
VALUES ('m.alami@magasin.com', 'm.alami@magasin.com', 5, 'Employe', ...);

-- Table AspNetUserRoles
INSERT INTO AspNetUserRoles (UserId, RoleId)
VALUES ([UserId], [MagasinierRoleId]);
```

---

## ?? Gestion des Utilisateurs

### Accès
`/Admin/Users`

### Fonctionnalités

#### ?? Filtres et Recherche
- **Recherche** : Par email ou nom
- **Type d'utilisateur** : Tous, Clients, Employés
- **Statut** : Tous, Actifs, Inactifs

#### ?? Tableau des Utilisateurs

Pour chaque utilisateur, affiche :
- ?? **Email**
- ?? **Nom complet** (Prénom + Nom)
- ??? **Type** : Client ou Employé
- ?? **Rôle(s)** : Badges colorés
  - ?? Administrateur (rouge)
  - ?? ResponsableAchat (jaune)
  - ?? Magasinier (bleu)
  - ?? Client (vert)
- ?? **Statut** : Actif / Inactif
- ?? **Actions** : 
  - ??? Voir détails
  - ?? Modifier
  - ?? Activer/Désactiver

#### ?? Statistiques
- Affiche le nombre total d'utilisateurs

---

## ?? Rôles et Permissions

### Rôle: Administrateur

**Accès complet à** :
- ? Toutes les fonctionnalités de l'espace employé
- ? Gestion des stocks
- ? Gestion des achats
- ? **Administration** :
  - Créer des employés
  - Gérer tous les utilisateurs
  - Voir les statistiques système

### Rôle: ResponsableAchat

**Accès à** :
- ? Gestion des achats et fournisseurs
- ? Tableaux de bord employé
- ? Administration
- ? Gestion des stocks (sauf lecture)

### Rôle: Magasinier

**Accès à** :
- ? Gestion des stocks et inventaires
- ? Réception des marchandises
- ? Tableaux de bord employé
- ? Administration
- ? Gestion des achats

---

## ?? Sécurité

### Contrôle d'Accès

**L'espace administration** est protégé par :
```csharp
[Authorize(Policy = RoleConstants.AdminPolicy)]
```

Cela signifie :
- ? Seuls les utilisateurs avec le rôle "Administrateur" peuvent accéder
- ? Les autres employés et clients voient une page "Accès refusé"

### Bonnes Pratiques

1. **Mots de passe** :
   - Minimum 6 caractères
   - Majuscule + minuscule + chiffre
   - Changez le mot de passe admin par défaut

2. **Gestion des comptes** :
   - Ne créez des administrateurs que si nécessaire
   - Utilisez "ResponsableAchat" ou "Magasinier" pour les employés standards
   - Désactivez les comptes au lieu de les supprimer

3. **Audit** :
   - Les actions sont loguées dans les logs de l'application
   - Surveillez les créations de comptes administrateurs

---

## ?? Tests

### Test 1: Connexion Admin

```
1. Aller sur /Account/Login
2. Email: admin@magasin.com
3. Mot de passe: Admin123!
4. Cliquer "Se connecter"
5. ? Redirection vers l'accueil
6. ? Menu "Espace Employé" visible
```

### Test 2: Accès Administration

```
1. Connecté en tant qu'admin
2. Aller sur /Employee/Index
3. Cliquer sur "Administration"
4. ? Affichage du tableau de bord admin
5. ? Statistiques visibles
```

### Test 3: Créer un Employé

```
1. Connecté en tant qu'admin
2. Aller sur /Admin/CreateEmployee
3. Remplir le formulaire :
   - CIN: AB123456
   - Rôle: Magasinier
   - Prénom: Test
   - Nom: Employé
   - Email: test.employe@magasin.com
   - Mot de passe: Test123!
   - Confirmation: Test123!
4. Cliquer "Créer l'employé"
5. ? Message de succès
6. ? Redirection vers /Admin/Index
```

### Test 4: Vérification en Base

```sql
-- Vérifier l'employé créé
SELECT * FROM Employe WHERE CIN = 'AB123456';

-- Vérifier le compte Identity
SELECT * FROM AspNetUsers WHERE Email = 'test.employe@magasin.com';

-- Vérifier le rôle assigné
SELECT u.Email, r.Name
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = 'test.employe@magasin.com';
```

### Test 5: Connexion avec le Nouvel Employé

```
1. Se déconnecter
2. Aller sur /Account/Login
3. Email: test.employe@magasin.com
4. Mot de passe: Test123!
5. ? Connexion réussie
6. ? Accès à l'espace employé
7. ? Rôle Magasinier visible
8. ? Pas d'accès à l'administration
```

---

## ?? Captures d'Écran des Pages

### Tableau de Bord Admin
```
???????????????????????????????????????????
?  ?? Administration                      ?
???????????????????????????????????????????
?  ????????????????  ????????????????   ?
?  ? Créer un     ?  ? Gérer les    ?   ?
?  ? Employé      ?  ? Utilisateurs ?   ?
?  ????????????????  ????????????????   ?
?                                         ?
?  ?? Statistiques Rapides               ?
?  ???????????????????????????????      ?
?  ?   50    ?   40    ?   10    ?      ?
?  ? Total   ? Clients ? Employés?      ?
?  ???????????????????????????????      ?
???????????????????????????????????????????
```

---

## ?? Démarrage Rapide

### Première Utilisation

1. **Démarrer l'application**
   ```bash
   dotnet run
   ```

2. **Les rôles et l'admin sont créés automatiquement** ?

3. **Se connecter en tant qu'admin**
   - Email: `admin@magasin.com`
   - Mot de passe: `Admin123!`

4. **Créer vos employés**
   - Aller sur `/Admin/CreateEmployee`
   - Créer les comptes nécessaires

5. **Changer le mot de passe admin** ??

---

## ?? Ressources

- **Documentation complète** : `IDENTITY_SETUP.md`
- **Connexion Client/Employe** : `IDENTITY_CONNECTION.md`
- **Inscription client** : `REGISTRATION_COMPLETE.md`

---

**?? Votre système d'administration est complet et fonctionnel !**

L'administrateur peut maintenant :
? Se connecter avec le compte par défaut  
? Accéder à l'espace administration  
? Créer des employés avec différents rôles  
? Gérer tous les utilisateurs du système  
? Consulter les statistiques  
