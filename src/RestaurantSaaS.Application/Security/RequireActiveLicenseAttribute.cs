namespace RestaurantSaaS.Application.Security;

/// Put on a request that sells (order, payment, bill). M09 and M11 use it.
[AttributeUsage(AttributeTargets.Class)]
public sealed class RequireActiveLicenseAttribute : Attribute
{
}
