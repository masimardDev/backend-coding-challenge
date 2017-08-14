using System;
using System.Configuration;

namespace Application.Configuration
{
    [ConfigurationCollection(typeof(IndexElement), AddItemName="index", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class IndexElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new IndexElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            IndexElement indexElement = element as IndexElement;
            if (element == null)
                throw new ArgumentNullException("element");

            return indexElement.Name;
        }

        public IndexElement this[int index]
        {
            get { return base.BaseGet(index) as IndexElement; }
        }

        public new IndexElement this[string key]
        {
            get { return base.BaseGet(key) as IndexElement; }
        }
    }
}
