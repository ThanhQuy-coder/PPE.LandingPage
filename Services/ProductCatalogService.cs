using System.Net.Http.Json;
using PPE.LandingPage.Models;

namespace PPE.LandingPage.Services;

public sealed class ProductCatalogService
{
    private readonly HttpClient _httpClient;
    private Task<CatalogSnapshot>? _loadTask;
    private CatalogSnapshot? _cachedSnapshot;

    public ProductCatalogService(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
