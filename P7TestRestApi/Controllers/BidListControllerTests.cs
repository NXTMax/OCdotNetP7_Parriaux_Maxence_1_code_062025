using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Data;
using P7CreateRestApi.Models;
using P7TestRestApi.Helpers;

namespace P7TestRestApi.Controllers;

public class BidListControllerTests
{
    private static (BidListController controller, LocalDbContext ctx) CreateSut(string? name = null)
    {
        var ctx = TestDbContextFactory.Create(name);
        return (new BidListController(ctx), ctx);
    }

    [Fact]
    public async Task GetBidLists_ReturnsAll()
    {
        var (sut, ctx) = CreateSut();
        ctx.BidLists.AddRange(
            new BidList { Account = "A", BidType = "T", BidQuantity = 10 },
            new BidList { Account = "B", BidType = "T", BidQuantity = 20 }
        );
        await ctx.SaveChangesAsync();

        var result = await sut.GetBidLists();

        var list = Assert.IsType<List<BidList>>(result.Value);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetBidList_ReturnsNotFound_WhenMissing()
    {
        var (sut, _) = CreateSut();

        var result = await sut.GetBidList(123);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetBidList_ReturnsEntity_WhenExists()
    {
        var (sut, ctx) = CreateSut();
        var entity = new BidList { Account = "A", BidType = "T", BidQuantity = 10 };
        ctx.BidLists.Add(entity);
        await ctx.SaveChangesAsync();

        var result = await sut.GetBidList(entity.BidListId);

        var value = Assert.IsType<BidList>(result.Value);
        Assert.Equal(entity.BidListId, value.BidListId);
    }

    [Fact]
    public async Task PutBidList_ReturnsBadRequest_OnIdMismatch()
    {
        var (sut, ctx) = CreateSut();
        var entity = new BidList { Account = "A", BidType = "T", BidQuantity = 1 };
        ctx.BidLists.Add(entity);
        await ctx.SaveChangesAsync();

        var result = await sut.PutBidList(entity.BidListId + 1, entity);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task PutBidList_ReturnsNoContent_OnValidUpdate()
    {
        var (sut, ctx) = CreateSut();
        var entity = new BidList { Account = "A", BidType = "T", BidQuantity = 1 };
        ctx.BidLists.Add(entity);
        await ctx.SaveChangesAsync();

        entity.BidQuantity = 42;
        var result = await sut.PutBidList(entity.BidListId, entity);

        Assert.IsType<NoContentResult>(result);
        var updated = await ctx.BidLists.AsNoTracking().SingleAsync(b => b.BidListId == entity.BidListId);
        Assert.Equal(42, updated.BidQuantity);
    }

    [Fact]
    public async Task PutBidList_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var (sut, ctx) = CreateSut();
        var entity = new BidList { Account = "A", BidType = "T" };
        ctx.BidLists.Add(entity);
        await ctx.SaveChangesAsync();

        sut.ModelState.AddModelError("Account", "The Account field is required.");

        var result = await sut.PutBidList(entity.BidListId, entity);

        // With [ApiController], the framework would return 400 before reaching action.
        // In unit test, we simulate it and expect BadRequest.
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PostBidList_CreatesEntity()
    {
        var (sut, ctx) = CreateSut();
        var toCreate = new BidList { Account = "A", BidType = "T", BidQuantity = 11 };

        var result = await sut.PostBidList(toCreate);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var value = Assert.IsType<BidList>(created.Value);
        Assert.True(value.BidListId > 0);
        Assert.Equal("A", value.Account);
    }

    [Fact]
    public async Task PostBidList_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var (sut, _) = CreateSut();
        sut.ModelState.AddModelError("BidType", "The BidType field is required.");

        var result = await sut.PostBidList(new BidList { Account = "A" });

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<SerializableError>(bad.Value);
    }

    [Fact]
    public async Task DeleteBidList_ReturnsNotFound_WhenMissing()
    {
        var (sut, _) = CreateSut();

        var result = await sut.DeleteBidList(404);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteBidList_ReturnsNoContent_WhenDeleted()
    {
        var (sut, ctx) = CreateSut();
        var entity = new BidList { Account = "A", BidType = "T", BidQuantity = 1 };
        ctx.BidLists.Add(entity);
        await ctx.SaveChangesAsync();

        var result = await sut.DeleteBidList(entity.BidListId);

        Assert.IsType<NoContentResult>(result);
        Assert.False(await ctx.BidLists.AnyAsync(b => b.BidListId == entity.BidListId));
    }
}
