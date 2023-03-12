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
}