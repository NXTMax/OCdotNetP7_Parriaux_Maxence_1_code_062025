using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Data;
using P7CreateRestApi.Models;
using P7TestRestApi.Helpers;

namespace P7TestRestApi.Controllers;

public class RatingControllerTests
{
    private static (RatingController controller, LocalDbContext ctx) CreateSut()
    {
        var ctx = TestDbContextFactory.Create();
        return (new RatingController(ctx), ctx);
    }

    [Fact]
    public async Task GetRatings_ReturnsAll()
    {
        var (sut, ctx) = CreateSut();
        ctx.Ratings.AddRange(
            new Rating { MoodysRating = "A" },
            new Rating { MoodysRating = "B" }
        );
        await ctx.SaveChangesAsync();

        var result = await sut.GetRatings();

        var list = Assert.IsType<List<Rating>>(result.Value);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetRating_ReturnsNotFound_WhenMissing()
    {
        var (sut, _) = CreateSut();

        var result = await sut.GetRating(99);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetRating_ReturnsEntity_WhenExists()
    {
        var (sut, ctx) = CreateSut();
        var added = ctx.Ratings.Add(new Rating { MoodysRating = "A" });
        await ctx.SaveChangesAsync();

        var result = await sut.GetRating(added.Entity.Id);

        Assert.Equal(added.Entity.Id, Assert.IsType<Rating>(result.Value).Id);
    }

    [Fact]
    public async Task PutRating_ReturnsBadRequest_OnIdMismatch()
    {
        var (sut, ctx) = CreateSut();
        var entity = new Rating { MoodysRating = "A" };
        ctx.Ratings.Add(entity);
        await ctx.SaveChangesAsync();

        var result = await sut.PutRating(entity.Id + 1, entity);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task PutRating_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var (sut, ctx) = CreateSut();
        var entity = new Rating { MoodysRating = "A" };
        ctx.Ratings.Add(entity);
        await ctx.SaveChangesAsync();

        sut.ModelState.AddModelError("MoodysRating", "The MoodysRating field is required.");

        var result = await sut.PutRating(entity.Id, entity);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PutRating_ReturnsNoContent_OnValidUpdate()
    {
        var (sut, ctx) = CreateSut();
        var entity = new Rating { MoodysRating = "A" };
        ctx.Ratings.Add(entity);
        await ctx.SaveChangesAsync();

        entity.MoodysRating = "AAA";
        var result = await sut.PutRating(entity.Id, entity);

        Assert.IsType<NoContentResult>(result);
        var updated = await ctx.Ratings.AsNoTracking().SingleAsync(r => r.Id == entity.Id);
        Assert.Equal("AAA", updated.MoodysRating);
    }

    [Fact]
    public async Task PostRating_CreatesEntity()
    {
        var (sut, _) = CreateSut();
        var toCreate = new Rating { MoodysRating = "A" };

        var result = await sut.PostRating(toCreate);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var value = Assert.IsType<Rating>(created.Value);
        Assert.True(value.Id > 0);
    }

    [Fact]
    public async Task PostRating_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var (sut, _) = CreateSut();
        sut.ModelState.AddModelError("MoodysRating", "The MoodysRating field is required.");

        var result = await sut.PostRating(new Rating());

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<SerializableError>(bad.Value);
    }

    [Fact]
    public async Task DeleteRating_ReturnsNotFound_WhenMissing()
    {
        var (sut, _) = CreateSut();

        var result = await sut.DeleteRating(404);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteRating_ReturnsNoContent_WhenDeleted()
    {
        var (sut, ctx) = CreateSut();
        var added = ctx.Ratings.Add(new Rating { MoodysRating = "B" });
        await ctx.SaveChangesAsync();

        var result = await sut.DeleteRating(added.Entity.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.False(await ctx.Ratings.AnyAsync(r => r.Id == added.Entity.Id));
    }
}
