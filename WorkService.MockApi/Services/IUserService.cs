using WorkService.MockApi.Helpers;
using WorkService.MockApi.Models;
using WorkService.MockApi.Models.Iot;
using WorkService.MockApi.Repositorys;

namespace WorkService.MockApi.Services
{
    public interface IUserService
    {
        Task CreateUserAsync(string username, string password);
        Task ChangePasswordAsync(string username, string newPassword);
        Task DeleteUserAsync(string username);
        Task LoginAsync(string username, string password);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task CreateUserAsync(string username, string password)
        {
            if (await _userRepository.ExistsAsync(x => x.Username == username))
                throw new Exception("用户已存在");

            var salt = PasswordHelper.GenerateSalt();
            var hash = PasswordHelper.Md5WithSalt(password, salt);

            var user = new MqttUser
            {
                Username = username,
                PasswordHash = hash,
                Salt = salt,
                Created = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(string username, string newPassword)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                throw new Exception("用户不存在");

            var salt = PasswordHelper.GenerateSalt();
            var hash = PasswordHelper.Md5WithSalt(newPassword, salt);

            user.Salt = salt;
            user.PasswordHash = hash;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                throw new Exception("用户不存在");

            _userRepository.Delete(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user == null)
                throw new Exception("用户不存在");

            var hash = PasswordHelper.Md5WithSalt(password, user.Salt);
            if (hash != user.PasswordHash)
                throw new Exception("用户名或密码错误");
        }
    }
}
