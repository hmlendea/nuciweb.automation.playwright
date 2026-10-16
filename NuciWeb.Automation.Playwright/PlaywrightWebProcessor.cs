using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Playwright;

namespace NuciWeb.Automation.Playwright
{
    /// <summary>
    /// Implements the <see cref="IWebProcessor"/> interface using Playwright to perform web automation tasks.
    /// </summary>
    /// <param name="page">The Playwright page instance used to interact with the web browser.</param>
    public sealed class PlaywrightWebProcessor(IPage page) : WebProcessor, IWebProcessor
    {
        readonly IPage defaultPage = page;
        readonly IBrowserContext browserContext = page.Context;
        readonly Dictionary<string, IPage> pages = new()
        {
            ["main"] = page
        };
        int nextPageId = 1;
        IFrame currentFrame;
        IDialog pendingDialog;

        protected override bool PerformDoesElementExist(string xpath)
        {
            try
            {
                ILocator locator = GetLocator(xpath);
                int count = locator.CountAsync().GetAwaiter().GetResult();
                return count > 0;
            }
            catch
            {
                return false;
            }
        }

        protected override bool PerformIsCheckboxChecked(string xpath)
            => GetFirstLocator(xpath).IsCheckedAsync().GetAwaiter().GetResult();

        protected override bool PerformIsElementVisible(string xpath)
        {
            try
            {
                ILocator locator = GetFirstLocator(xpath);
                return locator.IsVisibleAsync().GetAwaiter().GetResult();
            }
            catch
            {
                return false;
            }
        }

        protected override bool PerformIsSelected(string xpath)
            => GetFirstLocator(xpath).IsCheckedAsync().GetAwaiter().GetResult();

        protected override IEnumerable<string> PerformGetAttribute(string xpath, string attribute)
            => GetLocators(xpath).Select(x => x.GetAttributeAsync(attribute).GetAwaiter().GetResult());

        protected override IEnumerable<string> PerformGetSelectedText(string xpath)
            => GetLocators(xpath)
                .SelectMany(x => x.EvaluateAsync<string[]>("select => Array.from(select.selectedOptions).map(o => o.text)")
                    .GetAwaiter()
                    .GetResult());

        protected override IEnumerable<string> PerformGetText(string xpath)
            => GetLocators(xpath).Select(x => x.InnerTextAsync().GetAwaiter().GetResult());

        protected override int PerformGetSelectOptionsCount(string xpath)
            => GetFirstLocator(xpath)
                .EvaluateAsync<int>("select => select.options.length")
                .GetAwaiter()
                .GetResult();

        protected override string PerformExecuteScript(string script)
        {
            object result = GetCurrentPage().EvaluateAsync<object>(script).GetAwaiter().GetResult();

            if (result is null)
            {
                return null;
            }

            return result.ToString();
        }

        protected override string PerformGetPageSource()
            => GetCurrentPage().ContentAsync().GetAwaiter().GetResult();

        protected override string PerformNewTab(string url)
        {
            IPage newPage = browserContext.NewPageAsync().GetAwaiter().GetResult();
            RegisterPage(newPage);

            newPage.GotoAsync(url).GetAwaiter().GetResult();

            return GetPageId(newPage);
        }

        protected override void PerformAcceptAlert()
        {
            if (pendingDialog is null)
            {
                return;
            }

            pendingDialog.AcceptAsync().GetAwaiter().GetResult();
            pendingDialog = null;
        }

        protected override void PerformClick(string xpath)
            => GetFirstLocator(xpath).ClickAsync().GetAwaiter().GetResult();

        protected override void PerformCloseTab(string tab)
        {
            if (!pages.TryGetValue(tab, out IPage pageToClose))
            {
                return;
            }

            pageToClose.CloseAsync().GetAwaiter().GetResult();
            pages.Remove(tab);
        }

        protected override void PerformDismissAlert()
        {
            if (pendingDialog is null)
            {
                return;
            }

            pendingDialog.DismissAsync().GetAwaiter().GetResult();
            pendingDialog = null;
        }

        protected override void PerformGoToUrl(string url, int httpRetries, TimeSpan retryDelay)
        {
            IPage currentPage = GetCurrentPage();

            if (currentPage.Url.Equals(url, StringComparison.Ordinal))
            {
                return;
            }

            string errorSelectorChrome = Select.ByClass("error-code");
            string anythingSelector = Select.ByXPath(@"/html/body/*");

            for (int attempt = 0; attempt < httpRetries; attempt++)
            {
                try
                {
                    currentPage.GotoAsync(url, new() { WaitUntil = WaitUntilState.DOMContentLoaded }).GetAwaiter().GetResult();
                }
                catch
                {
                    // Retry loop handles transient navigation failures.
                }

                for (int i = 0; i < 3; i++)
                {
                    WaitForElementToExist(anythingSelector);
                    if (DoesElementExist(anythingSelector))
                    {
                        break;
                    }

                    currentPage.GotoAsync(url, new() { WaitUntil = WaitUntilState.DOMContentLoaded }).GetAwaiter().GetResult();
                }

                if (!IsAnyElementVisible(errorSelectorChrome))
                {
                    return;
                }

                GoToUrl("about:blank");
                Wait(retryDelay);
            }

            throw new Exception($"Failed to load the requested URL after {httpRetries} attempts");
        }

