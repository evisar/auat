using auat.framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sample.tests
{
    public class CalculatorPage : PageObject, sample.tests.ICalculatorPage , IDisposable
    {
        public decimal Result
        {
            get
            {
                var value = GetValue("Result");
                return Math.Round(decimal.Parse(value), 2);
            }
        }

        private void Apply(decimal a, decimal b, string op)
        {
            foreach (var c in a.ToString())
            {
                this[c.ToString()].Click();
            }

            this[op].Click();

            foreach (var c in b.ToString())
            {
                this[c.ToString()].Click();
            }
            this["="].Click();
        }

        public void Add(decimal a, decimal b)
        {
            Apply(a, b, "+");
        }

        public void Substract(decimal a, decimal b)
        {
            Apply(a, b, "-");
        }

        public void Multiply(decimal a, decimal b)
        {
            Apply(a, b, "x");
        }

        public void Divide(decimal a, decimal b)
        {
            Apply(a, b, "/");
        }

        public void Dispose()
        {
            this.Driver.Close();
        }
    }
}
