# auat

Automate, Automate, Automate: UI Testing

Adding new features is easy. Making sure that you have not broken old features is very hard. We developers usually tend to heavily rely on others for this assurance, hence the word Quality Assurance/QA.  QA job is hard, probably one of the hardest in this business but never appreciated enough. Why? Well imagine adding new features was analogy to “brining a new cat to life”, so any time you bring a new cat to life, your risk killing and hurting several cats somewhere else. Would you risk?  Harsh world you say?! Harsh world it is, the world of software – if it weren’t for QA. QA is the social welfare and healthcare system of the software industry (cats’ ecosystem), without it our world would be full of dead or orphaned cats.
But, automated UI testing is fragile? Yes, it is very fragile.
And in this article I will propose a combination of tools and methods to make the maintainable. Let’s go.
Let’s take a simple web calculator and create UI test automation cases for it, with these principles.
Step 1: Take a feature, and extract an interface out of it, not a User Interface, but a software interface, a contract.
 
So our calculator page supports typing decimal numbers, and applying operations on those numbers, and one of those operations is [+], adding 2 numbers and checking out the result on the top box.
Let’s add couple of more operations:
    public interface ICalculatorPage
    {
        void Add(decimal a, decimal b);
        void Substract(decimal a, decimal b);
        void Multiply(decimal a, decimal b);
        void Divide(decimal a, decimal b);        
        decimal Result { get; }        
    }
Now we have abstracted away the calculator page, the fact that it is a web application, which contains buttons and URL and a web browser involved, that we need to communicate with it. Let’s just go and write test cases for this interface.
public class CalculatorTests
    {
        ICalculatorPage _calc = null;

        [TestCase(3.1,4.2, ExpectedResult=7.3)]
        public decimal Calculator_Add_Test(decimal a, decimal b)
        {
            _calc.Add(a, b);
            return _calc.Result;
        }

        [TestCase(10.10, 2.02, ExpectedResult=8.08)]
        public decimal Calculator_Substract_Test(decimal a, decimal b)
        {
            _calc.Substract(a, b);
            return _calc.Result;
        }

        [TestCase(12.34, 35.56, ExpectedResult=438.81)]
        public decimal Calculator_Multiply_Test(decimal a, decimal b)
        {
            _calc.Multiply(a, b);
            return _calc.Result;
        }

        [TestCase(111, 2.5)]
        public decimal Calculator_Divide_Test(decimal a, decimal b)
        {
            _calc.Divide(a, b);
            return _calc.Result;
        }
    }
I am using NUnit here, because it provides much more flexibility in terms of creating more data driven and configurable test cases. Here I just created 4 test cases, if I wanted to test more scenarios, I just need to add them through the TestCase attribute.
If we run these tests now, obviously they are all going to fail.
Implement the ICalculatorPage!
Before implementing the interface, we need a way to communicate with the browser and the given page. So we are going to use Selenium Web driver for browser communication, and we are going to use DOM/Key mapping for interacting with individual controls on the page.
We create a custom configuration section to describe the test pages, their location, and controls within that page and how we can find them on the DOM.
 

In this particular case, we are kind of lucky, because the page elements with which we interact in the page are simply to find, usually it’s not THIS simple. Usually you need to spend significant time to determine what is a unique DOM query that represent an element, think jQuery.
So the individual multiply [x] button, is represented by the following HTML in the DOM.
<input type="button" class="calc_btn" value="x" onclick="javascript:f_calc('calc','*');">
Going through all the required elements (this is necessary, we don’t have to map everything, only the parts we test). And put them in our custom configuration section. In the section we define the name of the element that we are going to address in our Page Objects, and how are we going to search the DOM for them: by and value.
By, represents an enum with DOM search methods:
public enum FindBy { Name, Id, Value, Href, XPath };
And value simply is a query, think of it as if you are making a jQuery search.
<tests browser="OpenQA.Selenium.Chrome.ChromeDriver, WebDriver">
    <page type="sample.tests.CalculatorPage, sample.tests" url="http://localhost:32150/calculator.html">
      <element name="1" by="Value" value="1" />
      <element name="2" by="Value" value="2" />
      <element name="3" by="Value" value="3" />
      <element name="4" by="Value" value="4" />
      <element name="5" by="Value" value="5" />
      <element name="6" by="Value" value="6" />
      <element name="7" by="Value" value="7" />
      <element name="8" by="Value" value="8" />
      <element name="9" by="Value" value="9" />
      <element name="0" by="Value" value="0" />
      <element name="+" by="Value" value="+" />
      <element name="-" by="Value" value="-" />
      <element name="x" by="Value" value="x" />
      <element name="/" by="Value" value="/" />
      <element name="." by="Value" value="," />
      <element name="=" by="Value" value="=" />
      <element name="Back" by="Value" value="&#8592;" />
      <element name="Result" by="Id" value="calc_result" />
      <element name="CE" by="Value" value="CE" />
    </page>
  </tests>
This step is crucial! If we don’t do this step, we risk our tests being fragile because the risk that a control might disappear is low, but that it might change attributes, properties, identifiers is actually high, and this is what makes test automation so fragile, and hence the point of controlling this fragility.
Now let’s create an abstract PageObject. What is a PageObject?
It’s an abstraction of the feature under test to make interaction with it much more natural, decoupled and maintainable.
http://martinfowler.com/bliki/PageObject.html
So the Calculator Page Object will be a class inheriting from a base abstract PageObject and implementing the ICalculatorPage.  
Our base PageObject will provide features to search controls by name and query them in the configuration and decouple the page from DEOM queries.
Here what a part of the abstract page object looks like:
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

And then we inherit by creating an actual Calculator Page:
public class CalculatorPage : PageObject, sample.tests.ICalculatorPage
    {
        public decimal Result
        {
            get
            {
//this will search for the Result element
                //and get it's value attribute
                var value = GetValue("Result");
                return Math.Round(decimal.Parse(value), 2);
            }
        }

        public void Add(decimal a, decimal b)
        {
      //this will simulate pressing in e.g. 3.141592563 buttons sequencially in the  page
            Action<decimal> pressButtons = (buttons) =>
                {
       //transform the number to searies of chars, and find and press the button with the char name
                    //see configuration
                    foreach (var c in buttons.ToString())
                        this[c.ToString()].Click();
                };

            pressButtons(a);

            this["+"].Click();

            pressButtons(b);
        }
    }

Automated tests framework design and approach. (The code is much smaller than the diagram)
 
We can run the tests. (you need to make sure that you have downloaded Selenium web driver nugget package, NUnit and NUnit adapter, or NUnit VS extension). Prepared configuration and make sure that your code is compiling and building successfully. If it does, 4 test cases should show up on the test explorer. 
 
If you run them, a new browser should be spawned and apply the test cases.
If you want to speed up test execution and don’t want to SEE the test executing, you can use a “headless“ browser like PhantomJS. It renders everything in memory and the whole execution works under the hood. Selenium supports PhantomJS too. 
To use PhantomJS, you need to download PhantomJS and the Selenium driver, the same as for Chrome and configure it in our custom tests configuration section. Now the UI tests will behave as if they were simple unit or integration tests.
Happy testing! 


