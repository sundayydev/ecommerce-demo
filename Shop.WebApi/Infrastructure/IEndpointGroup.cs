namespace Shop.WebApi.Infrastructure;

public interface  IEndpointGroup
{
    /// <summary>
    /// The route prefix for this endpoint group.
    /// Defaults to <c>/api/{ClassName}</c>. Override to specify a custom or nested path.
    /// </summary>
    static virtual string? RoutePrefix => null;

    static abstract void Map(RouteGroupBuilder groupBuilder);
}