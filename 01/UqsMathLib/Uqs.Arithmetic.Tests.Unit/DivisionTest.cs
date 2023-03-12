namespace Uqs.Arithmetic.Tests.Unit;

public class DivisionTest
{
    [Fact]
    public void Divide_DivisibleIntegers_WholeNumber()
    {
        int dividend = 10;
        int divisor = 5;

        decimal expectedQuotient = 2;
        decimal actualQuotient = Division.Divide(dividend, divisor);
        Assert.Equal(expectedQuotient, actualQuotient);
    }

    [Fact]
    public void Divide_IndivisibleIntegers_DecimalNumber()
    {
        // Arrange
        int dividend = 10;
        int divisor = 4;
        decimal expectedQuotient = 2.5m;
        // Act
        decimal actualQuotient = Division.Divide(dividend, divisor);
        // Assert
        Assert.Equal(expectedQuotient, actualQuotient);
    }
}