using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Solution_Magasin.Models;
using Solution_Magasin.ViewModels;

namespace Solution_Magasin.Controllers;

/// <summary>
/// Controller for visitor (public) access to product catalog
/// No authentication required
/// </summary>
public class VisitorController : Controller
{
    private readonly DotnetProjectContext _dbContext;
    private readonly ILogger<VisitorController> _logger;

    public VisitorController(
        DotnetProjectContext dbContext,
        ILogger<VisitorController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Display product catalog with filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Catalog(int? categoryId, string? search, int page = 1)
    {
        const int pageSize = 12;
        
        var query = _dbContext.Articles
            .Include(a => a.IdCatNavigation)
            .Include(a => a.Stocks)
            .Include(a => a.Reviews)
            .AsQueryable();

        // Apply category filter
        if (categoryId.HasValue)
        {
            query = query.Where(a => a.IdCat == categoryId.Value);
        }

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(a => 
                a.NomArt.Contains(search) || 
                a.DesignationArt.Contains(search) ||
                a.ReferenceArt.Contains(search));
        }

        // Get total count for pagination
        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        // Get paginated products
        var products = await query
            .OrderBy(a => a.NomArt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new ProductCardViewModel
            {
                IdArticle = a.IdArticle,
                ReferenceArt = a.ReferenceArt,
                NomArt = a.NomArt,
                DesignationArt = a.DesignationArt,
                PrixUnit = a.PrixUnit,
                ImagePath = a.ImagePath,
                CategoryName = a.IdCatNavigation != null ? a.IdCatNavigation.NomCat : null,
                StockQuantity = a.Stocks.Sum(s => s.QteDispo),
                AverageRating = a.Reviews.Any() ? a.Reviews.Average(r => r.Rating) : null,
                ReviewCount = a.Reviews.Count
            })
            .ToListAsync();

        // Get all categories for filter
        var categories = await _dbContext.Categories
            .OrderBy(c => c.NomCat)
            .ToListAsync();

        var viewModel = new ProductCatalogViewModel
        {
            Products = products,
            Categories = categories,
            SelectedCategoryId = categoryId,
            SearchQuery = search,
            CurrentPage = page,
            TotalPages = totalPages,
            PageSize = pageSize
        };

        return View(viewModel);
    }

    /// <summary>
    /// Display detailed product information
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ProductDetails(int id)
    {
        var article = await _dbContext.Articles
            .Include(a => a.IdCatNavigation)
            .Include(a => a.Stocks)
            .Include(a => a.Reviews)
                .ThenInclude(r => r.IdClientNavigation)
            .FirstOrDefaultAsync(a => a.IdArticle == id);

        if (article == null)
        {
            return NotFound();
        }

        // Get reviews
        var reviews = article.Reviews
            .OrderByDescending(r => r.IdReview)
            .Select(r => new ProductReviewViewModel
            {
                IdReview = r.IdReview,
                ClientName = r.IdClientNavigation != null 
                    ? $"{r.IdClientNavigation.PrenomClient} {r.IdClientNavigation.NomClient}" 
                    : "Client",
                Rating = r.Rating,
                Comment = r.Comment,
                DateReview = null // Review model doesn't have DateReview
            })
            .ToList();

        // Get related products (same category)
        var relatedProducts = new List<ProductCardViewModel>();
        if (article.IdCat.HasValue)
        {
            relatedProducts = await _dbContext.Articles
                .Include(a => a.IdCatNavigation)
                .Include(a => a.Stocks)
                .Include(a => a.Reviews)
                .Where(a => a.IdCat == article.IdCat && a.IdArticle != id)
                .Take(4)
                .Select(a => new ProductCardViewModel
                {
                    IdArticle = a.IdArticle,
                    ReferenceArt = a.ReferenceArt,
                    NomArt = a.NomArt,
                    DesignationArt = a.DesignationArt,
                    PrixUnit = a.PrixUnit,
                    ImagePath = a.ImagePath,
                    CategoryName = a.IdCatNavigation != null ? a.IdCatNavigation.NomCat : null,
                    StockQuantity = a.Stocks.Sum(s => s.QteDispo),
                    AverageRating = a.Reviews.Any() ? a.Reviews.Average(r => r.Rating) : null,
                    ReviewCount = a.Reviews.Count
                })
                .ToListAsync();
        }

        var viewModel = new ProductDetailsViewModel
        {
            IdArticle = article.IdArticle,
            ReferenceArt = article.ReferenceArt,
            NomArt = article.NomArt,
            DesignationArt = article.DesignationArt,
            PrixUnit = article.PrixUnit,
            ImagePath = article.ImagePath,
            CategoryName = article.IdCatNavigation?.NomCat,
            StockQuantity = article.Stocks.Sum(s => s.QteDispo),
            Reviews = reviews,
            AverageRating = reviews.Any() ? reviews.Average(r => r.Rating ?? 0) : null,
            ReviewCount = reviews.Count,
            RelatedProducts = relatedProducts
        };

        return View(viewModel);
    }

    /// <summary>
    /// Home page for visitors
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // Get featured products (most reviewed or newest)
        var featuredProducts = await _dbContext.Articles
            .Include(a => a.IdCatNavigation)
            .Include(a => a.Stocks)
            .Include(a => a.Reviews)
            .OrderByDescending(a => a.Reviews.Count)
            .ThenByDescending(a => a.DateAjout)
            .Take(8)
            .Select(a => new ProductCardViewModel
            {
                IdArticle = a.IdArticle,
                ReferenceArt = a.ReferenceArt,
                NomArt = a.NomArt,
                DesignationArt = a.DesignationArt,
                PrixUnit = a.PrixUnit,
                ImagePath = a.ImagePath,
                CategoryName = a.IdCatNavigation != null ? a.IdCatNavigation.NomCat : null,
                StockQuantity = a.Stocks.Sum(s => s.QteDispo),
                AverageRating = a.Reviews.Any() ? a.Reviews.Average(r => r.Rating) : null,
                ReviewCount = a.Reviews.Count
            })
            .ToListAsync();

        // Get categories
        var categories = await _dbContext.Categories
            .OrderBy(c => c.NomCat)
            .Take(6)
            .ToListAsync();

        ViewBag.FeaturedProducts = featuredProducts;
        ViewBag.Categories = categories;

        return View();
    }
}
