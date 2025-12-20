using Microsoft.AspNetCore.Http;

namespace Solution_Magasin.Services;

/// <summary>
/// Service pour gérer le téléchargement et la gestion des images produits
/// </summary>
public interface IImageUploadService
{
    Task<string?> UploadImageAsync(IFormFile imageFile, string folder = "products");
    Task<bool> DeleteImageAsync(string? imagePath);
    bool IsValidImage(IFormFile imageFile);
}

public class ImageUploadService : IImageUploadService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ImageUploadService> _logger;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private readonly long _maxFileSize = 5 * 1024 * 1024; // 5 MB

    public ImageUploadService(IWebHostEnvironment environment, ILogger<ImageUploadService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    /// <summary>
    /// Télécharge une image et retourne le chemin relatif
    /// </summary>
    public async Task<string?> UploadImageAsync(IFormFile imageFile, string folder = "products")
    {
        try
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            // Validation
            if (!IsValidImage(imageFile))
            {
                _logger.LogWarning("Tentative de téléchargement d'un fichier invalide: {FileName}", imageFile.FileName);
                return null;
            }

            // Créer le dossier uploads s'il n'existe pas
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Générer un nom de fichier unique
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Sauvegarder le fichier
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Retourner le chemin relatif pour la base de données
            var relativePath = $"/uploads/{folder}/{fileName}";
            _logger.LogInformation("Image téléchargée avec succès: {RelativePath}", relativePath);

            return relativePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du téléchargement de l'image: {FileName}", imageFile?.FileName);
            return null;
        }
    }

    /// <summary>
    /// Supprime une image du système de fichiers
    /// </summary>
    public async Task<bool> DeleteImageAsync(string? imagePath)
    {
        try
        {
            if (string.IsNullOrEmpty(imagePath))
                return true;

            // Convertir le chemin relatif en chemin physique
            var physicalPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (File.Exists(physicalPath))
            {
                await Task.Run(() => File.Delete(physicalPath));
                _logger.LogInformation("Image supprimée: {ImagePath}", imagePath);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression de l'image: {ImagePath}", imagePath);
            return false;
        }
    }

    /// <summary>
    /// Valide si le fichier est une image valide
    /// </summary>
    public bool IsValidImage(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return false;

        // Vérifier la taille
        if (imageFile.Length > _maxFileSize)
        {
            _logger.LogWarning("Fichier trop volumineux: {FileName}, Taille: {Size} bytes", imageFile.FileName, imageFile.Length);
            return false;
        }

        // Vérifier l'extension
        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            _logger.LogWarning("Extension de fichier non autorisée: {Extension}", extension);
            return false;
        }

        // Vérifier le type MIME
        var validMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
        if (!validMimeTypes.Contains(imageFile.ContentType.ToLowerInvariant()))
        {
            _logger.LogWarning("Type MIME non autorisé: {ContentType}", imageFile.ContentType);
            return false;
        }

        return true;
    }
}
