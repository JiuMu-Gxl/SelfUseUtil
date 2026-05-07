using System.Collections.Generic;
using WorkService.MockApi.Models.Mock;

namespace WorkService.MockApi.Helper
{
    public static class UserRepository
    {
        public static List<User> userList = new List<User>();

        static UserRepository()
        {
            for (int i = 1; i <= 10; i++)
            {
                userList.Add(new User
                {
                    Id = i,
                    Name = $"用户{i}",
                    Email = "145634874@qq.com"
                });
            }
        }
    }
}
