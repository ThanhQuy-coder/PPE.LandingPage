using System.Net.Http.Json;
using PPE.LandingPage.Models;

namespace PPE.LandingPage.Services;

public sealed class ProductCatalogService
{
    private readonly HttpClient _httpClient;
    private Task<CatalogSnapshot>? _loadTask;
    private CatalogSnapshot? _cachedSnapshot;
    private Task<HomeCatalogSummary>? _homeLoadTask;
    private HomeCatalogSummary? _cachedHomeSummary;

    public ProductCatalogService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HomeCatalogSummary> GetHomeSummaryAsync(bool forceReload = false)
    {
        if (!forceReload && _cachedSnapshot is not null)
        {
            var featured = SelectFeaturedProducts(_cachedSnapshot.Products, 8);
            var intro = _cachedSnapshot.Products.FirstOrDefault(p => p.Id == "product-0003")
                        ?? _cachedSnapshot.Products.FirstOrDefault();
            return new HomeCatalogSummary(
                _cachedSnapshot.Products.Count,
                _cachedSnapshot.Categories,
                featured,
                intro?.PrimaryImageUrl ?? BrandInfo.PlaceholderImage,
                intro?.Name ?? "Sản phẩm bảo hộ lao động Gia Hoàng");
        }

        if (!forceReload && _cachedHomeSummary is not null)
        {
            return _cachedHomeSummary;
        }

        if (forceReload || _homeLoadTask is null || _homeLoadTask.IsFaulted || _homeLoadTask.IsCanceled)
        {
            _homeLoadTask = LoadHomeSummaryInternalAsync();
        }

        return await _homeLoadTask;
    }

    private async Task<HomeCatalogSummary> LoadHomeSummaryInternalAsync()
    {
        var dto = await _httpClient.GetFromJsonAsync<HomeCatalogSummaryDto>("data/home_catalog.json");
        if (dto is null)
        {
            return new HomeCatalogSummary(0, [], [], BrandInfo.PlaceholderImage, "Sản phẩm bảo hộ lao động Gia Hoàng");
        }

        _cachedHomeSummary = new HomeCatalogSummary(
            dto.TotalProducts,
            dto.Categories ?? [],
            dto.FeaturedProducts ?? [],
            string.IsNullOrWhiteSpace(dto.IntroImageUrl) ? BrandInfo.PlaceholderImage : dto.IntroImageUrl,
            string.IsNullOrWhiteSpace(dto.IntroImageAlt) ? "Sản phẩm bảo hộ lao động Gia Hoàng" : dto.IntroImageAlt);

        return _cachedHomeSummary;
    }

    public async Task<CatalogSnapshot> GetCatalogAsync(bool forceReload = false)
    {
        if (!forceReload && _cachedSnapshot is not null)
        {
            return _cachedSnapshot;
        }

        if (forceReload || _loadTask is null || _loadTask.IsFaulted || _loadTask.IsCanceled)
        {
            _loadTask = LoadInternalAsync();
        }

        return await _loadTask;
    }

    private async Task<CatalogSnapshot> LoadInternalAsync()
    {
        var products = await _httpClient.GetFromJsonAsync<List<ProductItem>>("data/normalized_products.json")
            ?? [];

        var cleanProducts = products
            .Where(p => !string.IsNullOrWhiteSpace(p.Id) && !string.IsNullOrWhiteSpace(p.Name))
            .ToList();

        var categoryMap = new Dictionary<string, (string Name, int Count, string CoverImage)>(StringComparer.OrdinalIgnoreCase);

        foreach (var product in cleanProducts)
        {
            var count = Math.Min(product.CategoryIds.Count, product.CategoryNames.Count);
            for (var i = 0; i < count; i++)
            {
                var catId = product.CategoryIds[i];
                var catName = product.CategoryNames[i];
                if (string.IsNullOrWhiteSpace(catId) || string.IsNullOrWhiteSpace(catName))
                {
                    continue;
                }

                if (categoryMap.TryGetValue(catId, out var existing))
                {
                    categoryMap[catId] = (existing.Name, existing.Count + 1, existing.CoverImage);
                }
                else
                {
                    categoryMap[catId] = (catName, 1, product.PrimaryImageUrl);
                }
            }
        }

        var categories = categoryMap
            .OrderBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase)
            .Select(kv => new ProductCategory
            {
                Id = kv.Key,
                Name = kv.Value.Name,
                ProductCount = kv.Value.Count,
                CoverImageUrl = kv.Value.CoverImage
            })
            .ToList();

        _cachedSnapshot = new CatalogSnapshot(cleanProducts, categories);
        return _cachedSnapshot;
    }

    public static IReadOnlyList<ProductItem> SelectFeaturedProducts(IReadOnlyList<ProductItem> products, int maxCount = 8)
    {
        if (products.Count <= maxCount)
        {
            return products;
        }

        var selected = new List<ProductItem>(maxCount);
        var seenCategories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Pick one representative product per category first for diversity.
        foreach (var product in products)
        {
            if (selected.Count >= maxCount)
            {
                break;
            }

            if (seenCategories.Add(product.PrimaryCategoryId))
            {
                selected.Add(product);
            }
        }

        if (selected.Count < maxCount)
        {
            var selectedIds = selected.Select(p => p.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var product in products)
            {
                if (selected.Count >= maxCount)
                {
                    break;
                }

                if (selectedIds.Add(product.Id))
                {
                    selected.Add(product);
                }
            }
        }

        return selected;
    }

    public static IReadOnlyList<ProductItem> FilterProducts(
        IReadOnlyList<ProductItem> products,
        string? searchTerm,
        string? categoryId)
    {
        IEnumerable<ProductItem> query = products;

        if (!string.IsNullOrWhiteSpace(categoryId) && !string.Equals(categoryId, "all", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(p => p.CategoryIds.Contains(categoryId, StringComparer.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(p =>
                p.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                || p.PrimaryCategoryName.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        return query.ToList();
    }
}

public sealed record CatalogSnapshot(
    IReadOnlyList<ProductItem> Products,
    IReadOnlyList<ProductCategory> Categories);

public sealed record HomeCatalogSummary(
    int TotalProducts,
    IReadOnlyList<ProductCategory> Categories,
    IReadOnlyList<ProductItem> FeaturedProducts,
    string IntroImageUrl,
    string IntroImageAlt);

internal sealed class HomeCatalogSummaryDto
{
    public int TotalProducts { get; set; }
    public List<ProductCategory>? Categories { get; set; }
    public List<ProductItem>? FeaturedProducts { get; set; }
    public string? IntroImageUrl { get; set; }
    public string? IntroImageAlt { get; set; }
}
