# WebDriverInitialiser

## Purpose

`WebDriverInitialiser` is a static factory class that creates and configures Playwright browser sessions. It provides methods to initialise Firefox, Chromium, or an automatically selected browser based on system availability.

## Location

`NuciWeb.Automation.Playwright/WebDriverInitialiser.cs`

## Public API

| Method | Parameters | Returns | Description |
|---|---|---|---|
| `InitialiseAvailableWebDriver` | `bool isDebugModeEnabled = true`, `int pageLoadTimeout = 90` | `PlaywrightSession` | Prefers Firefox if `/usr/bin/firefox` exists; otherwise uses Chromium |
| `InitialiseFirefoxDriver` | `bool isDebugModeEnabled = true`, `int pageLoadTimeout = 90` | `PlaywrightSession` | Creates a Firefox browser session |
| `InitialiseChromeDriver` | `bool isDebugModeEnabled = true`, `int pageLoadTimeout = 90` | `PlaywrightSession` | Creates a Chromium browser session |

## Internal implementation

### `InitialiseSession(BrowserType browserType, bool isDebugModeEnabled, int pageLoadTimeout)`

1. **User agent fetching** — Creates a `UserAgentFetcher` from `NuciWeb.HTTP` and calls `GetUserAgent()` synchronously.
2. **Playwright creation** — Calls `Microsoft.Playwright.Playwright.CreateAsync()` synchronously.
3. **Browser launch** — Calls `LaunchBrowser` with the specified `BrowserType` and debug mode flag.
4. **Context creation** — Creates a `BrowserNewContextOptions` with:
   - `UserAgent` from step 1
   - `ViewportSize` of 1920x1080
5. **Timeout configuration** — Sets `context.SetDefaultNavigationTimeout(pageLoadTimeout * 1000)`.
6. **Image blocking (headless mode)** — When `isDebugModeEnabled` is `false`, registers a route to abort requests matching `**/*.{png,jpg,jpeg,gif,webp,svg,ico,avif}`.
7. **Page creation** — Creates a new page via `context.NewPageAsync()` and sets its default navigation timeout.
8. **Session return** — Returns a new `PlaywrightSession` encapsulating the playwright instance, browser, context, and page.

### `LaunchBrowser(IPlaywright playwright, BrowserType browserType, bool isDebugModeEnabled)`

Creates `BrowserTypeLaunchOptions` with `Headless = !isDebugModeEnabled` and launches the appropriate browser:

- `BrowserType.Firefox` → `playwright.Firefox.LaunchAsync()`
- `BrowserType.Webkit` → `playwright.Webkit.LaunchAsync()`
- Default (`BrowserType.Chromium`) → `playwright.Chromium.LaunchAsync()`

### `BrowserType` enum

Internal enum with three values: `Chromium`, `Firefox`, `Webkit`.

## Configuration parameters

| Parameter | Default | Effect |
|---|---|---|
| `isDebugModeEnabled` | `true` | When `false`, enables headless mode and blocks image requests |
| `pageLoadTimeout` | `90` (seconds) | Navigation timeout applied to both context and page |

## Return value

`PlaywrightSession` — a sealed class implementing `IAsyncDisposable` that holds:
- `IPlaywright` — the Playwright instance
- `IBrowser` — the launched browser
- `IBrowserContext` — the browser context
- `IPage` — the initial page

## Disposal

The returned `PlaywrightSession` must be disposed via `await session.DisposeAsync()` to release browser resources. Disposal order: context, browser, playwright instance.

## Dependencies

- `NuciWeb.HTTP` — `UserAgentFetcher` for user agent strings
- `Microsoft.Playwright` — Playwright API for browser automation
- `System.IO` — `File.Exists` for Firefox detection

## Tests

No unit tests exist in this repository for `WebDriverInitialiser`. Verification relies on manual inspection and the NuciWeb.Automation test suite.
