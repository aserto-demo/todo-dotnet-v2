using System.Threading.Tasks;
using Aserto.TodoApp.Domain.Models;
using Aserto.TodoApp.Domain.Services;
using Aserto.TodoApp.Domain.Services.Communication;
using System;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Aserto.TodoApp.Configuration;
using System.Text;

namespace Aserto.TodoApp.Services
{
  public class UserService : IUserService
  {
    static readonly HttpClient client = new HttpClient();

    private readonly IOptions<AsertoConfig> _config;

    private readonly string authorizerServiceUrl;

    public UserService(IOptions<AsertoConfig> config)
    {
      _config = config;
      authorizerServiceUrl = _config.Value.ServiceUrl.Replace(":8443", "");
    }

    private async Task<GetUserResponse> GetUserById(string id)
    {
      try
      {
        var url = $"{authorizerServiceUrl}/api/v1/dir/users/{id}?fields.mask=id,display_name,picture,email";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("basic", _config.Value.AuthorizerApiKey);
        request.Headers.Add("aserto-tenant-id", _config.Value.TenantId);
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        UserResponse user = JsonSerializer.Deserialize<UserResponse>(responseBody);
        return new GetUserResponse(true, user.result);
      }
      catch (HttpRequestException e)
      {
        Console.WriteLine("\nException Caught!");
        Console.WriteLine("Message :{0} ", e.Message);
        return new GetUserResponse($"An error occurred when getting user: {e.Message}");
      }
    }

    private class UserIdentityPayload
    {
      public string identity { get; set; }
    }

    private async Task<GetUserIdentityResponse> GetUserIdentityBySub(string sub)
    {

      var url = $"{authorizerServiceUrl}/api/v1/dir/identities";
      var payload = new UserIdentityPayload { identity = sub };
      string jsonString = JsonSerializer.Serialize(payload);
      var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
      var request = new HttpRequestMessage(HttpMethod.Post, url);
      request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("basic", _config.Value.AuthorizerApiKey);
      request.Headers.Add("aserto-tenant-id", _config.Value.TenantId);
      request.Content = content;
      HttpResponseMessage response = await client.SendAsync(request);
      response.EnsureSuccessStatusCode();
      string responseBody = await response.Content.ReadAsStringAsync();
      UserIdentity identity = JsonSerializer.Deserialize<UserIdentity>(responseBody);
      return new GetUserIdentityResponse(true, identity.id);
    }

    public async Task<GetUserResponse> Get(string sub)
    {

      if (sub != "undefined")
      {
        var identity = await GetUserIdentityBySub(sub);
        return await GetUserById(identity.UserId);
      }
      else
      {
        return new GetUserResponse("No user identity provided");
      }

    }
  }
}