# Repository overview

## Purpose

NuciWeb.Automation.Playwright provides a Playwright-based implementation of the `IWebProcessor` interface from the NuciWeb.Automation package. It allows .NET applications to drive real browsers (Firefox, Chromium, Webkit) through a common automation abstraction, keeping higher-level automation logic independent from specific browser tooling.

## Scope

### What the repository owns

- The `PlaywrightWebProcessor` class, which adapts the Playwright API to the `IWebProcessor` interface.
- The `WebDriverInitialiser` class, which creates and configures Playwright browser sessions.
- The `PlaywrightSession` class, which encapsulates an active Playwright browser session (playwright instance, browser, context, page).
- The NuGet package definition, build configuration, and CI pipeline.

### What the repository deliberately does not own

- The `IWebProcessor` interface and `WebProcessor` base class — these are defined in the external `NuciWeb.Automation` package.
- Browser engine binaries — these are provided by the Playwright runtime and must be installed separately.
- User agent fetching — delegated to the `NuciWeb.HTTP` package's `UserAgentFetcher`.
- Application-level automation orchestration — consuming applications own this logic.
- Persistent state — the library maintains no persistent state; all state is ephemeral within browser sessions.

## Entry points

### Public API

| Entry point | Type | Purpose |
|---|---|---|
| `WebDriverInitialiser.InitialiseAvailableWebDriver` | Static method | Creates a Playwright browser session, preferring Firefox when available, otherwise Chromium |
| `WebDriverInitialiser.InitialiseFirefoxDriver` | Static method | Creates a Firefox browser session |
| `WebDriverInitialiser.InitialiseChromeDriver` | Static method | Creates a Chromium browser session |
| `PlaywrightWebProcessor(IPage page)` | Constructor | Creates a processor wrapping a Playwright page |
| `PlaywrightSession` | Class | Encapsulates an active browser session; implements `IAsyncDisposable` |

### Build and test

- Build: `dotnet build`
- Test: `dotnet test`
- Playwright browser installation: `pwsh bin/Debug/net10.0/playwright.ps1 install`

## Principal capabilities

- Browser automation through the `IWebProcessor` abstraction (navigation, clicking, text input, selection, alerts, tabs, iframes, script execution).
- Automatic browser session initialisation with configurable headless mode, viewport size, user agent, and navigation timeout.
- Image request blocking in headless mode for improved performance.
- Retry logic for navigation with Chromium error-page detection.
- Element location via XPath selectors with visibility-aware waiting.
