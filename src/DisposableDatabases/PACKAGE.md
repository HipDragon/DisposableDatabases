# Disposable Databases
A simple and flexible solution for creating disposable databases for integration testing.

## Introduction
Using a disposable database for integration testing ensures that tests run in a controlled environment without impacting
other databases. Each test or test suite can create a new, isolated database, execute the tests, and then automatically
clean up afterward by disposing of (dropping) the database. This approach helps developers by allowing them to enable
parallel test execution, alleviating one concern in managing multiple threads, although developers still need to ensure
their code can handle concurrent execution.

### Available Packages
| Package                        | Supports      | Version                                                                                                                                                   |
|--------------------------------|---------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------|
| DisposableDatabases            | Core          | [![Nuget](https://img.shields.io/nuget/v/DisposableDatabases.svg?colorB=green)](https://www.nuget.org/packages/DisposableDatabases)                       |
| DisposableDatabases.PostgreSql | PostgreSQL    | [![Nuget](https://img.shields.io/nuget/v/DisposableDatabases.PostgreSql.svg?colorB=green)](https://www.nuget.org/packages/DisposableDatabases.PostgreSql) |
| DisposableDatabases.SqlServer  | MS SQL Server | [![Nuget](https://img.shields.io/nuget/v/DisposableDatabases.SqlServer.svg?colorB=green)](https://www.nuget.org/packages/DisposableDatabases.SqlServer)   |
