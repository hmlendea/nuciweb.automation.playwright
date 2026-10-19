# Data model

## Overview

The NuciWeb.Automation.Playwright library does not define its own domain entities or data models. All data structures are provided by external packages:

- `NuciWeb.Automation` — defines the `IWebProcessor` interface and `WebProcessor` base class
- `NuciWeb.HTTP` — provides `UserAgentFetcher` for user agent strings
- `Microsoft.Playwright` — provides Playwright-specific types (`IPage`, `IBrowserContext`, etc.)

## External dependencies

### NuciWeb.Automation

The `IWebProcessor` interface defines the public contract for browser automation:

```csharp
public interface IWebProcessor
{
    // Navigation
    void GoToUrl(string url, int httpRetries = 3, TimeSpan retryDelay = default);
    void Refresh();
    string NewTab(string url);
    void CloseTab(string tab);
    void SwitchToTab(string tab);

    // Element interaction
    void Click(string xpath);
    void SetText(string xpath, string text);
    void MoveToElement(string xpath);
    bool DoesElementExist(string xpath);
    bool IsElementVisible(string xpath);
    bool IsCheckboxChecked(string xpath);
    bool IsSelected(string xpath);

    // Text and attribute retrieval
    IEnumerable<string> GetText(string xpath);
    IEnumerable<string> GetAttribute(string xpath, string attribute);
    IEnumerable<string> GetSelectedText(string xpath);
    string GetPageSource();

    // Select options
    void SelectOptionByIndex(string xpath, int index);
    void SelectOptionByText(string xpath, string text);
    void SelectOptionByValue(string xpath, object value);
    int GetSelectOptionsCount(string xpath);

    // Script execution
    string ExecuteScript(string script);

    // Alert handling
    void AcceptAlert();
    void DismissAlert();

    // Iframe management
    void SwitchToIframe(string xpath);
}
```

### NuciWeb.HTTP

Provides `UserAgentFetcher.GetUserAgent()` used by `WebDriverInitialiser` to set the browser user agent.

### Microsoft.Playwright

Provides the underlying browser automation API:

- `IPage` — represents a browser page
- `IBrowserContext` — manages browser state and contexts
- `IBrowser` — the browser instance
- `IPlaywright` — the Playwright runtime
- `IFrame` — iframe within a page
- `IDialog` — JavaScript alert/confirm/prompt
- `ILocator` — element selector and interaction

## Data flow

### Application → PlaywrightWebProcessor

The application calls `IWebProcessor` methods, which are implemented by `PlaywrightWebProcessor`. Each method translates the synchronous call into a Playwright async operation.

### PlaywrightWebProcessor → Playwright

`PlaywrightWebProcessor` uses Playwright APIs to perform browser actions:

- Navigation: `IPage.GotoAsync()`
- Element location: `IPage.Locator()` or `IFrame.Locator()`
- Interaction: `ILocator.ClickAsync()`, `ILocator.FillAsync()`, etc.
- Script execution: `IPage.EvaluateAsync()`
- Alert handling: `IDialog.AcceptAsync()` / `DismissAsync()`

### WebDriverInitialiser → NuciWeb.HTTP

`WebDriverInitialiser` calls `UserAgentFetcher.GetUserAgent()` to obtain a user agent string for browser configuration.

## State management

The library maintains no persistent state:

- `PlaywrightWebProcessor` holds transient state (`pages`, `currentFrame`, `pendingDialog`) scoped to the automation session.
- `PlaywrightSession` holds the browser session state (playwright, browser, context, page).
- All state is ephemeral and released when the session is disposed.

## Configuration data

Configuration is passed as method parameters:

- `isDebugModeEnabled` — controls headless mode and image blocking
- `pageLoadTimeout` — navigation timeout in seconds
- `httpRetries` and `retryDelay` — navigation retry parameters

## Data transformations

The library performs minimal data transformations:

- XPath selectors are converted to Playwright selectors (`xpath=...`)
- JavaScript script results are converted to strings via `ToString()`
- User agent strings are passed directly to Playwright context options

## Data boundaries

### External boundaries

- **NuciWeb.Automation** — contract interface (`IWebProcessor`)
- **NuciWeb.HTTP** — user agent fetching
- **Microsoft.Playwright** — browser automation engine

### Internal boundaries

- **PlaywrightWebProcessor** — adapter layer hiding Playwright types
- **WebDriverInitialiser** — session factory
- **PlaywrightSession** — session container

## Testing data

No unit tests exist in this repository. Verification relies on manual inspection and the NuciWeb.Automation test suite.
