using System.Threading.Tasks;
using Aserto.TodoApp.Domain.Models;
using Aserto.TodoApp.Domain.Services;
using Aserto.TodoApp.Domain.Services.Communication;
using System;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Aserto.AspNetCore.Middleware.Clients.Directory.V3;
using Aserto.AspNetCore.Middleware.Options;
using Aserto.TodoApp.Options;

namespace Aserto.TodoApp.Services
{
    public class UserService : IUserService
    {
        private readonly Aserto.AspNetCore.Middleware.Clients.Directory.V3.Directory directoryClient;
        private readonly DirectoryConfig opts;

        public UserService(IOptions<DirectoryConfig> config)
        {
            this.opts = config.Value;
            if (!opts.IsValid())
            {
                throw new Exception("Invalid config. Service url can not be empty");
            }

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Debug)
                .AddConsole();
            });

            var options = new AsertoDirectoryOptions(opts.ServiceUrl, opts.APIKey, opts.TenantID, opts.Insecure);
            directoryClient = new Aserto.AspNetCore.Middleware.Clients.Directory.V3.Directory(options, loggerFactory);
        }

        private async Task<GetUserResponse> GetUserBySub(string sub)
        {
            try
            {
                var getRelationResponse = await directoryClient.GetRelationAsync(subjectType: "user", objType: "identity", objId: sub, relationName: "identifier");

                var objType = getRelationResponse.Result.SubjectType;
                var objKey = getRelationResponse.Result.SubjectId;
                var getObjResponse = await directoryClient.GetObjectAsync(objType, objKey);

                var user = new User
                {
                    id = getObjResponse.Result.Id,
                    email = getObjResponse.Result.Properties.Fields["email"].StringValue,
                    picture = getObjResponse.Result.Properties.Fields["picture"].StringValue,
                    display_name = getObjResponse.Result.DisplayName,
                };
                return new GetUserResponse(true, user);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", ex.Message);
                return new GetUserResponse($"An error occurred when getting user: {ex.Message}");
            }
        }

        private async Task<GetUserResponse> GetById(string userId)
        {
            try
            {
                var getObjResponse = await directoryClient.GetObjectAsync("user", userId);

                var user = new User
                {
                    id = getObjResponse.Result.Id,
                    email = getObjResponse.Result.Properties.Fields["email"].StringValue,
                    picture = getObjResponse.Result.Properties.Fields["picture"].StringValue,
                    display_name = getObjResponse.Result.DisplayName,
                };
                return new GetUserResponse(true, user);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", ex.Message);
                return new GetUserResponse($"An error occurred when getting user: {ex.Message}");
            }
        }

        public async Task<GetUserResponse> Get(string sub)
        {
            if (sub != "undefined")
            {
                return await GetUserBySub(sub);
            }
            else
            {
                return new GetUserResponse("No user identity provided");
            }
        }

        public async Task<GetUserResponse> GetByUserId(string objectId)
        {
            if (objectId != "undefined")
            {
                return await GetById(objectId);
            }
            else
            {
                return new GetUserResponse("No user identity provided");
            }
        }
    }
}
