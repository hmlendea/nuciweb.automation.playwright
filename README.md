[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/funding) [![Build Status](https://github.com/hmlendea/nuciweb.automation.playwright/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nuciweb.automation.playwright/actions/workflows/dotnet.yml) [![Latest Release](https://img.shields.io/github/v/release/hmlendea/nuciweb.automation.playwright)](https://github.com/hmlendea/nuciweb.automation.playwright/releases/latest)

# NuciWeb.Automation.Playwright

## 📑 Table of Contents
- [About](#about)
- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Usage](#usage)
- [Browser Initialisation](#browser-initialisation)
- [Notes](#notes)
- [Architecture](#architecture)
- [Privacy and Data](#privacy-and-data)
- [Security](#security)
- [Contributing](#contributing)
- [Code of Conduct](#code-of-conduct)
- [License](#license)

## About

NuciWeb.Automation.Playwright provides a Playwright-based implementation for the `NuciWeb.Automation` abstractions.
It is intended for projects that want to drive a real browser through the `IWebProcessor` interface while keeping higher-level automation logic independent from browser tooling.

The package exposes two main building blocks:

- `PlaywrightWebProcessor`, an `IWebProcessor` implementation backed by Playwright pages
- `WebDriverInitialiser`, a helper that creates a configured Playwright browser session

## Features

- Implements browser automation operations through the `NuciWeb.Automation` processor model
- Supports navigation, clicking, text input, selection, alerts, tabs, iframes, and script execution
- Includes automatic browser session initialisation for Firefox and Chromium
- Supports headless execution for non-debug scenarios
- Applies practical browser defaults such as configurable navigation timeout and optional image blocking

## Requirements

- .NET 10 or newer
- Playwright browsers installed for .NET runtime:

```bash
pwsh bin/Debug/net10.0/playwright.ps1 install
```

## Installation

[![Get it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciWeb.Automation.Playwright)

**.NET CLI**:
```bash
dotnet add package NuciWeb.Automation.Playwright
```

**Package Manager**:
```powershell
Install-Package NuciWeb.Automation.Playwright
```

## Usage

Create a Playwright session, pass its main page to `PlaywrightWebProcessor`, and use the processor through the `NuciWeb.Automation` API:

```csharp
using NuciWeb.Automation;
using NuciWeb.Automation.Playwright;

await using PlaywrightSession session = WebDriverInitialiser.InitialiseAvailableWebDriver(
    isDebugModeEnabled: false,
    pageLoadTimeout: 90);

IWebProcessor processor = new PlaywrightWebProcessor(session.Page);

processor.GoToUrl("https://example.com");
processor.SetText("//input[@name='q']", "nuciweb");
processor.Click("//button[@type='submit']");
```

If you want to target a specific browser explicitly, use one of the dedicated initialisers:

```csharp
await using PlaywrightSession firefox = WebDriverInitialiser.InitialiseFirefoxDriver();
await using PlaywrightSession chromium = WebDriverInitialiser.InitialiseChromeDriver(isDebugModeEnabled: false);
```

## Browser Initialisation

`WebDriverInitialiser.InitialiseAvailableWebDriver()` chooses the browser as follows:

1. If `/usr/bin/firefox` exists, it creates a Firefox browser session.
2. Otherwise, it creates a Chromium browser session.

Each session is configured with:

- a configurable page load timeout
- a configurable user agent
- 1920x1080 viewport size

When debug mode is disabled, the initialiser enables headless mode and blocks common image file requests.

## Notes

- XPath selectors are used throughout the processor API.
- Navigation includes retry logic and a Chromium error-page check before failing.
- Make sure Playwright browser binaries are installed in the runtime environment before running automations.

## 🏗️ Architecture

NuciWeb.Automation.Playwright is a Playwright-based adapter that implements the `IWebProcessor` interface from NuciWeb.Automation.

See the [architecture documentation](./ARCHITECTURE.md) for the system context, principal components, runtime flows, ownership boundaries, dependencies, constraints, and extension points.

## 🛡️ Privacy and Data

NuciWeb.Automation.Playwright does not collect, store, or transmit personal data. It operates as a browser automation library that interacts with web pages through the user's own browser session.

For the detailed description of how the application handles privacy and personal data, see [PRIVACY.md](./PRIVACY.md).

The library does not collect, store, or transmit personal data. All operations occur locally within the user's environment, and no telemetry, analytics, or crash reporting functionality is included.

## 🔒 Security

NuciWeb.Automation.Playwright is a browser automation library that operates locally within the user's environment. Security concerns related to the library itself should be reported through the repository's security policy.

For the detailed security policy, including how to report vulnerabilities, see [SECURITY.md](./SECURITY.md).

## 🤝 Contributing

You are welcome to submit any suggestion, feedback, or modification to this project.

When doing so, please:
- Maintain cross-platform compatibility
- Preserve the existing public contract unless a breaking change is intentional
- Submit focused pull requests that conform to the existing code style
- Maintain your branch synchronised with `master`
- Revise the documentation when functionality changes
- Properly test all modifications, including edge cases and error conditions
- Include tests for additional or modified functionality
- Raise a new [issue](https://github.com/hmlendea/nuciweb.automation.playwright/issues) for problems or suggestions

See the [contributing guidelines](./CONTRIBUTING.md) for details on how to report issues, suggest enhancements, and submit changes.

## 📜 Code of Conduct

See the [Code of Conduct](./CODE_OF_CONDUCT.md) for the standards of conduct expected in all project spaces.

## License

This project is licensed under the `GNU General Public License v3.0` or later. See [LICENSE](./LICENSE) for details.