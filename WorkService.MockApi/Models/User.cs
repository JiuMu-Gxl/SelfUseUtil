using System;

namespace WorkService.MockApi.Models
{
    public class User
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public String Email { get; set; }
    }
}
