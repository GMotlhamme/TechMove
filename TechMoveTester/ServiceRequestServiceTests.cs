using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Models.Domain;
using TechMove.Services;

namespace TechMoveTester;

public class ServiceRequestServiceTests
{
    [Fact]
    public async Task CanCreateRequestAsync_ReturnsFalse_WhenContractIsExpired()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TechMoveDbContext>()
            .UseInMemoryDatabase(databaseName: "ExpiredContractDb")
            .Options;

        using var context = new TechMoveDbContext(options);

        context.Contracts.Add(new Contract
        {
            Id = 1,
            Status = "Expired",
            ServiceLevel = "Premium",
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 12, 31)
        });

        await context.SaveChangesAsync();

        var service = new ServiceRequestService(context);

        // Act
        var result = await service.CanCreateRequestAsync(1);

        // Assert
        Assert.False(result);
    }
    [Fact]
    public async Task CanCreateRequestAsync_ReturnsFalse_WhenStatusIsEmpty()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TechMoveDbContext>()
            .UseInMemoryDatabase(databaseName: "ExpiredContractDb")
            .Options;

        using var context = new TechMoveDbContext(options);

        context.Contracts.Add(new Contract
        {
            Id = 2,
            Status = "",
            ServiceLevel = "Premium",
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 12, 31)
        });

        await context.SaveChangesAsync();

        var service = new ServiceRequestService(context);

        // Act
        var result = await service.CanCreateRequestAsync(1);

        // Assert
        Assert.False(result);
    }
}
