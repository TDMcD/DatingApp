using System;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{
    #region Public Methods
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
    {
        if (await IsUserExists(registerDTO.Username))
        {
            return BadRequest("Username is taken");
        }

        using var hmac = new System.Security.Cryptography.HMACSHA512();
        AppUser user = new()
        {
            Username = registerDTO.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserDTO
        {
            Username = user.Username,
            Token = tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
    {
        AppUser? user = await context.Users.FirstOrDefaultAsync(u => u.Username.ToLower().Equals(loginDTO.Username.ToLower()));

        if (user == null)
        {
            return Unauthorized("Invalid username");
        }

        using var hmac = new System.Security.Cryptography.HMACSHA512(user.PasswordSalt);
        byte[] computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginDTO.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i])
            {
                return Unauthorized("Invalid password");
            }
        }

        return new UserDTO
        {
            Username = user.Username,
            Token = tokenService.CreateToken(user)
        };
    }
    #endregion
    #region Private Methods
    private async Task<bool> IsUserExists(string username)
    {
        return await context.Users.AnyAsync(u => u.Username.ToLower().Equals(username.ToLower()));
    }
    #endregion
}
