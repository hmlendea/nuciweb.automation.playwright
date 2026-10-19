# Quick start

## Prerequisites

- .NET 10.0 SDK
- Playwright browsers installed (see [Configuration](configuration.md#playwright-browsers))

## Installation

```bash
dotnet add package NuciWeb.Automation.Playwright
```

## Basic usage

```csharp
using NuciWeb.Automation;
using NuciWeb.Automation.Playwright;

// Create a browser session
await using PlaywrightSession session = WebDriverInitialiser.InitialiseAvailableWebDriver();

// Create the web processor
IWebProcessor processor = new PlaywrightWebProcessor(session.Page);

// Navigate to a page
processor.GoToUrl("https://example.com");

// Click an element
processor.Click("//button[@id='submit']");

// Set text in a field
processor.SetText("//input[@name='username']", "myuser");

// Get page source
string source = processor.GetPageSource();

// Execute JavaScript
string result = processor.ExecuteScript("return document.title;");

// Clean up
await session.DisposeAsync();
```

## Headless mode

```csharp
await using PlaywrightSession session = WebDriverInitialiser.InitialiseAvailableWebDriver(
    isDebugModeEnabled: false,
    pageLoadTimeout: 60);
```

## Explicit browser selection

```csharp
// Firefox
await using PlaywrightSession session = WebDriverInitialiser.InitialiseFirefoxDriver();

// Chromium
await using PlaywrightSession session = WebDriverInitialiser.InitialiseChromeDriver();
```

## Tab management

```csharp
// Open a new tab
string tabId = processor.NewTab("https://example.com/page2");

// Switch to the new tab
processor.SwitchToTab(tabId);

// Close the tab
processor.CloseTab(tabId);
```

## Iframe handling

```csharp
// Switch to an iframe
processor.SwitchToIframe("//iframe[@id='myframe']");

// Interact with elements inside the iframe
processor.Click("//button[@id='iframe-button']");
```

## Alert handling

```csharp
// Accept an alert
processor.AcceptAlert();

// Dismiss an alert
processor.DismissAlert();
```

## Next steps

- Read the [Architecture](architecture.md) document for design details
- Review [Configuration](configuration.md) for all options
- See [Data model](data-model.md) for interface contracts
- Refer to [Maintenance](maintenance.md) for development and contribution guidelines
