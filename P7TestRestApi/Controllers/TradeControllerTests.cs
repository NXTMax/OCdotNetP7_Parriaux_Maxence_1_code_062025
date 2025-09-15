using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Data;
using P7CreateRestApi.Models;
using P7TestRestApi.Helpers;

namespace P7TestRestApi.Controllers;

public class TradeControllerTests
{
    private static (TradeController controller, LocalDbContext ctx) CreateSut()
    {
        var ctx = TestDbContextFactory.Create();
        return (new TradeController(ctx), ctx);
    }

    [Fact]
    public async Task GetTrades_ReturnsAll()
    {
        var (sut, ctx) = CreateSut();
        ctx.Trades.AddRange(
            new Trade { Account = "A", Type = "T", BuyQuantity = 1 },
            new Trade { Account = "B", Type = "T", BuyQuantity = 2 }
        );
        await ctx.SaveChangesAsync();

        var result = await sut.GetTrades();

        var list = Assert.IsType<List<Trade>>(result.Value);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetTrade_ReturnsNotFound_WhenMissing()
    {
        var (sut, _) = CreateSut();

        var result = await sut.GetTrade(9);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetTrade_ReturnsEntity_WhenExists()
    {
        var (sut, ctx) = CreateSut();
        var added = ctx.Trades.Add(new Trade { Account = "A", Type = "T", BuyQuantity = 1 });
        await ctx.SaveChangesAsync();

        var result = await sut.GetTrade(added.Entity.TradeId);

        Assert.Equal(added.Entity.TradeId, Assert.IsType<Trade>(result.Value).TradeId);
    }

    [Fact]
    public async Task PutTrade_ReturnsBadRequest_OnIdMismatch()
    {
        var (sut, ctx) = CreateSut();
        var entity = new Trade { Account = "A", Type = "T", BuyQuantity = 1 };
        ctx.Trades.Add(entity);
        await ctx.SaveChangesAsync();

        var result = await sut.PutTrade(entity.TradeId + 1, entity);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task PutTrade_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var (sut, ctx) = CreateSut();
        var entity = new Trade { Account = "A", Type = "T", BuyQuantity = 1 };
        ctx.Trades.Add(entity);
        await ctx.SaveChangesAsync();

        sut.ModelState.AddModelError("Account", "The Account field is required.");

        var result = await sut.PutTrade(entity.TradeId, entity);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PutTrade_ReturnsNoContent_OnValidUpdate()
    {
        var (sut, ctx) = CreateSut();
        var entity = new Trade { Account = "A", Type = "T", BuyQuantity = 1 };
        ctx.Trades.Add(entity);
        await ctx.SaveChangesAsync();

        entity.BuyQuantity = 99;
        var result = await sut.PutTrade(entity.TradeId, entity);

        Assert.IsType<NoContentResult>(result);
        var updated = await ctx.Trades.AsNoTracking().SingleAsync(t => t.TradeId == entity.TradeId);
        Assert.Equal(99, updated.BuyQuantity);
    }

    [Fact]
    public async Task PostTrade_CreatesEntity()
    {
        var (sut, _) = CreateSut();
        var toCreate = new Trade { Account = "A", Type = "T", BuyQuantity = 5 };

        var result = await sut.PostTrade(toCreate);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var value = Assert.IsType<Trade>(created.Value);
        Assert.True(value.TradeId > 0);
    }

    [Fact]
    public async Task PostTrade_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var (sut, _) = CreateSut();
        sut.ModelState.AddModelError("Type", "The Type field is required.");

        var result = await sut.PostTrade(new Trade { Account = "A" });

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<SerializableError>(bad.Value);
    }

    [Fact]
    public async Task DeleteTrade_ReturnsNotFound_WhenMissing()
    {
        var (sut, _) = CreateSut();

        var result = await sut.DeleteTrade(404);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteTrade_ReturnsNoContent_WhenDeleted()
    {
        var (sut, ctx) = CreateSut();
        var added = ctx.Trades.Add(new Trade { Account = "A", Type = "T", BuyQuantity = 1 });
        await ctx.SaveChangesAsync();

        var result = await sut.DeleteTrade(added.Entity.TradeId);

        Assert.IsType<NoContentResult>(result);
        Assert.False(await ctx.Trades.AnyAsync(t => t.TradeId == added.Entity.TradeId));
    }
}
