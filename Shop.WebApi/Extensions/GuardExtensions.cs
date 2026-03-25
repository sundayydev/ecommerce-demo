using System.Runtime.CompilerServices;
using Ardalis.GuardClauses;

namespace Shop.WebApi.Extensions;

public static class GuardExtensions
{
    public static void AnonymousMethod(this IGuardClause guardClause, Delegate input, [CallerArgumentExpression("input")] string? parameterName = null)
    {
        if (input.Method.IsCollectible || input.Method.Name.Contains("<", StringComparison.Ordinal))
        {
            throw new ArgumentException("Endpoint handler bắt buộc phải là một method có tên rõ ràng (Named Method), không được dùng Lambda ẩn danh.", parameterName);
        }
    }
}