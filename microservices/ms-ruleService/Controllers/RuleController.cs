using Microsoft.AspNetCore.Mvc;
using ruleService.Interfaces;
using ruleService.Models;
using ruleService.Utilities;
using System.Net.Http.Headers;
using System.Text.Json;
using Newtonsoft.Json;

namespace ruleService.Controllers;

[ApiController]
public class RuleController : ControllerBase
{
    private readonly IRuleService _ruleService;
    private readonly IParentService _parentService;
    private readonly ILogger<RuleController> _logger;
    private readonly IConfigurationSettings _config;

    public RuleController(ILogger<RuleController> logger, IRuleService ruleService, IParentService parentService, IConfigurationSettings config)
    {
        _logger = logger;
        _ruleService = ruleService;
        _parentService = parentService;
        _config = config;
    }

    /// <summary>Retrieves Sigma rule.</summary>
    /// <param name="id">Rule's ID.</param>
    /// <returns>An action result containing Sigma rule with specified ID.</returns>
    [HttpGet("/getRule")]
    public ActionResult<string> GetRule(string id)
    {
        try
        {
            return Ok(_parentService.GetParentRule(id).Value);
        }
        catch (System.Exception ex)
        {
            _logger.LogError($"occured while getting Parent Rule: {ex.Message}");
            return StatusCode(StatusCodes.Status404NotFound);
        }
    }

    /// <summary>Retrieves custom tree structure.</summary>
    /// <returns>An action result containing a tree structure.</returns>
    [HttpGet("/getCustomTree")]
    public ActionResult<string> GetCustomTree()
    {
        try
        {
            var header = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            return Ok(_ruleService.GetCustomTree(header.Parameter));
        }
        catch (System.Exception ex)
        {
            _logger.LogError($"occured while getting Tree: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
