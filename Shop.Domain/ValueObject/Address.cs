namespace Shop.Domain.ValueObject;

public class Address : Common.ValueObject
{
    public string Detail { get; }        // Số nhà, đường
    public string Ward { get; }          // Xã / Phường
    public string City { get; }          // Thành phố / Tỉnh

    public Address(string detail, string ward, string city)
    {
        if (string.IsNullOrWhiteSpace(detail))
            throw new ArgumentException("Detail address is required");

        if (string.IsNullOrWhiteSpace(ward))
            throw new ArgumentException("Ward is required");

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required");

        Detail = detail.Trim();
        Ward = ward.Trim();
        City = city.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Detail;
        yield return Ward;
        yield return City;
    }

    // Helper (rất hữu ích cho UI)
    public override string ToString()
    {
        return $"{Detail}, {Ward}, {City}";
    }
}