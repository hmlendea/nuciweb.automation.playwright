# Maintenance

## Repository structure

```
nuciweb.automation.playwright/
├── NuciWeb.Automation.Playwright.slnx          # Solution file
├── NuciWeb.Automation.Playwright/
│   ├── NuciWeb.Automation.Playwright.csproj    # Project file
│   ├── PlaywrightWebProcessor.cs               # IWebProcessor adapter
│   ├── WebDriverInitialiser.cs                 # Session factory
│   └── PlaywrightSession.cs                    # Session container
├── docs/                                       # Documentation
│   ├── INDEX.md
│   ├── quick-start.md
│   ├── architecture.md
│   ├── components/
│   │   ├── playwright-web-processor.md
│   │   ├── web-driver-initialiser.md
│   │   └── playwright-session.md
│   ├── data-model.md
│   ├── configuration.md
│   └── maintenance.md
├── LICENSE                                     # GPL-3.0 license
├── README.md
├── ARCHITECTURE.md
├── CONTRIBUTING.md
├── CODE_OF_CONDUCT.md
├── PRIVACY.md
├── SECURITY.md
└── ROADMAP.md
```

## Build commands

### Restore

```bash
dotnet restore
```

### Build

```bash
dotnet build --configuration Debug
```

### Clean build

```bash
dotnet clean && dotnet build --configuration Debug
```

### Pack (NuGet)

```bash
dotnet pack --configuration Release
```

## Playwright browser installation

After building, install Playwright browsers:

```bash
pwsh bin/Debug/net10.0/playwright.ps1 install
```

## Dependency updates

### NuGet packages

Update package versions in `NuciWeb.Automation.Playwright.csproj`:

| Package | Current | Check |
|---|---|---|
| `NuciWeb` | 4.0.0 | `dotnet list package --outdated` |
| `NuciWeb.Automation` | 1.0.0 | `dotnet list package --outdated` |
| `NuciWeb.HTTP` | 1.5.0 | `dotnet list package --outdated` |
| `Microsoft.Playwright` | 1.59.0 | `dotnet list package --outdated` |

### Playwright version

When updating `Microsoft.Playwright`, run the browser installer script to ensure compatible browser binaries:

```bash
pwsh bin/Debug/net10.0/playwright.ps1 install
```

## Adding new IWebProcessor methods

1. Add the method to the `IWebProcessor` interface in `NuciWeb.Automation`.
2. Add the corresponding `Perform*` override in `WebProcessor` base class.
3. Implement the method in `PlaywrightWebProcessor.cs`.
4. Add documentation to `docs/components/playwright-web-processor.md`.
5. Update `docs/data-model.md` if the interface signature changes.

## Adding new browser types

1. Add the new type to the `BrowserType` enum in `WebDriverInitialiser.cs`.
2. Add a case in `LaunchBrowser` to handle the new type.
3. Add a factory method (e.g., `InitialiseSafariDriver`) if needed.
4. Update `docs/components/web-driver-initialiser.md`.

## Testing

No unit tests exist in this repository. To add tests:

1. Create a `tests/` directory with a test project.
2. Reference `NuciWeb.Automation.Playwright` and a test framework (xUnit/NUnit).
3. Test `PlaywrightWebProcessor` methods against a local Playwright instance.
4. Test `WebDriverInitialiser` factory methods for session creation.

## Documentation updates

When modifying source code:

1. Update the corresponding component documentation in `docs/components/`.
2. Update `docs/data-model.md` if interfaces or data structures change.
3. Update `docs/configuration.md` if configuration parameters change.
4. Update `docs/architecture.md` if the architecture changes.
5. Update `docs/quick-start.md` if usage examples change.
6. Update `docs/INDEX.md` if the documentation structure changes.

## Release process

1. Update version in `.csproj` if needed.
2. Ensure all documentation is up to date.
3. Build in Release configuration.
4. Pack NuGet package.
5. Publish to NuGet.org.
6. Create a Git tag.
7. Update `README.md` with release notes if needed.

## Troubleshooting

### Playwright browsers not found

Run the browser installer script:

```bash
pwsh bin/Debug/net10.0/playwright.ps1 install
```

### Firefox not detected

`InitialiseAvailableWebDriver` checks `/usr/bin/firefox`. If Firefox is installed elsewhere, use `InitialiseFirefoxDriver` directly or adjust the path check.

### Navigation timeouts

Increase `pageLoadTimeout` parameter in `WebDriverInitialiser` factory methods.

### Element not found

Check that XPath selectors are correct. `GetFirstLocator` and `GetLocators` wait up to `DefaultTimeout` (30 seconds) for elements to appear.
