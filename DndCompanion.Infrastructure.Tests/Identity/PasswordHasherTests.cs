using Infrastructure.Identity;

namespace DndCompanion.Infrastructure.Tests.Identity;

public class PasswordHasherTests
{
    [Fact]
    public void Verify_ReturnsTrue_ForCorrectPassword()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.Hash("my-password");
        Assert.True(hasher.Verify("my-password", hash));
    }

    [Fact]
    public void Verify_ReturnsFalse_ForWrongPassword()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.Hash("my-password");
        Assert.False(hasher.Verify("wrong-password", hash));
    }
}