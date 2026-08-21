using DndCompanion.Application.Abstractions.Identity;
using Infrastructure.Identity;

namespace DndCompanion.Infrastructure.Tests.Identity;

public class PasswordHasherTests
{
    private readonly IPasswordHasher _hasher = new PasswordHasher();

    [Fact]
    public void Verify_ReturnsTrue_ForCorrectPassword()
    {
        var hash = _hasher.Hash("my-password");
        Assert.True(_hasher.Verify("my-password", hash));
    }

    [Fact]
    public void Verify_ReturnsFalse_ForWrongPassword()
    {
        var hash = _hasher.Hash("my-password");
        Assert.False(_hasher.Verify("wrong-password", hash));
    }

    [Fact]
    public void Hash_ProducesDifferentOutput_ForSamePasswordOnEachCall()
    {
        var hash1 = _hasher.Hash("my-password");
        var hash2 = _hasher.Hash("my-password");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Verify_ReturnsTrue_ForBothHashes_WhenSamePasswordHashedTwice()
    {
        var hash1 = _hasher.Hash("my-password");
        var hash2 = _hasher.Hash("my-password");

        Assert.True(_hasher.Verify("my-password", hash1));
        Assert.True(_hasher.Verify("my-password", hash2));
    }

    [Fact]
    public void Verify_IsCaseSensitive()
    {
        var hash = _hasher.Hash("MyPassword");
        Assert.False(_hasher.Verify("mypassword", hash));
    }
}