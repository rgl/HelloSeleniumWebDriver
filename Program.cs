using CommandLine;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
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
                Default="https://en.m.wikipedia.org/wiki/Main_Page",
                HelpText="Web page to open")]
            public string Url { get; set; }

            [Option(
                "chromedriver-log-path",
                Default="chromedriver.log",
                HelpText="chromedriver log path")]
            public string ChromedriverLogPath { get; set; }

            [Option(
                "chromedriver-url",
                Default="",
                HelpText="remote chromedriver url. when used, chromedriver will not be started locally, instead, it will be used remotely.")]
            public string ChromedriverUrl { get; set; }

            [Option(
                "screenshot-path",
                Default="screenshot.png",
                HelpText="Screenshot output path")]
            public string ScreenshotPath { get; set; }

            [Option(
                "window-size",
                Default = "800x600",
                HelpText = "Browser Window size. Note that --window-client-size takes precedence over this value.")]
            public string WindowSize { get; set; }

            [Option(
                "window-client-size",
                Default = "",
                HelpText = "Browser Window client (or content) size.")]
            public string WindowClientSize { get; set; }

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
            var windowSize = ParseSize(string.IsNullOrEmpty(options.WindowClientSize) ? options.WindowSize : options.WindowClientSize);
            var windowClientSize = ParseSize(options.WindowClientSize);

            // see https://sites.google.com/a/chromium.org/chromedriver/capabilities
            var chromeOptions = new ChromeOptions();

            if (windowSize.HasValue)
            {
                if (!windowClientSize.HasValue)
                {
                    Console.WriteLine($"Configuring the window size to {windowSize.Value}...");
                }
                chromeOptions.AddArguments($"--window-size={windowSize.Value.Width},{windowSize.Value.Height}");
            }

            // enable headless mode when requested.
            // see https://developers.google.com/web/updates/2017/04/headless-chrome
            // see https://intoli.com/blog/running-selenium-with-headless-chrome/
            if (options.Headless == "1" || options.Headless == "true")
            {
                Console.WriteLine("Configuring chrome to run headless...");
                chromeOptions.AddArguments("--headless");
                chromeOptions.AddArguments("--disable-gpu");
            }

            WebDriver wd;

            if (string.IsNullOrEmpty(options.ChromedriverUrl))
            {
                Console.WriteLine("Using local chrome web-driver...");

                var service = ChromeDriverService.CreateDefaultService();
                service.LogPath = options.ChromedriverLogPath;
                service.EnableVerboseLogging = true;

                wd = new ChromeDriver(service, chromeOptions);
            }
            else
            {
                Console.WriteLine($"Using remote chrome web-driver at {options.ChromedriverUrl}...");

                wd = new RemoteWebDriver(new Uri(options.ChromedriverUrl), chromeOptions);
            }

            using (wd)
            {
                // show information about the browser.
                var browserName = wd.Capabilities.GetCapability("browserName");
                var browserVersion = wd.Capabilities.GetCapability("browserVersion");
                var browserPlatform = wd.Capabilities.GetCapability("platformName");
                Console.WriteLine($"Using {browserName}/{browserVersion} {browserPlatform}");

                // move the window to the top-left corner.
                Console.WriteLine("Moving the window to the top-left corner of the screen...");
                wd.Manage().Window.Position = new Point(0, 0);

                // resize the window client size.
                if (windowClientSize.HasValue)
                {
                    Console.WriteLine($"Resizing the window client (content) size to {windowClientSize.Value}...");
                    var js = (IJavaScriptExecutor)wd;
                    var initialWindowSize = wd.Manage().Window.Size;
                    var initialWindowPadding = (IReadOnlyCollection<object>)js.ExecuteScript("return [window.outerWidth-window.innerWidth, window.outerHeight-window.innerHeight];");
                    wd.Manage().Window.Size = new Size(
                        windowClientSize.Value.Width + Convert.ToInt32(initialWindowPadding.ElementAt(0)),
                        windowClientSize.Value.Height + Convert.ToInt32(initialWindowPadding.ElementAt(1))
                    );
                }

                Console.WriteLine($"Browsing to {options.Url}...");
                wd.Navigate().GoToUrl(options.Url);

                Console.WriteLine($"Waiting for the page to load...");
                wd.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                wd.FindElement(By.CssSelector("#searchInput"));

                Console.WriteLine($"Saving screenshot to {options.ScreenshotPath}...");
                wd.GetScreenshot().SaveAsFile(options.ScreenshotPath, ScreenshotImageFormat.Png);

                return 0;
            }
        }

        private static Size? ParseSize(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var parts = value.Split('x');

            if (parts.Length != 2)
            {
                return null;
            }

            return new Size(int.Parse(parts[0]), int.Parse(parts[1]));
        }
    }
}
