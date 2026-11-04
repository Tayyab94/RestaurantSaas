namespace RestaurantSaaS.Application.Abstractions;

public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string hash);

    /// <summary>A valid hash of a random password, used to keep sign-in timing the same when the user does not exist.</summary>
    string DummyHash { get; }
}
