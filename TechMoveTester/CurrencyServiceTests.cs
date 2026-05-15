using TechMove.Services;

namespace TechMoveTester;

public class CurrencyServiceTests
{
    
    [Fact]
    public void CalculateZar_ReturnsCorrectValue()
    {
        // Arrange
        var service = new CurrencyApiAdapterService();

        decimal usd = 100;
        decimal rate = 18.50m;

        // Act
        var result = service.CalculateZar(usd, rate);

        // Assert
        Assert.Equal(1850m, result);
    }
}
