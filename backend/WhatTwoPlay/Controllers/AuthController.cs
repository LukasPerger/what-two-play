using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using WhatTwoPlay.Util;

namespace WhatTwoPlay.Controllers;

[Route("auth")]
public sealed class AuthController(ILogger<AuthController> logger) : BaseController
{
    [HttpGet]
    [Route("steam")]
    [ProducesResponseType<ChallengeResult>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<ChallengeResult> SteamLogin()
    {
        var res = Challenge(new AuthenticationProperties { RedirectUri = "/" }, "Steam");
        logger.LogInformation("Trying to login with steam");
        return res;
    }

    [HttpGet]
    [Route("logout")]
    [ProducesResponseType<RedirectResult>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async ValueTask<ActionResult<RedirectResult>> Logout()
    {
        await HttpContext.SignOutAsync();
        logger.LogInformation("Trying to logout from steam");
        return Redirect("/");
    }
}
