# Security Policy

This policy defines how to report security vulnerabilities affecting NuciWeb.Automation.Playwright, the supported versions that receive security maintenance, and the coordinated disclosure process followed by the maintainers.

## 📑 Table of Contents

- [Supported Versions](#supported-versions)
- [Reporting a Vulnerability](#reporting-a-vulnerability)
- [Scope](#scope)
- [Disclosure Policy](#disclosure-policy)

## 🛡️ Supported Versions

Use this table to indicate which project versions currently receive security maintenance.

| Version | Distribution Channel | Supported |
|---------|--------------------|-----------|
| Latest version | NuGet | ✅ |
| Latest version | GitHub Releases | ✅ |
| Latest version | Unofficial third-party distribution channels | ❌ |
| Preceding versions | Any distribution channel | ❌ |

## 🚨 Reporting a Vulnerability

Please do not disclose suspected vulnerabilities publicly before maintainers have had an opportunity to validate and remediate them.

To report a vulnerability:
- [GitHub Security Advisories](https://github.com/hmlendea/nuciweb.automation.playwright/security/advisories)
- Contact the maintainers directly

## 📌 Scope

The subsequent report categories are in scope for this repository:
- Browser automation library behaviour and Playwright integration
- Session initialisation and browser launch configuration
- Web processor implementation and element interaction logic

The subsequent categories are out of scope unless explicitly stated to the contrary:
- Third-party browser engine (Playwright, Firefox, Chromium) vulnerabilities
- Issues in downstream packages or consuming applications
- Social engineering or physical security concerns

## 📢 Disclosure Policy

This project follows coordinated disclosure:
1. Vulnerabilities are investigated privately.
2. A remediation plan is prepared and validated.
3. Public disclosure is published after a fix, mitigation, or agreed risk decision is available.
4. Credit is attributed in accordance with reporter preference and project policy.