using System;
using System.Threading.Tasks;
using NuciWeb.HTTP;
using Microsoft.Playwright;
using System.IO;

namespace NuciWeb.Automation.Playwright
{
    /// <summary>
    /// Represents an active Playwright browser session.
    /// </summary>
    public sealed class PlaywrightSession(
        IPlaywright playwright,
        IBrowser browser,
        IBrowserContext context,
        IPage page) : IAsyncDisposable
    {
        public IPlaywright Playwright { get; } = playwright;

        public IBrowser Browser { get; } = browser;

        public IBrowserContext Context { get; } = context;

        public IPage Page { get; } = page;

        public async ValueTask DisposeAsync()
        {
            await Context.CloseAsync();
            await Browser.CloseAsync();
            Playwright.Dispose();
        }
    }

    /// <summary>
    /// Provides helper methods to initialise Playwright browser sessions.
    /// </summary>
    public static class WebDriverInitialiser
    {
        const int DefaultViewportWidth = 1920;
        const int DefaultViewportHeight = 1080;

        /// <summary>
        /// Initialises a Playwright browser session based on available browsers.
        /// Prefers Firefox when available, otherwise falls back to Chromium.
        /// </summary>
        public static PlaywrightSession InitialiseAvailableWebDriver(
            bool isDebugModeEnabled = true,
            int pageLoadTimeout = 90)
        {
            bool hasFirefox = File.Exists("/usr/bin/firefox");

            if (hasFirefox)
            {
                return InitialiseFirefoxDriver(isDebugModeEnabled, pageLoadTimeout);
            }

            return InitialiseChromeDriver(isDebugModeEnabled, pageLoadTimeout);
        }

        /// <summary>
        /// Initialises a Playwright Firefox browser session.
        /// </summary>
        public static PlaywrightSession InitialiseFirefoxDriver(
            bool isDebugModeEnabled = true,
            int pageLoadTimeout = 90)
            => InitialiseSession(BrowserType.Firefox, isDebugModeEnabled, pageLoadTimeout);

        /// <summary>
        /// Initialises a Playwright Chromium browser session.
        /// </summary>
        public static PlaywrightSession InitialiseChromeDriver(
            bool isDebugModeEnabled = true,
            int pageLoadTimeout = 90)
            => InitialiseSession(BrowserType.Chromium, isDebugModeEnabled, pageLoadTimeout);

        static PlaywrightSession InitialiseSession(
            BrowserType browserType,
            bool isDebugModeEnabled,
            int pageLoadTimeout)
        {
            IUserAgentFetcher userAgentFetcher = new UserAgentFetcher();
            string userAgent = userAgentFetcher.GetUserAgent().GetAwaiter().GetResult();

            IPlaywright playwright = Microsoft.Playwright.Playwright.CreateAsync().GetAwaiter().GetResult();
            IBrowser browser = LaunchBrowser(playwright, browserType, isDebugModeEnabled);

            BrowserNewContextOptions contextOptions = new()
            {
                UserAgent = userAgent,
                ViewportSize = new()
                {
                    Width = DefaultViewportWidth,
                    Height = DefaultViewportHeight
                }
            };

            IBrowserContext context = browser.NewContextAsync(contextOptions).GetAwaiter().GetResult();
            context.SetDefaultNavigationTimeout(pageLoadTimeout * 1000);

            if (!isDebugModeEnabled)
            {
                context.RouteAsync("**/*.{png,jpg,jpeg,gif,webp,svg,ico,avif}", route => route.AbortAsync())
                    .GetAwaiter()
                    .GetResult();
            }

            IPage page = context.NewPageAsync().GetAwaiter().GetResult();
            page.SetDefaultNavigationTimeout(pageLoadTimeout * 1000);

            return new PlaywrightSession(playwright, browser, context, page);
        }

        static IBrowser LaunchBrowser(IPlaywright playwright, BrowserType browserType, bool isDebugModeEnabled)
        {
            BrowserTypeLaunchOptions launchOptions = new()
            {
                Headless = !isDebugModeEnabled
            };

            return browserType switch
            {
                BrowserType.Firefox => playwright.Firefox.LaunchAsync(launchOptions).GetAwaiter().GetResult(),
                BrowserType.Webkit => playwright.Webkit.LaunchAsync(launchOptions).GetAwaiter().GetResult(),
                _ => playwright.Chromium.LaunchAsync(launchOptions).GetAwaiter().GetResult()
            };
        }

        enum BrowserType
        {
            Chromium,
            Firefox,
            Webkit
        }
    }
}