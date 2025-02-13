using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GraphController : ControllerBase
{
  private readonly GraphServiceClient client;

  public GraphController(GraphServiceClient client)
  {
    this.client = client;
  }

  [HttpGet("me")]
  public async Task<IActionResult> GetMyProfile()
  {
    var user = await client.Me.GetAsync();
    return Ok(user);
  }

  [HttpGet("emails")]
  public async Task<IActionResult> GetRecentEmails()
  {
    var messages = await client.Me.Messages
        .GetAsync(requestConfig =>
        {
          requestConfig.QueryParameters.Top = 10;
          requestConfig.QueryParameters.Select = new[] { "subject", "sender", "receivedDateTime" };
        });

    return Ok(messages?.Value);
  }

  [HttpGet("conversation/{id}")]
  public async Task<IActionResult> GetConversation(string id)
  {
    var message = await client.Me.Messages[id].GetAsync();
    return Ok(message);
  }
}
