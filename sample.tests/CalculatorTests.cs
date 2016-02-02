using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sample.tests
{
    public class CalculatorTests
    {
        ICalculatorPage _calc;

        #region Configure
        [SetUp]
        public void Init()
        {
            _calc = new CalculatorPage();
        }

        [TearDown]
        public void Cleanup()
        {
            var disposable = (_calc as IDisposable);
            if(disposable!=null) disposable.Dispose();
        }
        #endregion

        [TestCase(3.1,4.2)]
        public void Calculator_Add_Test(decimal a, decimal b)
        {
            _calc.Add(a, b);
            Assert.AreEqual(7.3, _calc.Result);
        }

        [TestCase(10.10, 2.02)]
        public void Calculator_Substract_Test(decimal a, decimal b)
        {
            _calc.Substract(a, b);
            Assert.AreEqual(8.08, _calc.Result);
        }

        [TestCase(12.34, 35.56)]
        public void Calculator_Multiply_Test(decimal a, decimal b)
        {
            _calc.Multiply(a, b);
            Assert.AreEqual(438.81, _calc.Result);
        }

        [TestCase(111, 2.5)]
        public void Calculator_Divide_Test(decimal a, decimal b)
        {
            _calc.Divide(a, b);
            Assert.AreEqual(44.4, _calc.Result);
        }
    }
}
