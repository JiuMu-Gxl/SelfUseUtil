using Microsoft.EntityFrameworkCore;
using WorkService.MockApi.Models;
using WorkService.MockApi.Models.Iot;

namespace WorkService.MockApi.Repositorys
{
    public interface IUserRepository : IRepositoryBase<MqttUser>
    {
        Task<MqttUser?> GetByUsernameAsync(string username);
    }

    public class UserRepository : RepositoryBase<MqttUser>, IUserRepository
    {
        public UserRepository(MqttDbContext db) : base(db) { }

        public async Task<MqttUser?> GetByUsernameAsync(string username)
        {
            return await _set.FirstOrDefaultAsync(x => x.Username == username);
        }
    }
}
