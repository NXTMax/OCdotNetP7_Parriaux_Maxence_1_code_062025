using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Data;
using P7CreateRestApi.Models;
using P7TestRestApi.Helpers;

namespace P7TestRestApi.Controllers;

public class CurveControllerTests
{
    private static (CurveController controller, LocalDbContext ctx) CreateSut()
    {
        var ctx = TestDbContextFactory.Create();
        return (new CurveController(ctx), ctx);
    }

    [Fact]
    public async Task GetCurvePoints_ReturnsAll()
    {
        var (sut, ctx) = CreateSut();
        ctx.CurvePoints.AddRange(
            new CurvePoint { Term = 1, Value = 1 },
            new CurvePoint { Term = 2, Value = 2 }
        );
        await ctx.SaveChangesAsync();

        var result = await sut.GetCurvePoints();

        var list = Assert.IsType<List<CurvePoint>>(result.Value);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetCurvePoint_ReturnsNotFound_WhenMissing()
    {
        var (sut, _) = CreateSut();

        var result = await sut.GetCurvePoint(123);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetCurvePoint_ReturnsEntity_WhenExists()
    {
        var (sut, ctx) = CreateSut();
        var added = ctx.CurvePoints.Add(new CurvePoint { Term = 1, Value = 3 });
        await ctx.SaveChangesAsync();

        var result = await sut.GetCurvePoint(added.Entity.Id);

        Assert.Equal(added.Entity.Id, Assert.IsType<CurvePoint>(result.Value).Id);
    }

    [Fact]
    public async Task PutCurvePoint_ReturnsBadRequest_OnIdMismatch()
    {
        var (sut, ctx) = CreateSut();
        var entity = new CurvePoint { Term = 1, Value = 1 };
        ctx.CurvePoints.Add(entity);
        await ctx.SaveChangesAsync();

        var result = await sut.PutCurvePoint(entity.Id + 1, entity);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task PutCurvePoint_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var (sut, ctx) = CreateSut();
        var entity = new CurvePoint { Term = 1, Value = 1 };
        ctx.CurvePoints.Add(entity);
        await ctx.SaveChangesAsync();

        sut.ModelState.AddModelError("Term", "The Term field is required.");

        var result = await sut.PutCurvePoint(entity.Id, entity);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PutCurvePoint_ReturnsNoContent_OnValidUpdate()
    {
        var (sut, ctx) = CreateSut();
        var entity = new CurvePoint { Term = 1, Value = 1 };
        ctx.CurvePoints.Add(entity);
        await ctx.SaveChangesAsync();

        entity.Value = 99;
        var result = await sut.PutCurvePoint(entity.Id, entity);

        Assert.IsType<NoContentResult>(result);
        var updated = await ctx.CurvePoints.AsNoTracking().SingleAsync(c => c.Id == entity.Id);
        Assert.Equal(99, updated.Value);
    }

    [Fact]
    public async Task PostCurvePoint_CreatesEntity()
    {
        var (sut, _) = CreateSut();
        var toCreate = new CurvePoint { Term = 1, Value = 2 };

        var result = await sut.PostCurvePoint(toCreate);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var value = Assert.IsType<CurvePoint>(created.Value);
        Assert.True(value.Id > 0);
    }

    [Fact]
    public async Task PostCurvePoint_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var (sut, _) = CreateSut();
        sut.ModelState.AddModelError("Value", "The Value field is required.");

        var result = await sut.PostCurvePoint(new CurvePoint { Term = 1 });

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<SerializableError>(bad.Value);
    }

    [Fact]
    public async Task DeleteCurvePoint_ReturnsNotFound_WhenMissing()
    {
        var (sut, _) = CreateSut();

        var result = await sut.DeleteCurvePoint(404);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteCurvePoint_ReturnsNoContent_WhenDeleted()
    {
        var (sut, ctx) = CreateSut();
        var added = ctx.CurvePoints.Add(new CurvePoint { Term = 1, Value = 1 });
        await ctx.SaveChangesAsync();

        var result = await sut.DeleteCurvePoint(added.Entity.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.False(await ctx.CurvePoints.AnyAsync(c => c.Id == added.Entity.Id));
    }
}
