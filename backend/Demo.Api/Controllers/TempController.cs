using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using System.Threading.Tasks;

[ApiController]
[Route("api/manual-graph")]
[AllowAnonymous]
public class GraphManualAuthController : ControllerBase
{
  [HttpGet("me")]
  public async Task<IActionResult> GetMyProfile([FromHeader] string authorization)
  {
    var accessToken = ExtractAccessToken(authorization);
    if (string.IsNullOrWhiteSpace(accessToken))
    {
      return BadRequest("Access token is required.");
    }

    var graphClient = GetGraphClient(accessToken);
    var user = await graphClient.Me.GetAsync();
    return Ok(user);
  }

  [HttpGet("emails")]
  public async Task<IActionResult> GetRecentEmails([FromHeader] string authorization)
  {
    var accessToken = ExtractAccessToken(authorization);
    if (string.IsNullOrWhiteSpace(accessToken))
    {
      return BadRequest("Access token is required.");
    }

    var graphClient = GetGraphClient(accessToken);
    var messages = await graphClient.Me.Messages.GetAsync(requestConfig =>
    {
      requestConfig.QueryParameters.Top = 10;
      requestConfig.QueryParameters.Select = new[] { "subject", "sender", "receivedDateTime" };
    });

    return Ok(messages?.Value);
  }

  [HttpGet("conversation/{id}")]
  public async Task<IActionResult> GetConversation([FromHeader] string authorization, string id)
  {
    var accessToken = ExtractAccessToken(authorization);
    if (string.IsNullOrWhiteSpace(accessToken))
    {
      return BadRequest("Access token is required.");
    }

    var graphClient = GetGraphClient(accessToken);
    var message = await graphClient.Me.Messages[id].GetAsync();
    return Ok(message);
  }

  private GraphServiceClient GetGraphClient(string accessToken)
  {
    var authProvider = new CustomAuthenticationProvider(accessToken);
    return new GraphServiceClient(authProvider);
  }

  private string ExtractAccessToken(string authorizationHeader)
  {
    if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
    {
      return string.Empty;
    }
    return authorizationHeader.Substring("Bearer ".Length).Trim();
  }
}
