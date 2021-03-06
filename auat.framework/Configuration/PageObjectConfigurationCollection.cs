﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auat.framework.Configuration
{
    public class PageObjectConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PageObjectConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as PageObjectConfiguration).Type;
        }

        public void Add(PageObjectConfiguration pageConfig)
        {
            base.BaseAdd(pageConfig);
        }
    }
}
