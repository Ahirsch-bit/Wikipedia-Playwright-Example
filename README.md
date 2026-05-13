# Wikipedia automation (lightweight)

Small Playwright + NUnit project that opens the English Wikipedia article *Test automation*, pulls the “Test-driven development” heading and the paragraph that follows it from the UI, fetches the same section from the MediaWiki parse API, normalizes both strings, and checks that **word frequencies** match.

---

## Requirements, build, and run

**You need**

- **.NET SDK 10** (projects target `net10.0`)
- **PowerShell** (`pwsh` or Windows PowerShell)

**Build** (from the repo root, same folder as the `.sln`):

```powershell
dotnet restore .\WikipediaAutomationProject.sln
dotnet build .\WikipediaAutomationProject.sln -c Debug
```

**Install browsers** (first time, or after a clean output folder):

```powershell
pwsh .\bin\Debug\net10.0\playwright.ps1 install
```


**Run tests**

```powershell
dotnet test .\WikipediaAutomationProject.csproj -c Debug
```

`BaseTest` launches Chromium with `Headless = false` so you can watch the page load. For CI, I would flip that to headless and probably reuse a single browser instance across tests instead of spinning one up per class.

---

## Project layout (by folder)

- **`Pages`** — thin page objects: `BasePage` for navigation, `TestAutomationWikipdediaPage` for the TDD section locators and text extraction.
- **`Api`** — `WikipediaApiClient` calls the parse API, `WikipediaContract` is the JSON shape. The client strips scripts, styles, superscripts, ordered lists, and the references wrapper with HtmlAgilityPack before taking inner text, so we are comparing readable text, not raw HTML.
- **`Services`** — `TextNormalizer` and `WordCounter`. No static “framework”; just helpers the test wires together.
- **`Tests`** — `BaseTest` owns Playwright lifecycle; `WikipediaTests` is the scenario.

Everything ships in one assembly. If this ever grew into something multiple teams or repos consumed, I would split UI helpers, API clients, and tests into separate projects or versioned packages and treat compatibility seriously.

---

## Why `HttpClient` instead of Playwright for the API half

Playwright can drive HTTP from the same process as the browser, which is great when you need **shared context** between an API step and a UI step (cookies, storage state, tracing one user journey end-to-end).

Here the API call is only there to get a second text blob to compare against the UI. There is no session handoff and no need to correlate with a specific browser context. `HttpClient` is simpler, has fewer dependencies in the mental model, and keeps “browser stuff” in Playwright and “HTTP stuff” in plain .NET.

In a production-grade stack I might still use a typed client, Polly retries, centralized base URLs per environment, and contract tests against OpenAPI—or keep Playwright’s request context when the story genuinely mixes UI and authenticated API calls.

---

## Normalization and the assertion

Wikipedia will never give you a byte-for-byte match between API HTML and what a user sees. The pipeline is intentionally boring: normalize both sides the same way (drop bracket references like `[4]`, turn punctuation into word separators, lowercase), then split on whitespace and count.

The assertion uses NUnit’s `Is.EquivalentTo` on the two dictionaries so order does not matter but **counts** do.

---

## Dictionary vs `HashSet` for words

`WordCounter` builds a `Dictionary<string, int>` because the test cares about **how often** each token appears. A direct frequency comparison catches cases where the UI and API agree on vocabulary but disagree on repetition.

If the requirement were only “do the two sides mention the same distinct words?” with no frequency signal, a `HashSet<string>` per side (or sorting two deduplicated lists) would be enough and slightly cheaper to reason about.

---

## If infrastructure had been in scope

This repo stays thin on purpose. In a production grade infrastructure, I would have pushed the same ideas into a more “framework-shaped” solution, along the lines of:

- **Solution layout** — separate projects for reusable UI automation (locators, waits, shared browser setup), API clients and response normalization, and the test assembly that references both. Shared libraries would be versioned (NuGet or internal feeds) when more than one suite needs them.
- **Logging and observability** — structured logs, correlation IDs, and distributed tracing across API and UI steps to make debugging easier when something goes wrong.
- **Reporting** — HTML (or similar) reports with traces and failure artifacts so triage does not depend on scrolling console output.
- **Pipeline and operations** — CI, retries for flaky network, environment-based configuration, sensible parallelization, structured logging, and optional contract or schema checks on API responses.
- **Exception Handling** — retries, timeouts, and custom exceptions for common failure modes (for example, “section not found” from the API or “element not found” from the UI) to make test results more actionable.

Since the scope of the assignment is to demonstrate a single test end-to-end, I kept the code focused on the core scenario and avoided adding infrastructure that would be overkill for this size of project. The above points are areas I would invest in as the suite grew in size and complexity to maintain reliability and ease of maintenance.

---

## Small operational note

The Wikipedia API expects a descriptive `User-Agent`. Replace the placeholder in `WikipediaApiClient` with something that identifies you before you run this often or from shared infrastructure.
