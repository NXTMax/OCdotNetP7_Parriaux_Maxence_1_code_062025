using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Dtos;
using P7CreateRestApi.Models;

namespace P7TestRestApi.Controllers;

public class UserControllerTests
{
    private static Mock<UserManager<User>> CreateUserManagerMock(IList<User>? users = null)
    {
        users ??= new List<User>();
        var store = new Mock<IUserStore<User>>();
        var mgr = new Mock<UserManager<User>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!
        );

        mgr.SetupGet(m => m.Users).Returns(users.AsQueryable());

        mgr.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
           .ReturnsAsync((string id) => users.SingleOrDefault(u => u.Id == id));

        mgr.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
           .ReturnsAsync((User u, string _) =>
           {
               u.Id ??= Guid.NewGuid().ToString("N");
               users.Add(u);
               return IdentityResult.Success;
           });

        mgr.Setup(m => m.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
           .ReturnsAsync(IdentityResult.Success);

        mgr.Setup(m => m.UpdateAsync(It.IsAny<User>()))
           .ReturnsAsync(IdentityResult.Success);

        mgr.Setup(m => m.DeleteAsync(It.IsAny<User>()))
           .ReturnsAsync((User u) =>
           {
               users.Remove(u);
               return IdentityResult.Success;
           });

        return mgr;
    }

    [Fact]
    public void GetAll_ReturnsOk_WithUsers()
    {
        var users = new List<User>
        {
            new User { Id = "1", UserName = "u1", FullName = "U One", Email = "u1@x.com" },
            new User { Id = "2", UserName = "u2", FullName = "U Two", Email = "u2@x.com" }
        };
        var um = CreateUserManagerMock(users);
        var sut = new UserController(um.Object);

        var result = sut.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<UserDisplayDto>>(ok.Value);
        Assert.Equal(2, list.Count);
        Assert.Contains(list, u => u.Id == "1");
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        var um = CreateUserManagerMock();
        var sut = new UserController(um.Object);

        var result = await sut.GetById("nope");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User not found", notFound.Value);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenExists()
    {
        var backing = new List<User> { new User { Id = "abc", UserName = "john", FullName = "John Doe", Email = "j@d.com" } };
        var um = CreateUserManagerMock(backing);
        var sut = new UserController(um.Object);

        var result = await sut.GetById("abc");

        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<UserDisplayDto>(ok.Value);
        Assert.Equal("abc", dto.Id);
        Assert.Equal("john", dto.UserName);
    }

    [Fact]
    public async Task Register_ReturnsCreated_WhenSuccess()
    {
        var users = new List<User>();
        var um = CreateUserManagerMock(users);
        var sut = new UserController(um.Object);
        var dto = new UserRegistrationDto
        {
            Username = "alice",
            FullName = "Alice A",
            Email = "a@a.com",
            Password = "P@ssw0rd!"
        };

        var result = await sut.Register(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var user = Assert.IsType<User>(created.Value);
        Assert.Equal("alice", user.UserName);
        Assert.False(string.IsNullOrWhiteSpace(user.Id));
    }

    [Fact]
    public async Task Register_ReturnsProblem_WhenCreateFails()
    {
        var um = CreateUserManagerMock();
        um.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
          .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "bad" }));
        var sut = new UserController(um.Object);
        var dto = new UserRegistrationDto { Username = "x", FullName = "X", Email = "x@x.com", Password = "P@ssw0rd!" };

        var result = await sut.Register(dto);

        var problem = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, problem.StatusCode);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var um = CreateUserManagerMock();
        var sut = new UserController(um.Object);
        sut.ModelState.AddModelError("Email", "The Email field is required.");

        var result = await sut.Register(new UserRegistrationDto { Username = "u", FullName = "F", Password = "P@ssw0rd!" });

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(bad.Value);
    }

    [Fact]
    public async Task Register_RoleAssignmentFails_DeletesUser_AndReturnsProblem()
    {
        var users = new List<User>();
        var um = CreateUserManagerMock(users);
        um.Setup(m => m.AddToRoleAsync(It.IsAny<User>(), "User"))
          .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "role fail" }));
        var sut = new UserController(um.Object);
        var dto = new UserRegistrationDto { Username = "x", FullName = "X", Email = "x@x.com", Password = "P@ssw0rd!" };

        var result = await sut.Register(dto);

        var problem = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, problem.StatusCode);
        Assert.Empty(users); // deleted
        um.Verify(m => m.DeleteAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ReturnsNotFound_WhenMissing()
    {
        var um = CreateUserManagerMock();
        var sut = new UserController(um.Object);
        var dto = new UserRegistrationDto { Username = "x", FullName = "X", Email = "x@x.com", Password = "P@ssw0rd!" };

        var result = await sut.UpdateUser("nope", dto);

        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User not found", nf.Value);
    }

    [Fact]
    public async Task UpdateUser_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var list = new List<User> { new User { Id = "1", UserName = "old", FullName = "Old Name", Email = "o@o.com" } };
        var um = CreateUserManagerMock(list);
        var sut = new UserController(um.Object);
        sut.ModelState.AddModelError("Username", "The Username field is required.");

        var result = await sut.UpdateUser("1", new UserRegistrationDto { FullName = "F", Email = "e@e.com", Password = "P@ssw0rd!" });

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(bad.Value);
    }

    [Fact]
    public async Task UpdateUser_ReturnsNoContent_WhenSuccess()
    {
        var list = new List<User> { new User { Id = "1", UserName = "old", FullName = "Old Name", Email = "o@o.com" } };
        var um = CreateUserManagerMock(list);
        var sut = new UserController(um.Object);
        var dto = new UserRegistrationDto { Username = "new", FullName = "New Name", Email = "n@n.com", Password = "P@ssw0rd!" };

        var result = await sut.UpdateUser("1", dto);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal("new", list[0].UserName);
        Assert.Equal("New Name", list[0].FullName);
        Assert.Equal("n@n.com", list[0].Email);
    }

    [Fact]
    public async Task UpdateUser_ReturnsProblem_WhenUpdateFails()
    {
        var list = new List<User> { new User { Id = "1", UserName = "u", FullName = "F", Email = "e@e.com" } };
        var um = CreateUserManagerMock(list);
        um.Setup(m => m.UpdateAsync(It.IsAny<User>()))
          .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "update bad" }));
        var sut = new UserController(um.Object);
        var dto = new UserRegistrationDto { Username = "u2", FullName = "F2", Email = "e2@e.com", Password = "P@ssw0rd!" };

        var result = await sut.UpdateUser("1", dto);

        var problem = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, problem.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNotFound_WhenMissing()
    {
        var um = CreateUserManagerMock();
        var sut = new UserController(um.Object);

        var result = await sut.DeleteUser("nope");

        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User not found", nf.Value);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNoContent_WhenDeleted()
    {
        var list = new List<User> { new User { Id = "1", UserName = "u", FullName = "F", Email = "e@e.com" } };
        var um = CreateUserManagerMock(list);
        var sut = new UserController(um.Object);

        var result = await sut.DeleteUser("1");

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(list);
    }

    [Fact]
    public async Task DeleteUser_ReturnsProblem_WhenDeleteFails()
    {
        var list = new List<User> { new User { Id = "1", UserName = "u", FullName = "F", Email = "e@e.com" } };
        var um = CreateUserManagerMock(list);
        um.Setup(m => m.DeleteAsync(It.IsAny<User>()))
          .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "delete bad" }));
        var sut = new UserController(um.Object);

        var result = await sut.DeleteUser("1");

        var problem = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, problem.StatusCode);
        Assert.Single(list); // not removed
    }
}
