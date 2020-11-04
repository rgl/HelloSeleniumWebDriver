using CommandLine;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HelloSeleniumWebDriver
{
    class Program
    {
        private class Options
        {
            [Option(
                Default="https://google.com",
                HelpText="Web page to open")]
            public string Url { get; set; }

            [Option(
                "chromedriver-log-path",
                Default="chromedriver.log",
                HelpText="chromedriver log path")]
            public string ChromedriverLogPath { get; set; }

            [Option(
                "screenshot-path",
                Default="screenshot.png",
                HelpText="Screenshot output path")]
            public string ScreenshotPath { get; set; }

            [Option(
                Default="false",
                HelpText="Run the browser in headless mode")]
            public string Headless { get; set; }
        }

        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<Options>(args)
                .MapResult(Run, _ => 1);
        }

        static int Run(Options options)
        {
            // see https://sites.google.com/a/chromium.org/chromedriver/capabilities
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--window-size=800,600");

            // enable headless mode when requested.
            // see https://developers.google.com/web/updates/2017/04/headless-chrome
            // see https://intoli.com/blog/running-selenium-with-headless-chrome/
            if (options.Headless == "1" || options.Headless == "true")
            {
                Console.WriteLine("Configuring chrome to run headless...");
                chromeOptions.AddArguments("--headless");
                chromeOptions.AddArguments("--disable-gpu");
            }

            using var service = ChromeDriverService.CreateDefaultService();
            service.LogPath = options.ChromedriverLogPath;
            service.EnableVerboseLogging = true;

            using var wd = new ChromeDriver(service, chromeOptions);
            var js = (IJavaScriptExecutor)wd;

            // resize the window client size to 800x600.
            wd.Manage().Window.Position = new Point(0, 0);
            var initialWindowSize = wd.Manage().Window.Size;
            var initialWindowPadding = (IReadOnlyCollection<object>)js.ExecuteScript("return [window.outerWidth-window.innerWidth, window.outerHeight-window.innerHeight];");
            wd.Manage().Window.Size = new Size(
               800 + Convert.ToInt32(initialWindowPadding.ElementAt(0)),
               600 + Convert.ToInt32(initialWindowPadding.ElementAt(1))
            );

            Console.WriteLine($"Browsing to {options.Url}...");
            wd.Navigate().GoToUrl(options.Url);

            Console.WriteLine($"Saving screenshot to {options.ScreenshotPath}...");
            wd.GetScreenshot().SaveAsFile(options.ScreenshotPath, ScreenshotImageFormat.Png);

            return 0;
        }
    }
}
