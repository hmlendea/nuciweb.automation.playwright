# Architecture

## Overview

NuciWeb.Automation.Playwright is an adapter library that bridges the NuciWeb.Automation `IWebProcessor` interface with Microsoft Playwright's browser automation API. It enables NuciWeb applications to use Playwright as their web automation engine without direct Playwright dependencies in the application layer.

## Architectural pattern

**Adapter pattern** — `PlaywrightWebProcessor` adapts Playwright's asynchronous API to the synchronous `IWebProcessor` interface defined by NuciWeb.Automation.

## Component diagram

```
┌─────────────────────────────────────────────────────────┐
│                    Application Layer                     │
│                    (NuciWeb consumer)                     │
└──────────────────────┬──────────────────────────────────┘
                       │
                       │ IWebProcessor
                       │
┌──────────────────────▼──────────────────────────────────┐
│              NuciWeb.Automation                         │
│  ┌───────────────────────────────────────────────────┐  │
│  │  IWebProcessor (interface)                       │  │
│  │  WebProcessor (abstract base class)              │  │
│  └───────────────────────────────────────────────────┘  │
└──────────────────────┬──────────────────────────────────┘
                       │
                       │ extends / implements
                       │
┌──────────────────────▼──────────────────────────────────┐
│              NuciWeb.Automation.Playwright              │
│  ┌───────────────────────────────────────────────────┐  │
│  │  PlaywrightWebProcessor (sealed)                   │  │
│  │  - Implements IWebProcessor via Playwright       │  │
│  │  - Synchronous wrapper around async Playwright   │  │
│  └───────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────┐  │
│  │  WebDriverInitialiser (static factory)             │  │
│  │  - Creates PlaywrightSession instances             │  │
│  └───────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────┐  │
│  │  PlaywrightSession (sealed, IAsyncDisposable)      │  │
│  │  - Holds IPlaywright, IBrowser, IBrowserContext,   │  │
│  │    IPage                                           │  │
│  └───────────────────────────────────────────────────┘  │
└──────────────────────┬──────────────────────────────────┘
                       │
                       │ uses
                       │
┌──────────────────────▼──────────────────────────────────┐
│              External Dependencies                      │
│  ┌───────────────────────────────────────────────────┐  │
│  │  Microsoft.Playwright 1.59.0                       │  │
│  │  - Browser automation engine                       │  │
│  └───────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────┐  │
│  │  NuciWeb.HTTP 1.5.0                                │  │
│  │  - UserAgentFetcher for browser user agent         │  │
│  └───────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────┐  │
│  │  NuciWeb 4.0.0                                     │  │
│  │  - Core NuciWeb library                            │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

## Layers

### 1. Application layer

Consumes `IWebProcessor` through NuciWeb.Automation. The application is unaware of Playwright.

### 2. Contract layer (NuciWeb.Automation)

Defines `IWebProcessor` interface and `WebProcessor` abstract base class. Provides the `Perform*` method pattern that concrete implementations override.

### 3. Adapter layer (NuciWeb.Automation.Playwright)

Three components:

- **`PlaywrightWebProcessor`** — Implements `IWebProcessor` by translating each method call to Playwright API calls. Blocks on async operations using `.GetAwaiter().GetResult()`.
- **`WebDriverInitialiser`** — Static factory that creates `PlaywrightSession` instances with configured browser type, viewport, user agent, and timeouts.
- **`PlaywrightSession`** — Container for the Playwright runtime, browser, context, and page. Implements `IAsyncDisposable` for resource cleanup.

### 4. Infrastructure layer

External packages providing browser automation and HTTP utilities.

## Data flow

```
Application
    │
    │ calls IWebProcessor methods
    ▼
PlaywrightWebProcessor
    │
    │ translates to Playwright API calls
    ▼
Playwright (browser automation)
    │
    │ controls
    ▼
Browser (Firefox / Chromium / WebKit)
```

## Session lifecycle

```
┌─────────────┐
│   Create    │
│ WebDriver-  │
│ Initialiser │
│ .Initialise │
│ *WebDriver()│
└──────┬──────┘
       │
       │ returns
       ▼
┌─────────────┐
│Playwright-  │
│ Session     │
│ (IAsyncDis- │
│ posable)    │
└──────┬──────┘
       │
       │ passed to
       ▼
┌─────────────┐
│Playwright-  │
│ WebProcessor│
│ (IWebProces-│
│ sor)        │
└──────┬──────┘
       │
       │ used for
       │ automation
       │
       │
       ▼
┌─────────────┐
│   Dispose   │
│             │
│ session     │
│ .Dispose-   │
│ Async()     │
└─────────────┘
```

## Concurrency model

- Single-threaded usage within an automation session
- All Playwright async calls are blocked synchronously
- No thread-safety guarantees; not designed for concurrent access

## Error handling strategy

- Element existence and visibility checks return `false` on failure
- Navigation failures throw after retry exhaustion
- Element lookup failures throw after timeout
- Alert and iframe operations throw on resolution failure

## Design decisions

1. **Synchronous wrapper** — `IWebProcessor` is synchronous; Playwright is async. The adapter blocks on async calls to maintain interface compatibility.
2. **Adapter pattern** — Decouples NuciWeb applications from Playwright, allowing alternative browser automation engines.
3. **Static factory** — `WebDriverInitialiser` uses static methods for simple session creation without dependency injection.
4. **IAsyncDisposable** — `PlaywrightSession` implements `IAsyncDisposable` for proper async resource cleanup.
5. **Image blocking in headless mode** — Reduces bandwidth and speeds up page loads during automated testing.
