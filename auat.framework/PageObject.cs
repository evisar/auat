using auat.framework.Configuration;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace auat.framework
{
    public abstract class PageObject        
    {
        readonly PageObjectConfiguration _config;
        readonly IWebDriver _driver;

        public IWebDriver Driver
        {
            get { return _driver; }
        } 


        public PageObject()
        {            

            var testConfig = TestConfiguration.Instance;

            _config =
                (from p in testConfig.Pages.OfType<PageObjectConfiguration>()
                where p.Type == this.GetType()
                select p).FirstOrDefault();

            _driver = Activator.CreateInstance(testConfig.BrowserType) as IWebDriver;

            _driver.Url = _config.Url.ToString();
            _driver.Navigate();
        }

        private IWebElement FindElementBy(string query, FindBy findBy = FindBy.Name)
        {
            switch (findBy)
            {
                case FindBy.Id:
                    return _driver.FindElement(By.Id(query));
                case FindBy.Name:
                    return _driver.FindElement(By.Name(query));
                case FindBy.XPath:
                    return _driver.FindElement(By.XPath(query));
                case FindBy.Href:
                    return _driver.FindElement(By.XPath(string.Format("//*[@href='{0}']", query)));
                case FindBy.Value:
                    return _driver.FindElement(By.XPath(string.Format("//*[@value='{0}']", query)));
                default:
                    throw new NotSupportedException();
            }
        }
        public virtual IWebElement this[string elem]
        {
            get
            {
                var domElem =
                    (from e in _config.Elements.OfType<ElementConfiguration>()
                     where e.Name == elem
                     select e).First();

                return FindElementBy(domElem.Value, domElem.FindBy);
            }
        }

        public virtual string GetValue(string elem)
        {
            var result = this[elem];
            var value = result.GetAttribute("value");
            return value;
        }
    }
}
