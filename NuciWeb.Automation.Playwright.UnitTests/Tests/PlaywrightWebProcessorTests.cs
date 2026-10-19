using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Moq;
using NUnit.Framework;
using NuciWeb.Automation.Playwright;

namespace NuciWeb.Automation.Playwright.UnitTests
{
    [TestFixture]
    public class PlaywrightWebProcessorTests
    {
        private Mock<IPage> mockPage = null!;
        private Mock<IBrowserContext> mockContext = null!;
        private Mock<IPage> mockNewPage = null!;
        private Mock<ILocator> mockLocator = null!;
        private PlaywrightWebProcessor processor = null!;

        [SetUp]
        public void SetUp()
        {
            mockPage = new Mock<IPage>();
            mockContext = new Mock<IBrowserContext>();
            mockNewPage = new Mock<IPage>();
            mockLocator = CreateVisibleLocator();

            mockPage.Setup(p => p.Context).Returns(mockContext.Object);
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);
            mockNewPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);
            mockContext.Setup(c => c.NewPageAsync()).ReturnsAsync(mockPage.Object);
            mockPage.Setup(p => p.GotoAsync(It.IsAny<string>())).ReturnsAsync((IResponse?)null);
            mockPage.Setup(p => p.Url).Returns("about:blank");

            processor = new PlaywrightWebProcessor(mockPage.Object);
            processor.NewTab();
        }

        [Test]
        public void DoesElementExist_WhenElementExists_ReturnsTrue()
        {
            Mock<ILocator> mockLocator = CreateVisibleLocator();
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);
            mockLocator.Setup(l => l.CountAsync()).ReturnsAsync(1);

            bool result = processor.DoesElementExist("//div[@id='test']");

            Assert.That(result, Is.True);
        }

        [Test]
        public void DoesElementExist_WhenElementDoesNotExist_ReturnsFalse()
        {
            Mock<ILocator> mockLocator = CreateVisibleLocator();
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);
            mockLocator.Setup(l => l.CountAsync()).ReturnsAsync(0);

            bool result = processor.DoesElementExist("//div[@id='test']");

            Assert.That(result, Is.False);
        }

        [Test]
        public void DoesElementExist_WhenExceptionOccurs_ReturnsFalse()
        {
            Mock<ILocator> mockLocator = CreateVisibleLocator();
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);
            mockLocator.Setup(l => l.CountAsync()).Throws<Exception>();

            bool result = processor.DoesElementExist("//div[@id='test']");

            Assert.That(result, Is.False);
        }

        [Test]
        public void UpdateCheckbox_WhenAlreadyChecked_DoesNotClick()
        {
            Mock<ILocator> mockLocator = CreateVisibleLocator();
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);
            mockLocator.Setup(l => l.IsCheckedAsync()).ReturnsAsync(true);

            processor.UpdateCheckbox("//input[@type='checkbox']", true);

            mockLocator.Verify(l => l.ClickAsync(), Times.Never);
        }

        [Test]
        public void UpdateCheckbox_WhenNotCheckedAndRequestedChecked_Clicks()
        {
            Mock<ILocator> mockLocator = CreateVisibleLocator();
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);
            mockLocator.Setup(l => l.IsCheckedAsync()).ReturnsAsync(false);

            processor.UpdateCheckbox("//input[@type='checkbox']", true);

            mockLocator.Verify(l => l.ClickAsync(), Times.Once);
        }

        [Test]
        public void IsElementVisible_WhenVisible_ReturnsTrue()
        {
            Mock<ILocator> mockLocator = CreateVisibleLocator();
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);
            mockLocator.Setup(l => l.IsVisibleAsync()).ReturnsAsync(true);

            bool result = processor.IsElementVisible("//div[@id='visible']");

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsElementVisible_WhenNotVisible_ReturnsFalse()
        {
            Mock<ILocator> mockLocator = CreateVisibleLocator();
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);
            mockLocator.Setup(l => l.IsVisibleAsync()).ReturnsAsync(false);

            bool result = processor.IsElementVisible("//div[@id='hidden']");

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsSelected_WhenSelected_ReturnsTrue()
        {
            Mock<ILocator> mockLocator = CreateVisibleLocator();
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);
            mockLocator.Setup(l => l.IsCheckedAsync()).ReturnsAsync(true);

            bool result = processor.IsSelected("//input[@type='checkbox']");

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsSelected_WhenNotSelected_ReturnsFalse()
        {
            Mock<ILocator> mockLocator = CreateVisibleLocator();
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);
            mockLocator.Setup(l => l.IsCheckedAsync()).ReturnsAsync(false);

            bool result = processor.IsSelected("//input[@type='checkbox']");

            Assert.That(result, Is.False);
        }

        [Test]
        public void GetAttribute_ReturnsAttributes()
        {
            Mock<ILocator> mockLocator = CreateVisibleLocator();
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);
            mockLocator.Setup(l => l.GetAttributeAsync(It.IsAny<string>())).ReturnsAsync("value1");

            IEnumerable<string> result = processor.GetAttributeOfMany("//input[@id='test']", "value");

            Assert.That(result, Is.EquivalentTo(new[] { "value1" }));
        }

        [Test]
        public void GetSelectedText_ReturnsSelectedText()
        {
            Mock<ILocator> mockLocator = CreateVisibleLocator();
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);
            mockLocator.Setup(l => l.EvaluateAsync<string[]>(
                It.IsAny<string>())).ReturnsAsync(new[] { "Option 1", "Option 2" });

            IEnumerable<string> result = processor.GetSelectedTextOfMany("//select[@id='test']");

            Assert.That(result, Is.EquivalentTo(new[] { "Option 1", "Option 2" }));
        }

        [Test]
        public void GetText_ReturnsText()
        {
            Mock<ILocator> mockLocator = CreateVisibleLocator();
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);
            mockLocator.Setup(l => l.InnerTextAsync()).ReturnsAsync("Test text");

            IEnumerable<string> result = processor.GetTextOfMany("//div[@id='test']");

            Assert.That(result, Is.EquivalentTo(new[] { "Test text" }));
        }

        [Test]
        public void GetTextOfMany_WhenLocatorCountInitiallyThrows_RetriesAndReturnsText()
        {
            Mock<ILocator> locator = CreateVisibleLocator();
            mockPage.Setup(page => page.Locator(It.IsAny<string>())).Returns(locator.Object);
            locator.SetupSequence(item => item.CountAsync())
                .ThrowsAsync(new InvalidOperationException())
                .ReturnsAsync(1);

            IEnumerable<string> result = processor.GetTextOfMany(TestData.ValidXPath);

            Assert.That(result, Is.EqualTo(new[] { "Test text" }));
        }

        [Test]
        public void GetTextOfMany_WhenMultipleElementsExist_ReturnsAllTexts()
        {
            Mock<ILocator> firstLocator = CreateVisibleLocator();
            Mock<ILocator> secondLocator = CreateVisibleLocator();
            Mock<ILocator> collectionLocator = CreateVisibleLocator();
            collectionLocator.Setup(item => item.CountAsync()).ReturnsAsync(2);
            collectionLocator.Setup(item => item.Nth(0)).Returns(firstLocator.Object);
            collectionLocator.Setup(item => item.Nth(1)).Returns(secondLocator.Object);
            firstLocator.Setup(item => item.InnerTextAsync()).ReturnsAsync("First");
            secondLocator.Setup(item => item.InnerTextAsync()).ReturnsAsync("Second");
            mockPage.Setup(page => page.Locator(It.IsAny<string>())).Returns(collectionLocator.Object);

            IEnumerable<string> result = processor.GetTextOfMany(TestData.ValidXPath);

            Assert.That(result, Is.EqualTo(new[] { "First", "Second" }));
        }

        [Test]
        public void SelectRandomOption_SelectsAnOption()
        {
            Mock<ILocator> mockLocator = CreateVisibleLocator();
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);
            mockLocator.Setup(l => l.EvaluateAsync<int>(
                It.IsAny<string>())).ReturnsAsync(5);

            processor.SelectRandomOption("//select[@id='test']");

            mockLocator.Verify(l => l.SelectOptionAsync(It.IsAny<SelectOptionValue>()), Times.Once);
        }

        [Test]
        public void ExecuteScript_ReturnsResult()
        {
            mockPage.Setup(p => p.EvaluateAsync<object>(
                It.IsAny<string>())).ReturnsAsync("script result");

            processor.ExecuteScript("return 42;");

            mockPage.Verify(p => p.EvaluateAsync<object>("return 42;", null), Times.Once);
        }

        [Test]
        public void ExecuteScript_WhenResultIsNull_DoesNotThrow()
        {
            mockPage.Setup(p => p.EvaluateAsync<object>(It.IsAny<string>())).ReturnsAsync((object)null!);

            Assert.That(() => processor.ExecuteScript("return null;"), Throws.Nothing);
        }

        [Test]
        public void GetPageSource_ReturnsContent()
        {
            mockPage.Setup(p => p.ContentAsync()).ReturnsAsync("<html>content</html>");

            string result = processor.GetPageSource();

            Assert.That(result, Is.EqualTo("<html>content</html>"));
        }

        [Test]
        public void GetPageSource_WhenCurrentTabWasClosed_UsesDefaultPage()
        {
            processor.CloseTab("main");
            mockPage.Setup(page => page.ContentAsync()).ReturnsAsync("<html>fallback</html>");

            string result = processor.GetPageSource();

            Assert.That(result, Is.EqualTo("<html>fallback</html>"));
        }

        [Test]
        public void NewTab_ReturnsPageId()
        {
            Mock<IPage> mockNewPage = new();
            mockContext.Setup(c => c.NewPageAsync()).ReturnsAsync(mockNewPage.Object);
            mockNewPage.Setup(p => p.GotoAsync(It.IsAny<string>())).ReturnsAsync((IResponse?)null);

            string result = processor.NewTab("https://example.com");

            Assert.That(result, Is.EqualTo("tab-1"));
        }

        [Test]
        public void NewTab_WhenContextReturnsAnExistingPage_ReusesItsId()
        {
            mockContext.Setup(context => context.NewPageAsync()).ReturnsAsync(mockPage.Object);

            string result = processor.NewTab(TestData.TestUrl);

            Assert.That(result, Is.EqualTo("main"));
        }

        [Test]
        public void AcceptAlert_WhenNoPendingDialog_DoesNothing()
        {
            processor.AcceptAlert();

            Assert.Pass("AcceptAlert completed without error when no pending dialog");
        }

        [Test]
        public void AcceptAlert_WhenDialogIsPending_AcceptsAndClearsIt()
        {
            Mock<IDialog> mockDialog = new();
            mockDialog.Setup(dialog => dialog.AcceptAsync(null)).Returns(Task.CompletedTask);
            mockPage.Raise(page => page.Dialog += null, mockPage.Object, mockDialog.Object);

            processor.AcceptAlert();
            processor.AcceptAlert();

            mockDialog.Verify(dialog => dialog.AcceptAsync(null), Times.Once);
        }

        [Test]
        public void Click_ClicksElement()
        {
            Mock<ILocator> mockLocator = CreateVisibleLocator();
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);

            processor.Click("//button[@id='click']");

            mockLocator.Verify(l => l.ClickAsync(), Times.Once);
        }

        [Test]
        public void Click_WhenLocatorCountInitiallyThrows_RetriesAndClicks()
        {
            Mock<ILocator> locator = CreateVisibleLocator();
            mockPage.Setup(page => page.Locator(It.IsAny<string>())).Returns(locator.Object);
            locator.SetupSequence(item => item.CountAsync())
                .ThrowsAsync(new InvalidOperationException())
                .ReturnsAsync(1);

            processor.Click(TestData.ValidXPath);

            locator.Verify(item => item.ClickAsync(), Times.Once);
        }

        [Test]
        public void CloseTab_ClosesTab()
        {
            mockContext.Setup(c => c.NewPageAsync()).ReturnsAsync(mockNewPage.Object);
            mockNewPage.Setup(p => p.GotoAsync(It.IsAny<string>())).ReturnsAsync((IResponse?)null);
            mockNewPage.Setup(p => p.CloseAsync(It.IsAny<PageCloseOptions?>())).Returns(Task.CompletedTask);
            processor.NewTab(TestData.TestUrl);
            processor.CloseTab("tab-1");

            mockNewPage.Verify(p => p.CloseAsync(It.IsAny<PageCloseOptions>()), Times.Once);
        }

        [Test]
        public void DismissAlert_WhenNoPendingDialog_DoesNothing()
        {
            processor.DismissAlert();

            Assert.Pass("DismissAlert completed without error when no pending dialog");
        }

        [Test]
        public void DismissAlert_WhenDialogIsPending_DismissesAndClearsIt()
        {
            Mock<IDialog> mockDialog = new();
            mockDialog.Setup(dialog => dialog.DismissAsync()).Returns(Task.CompletedTask);
            mockPage.Raise(page => page.Dialog += null, mockPage.Object, mockDialog.Object);

            processor.DismissAlert();
            processor.DismissAlert();

            mockDialog.Verify(dialog => dialog.DismissAsync(), Times.Once);
        }

        [Test]
        public void GoToUrl_NavigatesToUrl()
        {
            Mock<ILocator> bodyLocator = CreateVisibleLocator();
            Mock<ILocator> errorLocator = CreateVisibleLocator();
            errorLocator.Setup(locator => locator.CountAsync()).ReturnsAsync(0);
            mockPage.Setup(p => p.Url).Returns("about:blank");
            mockPage.Setup(p => p.GotoAsync(TestData.TestUrl, It.IsAny<PageGotoOptions?>()))
                .ReturnsAsync((IResponse?)null);
            mockPage.Setup(page => page.Locator("xpath=/html/body/*")).Returns(bodyLocator.Object);
            mockPage.Setup(page => page.Locator(It.Is<string>(selector => selector.Contains("error-code"))))
                .Returns(errorLocator.Object);

            processor.GoToUrl(TestData.TestUrl, 3, TimeSpan.Zero);

            mockPage.Verify(p => p.GotoAsync(TestData.TestUrl, It.IsAny<PageGotoOptions?>()), Times.Once);
        }

        [Test]
        public void GoToUrl_WhenInitialNavigationThrows_RetriesAndCompletes()
        {
            Mock<ILocator> bodyLocator = CreateVisibleLocator();
            Mock<ILocator> errorLocator = CreateVisibleLocator();
            errorLocator.Setup(locator => locator.CountAsync()).ReturnsAsync(0);
            mockPage.Setup(p => p.Url).Returns("about:blank");
            mockPage.SetupSequence(p => p.GotoAsync(TestData.TestUrl, It.IsAny<PageGotoOptions?>()))
                .ThrowsAsync(new InvalidOperationException())
                .ReturnsAsync((IResponse?)null);
            mockPage.Setup(page => page.Locator("xpath=/html/body/*")).Returns(bodyLocator.Object);
            mockPage.Setup(page => page.Locator(It.Is<string>(selector => selector.Contains("error-code"))))
                .Returns(errorLocator.Object);

            Assert.That(() => processor.GoToUrl(TestData.TestUrl, 1, TimeSpan.Zero), Throws.Nothing);
            mockPage.Verify(page => page.GotoAsync(TestData.TestUrl, It.IsAny<PageGotoOptions?>()), Times.Once);
        }

        [Test]
        public void GoToUrl_WhenBodyAppearsBetweenChecks_RetriesNavigation()
        {
            Mock<ILocator> bodyLocator = CreateVisibleLocator();
            Mock<ILocator> errorLocator = CreateVisibleLocator();
            int bodyChecks = 0;
            bodyLocator.Setup(locator => locator.CountAsync()).Returns(() =>
            {
                bodyChecks += 1;
                return Task.FromResult(bodyChecks == 2 ? 0 : 1);
            });
            errorLocator.Setup(locator => locator.CountAsync()).ReturnsAsync(0);
            mockPage.Setup(page => page.Url).Returns("about:blank");
            mockPage.Setup(page => page.GotoAsync(TestData.TestUrl, It.IsAny<PageGotoOptions?>()))
                .ReturnsAsync((IResponse?)null);
            mockPage.Setup(page => page.Locator("xpath=/html/body/*")).Returns(bodyLocator.Object);
            mockPage.Setup(page => page.Locator(It.Is<string>(selector => selector.Contains("error-code"))))
                .Returns(errorLocator.Object);

            processor.GoToUrl(TestData.TestUrl, 1, TimeSpan.Zero);

            mockPage.Verify(page => page.GotoAsync(TestData.TestUrl, It.IsAny<PageGotoOptions?>()), Times.Exactly(2));
        }

        [Test]
        public void GoToUrl_WhenErrorPageRemains_ThrowsAfterRetries()
        {
            mockPage.Setup(p => p.Url).Returns("about:blank");
            mockLocator.Setup(locator => locator.IsVisibleAsync()).ReturnsAsync(true);

            Assert.That(
                () => processor.GoToUrl(TestData.TestUrl, 1, TimeSpan.Zero),
                Throws.TypeOf<Exception>().With.Message.EqualTo("Failed to load the requested URL after 1 attempts"));
        }

        [Test]
        public void MoveToElement_HoversOverElement()
        {
            Mock<ILocator> mockLocator = CreateVisibleLocator();
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);

            processor.MoveToElement("//div[@id='hover']");

            mockLocator.Verify(l => l.HoverAsync(), Times.Once);
        }

        [Test]
        public void Refresh_ReloadsPage()
        {
            mockPage.Setup(p => p.ReloadAsync()).ReturnsAsync((IResponse?)null);

            processor.Refresh();

            mockPage.Verify(p => p.ReloadAsync(), Times.Once);
        }

        [Test]
        public void SelectOptionByIndex_SelectsOption()
        {
            Mock<ILocator> mockLocator = CreateVisibleLocator();
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);
            processor.SelectOptionByIndex("//select[@id='test']", 2);

            mockLocator.Verify(l => l.SelectOptionAsync(It.Is<SelectOptionValue>(value => value.Index == 2), null), Times.Once);
        }

        [Test]
        public void SelectOptionByText_SelectsOption()
        {
            Mock<ILocator> mockLocator = CreateVisibleLocator();
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);
            processor.SelectOptionByText("//select[@id='test']", "Option 2");

            mockLocator.Verify(l => l.SelectOptionAsync(It.Is<SelectOptionValue>(value => value.Label == "Option 2"), null), Times.Once);
        }

        [Test]
        public void SelectOptionByValue_SelectsOption()
        {
            Mock<ILocator> mockLocator = CreateVisibleLocator();
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);
            processor.SelectOptionByValue("//select[@id='test']", "value2");

            mockLocator.Verify(l => l.SelectOptionAsync(It.Is<SelectOptionValue>(value => value.Value == "value2"), null), Times.Once);
        }

        [Test]
        public void SetText_FillsInput()
        {
            Mock<ILocator> mockLocator = CreateVisibleLocator();
            mockPage.Setup(p => p.Locator(It.IsAny<string>())).Returns(mockLocator.Object);
            processor.SetText("//input[@id='text']", "test value");

            mockLocator.Verify(l => l.FillAsync("test value"), Times.Once);
        }

        [Test]
        public void SwitchToIframe_WhenIframeHandleIsMissing_Throws()
        {
            mockLocator.Setup(locator => locator.ElementHandleAsync(It.IsAny<LocatorElementHandleOptions?>()))
                .ReturnsAsync((IElementHandle)null!);

            Assert.That(
                () => processor.SwitchToIframe(TestData.ValidXPath),
                Throws.TypeOf<Exception>().With.Message.EqualTo(
                    $"No iframe with the XPath {TestData.ValidXPath} exists!"));
        }

        [Test]
        public void SwitchToIframe_WhenFrameIsUnresolved_Throws()
        {
            Mock<IElementHandle> mockElement = new();
            mockLocator.Setup(locator => locator.ElementHandleAsync(It.IsAny<LocatorElementHandleOptions?>()))
                .ReturnsAsync(mockElement.Object);
            mockElement.Setup(element => element.ContentFrameAsync()).ReturnsAsync((IFrame?)null);

            Assert.That(
                () => processor.SwitchToIframe(TestData.ValidXPath),
                Throws.TypeOf<Exception>().With.Message.EqualTo(
                    $"The iframe with the XPath {TestData.ValidXPath} could not be resolved!"));
        }

        [Test]
        public void SwitchToIframe_WhenFrameIsResolved_UsesFrameLocator()
        {
            Mock<IElementHandle> mockElement = new();
            Mock<IFrame> mockFrame = new();
            mockLocator.SetupSequence(locator => locator.ElementHandleAsync(It.IsAny<LocatorElementHandleOptions?>()))
                .ReturnsAsync(mockElement.Object)
                .ThrowsAsync(new InvalidOperationException());
            mockElement.Setup(element => element.ContentFrameAsync()).ReturnsAsync(mockFrame.Object);
            mockFrame.Setup(frame => frame.Locator(It.IsAny<string>(), It.IsAny<FrameLocatorOptions?>()))
                .Returns(mockLocator.Object);

            Assert.That(() => processor.SwitchToIframe(TestData.ValidXPath), Throws.TypeOf<InvalidOperationException>());
            mockFrame.Verify(frame => frame.Locator($"xpath={TestData.ValidXPath}", null), Times.Once);
        }

        [Test]
        public void SwitchToTab_SwitchesToTab()
        {
            mockContext.Setup(c => c.NewPageAsync()).ReturnsAsync(mockNewPage.Object);
            mockNewPage.Setup(p => p.GotoAsync(It.IsAny<string>())).ReturnsAsync((IResponse?)null);
            processor.NewTab(TestData.TestUrl);
            processor.SwitchToTab("main");

            Assert.That(processor.CurrentTab, Is.EqualTo("main"));
        }

        private static class TestData
        {
            public static string ValidXPath => "//div[@id='test']";
            public static string TestUrl => "https://example.com";
        }

        private static Mock<ILocator> CreateVisibleLocator()
        {
            Mock<ILocator> locator = new();
            locator.Setup(item => item.CountAsync()).ReturnsAsync(1);
            locator.Setup(item => item.First).Returns(locator.Object);
            locator.Setup(item => item.Nth(It.IsAny<int>())).Returns(locator.Object);
            locator.Setup(item => item.IsVisibleAsync()).ReturnsAsync(true);
            locator.Setup(item => item.ClickAsync()).Returns(Task.CompletedTask);
            locator.Setup(item => item.HoverAsync()).Returns(Task.CompletedTask);
            locator.Setup(item => item.FillAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
            locator.Setup(item => item.SelectOptionAsync(It.IsAny<SelectOptionValue>()))
                .ReturnsAsync(Array.Empty<string>());
            locator.Setup(item => item.GetAttributeAsync(It.IsAny<string>())).ReturnsAsync("value1");
            locator.Setup(item => item.InnerTextAsync()).ReturnsAsync("Test text");
            locator.Setup(item => item.EvaluateAsync<int>(It.IsAny<string>())).ReturnsAsync(5);
            locator.Setup(item => item.EvaluateAsync<string[]>(It.IsAny<string>()))
                .ReturnsAsync(new[] { "Option 1", "Option 2" });

            return locator;
        }
    }
}