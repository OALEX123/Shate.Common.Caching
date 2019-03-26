namespace Shate.Common.Caching
{
    public interface ICacheManager
    {
        /// <summary>
        /// Возвращает текущего кеш провайдера
        /// </summary>
        /// <returns></returns>
        IDataExchangeable GetCurrentCacheService();
    }
}
