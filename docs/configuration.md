# Configuration

## Build configuration

### Target framework

```xml
<TargetFramework>net10.0</TargetFramework>
```

### Package references

| Package | Version | Purpose |
|---|---|---|
| `NuciWeb` | 4.0.0 | Core NuciWeb library |
| `NuciWeb.Automation` | 1.0.0 | `IWebProcessor` interface and `WebProcessor` base class |
| `NuciWeb.HTTP` | 1.5.0 | `UserAgentFetcher` for user agent strings |
| `Microsoft.Playwright` | 1.59.0 | Playwright browser automation API |

### Project properties

```xml
<Nullable>enable</Nullable>
<ImplicitUsings>enable</ImplicitUsings>
<GenerateDocumentationFile>true</GenerateDocumentationFile>
<PackageLicenseExpression>GPL-3.0-only</PackageLicenseExpression>
<PackageReadmeFile>README.md</PackageReadmeFile>
<RepositoryUrl>https://github.com/hmlendea/nuciweb.automation.playwright</RepositoryUrl>
<RepositoryType>git</RepositoryType>
<PublishRepositoryUrl>true</PublishRepositoryUrl>
<IncludeSymbols>true</IncludeSymbols>
<SymbolPackageFormat>snupkg</SymbolPackageFormat>
<ContinuousIntegrationBuild>true</ContinuousIntegrationBuild>
```

## Runtime configuration

### Browser selection

`WebDriverInitialiser` provides three factory methods:

| Method | Browser | Selection logic |
|---|---|---|
| `InitialiseAvailableWebDriver()` | Firefox or Chromium | Prefers Firefox if `/usr/bin/firefox` exists; otherwise Chromium |
| `InitialiseFirefoxDriver()` | Firefox | Explicit Firefox |
| `InitialiseChromeDriver()` | Chromium | Explicit Chromium |

### Debug mode

| Parameter | Default | Effect when `true` | Effect when `false` |
|---|---|---|---|
| `isDebugModeEnabled` | `true` | Headed mode (visible browser) | Headless mode + image blocking |

### Page load timeout

| Parameter | Default | Unit |
|---|---|---|
| `pageLoadTimeout` | `90` | seconds |

Applied to both `IBrowserContext.SetDefaultNavigationTimeout()` and `IPage.SetDefaultNavigationTimeout()`.

### Navigation retries

| Parameter | Default | Unit |
|---|---|---|
| `httpRetries` | `3` | attempts |
| `retryDelay` | `default(TimeSpan)` | TimeSpan |

Used by `PerformGoToUrl` for retry logic.

### Viewport

Fixed at 1920x1080 in `BrowserNewContextOptions`.

### Image blocking (headless mode)

When `isDebugModeEnabled = false`, routes matching `**/*.{png,jpg,jpeg,gif,webp,svg,ico,avif}` are aborted.

## Environment requirements

### Playwright browsers

Playwright browsers must be installed. The project includes a PowerShell script at `bin/Debug/net10.0/playwright.ps1` that installs browsers:

```powershell
pwsh bin/Debug/net10.0/playwright.ps1 install
```

### Firefox detection

`InitialiseAvailableWebDriver` checks for `/usr/bin/firefox` on Linux. Adjust path for other platforms.

## NuGet packaging

The project is configured for NuGet publishing with:
- GPL-3.0-only license
- SourceLink enabled
- Symbol packages (snupkg)
- Repository metadata

## CI/CD

GitHub Actions workflows (if present) should:
1. Restore NuGet packages
2. Build the project
3. Run Playwright browser installation
4. Execute tests (if any)
5. Pack and publish to NuGet.org
