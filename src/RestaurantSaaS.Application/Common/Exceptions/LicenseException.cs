namespace RestaurantSaaS.Application.Common.Exceptions;

public sealed class LicenseException(string message) : AppException("license_blocked", message, 402)
{
}
