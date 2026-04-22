using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using x99AssessmentByTva.Application.Accounts.Commands.UploadBalances;
using x99AssessmentByTva.Application.Accounts.Queries.GetAccounts;
using x99AssessmentByTva.Application.Accounts.Queries.GetAnnualSummary;
using x99AssessmentByTva.Application.Accounts.Queries.GetMonthlyBalances;
using x99AssessmentByTva.Application.Accounts.Queries.GetPeriods;
using x99AssessmentByTva.Domain.Constants;

namespace x99AssessmentByTva.Server.Controllers;

[ApiController]
[Route("api/balances")]
[Authorize]
public sealed class BalancesController(ISender sender) : ControllerBase
{
    [HttpGet("periods")]
    public async Task<IActionResult> GetPeriods()
    {
        return Ok(await sender.Send(new GetPeriodsQuery()));
    }

    [HttpGet("{year:int}/{month:int}")]
    public async Task<IActionResult> GetMonthlyBalances(int year, int month)
    {
        return Ok(await sender.Send(new GetMonthlyBalancesQuery(year, month)));
    }

    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts()
    {
        return Ok(await sender.Send(new GetAccountsQuery()));
    }

    [HttpGet("annual/{year:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetAnnualSummary(int year)
    {
        return Ok(await sender.Send(new GetAnnualSummaryQuery(year)));
    }

    [HttpPost("upload")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Upload(
        [FromForm] IFormFile file,
        [FromForm] int year)
    {
        if (file is null || file.Length == 0)
        {
            return Problem(
                detail: "File is required and must not be empty",
                statusCode: StatusCodes.Status400BadRequest);
        }

        await using var stream = file.OpenReadStream();
        var result = await sender.Send(new UploadBalancesCommand(stream, file.FileName, year));
        return Ok(result);
    }
}
