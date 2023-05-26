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
            var metaData = new Grpc.Core.Metadata
            {
                { "Aserto-Tenant-Id", $"{this.opts.TenantID}" },
                { "Authorization", $"basic {this.opts.APIKey}" },
            };

            var getRelationRequest = new GetRelationRequest {
                Param = new RelationIdentifier {
                    Subject = new ObjectIdentifier { Type = "user" },
                    Object = new ObjectIdentifier { Type = "identity", Key = sub },
                    Relation = new RelationTypeIdentifier { ObjectType = "identity", Name = "identifier" },
                }
            };

            try
            {
                var getRelationResponse = await this.directoryReaderClient.GetRelationAsync(getRelationRequest, metaData);
                if (getRelationResponse.Results.Count == 0)
                {
                    return new GetUserResponse($"No user with identity: {sub}");
                }

                var getObjRequest = new GetObjectRequest { Param = getRelationResponse.Results[0].Subject };
                var getObjResponse = await this.directoryReaderClient.GetObjectAsync(getObjRequest, metaData);

                var user = new User
                {
                    id = getObjResponse.Result.Key,
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
            var metaData = new Grpc.Core.Metadata
            {
                { "Aserto-Tenant-Id", $"{this.opts.TenantID}" },
                { "Authorization", $"basic {this.opts.APIKey}" },
            };

            try
            {
                var objectIdentifier = new ObjectIdentifier { Type = "user", Key = userId };
                var getObjRequest = new GetObjectRequest { Param = objectIdentifier };
                var getObjResponse = await this.directoryReaderClient.GetObjectAsync(getObjRequest, metaData);

                var user = new User
                {
                    id = getObjResponse.Result.Key,
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
