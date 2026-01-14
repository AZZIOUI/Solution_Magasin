# ????? Espace Employé - Documentation Complète

## Vue d'ensemble

L'espace employé est un système complet de gestion pour les employés de l'entreprise (Administrateurs, Responsables d'Achat, et Magasiniers). Il offre des fonctionnalités adaptées à chaque rôle avec une interface intuitive et conviviale.

## ?? Rôles et Permissions

### Administrateur
- ? Accès complet à toutes les fonctionnalités
- ? Gestion des stocks
- ? Gestion des achats
- ? Gestion des présences
- ? Accès au panneau d'administration

### Responsable d'Achat
- ? Gestion des achats (création, consultation)
- ? Gestion des présences personnelles
- ? Pas d'accès à la gestion des stocks

### Magasinier
- ? Gestion des stocks (consultation, mise à jour)
- ? Gestion des notifications de stock
- ? Gestion des présences personnelles
- ? Pas d'accès à la gestion des achats

## ?? Fonctionnalités Principales

### 1. Tableau de Bord Employé

**Accès:** Tous les employés  
**Route:** `/Employee/Index`

**Caractéristiques:**
- Informations personnelles de l'employé
- Enregistrement de la présence (arrivée/départ)
- Statistiques personnalisées selon le rôle
- Accès rapide aux différentes sections
- Notifications en temps réel

**Statistiques affichées:**
- Présences du mois en cours
- Heures totales travaillées
- Alertes de stock (Magasinier/Admin)
- Articles en rupture (Magasinier/Admin)
- Achats du mois (Responsable Achat/Admin)
- Notifications non lues

### 2. Gestion des Stocks

**Accès:** Administrateur, Magasinier  
**Route:** `/Employee/StockManagement`

**Fonctionnalités:**

#### ?? Consultation des Stocks
- Liste complète de tous les articles avec leur stock
- Recherche par nom ou référence d'article
- Filtres :
  - Tous les articles
  - Stock faible (quantité ? minimum)
  - En rupture (quantité = 0)
- Visualisation du niveau de stock avec barre de progression
- Indicateurs visuels pour les alertes

#### ?? Mise à Jour du Stock
**Route:** `/Employee/UpdateStock/{id}`

- Modification de la quantité disponible
- Affichage de la quantité actuelle
- Calcul automatique de la différence
- Motif de modification optionnel
- Création automatique de notification si stock faible

**Process:**
1. Sélectionner un article depuis la liste
2. Entrer la nouvelle quantité
3. Ajouter un motif (optionnel)
4. Valider

#### ?? Notifications de Stock
**Route:** `/Employee/StockNotifications`

- Affichage de toutes les notifications
- Filtre : Non lues / Toutes
- Marquage des notifications comme lues
- Actualisation automatique toutes les 30 secondes
- Indicateurs visuels pour les urgences

**Types de notifications:**
- Stock faible (quantité ? stock minimum)
- Rupture de stock (quantité = 0)
- Modifications importantes

### 3. Gestion des Achats

**Accès:** Administrateur, Responsable d'Achat  
**Route:** `/Employee/PurchaseManagement`

#### ?? Liste des Achats

**Filtres disponibles:**
- Date de début
- Date de fin
- Fournisseur spécifique

**Informations affichées:**
- ID de l'achat
- Date et heure
- Fournisseur
- Contact du fournisseur
- Nombre d'articles
- Montant total

**Statistiques:**
- Total des achats
- Nombre total d'articles achetés
- Montant total dépensé

#### ?? Création d'un Achat
**Route:** `/Employee/CreatePurchase`

**Process:**
1. Sélectionner un fournisseur
2. Définir la date d'achat
3. Ajouter des articles :
   - Sélectionner l'article
   - Indiquer la quantité
   - Saisir le prix unitaire
   - Le montant se calcule automatiquement
4. Possibilité d'ajouter plusieurs articles
5. Calcul automatique du total général
6. Validation

**Effets de la création:**
- Création de l'achat dans la base de données
- Ajout des détails pour chaque article
- **Mise à jour automatique du stock** (quantité += quantité achetée)
- Mise à jour de la date de modification du stock

#### ??? Détails d'un Achat
**Route:** `/Employee/PurchaseDetails/{id}`

- Informations complètes sur le fournisseur
- Date et heure de l'achat
- Liste détaillée des articles :
  - Nom et référence
  - Quantité
  - Prix unitaire
  - Montant par article
- Montant total de l'achat

### 4. Gestion des Présences

**Accès:** Tous les employés  
**Route:** `/Employee/Presence`

#### ? Enregistrement de la Présence

**Arrivée:**
- Bouton "Enregistrer Arrivée" sur le tableau de bord
- Route: `/Employee/CheckIn` (POST)
- Enregistre automatiquement l'heure actuelle
- Une seule arrivée par jour

**Départ:**
- Bouton "Enregistrer Départ" visible après l'arrivée
- Route: `/Employee/CheckOut` (POST)
- Enregistre l'heure de sortie
- Calcul automatique de la durée

#### ?? Historique des Présences

**Affichage:**
- Liste complète des présences
- Filtre par période (défaut: dernier mois)
- Tableau avec colonnes :
  - Date
  - Jour de la semaine
  - Heure d'arrivée
  - Heure de départ
  - Durée de travail
  - Statut (Complète/En cours/Incomplète)

**Statistiques:**
- Total des présences
- Présences complètes
- Présences en cours
- Total d'heures travaillées

**États possibles:**
- ? **Complète**: Arrivée et départ enregistrés
- ?? **En cours**: Arrivée enregistrée, pas encore de départ
- ? **Incomplète**: Manque d'informations

## ?? Interface Utilisateur

### Design et Ergonomie

**Principes:**
- Interface moderne avec Bootstrap 5
- Icônes Bootstrap Icons pour une meilleure lisibilité
- Code couleur cohérent :
  - ?? Vert: Actions positives, stock OK
  - ?? Jaune: Avertissements, stock faible
  - ?? Rouge: Alertes, rupture de stock
  - ?? Bleu: Informations, liens
- Responsive design (mobile, tablette, desktop)

**Composants:**
- Cards pour les sections principales
- DataTables pour les listes (pagination, tri, recherche)
- Formulaires avec validation côté client et serveur
- Alertes temporaires (auto-dismiss après 5 secondes)
- Badges et indicateurs visuels

### Navigation

**Menu Principal:**
- Dropdown "Espace Employé" avec accès rapide
- Sous-menus organisés par fonctionnalité
- Fil d'Ariane (breadcrumb) sur chaque page

**Structure:**
```
Espace Employé
??? Tableau de Bord
??? Gestion des Stocks (Magasinier/Admin)
?   ??? Gérer les Stocks
?   ??? Notifications
??? Gestion des Achats (Resp. Achat/Admin)
?   ??? Gérer les Achats
?   ??? Nouvel Achat
??? Mes Présences
```

## ?? Sécurité

### Authentification et Autorisation

**Politique d'accès:**
```csharp
[Authorize(Policy = RoleConstants.EmployePolicy)]
```

**Vérifications:**
- Authentification obligatoire
- Contrôle des rôles au niveau du contrôleur
- Contrôle des rôles au niveau des actions
- Vérification dans les vues (affichage conditionnel)

**Protection:**
- Token Anti-Forgery sur tous les formulaires POST
- Validation des données côté serveur
- Messages d'erreur explicites
- Logging des actions importantes

## ?? Fonctionnalités Avancées

### 1. Calculs Automatiques

- **Niveau de stock**: Pourcentage basé sur stock max
- **Montant achat**: Quantité × Prix unitaire
- **Total achat**: Somme des montants
- **Durée présence**: Différence entre arrivée et départ
- **Heures totales**: Somme des durées

### 2. Notifications en Temps Réel

- **Auto-refresh**: Toutes les 30 secondes si notifications non lues
- **Badges**: Nombre de notifications non lues
- **Indicateurs visuels**: Couleurs d'alerte
- **Action rapide**: Marquer comme lu depuis la liste

### 3. Recherche et Filtrage

**Stock:**
- Recherche textuelle (nom, référence)
- Filtres prédéfinis (faible, rupture)
- Tri sur toutes les colonnes

**Achats:**
- Filtre par période (date début/fin)
- Filtre par fournisseur
- Tri chronologique

**Présences:**
- Filtre par période
- Tri par date décroissante

### 4. Export et Impression

Les DataTables permettent:
- Copie dans le presse-papier
- Export Excel
- Export PDF
- Impression

## ?? Maintenance et Support

### Logs

Le système enregistre:
- Accès aux différentes sections
- Erreurs et exceptions
- Actions importantes (création achat, mise à jour stock)

### Messages Utilisateur

**Types:**
- ? **Success**: Action réussie (vert)
- ?? **Warning**: Avertissement (jaune)
- ? **Error**: Erreur (rouge)
- ?? **Info**: Information (bleu)

**Durée:**
- Auto-dismiss après 5 secondes
- Fermeture manuelle possible

## ?? Statistiques et Rapports

### Tableau de Bord

**Pour tous:**
- Présences du mois
- Heures totales

**Pour Magasinier/Admin:**
- Alertes de stock
- Articles en rupture
- Notifications non lues

**Pour Resp. Achat/Admin:**
- Achats du mois
- Montant total dépensé

## ?? Bonnes Pratiques

### Pour les Magasiniers

1. **Vérifier les notifications quotidiennement**
2. **Mettre à jour les stocks après inventaire**
3. **Ajouter un motif lors des modifications**
4. **Traiter les alertes rapidement**

### Pour les Responsables d'Achat

1. **Vérifier les stocks faibles avant de commander**
2. **Renseigner les prix unitaires correctement**
3. **Vérifier les totaux avant validation**
4. **Consulter l'historique des achats fournisseur**

### Pour les Administrateurs

1. **Superviser les notifications importantes**
2. **Vérifier les statistiques régulièrement**
3. **Former les nouveaux employés**
4. **Contrôler les présences**

## ?? Support

En cas de problème:
1. Vérifier les messages d'erreur affichés
2. Consulter cette documentation
3. Contacter l'administrateur système
4. Vérifier les logs pour plus de détails

---

**Version:** 1.0  
**Dernière mise à jour:** 21/12/2024  
**Auteur:** Solution_Magasin Team
