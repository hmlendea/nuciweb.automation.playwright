# Repository structure

## Source tree layout

```
nuciweb.automation.playwright/
├── .github/
│   └── workflows/
│       └── dotnet.yml              # CI: build, restore, build, test
├── NuciWeb.Automation.Playwright/
│   ├── NuciWeb.Automation.Playwright.csproj   # Project file (net10.0, NuGet metadata)
│   ├── PlaywrightWebProcessor.cs   # IWebProcessor adapter implementation
│   ├── WebDriverInitialiser.cs     # Browser session factory + PlaywrightSession
│   ├── bin/                        # Build output (gitignored)
│   └── obj/                        # Build artifacts (gitignored)
├── ARCHITECTURE.md                 # Root architecture document
├── CODE_OF_CONDUCT.md              # Code of conduct
├── CONTRIBUTING.md                 # Contributing guidelines
├── LICENSE                         # GPL-3.0-or-later
├── PRIVACY.md                      # Privacy policy
├── README.md                       # Project overview and usage
├── SECURITY.md                     # Security policy
├── NuciWeb.Automation.Playwright.slnx  # SDK-style solution file
└── docs/                           # Generated documentation
```

## Module organisation

### NuciWeb.Automation.Playwright (main project)

A single .NET class library project targeting `net10.0`. It contains three source files:

| File | Namespace | Types | Responsibility |
|---|---|---|---|
| `PlaywrightWebProcessor.cs` | `NuciWeb.Automation.Playwright` | `PlaywrightWebProcessor` | Adapter implementing `IWebProcessor` via Playwright |
| `WebDriverInitialiser.cs` | `NuciWeb.Automation.Playwright` | `PlaywrightSession`, `WebDriverInitialiser`, `BrowserType` | Factory for browser sessions and session container |

### Build configuration

- **Solution file:** `NuciWeb.Automation.Playwright.slnx` — SDK-style, single project reference.
- **Project file:** `NuciWeb.Automation.Playwright.csproj` — targets `net10.0`, version `1.0.2`, licensed under GPL-3.0-or-later.
- **CI workflow:** `.github/workflows/dotnet.yml` — runs on `ubuntu-latest`, triggers on push and pull request to `master`.

### Dependencies

| Package | Version | Purpose |
|---|---|---|
| `NuciWeb` | 4.0.0 | Core NuciWeb abstractions |
| `NuciWeb.Automation` | 1.0.0 | `IWebProcessor` interface and `WebProcessor` base class |
| `NuciWeb.HTTP` | 1.5.0 | `UserAgentFetcher` for browser user agent strings |
| `Microsoft.Playwright` | 1.59.0 | Playwright browser automation API |

### Generated artifacts (gitignored)

- `bin/` and `obj/` directories contain build outputs and NuGet restore artifacts.
- `bin/Debug/net10.0/playwright.ps1` — Playwright CLI driver script for browser installation.
- `bin/Debug/net10.0/.playwright/` — Playwright Node.js driver and package.
