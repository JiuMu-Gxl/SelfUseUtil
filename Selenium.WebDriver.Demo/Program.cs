// See https://aka.ms/new-console-template for more information
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

using IWebDriver driver = new ChromeDriver();
driver.Navigate().GoToUrl("https://www.selenium.dev/selenium/web/web-form.html");
var title = driver.Title;
Console.WriteLine(title);
var url = driver.Url;
Console.WriteLine(url);

driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);

driver.Url = "https://www.selenium.dev/selenium/web/dynamic.html";
driver.FindElement(By.Id("adder")).Click();
IWebElement added = driver.FindElement(By.Id("box0"));
if (added != null)
{
    Console.WriteLine("找到了");
}
else
{
    Console.WriteLine("未找到");
}

driver.Close();