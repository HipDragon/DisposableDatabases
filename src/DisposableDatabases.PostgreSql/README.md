# Disposable Databases - PostgreSQL
An easy and flexible way to create disposable databases to use for integration testing.

## Introduction
Using a disposable database for integration testing ensures that tests run in a controlled environment without affecting
the production database. Each test suite or test case can create a new, isolated database, execute the tests, and then
clean up afterward. This also enables parallel test execution, reducing the overall time required for integration
testing.
