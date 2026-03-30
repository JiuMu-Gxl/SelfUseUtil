using WorkService.MockApi.Helper;
using WorkService.MockApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkService.MockApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public List<User> userList => UserRepository.userList;

        [HttpGet]
        public List<User> GetAllUser() {
            return userList;
        }

        [HttpGet("{id}")]
        public User QueryUserById(int id)
        {
            var queryItem = userList.FirstOrDefault(u => u.Id == id);
            if (queryItem == null)
            {
                throw new NotSupportedException("未查询到用户");
            }
            return queryItem;
        }

        [HttpPost]
        public List<User> AddUser(User user)
        {
            var id = userList.Max(u => u.Id);
            userList.Add(new User {
                Id = ++id,
                Name = user.Name,
                Email = user.Email
            });
            return userList;
        }

        [HttpPut("{id}")]
        public bool UpdateUser(int id, User user)
        {
            var updateUser = QueryUserById(id);
            if (updateUser == null)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(user.Name))
            {
                updateUser.Name = user.Name;
            }
            if (!string.IsNullOrEmpty(user.Email))
            {
                updateUser.Email = user.Email;
            }
            return true;
        }

        [HttpDelete("{id}")]
        public bool DeleteUser(int id)
        {
            var user = QueryUserById(id);
            if (user == null)
            {
                return false;
            }
            userList.Remove(user);
            return true;
        }
    }
}
