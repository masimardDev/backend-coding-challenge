using System.Configuration;

namespace Application.Configuration
{
    public class IndexElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)base["name"];
            }
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get
            {
                return (string)base["path"];
            }
        }
    }
}
