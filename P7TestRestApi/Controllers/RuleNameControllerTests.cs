using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Data;
using P7CreateRestApi.Models;
using P7TestRestApi.Helpers;

namespace P7TestRestApi.Controllers;

public class RuleNameControllerTests
{
    private static (RuleNameController controller, LocalDbContext ctx) CreateSut(string? name = null)
    {
        var ctx = TestDbContextFactory.Create(name);
        return (new RuleNameController(ctx), ctx);
    }

    [Fact]
    public async Task GetRuleNames_ReturnsAll()
    {
        var (sut, ctx) = CreateSut();
        ctx.RuleNames.AddRange(
            new RuleName { Name = "R1" },
            new RuleName { Name = "R2" }
        );
        await ctx.SaveChangesAsync();

        var result = await sut.GetRuleNames();

        var list = Assert.IsType<List<RuleName>>(result.Value);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetRuleName_ReturnsNotFound_WhenMissing()
    {
        var (sut, _) = CreateSut();

        var result = await sut.GetRuleName(99);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetRuleName_ReturnsEntity_WhenExists()
    {
        var (sut, ctx) = CreateSut();
        var added = ctx.RuleNames.Add(new RuleName { Name = "Any" });
        await ctx.SaveChangesAsync();

        var result = await sut.GetRuleName(added.Entity.Id);

        var value = Assert.IsType<RuleName>(result.Value);
        Assert.Equal(added.Entity.Id, value.Id);
    }

    [Fact]
    public async Task PutRuleName_ReturnsBadRequest_OnIdMismatch()
    {
        var (sut, ctx) = CreateSut();
        var entity = new RuleName { Name = "X" };
        ctx.RuleNames.Add(entity);
        await ctx.SaveChangesAsync();

        var result = await sut.PutRuleName(entity.Id + 1, entity);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task PutRuleName_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var (sut, ctx) = CreateSut();
        var entity = new RuleName { Name = "Old" };
        ctx.RuleNames.Add(entity);
        await ctx.SaveChangesAsync();

        sut.ModelState.AddModelError("Name", "The Name field is required.");

        var result = await sut.PutRuleName(entity.Id, entity);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PutRuleName_ReturnsNoContent_OnValidUpdate()
    {
        var (sut, ctx) = CreateSut();
        var entity = new RuleName { Name = "Old" };
        ctx.RuleNames.Add(entity);
        await ctx.SaveChangesAsync();

        entity.Name = "New";
        var result = await sut.PutRuleName(entity.Id, entity);

        Assert.IsType<NoContentResult>(result);
        var updated = await ctx.RuleNames.AsNoTracking().SingleAsync(r => r.Id == entity.Id);
        Assert.Equal("New", updated.Name);
    }

    [Fact]
    public async Task PostRuleName_CreatesEntity()
    {
        var (sut, ctx) = CreateSut();
        var toCreate = new RuleName { Name = "R" };

        var result = await sut.PostRuleName(toCreate);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var value = Assert.IsType<RuleName>(created.Value);
        Assert.True(value.Id > 0);
        Assert.Equal("R", value.Name);
    }

    [Fact]
    public async Task PostRuleName_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var (sut, _) = CreateSut();
        sut.ModelState.AddModelError("Name", "The Name field is required.");

        var result = await sut.PostRuleName(new RuleName());

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<SerializableError>(bad.Value);
    }

    [Fact]
    public async Task DeleteRuleName_ReturnsNotFound_WhenMissing()
    {
        var (sut, _) = CreateSut();

        var result = await sut.DeleteRuleName(123);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteRuleName_ReturnsNoContent_WhenDeleted()
    {
        var (sut, ctx) = CreateSut();
        var added = ctx.RuleNames.Add(new RuleName { Name = "Any" });
        ctx.RuleNames.Add(new RuleName { Name = "X" });
        await ctx.SaveChangesAsync();

        var result = await sut.DeleteRuleName(added.Entity.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.False(await ctx.RuleNames.AnyAsync(r => r.Id == added.Entity.Id));
    }
}
