# About
This is a test exercise, the goal of which was to develop a web app (SPA) using
Angular, which allows working with categories of products by using a proxy web
API, which has to be developed using <span>ASP</span>.NET Core. The proxy web
API had to be a proxy for another web API, which exposes the access to the
actual data.

Both apps had to be as light as possible, and it wasn't required to make them
ready for production, so they may miss important features and properties of
proper production-ready apps.

# Prerequisites
To run this project on Windows, make sure that you have:
* The latest LTS or Current version of Node.js with the npm package manager.
* The latest version of Visual Studio 2019 (any edition) with the
  "<span>ASP</span>.NET and web development" workload.

# Running the project
On Windows, open the solution file in Visual Studio, and hit F5 (start
debugging). The default web browser should open (or a new tab in it, if it's
already opened), showing the web app (the login view).

# Dependencies
Web app:
* Angular 8.<br />
  At the moment of creating this project, the latest is 9, but it wasn't used,
  because currently there is an issue in the latest .NET Core (3.1.4) preventing
  Angular 9 apps from being started by a .NET Core app. The test exercise
  required making it possible to run everything just by opening Visual Studio
  and hitting F5, so Angular 8 had to be used to meet this requirement.
* Angular Material.<br />
  It was chosen mostly because of its Table component, which supports sorting
  and filtering out of the box, but since it also provides other required
  components and its own design styling (Material Design), it was used for these
  purposes as well, to keep the design of the UI consistent as much as possible.
* Bootstrap.<br />
  Is used for the overall layout, and because of some components that Angular
  Material doesn't have.
* Jasmine.<br />
  Is used as a testing framework for unit and integration tests.
* Karma.<br />
  Is used as a test runner for Jasmine tests. It runs tests in the Chrome web
  browser, which must be installed in order to run such tests.

Web API:
* <span>ASP</span>.NET Core.<br />
  The recommended technology to build Web APIs in .NET. It was required by the
  test exercise.
* NSubstitute.<br />
  Is used in unit tests for creating fake and configurable dependencies of
  systems under test.
* NUnit.<br />
  Is used as a testing framework for unit and integration tests. There is also
  a dependency on its test adapter, which makes it possible to run NUnit tests
  using Visual Studio's built-in test runner.
* Polly.<br />
  Is used for handling transient errors that may happen between this app and the
  web API that it uses.