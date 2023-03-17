namespace Uqs.Customer.Test.Unit;

public class ProfileServiceTests
{
    [Fact]
    public void ChangeUsername_NullUsername_ArgumentNullException()
    {
        // Arrange
        ProfileService sut = new();
        
        //Act
        var e = Record.Exception(() => sut.ChangeUsername(null!));
        
        // Assert
        var ex = Assert.IsType<ArgumentNullException>(e);
        Assert.Equal("username", ex.ParamName);
        Assert.StartsWith("Null", ex.Message);
    }

    [Theory]
    [InlineData("AnameOf8", true)]
    [InlineData("NameOfChar12", true)]
    [InlineData("AnameOfChar13", false)]
    [InlineData("NameOf7", false)]
    [InlineData("", false)]
    public void ChangeUsernameVariousLengthUsernames_ArgumentOutOfRangeExceptionIfInvalid(string username,
        bool isValid)
    {
        // Arrange
        var sut = new ProfileService();
        
        // Act
        var e = Record.Exception(() => sut.ChangeUsername(username));
        
        // Assert
        if (isValid)
        {
            Assert.Null(e);
            return;
        }

        var ex = Assert.IsType<ArgumentOutOfRangeException>(e);
        Assert.Equal("username", ex.ParamName);
        Assert.StartsWith("Length", ex.Message);
    }
}