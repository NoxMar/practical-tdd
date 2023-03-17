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
}