        protected override void PerformMoveToElement(string xpath)
            => GetFirstLocator(xpath).HoverAsync().GetAwaiter().GetResult();

        protected override void PerformRefresh()
            => GetCurrentPage().ReloadAsync().GetAwaiter().GetResult();

        protected override void PerformSelectOptionByIndex(string xpath, int index)
            => GetFirstLocator(xpath)
                .SelectOptionAsync(new SelectOptionValue() { Index = index })
                .GetAwaiter()
                .GetResult();

        protected override void PerformSelectOptionByText(string xpath, string text)
            => GetFirstLocator(xpath)
                .SelectOptionAsync(new SelectOptionValue() { Label = text })
                .GetAwaiter()
                .GetResult();

        protected override void PerformSelectOptionByValue(string xpath, object value)
            => GetFirstLocator(xpath)
                .SelectOptionAsync(new SelectOptionValue() { Value = value.ToString() })
                .GetAwaiter()
                .GetResult();

        protected override void PerformSetText(string xpath, string text)
            => GetFirstLocator(xpath).FillAsync(text).GetAwaiter().GetResult();

        protected override void PerformSwitchToIframe(string xpath)
        {
            IElementHandle iframeHandle = GetFirstLocator(xpath).ElementHandleAsync().GetAwaiter().GetResult();

            if (iframeHandle is null)
            {
                throw new Exception($"No iframe with the XPath {xpath} exists!");
            }

            currentFrame = iframeHandle.ContentFrameAsync().GetAwaiter().GetResult();

            if (currentFrame is null)
            {
                throw new Exception($"The iframe with the XPath {xpath} could not be resolved!");
            }
        }

        protected override void PerformSwitchToTab(string tab)
        {
            if (string.IsNullOrWhiteSpace(tab))
            {
                return;
            }

            if (!pages.ContainsKey(tab))
            {
                throw new Exception($"The tab '{tab}' does not exist!");
            }

            currentFrame = null;
        }

        IPage GetCurrentPage()
        {
            if (!string.IsNullOrWhiteSpace(CurrentTab)
                && pages.TryGetValue(CurrentTab, out IPage currentPage))
            {
                return currentPage;
            }

            return defaultPage;
        }

        ILocator GetLocator(string xpath)
        {
            string selector = $"xpath={xpath}";

            if (currentFrame is not null)
            {
                return currentFrame.Locator(selector);
            }

            return GetCurrentPage().Locator(selector);
        }

        ILocator GetFirstLocator(string xpath)
        {
            DateTime beginTime = DateTime.Now;

            while (DateTime.Now - beginTime < DefaultTimeout)
            {
                try
                {
                    ILocator locator = GetLocator(xpath);
                    int count = locator.CountAsync().GetAwaiter().GetResult();

                    if (count > 0)
                    {
                        ILocator first = locator.First;

                        if (first.IsVisibleAsync().GetAwaiter().GetResult())
                        {
                            return first;
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    Wait();
                }
            }

            throw new Exception($"No element with the XPath {xpath} exists!");
        }

        IList<ILocator> GetLocators(string xpath)
        {
            DateTime beginTime = DateTime.Now;

            while (DateTime.Now - beginTime < DefaultTimeout)
            {
                try
                {
                    ILocator locator = GetLocator(xpath);
                    int count = locator.CountAsync().GetAwaiter().GetResult();

                    if (count > 0)
                    {
                        List<ILocator> locators = [];

                        for (int i = 0; i < count; i++)
                        {
                            locators.Add(locator.Nth(i));
                        }

                        return locators;
                    }
                }
                catch
                {
                }
                finally
                {
                    Wait();
                }
            }

            throw new Exception($"No elements with the XPath {xpath} exist!");
        }

        void RegisterPage(IPage newPage)
        {
            string pageId = GetPageId(newPage);
            pages[pageId] = newPage;
            newPage.Dialog += (_, dialog) => pendingDialog = dialog;
        }

        string GetPageId(IPage page)
        {
            foreach (KeyValuePair<string, IPage> pageEntry in pages)
            {
                if (ReferenceEquals(pageEntry.Value, page))
                {
                    return pageEntry.Key;
                }
            }

            string pageId = $"tab-{nextPageId}";
            nextPageId++;
            return pageId;
        }
    }
}