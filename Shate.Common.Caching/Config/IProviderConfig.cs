namespace Shate.Common.Caching
{
    public interface ICacheProviderConfig
    {
        /// <summary>
        /// Кеширование включено
        /// </summary>
        bool IsCacheEnabled { get; set; }
    }
}
