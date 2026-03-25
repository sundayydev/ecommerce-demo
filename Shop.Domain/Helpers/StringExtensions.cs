using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Shop.Domain.Helpers;

public static class StringExtensions
{
    public static string ToSlug(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var slug = input.ToLowerInvariant();

        slug = slug.Replace("đ", "d");

        var normalizedString = slug.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        // Ép lại thành chuỗi chuẩn không dấu
        slug = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

        slug = Regex.Replace(slug, @"\s+", "-").Trim('-');

        slug = Regex.Replace(slug, @"-+", "-");

        return slug;
    }
}