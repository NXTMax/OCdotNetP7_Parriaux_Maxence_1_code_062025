using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using P7CreateRestApi.Interfaces;
using P7CreateRestApi.Models;
using P7CreateRestApi.Dtos;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController(
        UserManager<User> userManager,
        ITokenService tokenService,
        SignInManager<User> signInManager
        ) : ControllerBase
    {             
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto creds)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.FindByNameAsync(creds.Username);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var pwdCheck = await signInManager.CheckPasswordSignInAsync(user, creds.Password, false);
            if (!pwdCheck.Succeeded)
                return Unauthorized("Invalid credentials");

            var token = tokenService.CreateToken(user);
            return Ok(new { token });
        }            
    }
}