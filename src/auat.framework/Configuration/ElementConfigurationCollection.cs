using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auat.framework.Configuration
{
    public class ElementConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ElementConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ElementConfiguration).Name;
        }
    }
}
