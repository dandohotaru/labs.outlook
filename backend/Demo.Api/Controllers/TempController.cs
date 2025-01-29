using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Text.RegularExpressions;
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

  [HttpPost("summary")]
  public async Task<IActionResult> GetEmailSummary([FromBody] MessageSummaryRequest request, [FromHeader] string authorization)
  {
    var accessToken = ExtractAccessToken(authorization);
    if (string.IsNullOrWhiteSpace(accessToken))
    {
      return BadRequest("Access token is required.");
    }

    var graphClient = GetGraphClient(accessToken);

    // Fetch email message without needing `.Request()`
    var message = await graphClient.Me.Messages[request.MessageId]
        .GetAsync(requestConfig =>
        {
          requestConfig.QueryParameters.Select = new[] { "body", "bodyPreview" };
        });

    if (message == null || string.IsNullOrEmpty(message.Body?.Content))
    {
      return NotFound("Message not found or empty.");
    }

    // Use plain text if available, otherwise clean HTML
    string emailContent = message.Body.ContentType == BodyType.Text
        ? message.Body.Content
        : StripHtml(message.Body.Content);

    // Generate summary
    string summary = GenerateSummary(emailContent);

    return Ok(new { summary });
  }

  private string GenerateSummary(string emailBody)
  {
    // Split into sentences using regex
    var sentences = Regex.Split(emailBody, @"(?<=[.!?])\s+")
                         .Where(s => !string.IsNullOrWhiteSpace(s))
                         .Take(2);

    return string.Join(" ", sentences) + (sentences.Count() < 2 ? "" : "...");
  }

  private string StripHtml(string html)
  {
    var doc = new HtmlDocument();
    doc.LoadHtml(html);
    return doc.DocumentNode.InnerText.Trim();
  }

  public class MessageSummaryRequest
  {
    public string MessageId { get; set; }
  }
}
