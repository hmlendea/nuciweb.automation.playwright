# Contributing to NuciWeb.Automation.Playwright

This document covers the guidelines and processes for contributing to this project, including how to report issues, suggest enhancements, submit code changes, and follow project standards.

## 📑 Table of Contents

- [How to Contribute](#how-to-contribute)
- [Reporting Issues](#reporting-issues)
- [Suggesting Enhancements](#suggesting-enhancements)
- [Code Contributions](#code-contributions)
- [Documentation](#documentation)
- [Security](#security)
- [License](#license)

## 🤝 How to Contribute

### Reporting Issues

- Search existing issues first.
- Use the issue templates if available.
- Provide clear reproduction steps.
- Include environment details.

### Suggesting Enhancements

- Check the roadmap and existing discussions.
- Explain the use case and expected behaviour.
- Consider implementation complexity.

### Code Contributions

#### Prerequisites

- .NET 10 or newer SDK
- Playwright browsers installed for .NET runtime

#### Development Setup

```bash
# Clone the repository
git clone https://github.com/hmlendea/nuciweb.automation.playwright.git
cd nuciweb.automation.playwright

# Install dependencies
dotnet restore

# Install Playwright browsers
pwsh bin/Debug/net10.0/playwright.ps1 install
```

#### Making Changes

1. Fork the repository.
2. Create a feature branch: `git checkout -b feature/your-feature-name`
3. Make your changes.
4. Commit with clear and descriptive messages.
5. Push to your fork.
6. Open a Pull Request.

#### Pull Request Guidelines

- Target the `master` branch.
- Keep PRs focused and atomic.
- Revise documentation if applicable.
- Ensure the CI checks pass.

### Documentation

- Revise relevant docs for changes.
- Follow the documentation style guide.
- Preview changes locally if possible.

## 🔒 Security

Report security vulnerabilities per the [Security Policy](./SECURITY.md).

## 📄 License

By contributing, you agree that your contributions will be licensed under the `GNU General Public License v3.0` or later.