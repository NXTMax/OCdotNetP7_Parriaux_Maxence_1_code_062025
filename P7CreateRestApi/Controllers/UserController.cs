using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using P7CreateRestApi.Models;
using P7CreateRestApi.Dtos;
using P7CreateRestApi.Mappers;
using NuGet.Protocol;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserController(UserManager<User> userManager) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = userManager.Users.ToUserDisplayDtoEnumerable().ToList();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");
            return Ok(user.ToUserDisplayDto());
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                UserName = registrationDto.Username,
                FullName = registrationDto.FullName!,
                Email = registrationDto.Email
            };

            try
            {
                var userCreation = await userManager.CreateAsync(user, registrationDto.Password!);
                if (!userCreation.Succeeded)
                {
                    return Problem(
                        title: "User registration failed",
                        detail: string.Join("; ", userCreation.Errors.Select(e => e.Description))
                    );
                }

                var roleAssignment = await userManager.AddToRoleAsync(user, "User");
                if (!roleAssignment.Succeeded)
                {
                    await userManager.DeleteAsync(user);
                    return Problem(
                        title: "User registration failed",
                        detail: string.Join("; ",
                            userCreation.Errors.Select(e => e.Description)
                                .Concat(roleAssignment.Errors.Select(e => e.Description)))
                    );
                }

                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.ToJson());
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserRegistrationDto registrationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            user.UserName = registrationDto.Username;
            user.FullName = registrationDto.FullName!;
            user.Email = registrationDto.Email;

            var userUpdate = await userManager.UpdateAsync(user);
            if (!userUpdate.Succeeded)
                return Problem(
                    title: "User update failed",
                    detail: string.Join("; ", userUpdate.Errors.Select(e => e.Description))
                );
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return Problem(
                    title: "User deletion failed",
                    detail: string.Join("; ", result.Errors.Select(e => e.Description))
                );

            return NoContent();
        }
    }
}