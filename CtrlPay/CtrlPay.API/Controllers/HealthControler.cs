using CtrlPay.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CtrlPay.API.Controllers;

[ApiController]
[Route("health")]
public class HealthControler : ControllerBase
{
    [HttpGet("api")]
    //GET : health/api
    public IActionResult Api() => Ok(new ReturnModel("H0", ReturnModelSeverityEnum.Ok));
}
