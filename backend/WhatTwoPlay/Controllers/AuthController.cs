using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using WhatTwoPlay.Util;

namespace WhatTwoPlay.Controllers;

[Route("/api/auth")]
public sealed class AuthController(ILogger<AuthController> logger) : BaseController
{
    [HttpGet]
    [Route("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult SteamLogin() =>
        Challenge(new AuthenticationProperties
        {
            RedirectUri = "/api/auth/me"
        }, "Steam");

    [HttpGet("me")]
    public IActionResult Me()
    {
        logger.LogInformation("Me");
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Unauthorized("Not logged in");
        }

        var steamId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var name = User.Identity?.Name;

        return Ok(new
        {
            steamId,
            name
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        return Ok("Logged out");
    }
}
