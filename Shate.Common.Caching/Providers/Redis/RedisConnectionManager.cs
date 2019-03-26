using System;
using System.Net;
//using Shate.Common.Caching.Logging;
using StackExchange.Redis;

namespace Shate.Common.Caching
{
    /// <summary>
    /// Redis connection wrapper
    /// </summary>
    public class RedisConnectionManager
    {
        private static readonly object Lock = new object();

        private static volatile Lazy<ConnectionMultiplexer> _lazyConnection;
        private static bool _isRedisInstanceAvaliable = true;

        public RedisConfig Config { get; }

        public RedisConnectionManager(RedisConfig config)
        {
            Config = config;
        }

        public IDatabase Database(int? db = null) => GetConnection().GetDatabase(db ?? -1);

        public IServer GetServer(EndPoint endPoint) => GetConnection().GetServer(endPoint);

        public EndPoint[] GetEndpoints() => GetConnection().GetEndPoints();

        public bool TryGetDatabase(out IDatabase database, int? db = null)
        {
            try
            {
                database = Database(db);
                return true;
            }
            catch (Exception)
            {
                database = null;
                return false;
            }
        }

        public void Dispose()
        {
            try
            {
                _lazyConnection?.Value.Dispose();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private ConnectionMultiplexer GetConnection()
        {
            if (IsConnectionEstablished())
            {
                return _lazyConnection.Value;
            }

            lock (Lock)
            {
                try
                {
                    EstablishConnection();
                }
                catch (Exception ex)
                {
                    //CachingLogger.Log.RedisNotAvailable(ex.GetBaseException().Message);
                    throw;
                }
            }

            return _lazyConnection.Value;
        }

        private bool IsConnectionEstablished()
        {
            return _lazyConnection != null && _lazyConnection.IsValueCreated && _lazyConnection.Value.IsConnected;
        }

        private void EstablishConnection()
        {
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(Config.ConnectionString));

            _lazyConnection.Value.ConnectionFailed += (sender, args) =>
            {
                //CachingLogger.Log.RedisConnectionFailed(args?.Exception?.Message);

                _isRedisInstanceAvaliable = false;
            };

            _lazyConnection.Value.ConnectionRestored += (sender, args) =>
            {
                //CachingLogger.Log.RedisConnectionRestored(args?.Exception?.Message);

                _isRedisInstanceAvaliable = true;
            };
        }
    }
}
