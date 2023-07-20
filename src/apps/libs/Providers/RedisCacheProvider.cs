using libs.contracts;
using Microsoft.Extensions.Configuration;
using Serilog;
using StackExchange.Redis;

namespace libs.Providers
{
    /// <summary>
    /// Redis implementation of a cache
    /// </summary>
    public class RedisCacheProvider : ICache
    {
        private readonly IDatabase _db;
        private readonly double _maxPingTimeSeconds;

        public RedisCacheProvider(IConfiguration config)
        {
            var servers = config.GetValue<string>("REDIS_SERVER", "localhost");
            Log.Information($"connecting to redis at {servers}");
            
            _maxPingTimeSeconds = config.GetValue("MAX_REDIS_PING_TIME_SECONDS", 30.0);

            var options = ConfigurationOptions.Parse(servers);
            options.Password = config.GetValue<string>("REDIS_PWD","");
            options.Ssl = config.GetValue<bool>("REDIS_SSL",false);
            options.AbortOnConnectFail = false;
            options.ConnectRetry = 10;
            
            var redis = ConnectionMultiplexer.Connect(options);
            _db = redis.GetDatabase(1);
        }
         
        public async Task<bool> Ping()
        {
            try
            {
                var pingTime = await _db.PingAsync();
                return pingTime.TotalSeconds < _maxPingTimeSeconds;
            }
            catch(Exception ex)
            {
                Log.Error($"Failed while pinging redis.{ex.Message}.{ex.StackTrace}", ex);
            }

            return false;

        }
         
        public string? GetKey(string key)
        {
            var value = _db.StringGet(new RedisKey(key));
            if (value.HasValue)
            {
                return value.ToString();
            }

            return null;
        }
         
        public Task SetKey(string key, string value, TimeSpan expiry)
        {
            return _db.StringSetAsync(new RedisKey(key), new RedisValue(value), expiry);
        }

    }
}
