using RestaurantSaaS.Infrastructure.Services;

namespace RestaurantSaaS.UnitTests.Security;

public class PasswordHasherTests
{
    private readonly Pbkdf2PasswordHasher _hasher = new();

    [Fact]
    public void A_password_verifies_against_its_own_hash()
    {
        var hash = _hasher.Hash("Secret#123");

        Assert.True(_hasher.Verify("Secret#123", hash));
        Assert.False(_hasher.Verify("secret#123", hash));
    }

    [Fact]
    public void The_same_password_gives_different_hashes_because_of_the_salt() =>
        Assert.NotEqual(_hasher.Hash("Secret#123"), _hasher.Hash("Secret#123"));

    [Theory]
    [InlineData("")]
    [InlineData("not-a-hash")]
    [InlineData("v1.abc.def.ghi")]
    public void A_broken_hash_never_verifies(string hash) =>
        Assert.False(_hasher.Verify("Secret#123", hash));
}
