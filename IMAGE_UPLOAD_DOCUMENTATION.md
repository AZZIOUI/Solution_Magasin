# ?? FONCTIONNALITÉ IMAGES PRODUITS - DOCUMENTATION COMPLÈTE

## ?? Vue d'ensemble

Le système de gestion d'images pour les produits est maintenant complètement implémenté. Il permet le téléchargement, l'affichage, la modification et la suppression d'images pour chaque article, avec stockage local et enregistrement des chemins en base de données.

## ? Fonctionnalités Implémentées

### 1. ??? Modification de la Base de Données

#### **Modèle Article mis à jour**
```csharp
[Column("image_path")]
[StringLength(500)]
[Unicode(false)]
public string? ImagePath { get; set; }
```

#### **Migration créée**
- Fichier: `Migrations/20251220000000_AddArticleImagePath.cs`
- Ajoute la colonne `image_path` de type `varchar(500)` nullable

**Pour appliquer la migration:**
```bash
dotnet ef database update
```

### 2. ?? Service de Téléchargement d'Images

#### **ImageUploadService**
Fichier: `Services/ImageUploadService.cs`

**Caractéristiques:**
- ? Upload d'images avec nom unique (GUID)
- ? Validation de format (JPG, JPEG, PNG, GIF, WEBP)
- ? Validation de taille (Max 5 MB)
- ? Validation de type MIME
- ? Suppression d'images
- ? Gestion des erreurs avec logging

**Formats acceptés:**
- `.jpg` / `.jpeg`
- `.png`
- `.gif`
- `.webp`

**Taille maximale:** 5 MB

**Dossier de stockage:** `wwwroot/uploads/products/`

#### **Méthodes principales:**

```csharp
// Upload une image
Task<string?> UploadImageAsync(IFormFile imageFile, string folder = "products")

// Supprime une image
Task<bool> DeleteImageAsync(string? imagePath)

// Valide une image
bool IsValidImage(IFormFile imageFile)
```

### 3. ?? ViewModel mis à jour

**ProductViewModel** enrichi avec:
```csharp
[Display(Name = "Image actuelle")]
public string? ImagePath { get; set; }

[Display(Name = "Image du produit")]
public IFormFile? ImageFile { get; set; }
```

### 4. ?? Contrôleur AdminController

#### **Modifications apportées:**

**? Products (Liste)**
- Affiche le chemin de l'image pour chaque produit

**? CreateProduct**
- Upload de l'image lors de la création
- Validation du format
- Enregistrement du chemin dans la base de données
- Messages d'erreur en cas de problème

**? EditProduct**
- Affiche l'image actuelle
- Permet de modifier l'image
- Supprime l'ancienne image si nouvelle image uploadée
- Conserve l'image actuelle si aucune nouvelle image

**? DeleteProduct**
- Supprime l'image du système de fichiers
- Supprime l'article de la base de données

### 5. ??? Vues Mises à Jour

#### **CreateProduct.cshtml**
**Caractéristiques:**
- Input de type `file` avec `accept="image/*"`
- Prévisualisation d'image en JavaScript
- Information sur les formats acceptés
- Form avec `enctype="multipart/form-data"`

**Aperçu d'image:**
```javascript
// Preview automatique lors de la sélection
document.getElementById('imageUpload').addEventListener('change', function(e) {
    const file = e.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function(event) {
            document.getElementById('preview').src = event.target.result;
            document.getElementById('imagePreview').style.display = 'block';
        }
        reader.readAsDataURL(file);
    }
});
```

#### **EditProduct.cshtml**
**Caractéristiques:**
- Affiche l'image actuelle (si existe)
- Placeholder si aucune image
- Input pour nouvelle image
- Prévisualisation de la nouvelle image
- Hidden input pour conserver l'ImagePath existant

#### **Products.cshtml**
**Caractéristiques:**
- Colonne "Image" dans la table
- Thumbnails 50x50px avec `object-fit: cover`
- Placeholder pour produits sans image
- Tooltips Bootstrap sur les images
- Aperçu de l'image dans le modal de suppression

### 6. ?? Configuration

