# Privacy and Personal Data

This document describes how NuciWeb.Automation.Playwright at https://github.com/hmlendea/nuciweb.automation.playwright handles personal data. It covers the application behaviour and verified integrations described below. Where the software is self-hosted, the instance operator may have separate responsibilities described below.

## 📑 Table of Contents

- [What This Document Covers](#what-this-document-covers)
- [Data We Handle](#data-we-handle)
- [Processing and Use](#processing-and-use)
- [Storage, Retention, and Deletion](#storage-retention-and-deletion)
- [External Processing and Integrations](#external-processing-and-integrations)
- [Data Protection and Security](#data-protection-and-security)
- [Document Changes](#document-changes)
- [Contact](#contact)

## 🔎 What This Document Covers

This document describes how NuciWeb.Automation.Playwright at https://github.com/hmlendea/nuciweb.automation.playwright handles personal data. It covers the application behaviour and verified integrations described below. Where the software is self-hosted, the instance operator may have separate responsibilities described below.

## 📥 Data We Handle

### Data Provided to the Application

The library does not request, collect, or store personal data from users. It operates as a browser automation tool that interacts with web pages through the user's own browser session. Any data entered into the automated browser (such as text input or navigation URLs) is provided by the calling application and is not stored by the library.

### Data Generated or Collected by the Application

The library does not generate or collect personal data automatically. It does not include telemetry, analytics, crash reporting, or update-check functionality. All operations occur locally within the user's environment.

### Data Received from Integrations

The library does not receive personal data from integrations or third parties. It interacts only with the local browser instance and the web pages being automated.

## 🧭 Processing and Use

The application processes the data described above for these verified functions:

- Browser automation operations — Data provided by the calling application (text input, navigation URLs, element interactions)
- XPath-based element location — Data provided by the calling application (XPath selectors)

All processing occurs locally within the user's environment and does not involve transmission of personal data to the project maintainers or any external service.

## 🗄️ Storage, Retention, and Deletion

The library does not persist any data to disk. It operates entirely in memory during the browser session lifecycle. When the Playwright session is disposed, all browser state, pages, and context are closed and their resources are released. No data is retained after the session ends.

## 🔗 External Processing and Integrations

The library has no built-in external data transfer. It does not send data to project maintainers, analytics services, crash reporters, or any external processor. All operations are confined to the local browser instance.

| Service or integration | Purpose | Data involved | Configuration or documentation |
|-----------------------|---------|---------------|--------------------------------|
| — | — | — | — |

## 🛡️ Data Protection and Security

The library does not collect, store, or transmit personal data, and therefore requires no data protection safeguards beyond standard software development practices. Security for any automated browser session is the responsibility of the instance operator, who controls the browser configuration, network environment, and any data entered into the automated pages. The project maintainers do not have access to data processed by the library.

## 🔄 Document Changes

Update this document when application data flows, storage, integrations, or deployment responsibilities change. The current version is published at https://github.com/hmlendea/nuciweb.automation.playwright/blob/main/PRIVACY.md.

## 📬 Contact

For questions about application data handling, contact the project maintainers through the repository at https://github.com/hmlendea/nuciweb.automation.playwright. Do not send passwords, access tokens, or other secrets.