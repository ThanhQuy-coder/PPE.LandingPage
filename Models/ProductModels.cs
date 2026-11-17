using System.Text.Json.Serialization;

namespace PPE.LandingPage.Models;

public sealed class ProductItem
{
    private static readonly HashSet<string> UnconfirmedValues = new(StringComparer.OrdinalIgnoreCase)
    {
        "",
        "đang cập nhật",
        "chưa cập nhật",
        "n/a",
        "na",
        "-",
        "không có"
    };

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("categoryIds")]
    public List<string> CategoryIds { get; set; } = [];

    [JsonPropertyName("categoryNames")]
    public List<string> CategoryNames { get; set; } = [];

    [JsonPropertyName("attributes")]
    public Dictionary<string, string> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    [JsonPropertyName("description")]
    public List<string> Description { get; set; } = [];

    [JsonPropertyName("imageUrls")]
    public List<string> ImageUrls { get; set; } = [];

    public string PrimaryCategoryId => CategoryIds.FirstOrDefault() ?? string.Empty;

    public string PrimaryCategoryName => CategoryNames.FirstOrDefault() ?? "Sản phẩm bảo hộ";

    public string PrimaryImageUrl => NormalizeImagePath(ImageUrls.FirstOrDefault());

    public IReadOnlyList<string> NormalizedImageUrls =>
        ImageUrls.Select(NormalizeImagePath).Where(path => !string.IsNullOrWhiteSpace(path)).ToList();

    public IReadOnlyList<KeyValuePair<string, string>> ConfirmedAttributes =>
        Attributes
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Key)
                         && !string.IsNullOrWhiteSpace(kv.Value)
                         && !UnconfirmedValues.Contains(kv.Value.Trim()))
            .ToList();

    public string? GetConfirmedAttribute(string key)
    {
        if (Attributes.TryGetValue(key, out var value)
            && !string.IsNullOrWhiteSpace(value)
            && !UnconfirmedValues.Contains(value.Trim()))
        {
            return value.Trim();
        }

        return null;
    }

    public static string NormalizeImagePath(string? rawPath)
    {
        if (string.IsNullOrWhiteSpace(rawPath))
        {
            return "images/placeholder-product.svg";
        }

        var trimmed = rawPath.Trim();
        if (trimmed.StartsWith("../", StringComparison.Ordinal))
        {
            trimmed = trimmed[3..];
        }
        else if (trimmed.StartsWith("./", StringComparison.Ordinal))
        {
            trimmed = trimmed[2..];
        }
        else if (trimmed.StartsWith("/", StringComparison.Ordinal))
        {
            trimmed = trimmed[1..];
        }

        return trimmed;
    }
}

public sealed class ProductCategory
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required int ProductCount { get; init; }
    public required string CoverImageUrl { get; init; }
}

public static class BrandInfo
{
    public const string ShortName = "Gia Hoàng";
    public const string UpperShortName = "GIA HOÀNG";
    public const string LegalName = "Công ty TNHH SX TM Bảo hộ lao động Gia Hoàng";
    public const string CatalogManufacturer = "Công ty TNHH SX TM BHLĐ Gia Hoàng";
    public const string HotlineDisplay = "090 120 023";
    public const string HotlineHref = "tel:090120023";
    public const string PlaceholderImage = "images/placeholder-product.svg";
}
