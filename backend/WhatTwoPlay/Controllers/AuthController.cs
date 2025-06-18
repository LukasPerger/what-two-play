using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using WhatTwoPlay.Util;

namespace WhatTwoPlay.Controllers;

[Route("/auth")]
public sealed class AuthController(ILogger<AuthController> logger) : BaseController
{
    [HttpGet]
    [Route("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult SteamLogin()
    {
        var res = Challenge(new AuthenticationProperties() { RedirectUri = "/auth/steamId" }, "Steam");
        logger.LogInformation("Trying to login with steam");

        return res;
    }
}
