# PlaywrightWebProcessor

## Purpose

`PlaywrightWebProcessor` is the adapter implementation of `IWebProcessor` from NuciWeb.Automation. It translates synchronous `IWebProcessor` method calls into Playwright's asynchronous browser automation API.

## Location

`NuciWeb.Automation.Playwright/PlaywrightWebProcessor.cs`

## Type declaration

```csharp
public sealed class PlaywrightWebProcessor(IPage page) : WebProcessor, IWebProcessor
```

## Internal state

| Field | Type | Purpose |
|---|---|---|
| `defaultPage` | `IPage` | The initial page passed to the constructor; fallback page when no tab is active |
| `browserContext` | `IBrowserContext` | The browser context obtained from the initial page; used to create new pages |
| `pages` | `Dictionary<string, IPage>` | Maps page identifiers to `IPage` instances; initial page registered under `"main"` |
| `nextPageId` | `int` | Counter for generating unique tab identifiers (`tab-1`, `tab-2`, ...) |
| `currentFrame` | `IFrame` | The currently selected iframe; `null` when operating on the main page |
| `pendingDialog` | `IDialog` | The most recent dialog captured via the page's `Dialog` event |

## Public contract

`PlaywrightWebProcessor` implements `IWebProcessor` by overriding `Perform*` methods from `WebProcessor`. All methods are synchronous wrappers around Playwright's async API.

### Navigation

| Method | Description |
|---|---|
| `PerformGoToUrl(string url, int httpRetries, TimeSpan retryDelay)` | Navigates to a URL with retry logic and Chromium error-page detection |
| `PerformRefresh()` | Reloads the current page |
| `PerformNewTab(string url)` | Creates a new tab, navigates to a URL, and returns the tab identifier |
| `PerformCloseTab(string tab)` | Closes a tab by identifier |
| `PerformSwitchToTab(string tab)` | Switches the active tab by identifier |

### Element interaction

| Method | Description |
|---|---|
| `PerformClick(string xpath)` | Clicks the first visible element matching the XPath |
| `PerformSetText(string xpath, string text)` | Fills text into the first visible element matching the XPath |
| `PerformMoveToElement(string xpath)` | Hovers over the first visible element matching the XPath |
| `PerformDoesElementExist(string xpath)` | Returns `true` if any element matches the XPath |
| `PerformIsElementVisible(string xpath)` | Returns `true` if the first matching element is visible |
| `PerformIsCheckboxChecked(string xpath)` | Returns `true` if the first matching checkbox is checked |
| `PerformIsSelected(string xpath)` | Returns `true` if the first matching element is selected |

### Text and attribute retrieval

| Method | Description |
|---|---|
| `PerformGetText(string xpath)` | Returns inner text of all matching elements |
| `PerformGetAttribute(string xpath, string attribute)` | Returns attribute values of all matching elements |
| `PerformGetSelectedText(string xpath)` | Returns selected option text from matching select elements |
| `PerformGetPageSource()` | Returns the current page's HTML content |

### Select options

| Method | Description |
|---|---|
| `PerformSelectOptionByIndex(string xpath, int index)` | Selects an option by index |
| `PerformSelectOptionByText(string xpath, string text)` | Selects an option by label text |
| `PerformSelectOptionByValue(string xpath, object value)` | Selects an option by value |
| `PerformGetSelectOptionsCount(string xpath)` | Returns the number of options in a select element |

### Script execution

| Method | Description |
|---|---|
| `PerformExecuteScript(string script)` | Executes JavaScript and returns the result as a string |

### Alert handling

| Method | Description |
|---|---|
| `PerformAcceptAlert()` | Accepts the pending dialog |
| `PerformDismissAlert()` | Dismisses the pending dialog |

### Iframe management

| Method | Description |
|---|---|
| `PerformSwitchToIframe(string xpath)` | Switches context to the iframe matching the XPath |

## Private helpers

| Helper | Purpose |
|---|---|
| `GetCurrentPage()` | Returns the active tab's page, or the default page if no tab is active |
| `GetLocator(string xpath)` | Creates a Playwright locator for the XPath, scoped to the current frame if set |
| `GetFirstLocator(string xpath)` | Waits for a visible element matching the XPath within `DefaultTimeout` |
| `GetLocators(string xpath)` | Waits for all elements matching the XPath within `DefaultTimeout` |
| `RegisterPage(IPage newPage)` | Registers a new page in the `pages` dictionary and subscribes to its dialog events |
| `GetPageId(IPage page)` | Returns the identifier for a registered page, or generates a new one |

## Error handling

- `PerformDoesElementExist` and `PerformIsElementVisible` catch exceptions and return `false`.
- `PerformGoToUrl` throws `Exception` if navigation fails after all retries.
- `PerformSwitchToIframe` throws `Exception` if the iframe element or its content frame cannot be resolved.
- `PerformSwitchToTab` throws `Exception` if the tab identifier does not exist.
- `GetFirstLocator` and `GetLocators` throw `Exception` if no matching element is found within `DefaultTimeout`.

## Concurrency

All Playwright async calls are blocked synchronously via `.GetAwaiter().GetResult()`. The class is not thread-safe; it is designed for single-threaded use within an automation session.

## Dependencies

- `NuciWeb.Automation` — `WebProcessor` base class and `IWebProcessor` interface
- `Microsoft.Playwright` — `IPage`, `IBrowserContext`, `IFrame`, `IDialog`, `ILocator`, `IElementHandle`, `SelectOptionValue`, `WaitUntilState`

## Tests

No unit tests exist in this repository for `PlaywrightWebProcessor`. Verification relies on manual inspection and the NuciWeb.Automation test suite.
