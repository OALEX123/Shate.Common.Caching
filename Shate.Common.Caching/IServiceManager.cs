namespace Shate.Common.Caching
{
    public interface IServiceManager
    {
        /// <summary>
        /// Возвращает активный кеш сервис
        /// </summary>
        /// <returns></returns>
        ICacheService GetCurrentCacheService();
    }
}