#### **Program.cs**
```csharp
// Service enregistré
builder.Services.AddScoped<IImageUploadService, ImageUploadService>();

// Middleware de fichiers statiques activé
app.UseStaticFiles();
```

## ?? Structure des Fichiers

```
Solution_Magasin/
??? wwwroot/
?   ??? uploads/
?       ??? products/          ? Images produits (créé automatiquement)
?           ??? {guid}.jpg
?           ??? {guid}.png
?           ??? ...
??? Services/
?   ??? ImageUploadService.cs  ? Service de gestion d'images
??? Models/
?   ??? Article.cs             ? Modèle avec ImagePath
??? ViewModels/
?   ??? ProductViewModel.cs    ? ViewModel avec IFormFile
??? Controllers/
?   ??? AdminController.cs     ? Logique d'upload/suppression
??? Views/Admin/
?   ??? Products.cshtml        ? Liste avec images
?   ??? CreateProduct.cshtml   ? Upload nouvelle image
?   ??? EditProduct.cshtml     ? Modifier/remplacer image
??? Migrations/
    ??? 20251220000000_AddArticleImagePath.cs
```

## ?? Utilisation

### Créer un Produit avec Image

1. **Navigation:** Admin > Produits > Nouveau Produit
2. **Remplir** les champs du formulaire
3. **Cliquer** sur "Choisir un fichier" pour l'image
4. **Sélectionner** une image (JPG, PNG, GIF, WEBP < 5MB)
5. **Prévisualiser** l'image sélectionnée
6. **Créer** le produit

**Résultat:**
- Image uploadée dans `/wwwroot/uploads/products/`
- Chemin enregistré dans la base de données
- Image visible dans la liste des produits

### Modifier l'Image d'un Produit

1. **Navigation:** Admin > Produits > Modifier (icône crayon)
2. **Voir** l'image actuelle (si existe)
3. **Choisir** une nouvelle image (optionnel)
4. **Prévisualiser** la nouvelle image
5. **Enregistrer** les modifications

**Résultat:**
- Ancienne image supprimée du serveur
- Nouvelle image uploadée
- Chemin mis à jour en base de données

### Supprimer un Produit avec Image

1. **Navigation:** Admin > Produits > Supprimer (icône poubelle)
2. **Voir** l'aperçu dans le modal (avec image si existe)
3. **Confirmer** la suppression

**Résultat:**
- Image supprimée du serveur
- Produit supprimé de la base de données

## ??? Sécurité et Validation

### Validations Côté Serveur

**1. Validation de Format**
```csharp
var allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
```

**2. Validation de Taille**
```csharp
var maxFileSize = 5 * 1024 * 1024; // 5 MB
```

**3. Validation de Type MIME**
```csharp
var validMimeTypes = { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
```

**4. Génération de Nom Unique**
```csharp
var fileName = $"{Guid.NewGuid()}{fileExtension}";
```

### Gestion des Erreurs

**Messages utilisateur:**
- ? "Format d'image invalide. Formats acceptés: JPG, PNG, GIF, WEBP (Max 5MB)"
- ? "Erreur lors du téléchargement de l'image"
- ? "L'image associée sera également supprimée" (lors de la suppression)

**Logging:**
- Toutes les opérations sont loggées
- Erreurs tracées pour debugging

## ?? Interface Utilisateur

### Liste des Produits
```html
<!-- Avec image -->
<img src="/uploads/products/xxx.jpg" class="img-thumbnail" style="width: 50px; height: 50px;" />

<!-- Sans image -->
<div class="bg-secondary text-white" style="width: 50px; height: 50px;">
    <i class="bi bi-image"></i>
</div>
```

### Formulaires
```html
<input type="file" class="form-control" accept="image/*" />
<div class="form-text">
    <i class="bi bi-info-circle"></i> Formats acceptés: JPG, PNG, GIF, WEBP (Max 5MB)
</div>
```

### Prévisualisation
```html
<div id="imagePreview" style="display: none;">
    <img id="preview" src="" class="img-thumbnail" style="max-width: 300px;" />
</div>
```

## ?? Base de Données

### Schéma Article

| Colonne | Type | Nullable | Description |
|---------|------|----------|-------------|
| image_path | varchar(500) | YES | Chemin relatif de l'image |

