# PlaywrightSession

## Purpose

`PlaywrightSession` encapsulates an active Playwright browser session, containing the Playwright instance, browser, browser context, and page. It implements `IAsyncDisposable` to ensure proper cleanup of browser resources.

## Location

`NuciWeb.Automation.Playwright/PlaywrightSession.cs`

## Type declaration

```csharp
public sealed class PlaywrightSession(
    IPlaywright playwright,
    IBrowser browser,
    IBrowserContext context,
    IPage page) : IAsyncDisposable
```

## Public properties

| Property | Type | Description |
|---|---|---|
| `Playwright` | `IPlaywright` | The Playwright instance |
| `Browser` | `IBrowser` — The launched browser |
| `Context` | `IBrowserContext` — The browser context |
| `Page` | `IPage` — The initial page |

## Disposal

Implements `IAsyncDisposable` with the following disposal order:
1. `Context.CloseAsync()` — closes the browser context
2. `Browser.CloseAsync()` — closes the browser
3. `Playwright.Dispose()` — disposes the Playwright instance

## Usage

```csharp
await using PlaywrightSession session = WebDriverInitialiser.InitialiseAvailableWebDriver();
```

## Key methods

- `DisposeAsync()` — releases all resources in the correct order
- No other public methods; session state is managed internally by Playwright

## Dependencies

- `Microsoft.Playwright` — `IPlaywright`, `IBrowser`, `IBrowserContext`, `IPage`
- `NuciWeb.Automation` — `IWebProcessor` interface (implemented by `PlaywrightWebProcessor`)

## Lifecycle

- Created by `WebDriverInitialiser`
- Used by `PlaywrightWebProcessor`
- Disposed by the consuming application via `await using` or explicit `DisposeAsync()`
