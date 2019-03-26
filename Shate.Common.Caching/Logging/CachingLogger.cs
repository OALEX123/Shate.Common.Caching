//using System;
//using Common.Logging;

//namespace Shate.Common.Caching.Logging
//{
//    internal class CachingLogger
//    {
//        private readonly ILog _logProvider = LogManager.GetLogger("CachingLogger.Providers");
//        private readonly ILog _logService = LogManager.GetLogger("CachingLogger.Services");

//        private readonly ILog _logProviderKeys = LogManager.GetLogger("CachingLogger.Providers.Keys");

//        private static readonly Lazy<CachingLogger> Instance =
//            new Lazy<CachingLogger>(() => new CachingLogger());

//        private CachingLogger()
//        { }

//        public static CachingLogger Log => Instance.Value;

//        public void LogKeyHistory(string key)
//        {
//            _logProviderKeys.Info(key);
//        }

//        public void LogKeyHistory(string[] keys)
//        {
//            LogKeyHistory(string.Join(",", keys));
//        }

//        public void LogSETKeyHistory(string key)
//        {
//            _logProviderKeys.Info($"SET {key}");
//        }

//        public void LogSETKeyHistory(string[] keys)
//        {
//            LogSETKeyHistory(string.Join(",", keys));
//        }

//        public void CacheManagerInstanceCreated(string defaultService)
//        {
//            _logService.InfoFormat($"Cache Manager instance created with default service {defaultService}.");
//        }

//        public void RedisFailed(string error)
//        {
//            _logProvider.Error($"Redis failed: {error}.");
//        }

//        public void RedisNotAvailable(string error)
//        {
//            _logProvider.Error($"Redis is not available: {error}.");
//        }

//        public void RedisConnectionRestored(string error)
//        {
//            _logProvider.Info($"Redis connection is restored: {error}.");
//        }

//        public void RedisConnectionFailed(string error)
//        {
//            _logProvider.Info($"Redis connection is failed: {error}.");
//        }

//        public void SwitchingCachingProvider(string message)
//        {
//            _logService.Info($"Switching caching provider {message}");
//        }

//        public void SwitchingCachingProviderFailed(string provider)
//        {
//            _logService.Info($"Switching caching provider {provider} failed. " +
//                             $"Any other available provider was not found or registered. " +
//                             $"Switching to default empty provider.");
//        }

//        public void ServiceProviderFailed(string serviceName, string mesage)
//        {
//            _logProvider.Error($"Providers for service {serviceName} failed with error: {mesage}.");
//        }

//        public void ServiceFailed(string serviceName, string error)
//        {
//            _logService.Error($"Service {serviceName} failed with error: {error}.");
//        }

//        public void ServiceTryDisable(string serviceName, int? period)
//        {
//            _logService.Info($"Trying disable service {serviceName} for {period} seconds.");
//        }

//        public void ServiceDisabled(string serviceName, int period)
//        {
//            _logService.Info($"Service {serviceName} disabled for {period} seconds.");
//        }

//        public void ServiceTryingRestore(string serviceName)
//        {
//            _logService.Info($"Trying restore {serviceName} as primary service.");
//        }

//        public void ServiceRestored(string serviceName)
//        {
//            _logService.Info($"Service {serviceName} restored.");
//        }
//    }
//}
