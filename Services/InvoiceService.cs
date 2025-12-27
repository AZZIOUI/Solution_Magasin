using Solution_Magasin.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Solution_Magasin.Services;

/// <summary>
/// Service for generating invoices (factures)
/// </summary>
public class InvoiceService
{
    private readonly DotnetProjectContext _dbContext;
    private readonly IWebHostEnvironment _environment;

    public InvoiceService(
        DotnetProjectContext dbContext,
        IWebHostEnvironment environment)
    {
        _dbContext = dbContext;
        _environment = environment;
    }

    /// <summary>
    /// Generate invoice HTML for a sale
    /// </summary>
    public async Task<string> GenerateInvoiceHtmlAsync(int venteId)
    {
        var vente = await _dbContext.Ventes
            .Include(v => v.IdClientNavigation)
            .Include(v => v.IdPaymentNavigation)
            .Include(v => v.DetailVentes)
                .ThenInclude(dv => dv.IdArticleNavigation)
            .FirstOrDefaultAsync(v => v.IdVente == venteId);

        if (vente == null)
        {
            throw new ArgumentException("Vente non trouvée", nameof(venteId));
        }

        var client = vente.IdClientNavigation;
        var payment = vente.IdPaymentNavigation;

        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang='fr'>");
        html.AppendLine("<head>");
        html.AppendLine("    <meta charset='UTF-8'>");
        html.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        html.AppendLine("    <title>Facture</title>");
        html.AppendLine("    <style>");
        html.AppendLine("        body { font-family: Arial, sans-serif; margin: 40px; }");
        html.AppendLine("        .header { text-align: center; margin-bottom: 30px; }");
        html.AppendLine("        .company-name { font-size: 24px; font-weight: bold; color: #333; }");
        html.AppendLine("        .invoice-title { font-size: 32px; font-weight: bold; color: #0066cc; margin: 20px 0; }");
        html.AppendLine("        .info-section { margin: 20px 0; }");
        html.AppendLine("        .info-row { display: flex; justify-content: space-between; margin: 10px 0; }");
        html.AppendLine("        .info-label { font-weight: bold; color: #666; }");
        html.AppendLine("        table { width: 100%; border-collapse: collapse; margin: 30px 0; }");
        html.AppendLine("        th { background-color: #0066cc; color: white; padding: 12px; text-align: left; }");
        html.AppendLine("        td { padding: 10px; border-bottom: 1px solid #ddd; }");
        html.AppendLine("        .total-section { text-align: right; margin-top: 30px; }");
        html.AppendLine("        .total-row { font-size: 18px; margin: 10px 0; }");
        html.AppendLine("        .total-amount { font-size: 24px; font-weight: bold; color: #0066cc; }");
        html.AppendLine("        .footer { margin-top: 50px; text-align: center; color: #666; font-size: 12px; }");
        html.AppendLine("        @media print {");
        html.AppendLine("            body { margin: 20px; }");
        html.AppendLine("            .no-print { display: none; }");
        html.AppendLine("        }");
        html.AppendLine("    </style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");

        // Header
        html.AppendLine("    <div class='header'>");
        html.AppendLine("        <div class='company-name'>AZZIOUI MAGASIN</div>");
        html.AppendLine("        <div class='invoice-title'>FACTURE</div>");
        html.AppendLine("    </div>");

        // Invoice info
        html.AppendLine("    <div class='info-section'>");
        html.AppendLine($"        <div class='info-row'>");
        html.AppendLine($"            <div><span class='info-label'>Numéro de facture:</span> INV-{vente.IdVente:D6}</div>");
        html.AppendLine($"            <div><span class='info-label'>Date:</span> {vente.DateVente?.ToString("dd/MM/yyyy") ?? "N/A"}</div>");
        html.AppendLine($"        </div>");
        html.AppendLine($"        <div class='info-row'>");
        html.AppendLine($"            <div><span class='info-label'>Client:</span> {client?.PrenomClient} {client?.NomClient}</div>");
        html.AppendLine($"            <div><span class='info-label'>Statut:</span> {vente.Status ?? "N/A"}</div>");
        html.AppendLine($"        </div>");
        html.AppendLine($"        <div class='info-row'>");
        html.AppendLine($"            <div><span class='info-label'>Email:</span> {client?.MailClient ?? "N/A"}</div>");
        html.AppendLine($"            <div><span class='info-label'>Téléphone:</span> {client?.TelClient ?? "N/A"}</div>");
        html.AppendLine($"        </div>");
        html.AppendLine($"        <div class='info-row'>");
        html.AppendLine($"            <div><span class='info-label'>Adresse de livraison:</span> {vente.AdresseLiv ?? client?.AdresseClient ?? "N/A"}</div>");
        html.AppendLine($"            <div><span class='info-label'>Méthode de paiement:</span> {payment?.Methode ?? "N/A"}</div>");
        html.AppendLine($"        </div>");
        html.AppendLine("    </div>");

        // Products table
        html.AppendLine("    <table>");
        html.AppendLine("        <thead>");
        html.AppendLine("            <tr>");
        html.AppendLine("                <th>Référence</th>");
        html.AppendLine("                <th>Produit</th>");
        html.AppendLine("                <th>Quantité</th>");
        html.AppendLine("                <th>Prix unitaire</th>");
        html.AppendLine("                <th>Montant</th>");
        html.AppendLine("            </tr>");
        html.AppendLine("        </thead>");
        html.AppendLine("        <tbody>");

        foreach (var detail in vente.DetailVentes)
        {
            var article = detail.IdArticleNavigation;
            html.AppendLine("            <tr>");
            html.AppendLine($"                <td>{article?.ReferenceArt ?? "N/A"}</td>");
            html.AppendLine($"                <td>{article?.NomArt ?? "N/A"}</td>");
            html.AppendLine($"                <td>{detail.QteDv ?? 0}</td>");
            html.AppendLine($"                <td>{article?.PrixUnit?.ToString("F2") ?? "0.00"} DH</td>");
            html.AppendLine($"                <td>{detail.MontantDv?.ToString("F2") ?? "0.00"} DH</td>");
            html.AppendLine("            </tr>");
        }

        html.AppendLine("        </tbody>");
        html.AppendLine("    </table>");

        // Total section
        html.AppendLine("    <div class='total-section'>");
        html.AppendLine($"        <div class='total-row'><span class='info-label'>Sous-total:</span> {vente.TotalV?.ToString("F2") ?? "0.00"} DH</div>");
        html.AppendLine($"        <div class='total-row'><span class='info-label'>TVA (20%):</span> {((vente.TotalV ?? 0) * 0.2).ToString("F2")} DH</div>");
        html.AppendLine($"        <div class='total-row total-amount'><span class='info-label'>TOTAL:</span> {((vente.TotalV ?? 0) * 1.2).ToString("F2")} DH</div>");
        html.AppendLine("    </div>");

        // Footer
        html.AppendLine("    <div class='footer'>");
        html.AppendLine("        <p>Merci pour votre achat!</p>");
        html.AppendLine("        <p>AZZIOUI MAGASIN - Votre magasin de confiance</p>");
        html.AppendLine("    </div>");

        html.AppendLine("    <div class='no-print' style='text-align: center; margin-top: 30px;'>");
        html.AppendLine("        <button onclick='window.print()' style='padding: 10px 20px; background-color: #0066cc; color: white; border: none; border-radius: 5px; cursor: pointer;'>Imprimer</button>");
        html.AppendLine("    </div>");

        html.AppendLine("</body>");
        html.AppendLine("</html>");

        return html.ToString();
    }

