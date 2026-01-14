# ?? IMAGES PRODUITS - GUIDE RAPIDE

## ?? Démarrage Rapide

### 1. Appliquer la Migration
```bash
dotnet ef database update
```

### 2. Créer un Produit avec Image
```
Admin ? Produits ? Nouveau Produit
? Remplir formulaire
? Choisir image (JPG/PNG/GIF/WEBP < 5MB)
? Créer
```

### 3. Modifier une Image
```
Admin ? Produits ? Modifier (icône crayon)
? Voir image actuelle
? Choisir nouvelle image (optionnel)
? Enregistrer
```

## ?? Formats Acceptés

| Format | Extension | MIME Type |
|--------|-----------|-----------|
| JPEG | `.jpg`, `.jpeg` | `image/jpeg` |
| PNG | `.png` | `image/png` |
| GIF | `.gif` | `image/gif` |
| WEBP | `.webp` | `image/webp` |

**Taille maximale:** 5 MB

## ?? Structure

```
wwwroot/
??? uploads/
    ??? products/
        ??? {guid}.jpg
        ??? {guid}.png
        ??? ...
```

## ??? Base de Données

```sql
-- Colonne ajoutée
ALTER TABLE Article ADD image_path varchar(500) NULL;

-- Exemple de valeur
'/uploads/products/a1b2c3d4-e5f6-7890-abcd-ef1234567890.jpg'
```

## ?? Interface

### Liste des Produits
- ? Thumbnail 50x50px
- ? Placeholder si pas d'image
- ? Tooltips sur hover

### Créer/Modifier
- ? Input file avec accept="image/*"
- ? Prévisualisation JavaScript
- ? Messages de validation

### Supprimer
- ? Aperçu dans modal
- ? Alerte que l'image sera supprimée

## ?? Service ImageUploadService

```csharp
// Upload
var path = await _imageUploadService.UploadImageAsync(file, "products");

// Supprimer
await _imageUploadService.DeleteImageAsync(imagePath);

// Valider
bool isValid = _imageUploadService.IsValidImage(file);
```

## ? Workflow Complet

### Créer
```
1. Upload fichier ? Validation
2. Générer GUID unique
3. Sauvegarder dans /wwwroot/uploads/products/
4. Enregistrer chemin en BDD
5. Afficher dans liste
```

### Modifier
```
1. Charger image actuelle
2. Si nouvelle image:
   - Supprimer ancienne
   - Upload nouvelle
   - Mettre à jour BDD
3. Si pas de nouvelle image:
   - Conserver actuelle
```

### Supprimer
```
1. Supprimer fichier physique
2. Supprimer article en BDD
```

## ??? Validation

### Côté Serveur
- ? Extension (.jpg, .png, .gif, .webp)
- ? Taille (< 5MB)
- ? Type MIME (image/*)
- ? Nom unique (GUID)

### Messages d'Erreur
```
"Format d'image invalide. Formats acceptés: JPG, PNG, GIF, WEBP (Max 5MB)"
"Erreur lors du téléchargement de l'image"
```

## ?? Exemples de Code

### Controller (CreateProduct)
```csharp
if (model.ImageFile != null)
{
    if (_imageUploadService.IsValidImage(model.ImageFile))
    {
        imagePath = await _imageUploadService.UploadImageAsync(model.ImageFile, "products");
    }
}

article.ImagePath = imagePath;
```

### View (Input)
```html
<input asp-for="ImageFile" 
       class="form-control" 
       type="file" 
       accept="image/*" />
```

### View (Preview)
```javascript
document.getElementById('imageUpload').addEventListener('change', function(e) {
    const file = e.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function(event) {
            document.getElementById('preview').src = event.target.result;
        }
        reader.readAsDataURL(file);
    }
});
```

## ? Checklist de Test

- [ ] Upload JPG ?
- [ ] Upload PNG ?
- [ ] Upload sans image ?
- [ ] Modifier image ?
- [ ] Supprimer produit avec image ?
- [ ] Fichier trop grand (> 5MB) ?
- [ ] Format invalide (.txt) ?
- [ ] Affichage dans liste ?
- [ ] Preview dans modal ?

## ?? Points Clés

1. **Migration requise** avant utilisation
2. **Dossier créé automatiquement** si n'existe pas
3. **GUID unique** pour chaque fichier
4. **Ancienne image supprimée** lors de la modification
5. **Image supprimée** lors de la suppression du produit
6. **Placeholder affiché** si pas d'image

## ?? Dépannage

### "Image non trouvée"
- Vérifier que `UseStaticFiles()` est configuré
- Vérifier les permissions du dossier uploads

### "Erreur lors du téléchargement"
- Vérifier la taille du fichier (< 5MB)
- Vérifier le format (JPG/PNG/GIF/WEBP)
- Consulter les logs

### "Migration non appliquée"
```bash
dotnet ef database update
```

## ?? Responsive

- Desktop: Grid avec thumbnails
- Tablet: Grid 2 colonnes
- Mobile: Liste verticale

## ?? Prêt à l'emploi!

Le système est **complètement fonctionnel** et prêt pour la production.

---

**Aide rapide:** Consultez `IMAGE_UPLOAD_DOCUMENTATION.md` pour plus de détails