**Exemple de valeur:**
```
/uploads/products/a1b2c3d4-e5f6-7890-abcd-ef1234567890.jpg
```

### Query Exemple
```sql
-- Récupérer tous les produits avec images
SELECT id_article, nom_art, image_path 
FROM Article 
WHERE image_path IS NOT NULL;

-- Mettre à jour une image
UPDATE Article 
SET image_path = '/uploads/products/new-image.jpg' 
WHERE id_article = 1;

-- Supprimer référence image
UPDATE Article 
SET image_path = NULL 
WHERE id_article = 1;
```

## ?? Configuration Avancée

### Modifier la Taille Maximale

Dans `ImageUploadService.cs`:
```csharp
private readonly long _maxFileSize = 10 * 1024 * 1024; // 10 MB
```

### Ajouter des Formats

Dans `ImageUploadService.cs`:
```csharp
private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };

var validMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp", "image/svg+xml" };
```

### Changer le Dossier d'Upload

Dans les appels au service:
```csharp
// Upload dans un autre dossier
var imagePath = await _imageUploadService.UploadImageAsync(model.ImageFile, "custom-folder");
// Résultat: /uploads/custom-folder/xxx.jpg
```

## ?? Points d'Attention

### 1. Migration de la Base de Données
**IMPORTANT:** Appliquez la migration avant d'utiliser la fonctionnalité
```bash
dotnet ef database update
```

### 2. Permissions du Dossier
Le dossier `wwwroot/uploads/` doit être accessible en écriture pour l'application.

### 3. Fichiers Existants
Si vous avez des produits existants dans la base de données, leur `image_path` sera `NULL` jusqu'à ce qu'une image soit uploadée.

### 4. Production
En production, considérez:
- ? CDN pour servir les images
- ? Compression d'images
- ? Redimensionnement automatique
- ? Backup régulier du dossier uploads

## ?? Responsive

Les images s'adaptent à tous les écrans:
- **Desktop:** Thumbnails 50x50px dans la liste
- **Mobile:** Images empilées verticalement
- **Modals:** Images adaptatives avec max-width

## ?? Cas d'Usage

### 1. Créer 10 Produits avec Images
```
1. Admin > Produits > Nouveau Produit
2. Remplir les infos + Upload image
3. Répéter pour chaque produit
4. Toutes les images dans /wwwroot/uploads/products/
5. Tous les chemins en base de données
```

### 2. Mettre à Jour une Image
```
1. Admin > Produits > Modifier
2. Voir l'image actuelle
3. Choisir nouvelle image
4. Sauvegarder
5. Ancienne image supprimée automatiquement
```

### 3. Produits Sans Image
```
1. Créer un produit sans image
2. Image = NULL en base
3. Placeholder affiché dans la liste
4. Modifier plus tard pour ajouter une image
```

## ? Tests Recommandés

### Tests Fonctionnels
- [ ] Créer un produit avec image JPG
- [ ] Créer un produit avec image PNG
- [ ] Créer un produit sans image
- [ ] Modifier un produit et changer l'image
- [ ] Modifier un produit sans changer l'image
- [ ] Supprimer un produit avec image
- [ ] Tenter d'uploader un fichier > 5MB
- [ ] Tenter d'uploader un fichier non-image
- [ ] Vérifier l'affichage dans la liste
- [ ] Vérifier l'aperçu dans le modal de suppression

### Tests de Sécurité
- [ ] Tenter d'uploader un fichier .exe renommé en .jpg
- [ ] Tenter d'uploader avec un MIME type invalide
- [ ] Vérifier que les noms de fichiers sont uniques (GUID)
- [ ] Vérifier que les anciennes images sont supprimées

## ?? Résultat Final

? **Système complet de gestion d'images produits**
- Upload simple et intuitif
- Validation robuste
- Stockage local sécurisé
- Chemins en base de données
- Interface user-friendly
- Prévisualisation en temps réel
- Gestion automatique des suppressions
- Messages d'erreur clairs
- Build réussi ?

---

**Date:** 20/12/2024  
**Version:** 1.0  
**Status:** ? Opérationnel
