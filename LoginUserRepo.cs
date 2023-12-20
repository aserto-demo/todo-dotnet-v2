using Grpc.Gateway.ProtocGenOpenapiv2.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Google.Rpc.Context.AttributeContext.Types;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using System;
using Aserto.TodoApp.Domain.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Aserto.TodoApp
{
    public class LoginUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public interface IUserRepository
    {
        Task<bool> Authenticate(string username, string password);
        Task<List<string>> GetUserNames();
    }

    public class UserRepository : IUserRepository
    {
        private List<LoginUser> _users = new List<LoginUser>
        {
            new LoginUser
            {
                Id = 1, Username = "rick@the-citadel.com", Password = "rick123"
            },
            new LoginUser
            {
                 Id = 1, Username = "morty@the-citadel.com", Password = "morty123"
            }          
        };
        public async Task<bool> Authenticate(string username, string password)
        {
            if (await Task.FromResult(_users.SingleOrDefault(x => x.Username == username && x.Password == password)) != null)
            {
                return true;
            }
            return false;
        }
        public async Task<List<string>> GetUserNames()
        {
            List<string> users = new List<string>();
            foreach (var user in _users)
            {
                users.Add(user.Username);
            }
            return await Task.FromResult(users);
        }






    }

}
