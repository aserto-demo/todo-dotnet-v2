using System.Threading.Tasks;
using Aserto.TodoApp.Domain.Services.Communication;
namespace Aserto.TodoApp.Domain.Services
{
    public interface IUserService
    {
        Task<GetUserResponse> Get(string sub);
        Task<GetUserResponse> GetByUserId(string objectId);
    }
}