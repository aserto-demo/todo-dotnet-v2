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
using static Aserto.Directory.Reader.V2.Reader;
using Grpc.Net.Client;
using Aserto.Directory.Reader.V2;
using Aserto.Directory.Common.V2;
using Aserto.TodoApp.Options;

namespace Aserto.TodoApp.Services
{
    public class UserService : IUserService
    {
        private readonly ReaderClient directoryReaderClient;
        private readonly DirectoryConfig opts;

        public UserService(IOptions<DirectoryConfig> config)
        {
            this.opts = config.Value;
            if (!opts.IsValid())
            {
                throw new Exception("Invalid config");
            }
            //TODO: define config
            var insecure = opts.Insecure;
            var directoryServiceURL = opts.ServiceUrl;

            var grpcChannelOptions = new GrpcChannelOptions { };

            if (insecure)
            {
                var httpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                };

                grpcChannelOptions = new GrpcChannelOptions { HttpHandler = httpHandler };
            }

            var channel = GrpcChannel.ForAddress(
                directoryServiceURL,
                grpcChannelOptions);

            this.directoryReaderClient = new ReaderClient(channel);
        }

        private async Task<GetUserResponse> GetUserBySub(string sub)
        {
            var getObjRequest = new GetObjectRequest();
            getObjRequest.Param = new ObjectIdentifier();
            getObjRequest.Param.Key = sub;
            getObjRequest.Param.Type = "user";

            var respUser = new User();
            try
            {
                //TODO: move this into config
                var tenantID = this.opts.TenantID;
                var dirApiKey = this.opts.APIKey;
                var metaData = new Grpc.Core.Metadata
                {
                    { "Aserto-Tenant-Id", $"{tenantID}" },
                    { "Authorization", $"basic {dirApiKey}" },
                };

                var getObjResponse = await this.directoryReaderClient.GetObjectAsync(getObjRequest, metaData);

                respUser.email = getObjResponse.Result.Properties.Fields["email"].StringValue;
                respUser.picture = getObjResponse.Result.Properties.Fields["picture"].StringValue;
                respUser.display_name = getObjResponse.Result.DisplayName;
                respUser.id = getObjResponse.Result.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", ex.Message);
                return new GetUserResponse($"An error occurred when getting user: {ex.Message}");
            }

            return new GetUserResponse(true, respUser);
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
    }
}