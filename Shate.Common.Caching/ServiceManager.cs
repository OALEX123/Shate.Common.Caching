using System;
using System.Collections.Generic;
using System.Linq;

namespace Shate.Common.Caching
{
    /// <summary>
    /// Cache manager wrapper
    /// </summary>
    internal class ServiceManager : IServiceManager
    {
        private readonly IEnumerable<ICacheService> _services;

        /// <summary>
        /// Приватный конструктор
        /// </summary>
        private ServiceManager(IEnumerable<ICacheService> services)
        {
            if (services == null || !services.Any())
            {
                throw new ArgumentNullException("At least one cache service should be registered.");
            }

            _services = services;

            InitServices();
        }

        public static IServiceManager Register(IEnumerable<ICacheService> services)
        {
            return new ServiceManager(services);
        }

        public ICacheService GetCurrentCacheService()
        {
            var activeService = _services.FirstOrDefault(s => s.IsActive);

            var primaryService = _services.FirstOrDefault(s => s.Priority == CacheServicePriority.Primary);

            return primaryService;

            //foreach (var service in _services.Where(s => s.Priority == CacheServicePriority.Secondary).OrderBy(s => s.IsActive))
            //{
            //    if (service.IsAvailable)
            //    {
            //        if (!service.IsActive)
            //        {
            //            //CachingLogger.Log.SwitchingCachingProvider($"from {activeService.Name} to {service.Name}");
            //            service.IsActive = true;
            //        }

            //        return service;
            //    }
            //}

            //CachingLogger.Log.SwitchingCachingProviderFailed(activeService.Name);

            return CacheServiceFactory.GetDefaultEmptyCacheService();
        }

        private void InitServices()
        {
            SetPriorityAndActiveService();
        }

        private void SetPriorityAndActiveService()
        {
            if (_services.All(s => s.Priority != CacheServicePriority.Primary))
            {
                _services.FirstOrDefault().Priority = CacheServicePriority.Primary;
            }

            _services.FirstOrDefault(s => s.Priority == CacheServicePriority.Primary).IsActive = true;
        }
    }
}
