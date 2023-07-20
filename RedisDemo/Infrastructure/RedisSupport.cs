using CSRedis;
using Newtonsoft.Json;

namespace RedisDemo.Infrastructure
{
    public class RedisSupport
    {
        private readonly CSRedisClient _csRedisClient;

        public RedisSupport(IConfiguration configuration)
        {
            _csRedisClient = new CSRedisClient(configuration["Redis:Connection"])
            {
                CurrentSerialize = JsonConvert.SerializeObject,
                CurrentDeserialize = JsonConvert.DeserializeObject
            };
        }

        public CSRedisClient Get()
        {
            return _csRedisClient;
        }
    }
}
