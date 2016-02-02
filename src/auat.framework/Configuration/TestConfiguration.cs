using auat.framework.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auat.framework.Configuration
{
    public class TestConfiguration : ConfigurationSection
    {
        #region Singleton Instance

        readonly static string SECTION_NAME = "tests";

        static readonly object _sync = new object();
        static volatile TestConfiguration _instance;

        public static TestConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_sync)
                    {
                        if (_instance == null)
                        {
                            _instance = (TestConfiguration)ConfigurationManager.GetSection(SECTION_NAME);
                        }
                    }
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        #endregion

        [ConfigurationProperty("browser")]
        [TypeConverter(typeof(TypeNameConverter))]
        public Type BrowserType
        {
            get
            {
                return (Type)this["browser"];
            }
            set
            {
                this["browser"] = value;
            }
        }

        [ConfigurationProperty("url")]
        public Uri Url
        {
            get
            {
                return (Uri)this["url"];
            }
            set
            {
                this["url"] = value;
            }
        }

        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(PageObjectConfiguration), AddItemName = "page")]
        public PageObjectConfigurationCollection Pages
        {
            get
            {
                return (PageObjectConfigurationCollection)this[""];
            }
            set
            {
                this[""] = value;
            }
        }
    }
}
