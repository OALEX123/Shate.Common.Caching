using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace Shate.Common.Caching
{
    public class CachingConfig : IConfigurationSectionHandler
    {
        /// <summary>
        /// Кеширование включено
        /// </summary>
        public bool IsCacheEnabled { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<ICacheProviderConfig> ProviderConfigs { get; private set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            try
            {
                return new CachingConfig
                {
                    IsCacheEnabled = XmlHelper.GetBool(section, "enabled"),
                    ProviderConfigs = new List<ICacheProviderConfig>
                    {
                        ReadRedisCfg(section),
                        ReadInMemoryCfg(section)
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception("CachingConfig section was not found in configuration file.");
            }
        }

        public static CachingConfig Read()
        {
            var cfg = ConfigurationManager.GetSection("caching") as CachingConfig;

            if (cfg == null)
            {
                throw new Exception("CachingConfig section was not found in configuration file.");
            }

            return cfg;
        }

        private ICacheProviderConfig ReadRedisCfg(XmlNode section)
        {
            var redisNode = section.SelectSingleNode("redis");

            if (redisNode == null)
            {
                return new RedisConfig { IsCacheEnabled = false };
            }

            return new RedisConfig
            {
                IsCacheEnabled = true,
                ConnectionString = XmlHelper.GetString(redisNode, "connectionString"),
                ScanPageSize = XmlHelper.GetInt(redisNode, "scanPageSize")
            };
        }

        private ICacheProviderConfig ReadInMemoryCfg(XmlNode section)
        {
            var memoryNode = section.SelectSingleNode("memory");

            return new InMemoryConfig
            {
                IsCacheEnabled = memoryNode != null && XmlHelper.GetBool(memoryNode, "enabled")
            };
        }
    }
}
