namespace Shate.Common.Caching
{
    public interface ICacheService : IDataExchangeable
    {
        /// <summary>
        /// Название сервиса
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Приоритет
        /// </summary>
        CacheServicePriority Priority { get; set; }

        /// <summary>
        /// Активный
        /// </summary>
        bool IsActive { get; set; }

        CacheServiceState State { get; }

        /// <summary>
        /// Провайдер кеша
        /// </summary>
        ICacheProvider Provider { get; }
    }
}
