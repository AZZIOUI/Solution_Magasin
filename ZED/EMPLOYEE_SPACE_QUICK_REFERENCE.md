# ?? Espace Employé - Guide de Référence Rapide

## Accès Rapide aux Fonctionnalités

### ?? Tableau de Bord
```
URL: /Employee/Index
Rôles: Tous les employés
```
- Vue d'ensemble de vos activités
- Enregistrement présence (arrivée/départ)
- Statistiques personnalisées
- Accès rapide aux sections

### ?? Gestion des Stocks (Magasinier/Admin)

#### Consulter les Stocks
```
URL: /Employee/StockManagement
Actions: Voir, Rechercher, Filtrer
```

#### Mettre à Jour un Stock
```
URL: /Employee/UpdateStock/{id}
Méthode: POST
Champs: Nouvelle quantité, Motif (optionnel)
```

#### Voir les Notifications
```
URL: /Employee/StockNotifications
Filtres: Non lues, Toutes
Action: Marquer comme lu
```

### ?? Gestion des Achats (Resp. Achat/Admin)

#### Consulter les Achats
```
URL: /Employee/PurchaseManagement
Filtres: Date début/fin, Fournisseur
```

#### Créer un Achat
```
URL: /Employee/CreatePurchase
Méthode: POST
Requis: Fournisseur, Date, Au moins 1 article
Effet: Mise à jour automatique du stock
```

#### Voir Détails d'un Achat
```
URL: /Employee/PurchaseDetails/{id}
Affichage: Fournisseur, Articles, Total
```

### ? Gestion des Présences

#### Enregistrer Arrivée
```
URL: /Employee/CheckIn (POST)
Limite: 1 fois par jour
Enregistre: Heure actuelle
```

#### Enregistrer Départ
```
URL: /Employee/CheckOut (POST)
Requis: Arrivée déjà enregistrée
Calcule: Durée automatiquement
```

#### Consulter l'Historique
```
URL: /Employee/Presence
Filtre: Par période
Affiche: Toutes les présences
```

## ?? Actions Principales par Rôle

### Administrateur
? Tout gérer (stocks, achats, présences)
? Accès panneau d'administration
? Superviser les notifications

### Responsable d'Achat
? Créer des achats
? Consulter historique achats
? Gérer sa présence

### Magasinier
? Mettre à jour les stocks
? Traiter les notifications
? Gérer sa présence

## ?? Recherche et Filtres

### Stocks
```
Recherche: Par nom ou référence
Filtres:
  - Tous les articles
  - Stock faible (? min)
  - En rupture (= 0)
```

### Achats
```
Filtres:
  - Date début
  - Date fin
  - Fournisseur spécifique
```

### Présences
```
Filtre:
  - Date de début (défaut: dernier mois)
```

## ?? Indicateurs Visuels

### États du Stock
- ?? **Vert**: Stock normal (> minimum)
- ?? **Jaune**: Stock faible (? minimum)
- ?? **Rouge**: Rupture de stock (= 0)

### Notifications
- ?? **Badge Jaune**: Non lue
- ? **Vert**: Lue

### Présences
- ? **Vert**: Complète (arrivée + départ)
- ?? **Jaune**: En cours (arrivée seulement)
- ? **Gris**: Incomplète

## ? Raccourcis Clavier

### Navigation
```
Alt + E: Espace Employé
Alt + S: Stocks (si autorisé)
Alt + A: Achats (si autorisé)
Alt + P: Présences
```

## ?? Notifications

### Créées Automatiquement Quand
- Stock ? Stock minimum
- Stock = 0
- Mise à jour importante du stock

### Actions Possibles
- Marquer comme lu
- Voir le stock concerné
- Filtrer par statut (lu/non lu)

## ?? Astuces

### Productivité
1. Utilisez les filtres pour trouver rapidement
2. DataTables permet de trier sur toutes les colonnes
3. Recherche en temps réel dans les tableaux
4. Exportez les données (Excel, PDF, Copie)

### Gestion du Temps
1. Enregistrez votre arrivée dès le matin
2. N'oubliez pas le départ en fin de journée
3. Consultez vos heures régulièrement
4. Vérifiez votre historique mensuel

### Gestion des Stocks
1. Traitez les alertes en priorité
2. Ajoutez toujours un motif de modification
3. Vérifiez les notifications quotidiennement
4. Utilisez les filtres pour l'inventaire

### Gestion des Achats
1. Vérifiez le stock avant de commander
2. Comparez les prix des fournisseurs
3. Groupez les achats quand possible
4. Vérifiez le total avant validation

## ?? Messages Courants

### Erreurs Fréquentes

**"Arrivée déjà enregistrée"**
- Cause: Tentative d'enregistrer 2 arrivées le même jour
- Solution: Une seule arrivée par jour autorisée

**"Présence introuvable"**
- Cause: ID de présence invalide
- Solution: Retourner au tableau de bord

**"Veuillez ajouter au moins un article"**
- Cause: Achat sans article
- Solution: Ajouter minimum 1 article à l'achat

**"Aucune fiche employé trouvée"**
- Cause: Pas de correspondance entre compte et employé
- Solution: Contacter l'administrateur

### Messages de Succès

? "Stock mis à jour avec succès"
? "Achat créé avec succès"
? "Arrivée enregistrée avec succès"
? "Départ enregistré avec succès"

## ?? Responsive Design

### Mobile
- Menu hamburger
- Cartes empilées verticalement
- Tableaux défilables horizontalement
- Boutons pleine largeur

### Tablette
- Menu complet
- Grille 2 colonnes
- Tableaux complets
- Espacement optimisé

### Desktop
- Menu complet avec dropdowns
- Grille 3-4 colonnes
- Tableaux complets avec filtres
- Toutes les fonctionnalités

## ?? Sécurité

### Bonnes Pratiques
1. Déconnectez-vous après utilisation
2. Ne partagez pas vos identifiants
3. Signalez les comportements suspects
4. Vérifiez les montants avant validation

### Protection des Données
- Token anti-forgery sur tous les formulaires
- Validation côté serveur
- Autorisation par rôle
- Logs des actions

## ?? Aide Rapide

### Problèmes Courants

**Q: Je ne vois pas le menu Espace Employé**
R: Vérifiez que vous avez le bon rôle (Admin/Resp. Achat/Magasinier)

**Q: Les notifications ne s'affichent pas**
R: Vérifiez votre rôle (Admin/Magasinier uniquement)

**Q: Je ne peux pas créer d'achat**
R: Fonction réservée aux Admin et Resp. Achat

**Q: Le tableau est vide**
R: Vérifiez les filtres appliqués

**Q: L'export ne fonctionne pas**
R: Assurez-vous que JavaScript est activé

### Contact Support
1. Vérifier cette documentation
2. Consulter l'administrateur système
3. Vérifier les logs (Admin uniquement)

---

**Conseil:** Gardez ce guide à portée de main pour une référence rapide ! ??

**Version:** 1.0  
**Dernière mise à jour:** 21/12/2024
