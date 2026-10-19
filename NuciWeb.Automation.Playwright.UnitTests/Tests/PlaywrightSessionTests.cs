using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Moq;
using NUnit.Framework;
using NuciWeb.Automation.Playwright;

namespace NuciWeb.Automation.Playwright.UnitTests
{
    [TestFixture]
    public class PlaywrightSessionTests
    {
        private Mock<IPlaywright> mockPlaywright = null!;
        private Mock<IBrowser> mockBrowser = null!;
        private Mock<IBrowserContext> mockContext = null!;
        private Mock<IPage> mockPage = null!;

        [SetUp]
        public void SetUp()
        {
            mockPlaywright = new Mock<IPlaywright>();
            mockBrowser = new Mock<IBrowser>();
            mockContext = new Mock<IBrowserContext>();
            mockPage = new Mock<IPage>();
        }

        [Test]
        public void Constructor_SetsProperties()
        {
            PlaywrightSession session = new(
                mockPlaywright.Object,
                mockBrowser.Object,
                mockContext.Object,
                mockPage.Object);

            Assert.That(session.Playwright, Is.EqualTo(mockPlaywright.Object));
            Assert.That(session.Browser, Is.EqualTo(mockBrowser.Object));
            Assert.That(session.Context, Is.EqualTo(mockContext.Object));
            Assert.That(session.Page, Is.EqualTo(mockPage.Object));
        }

        [Test]
        public void DisposeAsync_ClosesContextBrowserAndPlaywright()
        {
            PlaywrightSession session = new(
                mockPlaywright.Object,
                mockBrowser.Object,
                mockContext.Object,
                mockPage.Object);

            session.DisposeAsync().GetAwaiter().GetResult();

            mockContext.Verify(c => c.CloseAsync(), Times.Once);
            mockBrowser.Verify(b => b.CloseAsync(), Times.Once);
            mockPlaywright.Verify(p => p.Dispose(), Times.Once);
        }

        private static class TestData
        {
        }
    }
}