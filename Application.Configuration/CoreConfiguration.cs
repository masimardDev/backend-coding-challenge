using System;
using System.Configuration;

namespace Application.Configuration
{
    public class CoreConfiguration : ConfigurationSection
    {
        public static CoreConfiguration Instance
        {
            get
            {
                return (CoreConfiguration)ConfigurationManager.GetSection("core");
            }
        }

        #region Internal Connection Strings

        [ConfigurationProperty("internalEntityConnectionStringName", IsRequired = true)]
        public string InternalEntityConnectionStringName
        {
            get
            {
                return (string)this["internalEntityConnectionStringName"];
            }
            set
            {
                this["internalEntityConnectionStringName"] = value;
            }
        }

        [ConfigurationProperty("internalSqlConnectionStringName", IsRequired = true)]
        public string InternalSqlConnectionStringName
        {
            get
            {
                return (string)this["internalSqlConnectionStringName"];
            }
            set
            {
                this["internalSqlConnectionStringName"] = value;
            }
        }

        public string InternalEntityConnectionString
        {
            get
            {
                return String.Format(ConfigurationManager.ConnectionStrings[InternalEntityConnectionStringName].ConnectionString, InternalSqlConnectionString);
            }
        }

        public string InternalSqlConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[InternalSqlConnectionStringName].ConnectionString;
            }
        }

        #endregion
    }
}