    /// <summary>
    /// Save invoice to database and file system
    /// </summary>
    public async Task<Facture> CreateInvoiceAsync(int venteId)
    {
        // Check if invoice already exists
        var existingFacture = await _dbContext.Factures
            .FirstOrDefaultAsync(f => f.IdVente == venteId);

        if (existingFacture != null)
        {
            return existingFacture;
        }

        var vente = await _dbContext.Ventes
            .FirstOrDefaultAsync(v => v.IdVente == venteId);

        if (vente == null)
        {
            throw new ArgumentException("Vente non trouvée", nameof(venteId));
        }

        // Generate HTML content
        var htmlContent = await GenerateInvoiceHtmlAsync(venteId);

        // Save to file system
        var invoicesDir = Path.Combine(_environment.WebRootPath, "invoices");
        if (!Directory.Exists(invoicesDir))
        {
            Directory.CreateDirectory(invoicesDir);
        }

        var fileName = $"invoice_{venteId}_{DateTime.Now:yyyyMMddHHmmss}.html";
        var filePath = Path.Combine(invoicesDir, fileName);
        await File.WriteAllTextAsync(filePath, htmlContent);

        // Create database record
        var facture = new Facture
        {
            CodeFacture = $"INV-{venteId:D6}",
            DateFacture = DateOnly.FromDateTime(DateTime.Now),
            MontantTotal = (vente.TotalV ?? 0) * 1.2, // Include 20% TVA
            FilePath = $"/invoices/{fileName}",
            IdVente = venteId
        };

        _dbContext.Factures.Add(facture);
        await _dbContext.SaveChangesAsync();

        return facture;
    }

    /// <summary>
    /// Get invoice by ID
    /// </summary>
    public async Task<Facture?> GetInvoiceAsync(int factureId)
    {
        return await _dbContext.Factures
            .Include(f => f.IdVenteNavigation)
            .FirstOrDefaultAsync(f => f.IdFacture == factureId);
    }

    /// <summary>
    /// Get invoice by sale ID
    /// </summary>
    public async Task<Facture?> GetInvoiceByVenteIdAsync(int venteId)
    {
        return await _dbContext.Factures
            .FirstOrDefaultAsync(f => f.IdVente == venteId);
    }
}
