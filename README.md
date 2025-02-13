# Disposable Databases

An easy and flexible way to create disposable databases to use for integration testing.

## Introduction

Using a disposable database for integration testing ensures that tests run in a controlled environment without affecting
the production database. Each test suite or test case can create a new, isolated database, execute the tests, and then
clean up afterward. This also enables parallel test execution, reducing the overall time required for integration
testing.

### Available Packages

| Package                        | Supports      | Version                                                                                                                                                   |
|--------------------------------|---------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------|
| DisposableDatabases            | Core          | [![Nuget](https://img.shields.io/nuget/v/DisposableDatabases.svg?colorB=green)](https://www.nuget.org/packages/DisposableDatabases)                       |
| DisposableDatabases.PostgreSql | PostgreSQL    | [![Nuget](https://img.shields.io/nuget/v/DisposableDatabases.PostgreSql.svg?colorB=green)](https://www.nuget.org/packages/DisposableDatabases.PostgreSql) |
| DisposableDatabases.SqlServer  | MS SQL Server | [![Nuget](https://img.shields.io/nuget/v/DisposableDatabases.SqlServer.svg?colorB=green)](https://www.nuget.org/packages/DisposableDatabases.SqlServer)   |

## Writing Integration Tests
The **Disposable Databases** library enables the creation of isolated databases specifically for integration tests. By configuring database naming and creation strategies, you can generate temporary databases tailored to your testing needs. These disposable databases ensure clean and isolated test environments and are automatically deleted after use.
### Step 1: Install the Required Package
Install the NuGet package corresponding to the database type you'll use. For example:
- For **Microsoft SQL Server**:
``` sh
  dotnet add package DisposableDatabases.SqlServer
```
- For **PostgreSQL**:
``` sh
  dotnet add package DisposableDatabases.PostgreSql
```
### Step 2: Configure the Connection String and Database Naming Strategy
**Connection String**: The connection string must point to a database server with permissions to create and drop databases. Typically, this would allow using a shared testing server while ensuring disposable databases are isolated and temporary.
For example:
``` csharp
const string connectionString = "Server=localhost;Integrated Security=true;Trusted Connection=True;";
```
**Database Naming Strategy**:
The **Database Naming Strategy** defines how the names for disposable databases will be generated. You can choose from predefined strategies and extend them with decorators to customize database names with prefixes, suffixes, or other modifiers.
- **Available Naming Strategies**:
	- **`GuidDatabaseNamingStrategy`**: Generates unique names based on GUIDs (e.g., `"{GUID}"`), ensuring no collisions.
	- **`UserDefinedDatabaseNamingStrategy`**: Produces the same user-defined name (e.g., `"MyTestDatabase"`) for consistent identification.

- **Available Decorators**:
	- **`DatabaseNamingStrategyPrefixDecorator`**: Adds a user-defined prefix to database names (e.g., `"Prefix-{Name}"`).
	- **`DatabaseNamingStrategySuffixDecorator`**: Adds a suffix to database names (e.g., `"{Name}-Suffix"`).

**Example Configurations**:
- To create unique database names like `"MyTests-{GUID}"`:
``` csharp
  IDatabaseNamingStrategy namingStrategy =
      new DatabaseNamingStrategyPrefixDecorator(
          new GuidDatabaseNamingStrategy(),
          "MyTests-"
      );
```
- To produce a fixed name with a suffix, such as `"MyTestDatabase-Suffix"`:
``` csharp
  IDatabaseNamingStrategy namingStrategy =
      new DatabaseNamingStrategySuffixDecorator(
          new UserDefinedDatabaseNamingStrategy("MyTestDatabase"),
          "-Suffix"
      );
```
- To customize names with both a prefix and suffix, e.g., `"Prefix-{GUID}-Suffix"`:
``` csharp
  IDatabaseNamingStrategy namingStrategy =
      new DatabaseNamingStrategyPrefixDecorator(
          new DatabaseNamingStrategySuffixDecorator(
              new GuidDatabaseNamingStrategy(),
              "-Suffix"
          ),
          "Prefix-"
      );
```
### Step 3: Configure the Database Creation Strategy
The **Database Creation Strategy** defines how disposable databases are created and optionally initialized. These strategies determine what happens when a disposable database is created, whether it's just an empty database or fully initialized with a schema and data.
- **Available Strategies**:
	- **`CreateDatabaseOnlyStrategy`**: A minimal strategy that creates an empty database.
	- **`DatabaseCreationWithPostScriptExecutionStrategy`**: A strategy that runs an SQL script immediately after the database is created, allowing schema setup or data seeding.

The connection string and naming strategy (defined in Step 2) are required to configure the database creation strategy.
#### Example 1: Creating a Basic Disposable Database
This example combines a **connection string**, a GUID-based naming strategy with a prefix, and a lightweight creation strategy:
``` csharp
const string connectionString = "Server=localhost;Integrated Security=true;Trusted Connection=True;";
const string prefix = "MyTests-";

// Define the naming strategy: "MyTests-{GUID}"
IDatabaseNamingStrategy databaseNamingStrategy =
    new DatabaseNamingStrategyPrefixDecorator(
        new GuidDatabaseNamingStrategy(),
        prefix
    );

// Configure the database creation strategy
var disposableDatabaseCreationStrategy =
    new CreateDatabaseOnlyStrategy(
        connectionString,
        new SqlServerDatabaseService(),
        databaseNamingStrategy
    );

// Create and use the disposable database
using (IDisposableDatabase disposableDatabase = await disposableDatabaseCreationStrategy.CreateDatabaseAsync())
{
    Console.WriteLine($"Database Name: {disposableDatabase.DatabaseName}");
    Console.WriteLine($"Connection String: {disposableDatabase.ConnectionString}");

    // Use the database in your integration tests
}
```
In this setup:
- The **Database Naming Strategy** ensures unique names like `"MyTests-{GUID}"`.
- The **Database Creation Strategy** simply creates an empty, disposable database.

#### Example 2: Initializing a Database with a Post-Script
This example initializes a disposable database with a fixed name, appends a suffix, and executes an SQL script to prepare the database (e.g., setting up schema and seeding test data):
``` csharp
const string connectionString = "Server=localhost;Integrated Security=true;Trusted Connection=True;";
const string baseName = "MyFixedTestDB";
const string suffix = "-Suffix";
const string sqlScriptPath = "path_to_schema_setup.sql";

// Define the naming strategy: "MyFixedTestDB-Suffix"
IDatabaseNamingStrategy databaseNamingStrategy =
    new DatabaseNamingStrategySuffixDecorator(
        new UserDefinedDatabaseNamingStrategy(baseName),
        suffix
    );

// Configure the creation strategy to execute a script after creating the database
var disposableDatabaseCreationStrategy =
    new DatabaseCreationWithPostScriptExecutionStrategy(
        connectionString,
        new SqlServerDatabaseService(),
        databaseNamingStrategy,
        sqlScriptPath
    );

// Create and initialize the database
using (IDisposableDatabase disposableDatabase = await disposableDatabaseCreationStrategy.CreateDatabaseAsync())
{
    Console.WriteLine($"Database Name: {disposableDatabase.DatabaseName}");
    Console.WriteLine($"Connection String: {disposableDatabase.ConnectionString}");

    // The database is now ready with schema and data configured by the script
}
```
In this setup:
- The **Database Naming Strategy** fixes the name with a suffix (e.g., `"MyFixedTestDB-Suffix"`).
- The **Database Creation Strategy** sets up the schema and relevant data based on the SQL script passed to it.

### Choosing the Right Configuration

| Component | Purpose | Examples |
| --- | --- | --- |
| **Connection String** | Directs the creation strategy to the appropriate server. Default server permissions required. | Examples: `"Server=localhost;Integrated Security=True;"`. |
| **Database Naming Strategy** | Determines how disposable databases are named during creation. | Use GUID naming for uniqueness (`{GUID}`), or decorators for customizable names like `Prefix-{GUID}-Suffix`. |
| **Decorators** | Extend naming strategies by adding prefixes, suffixes, or other modifiers for readability. | Names like `"Integration-{GUID}"`, `"MyTests-2023-{GUID}`. |
| **CreateDatabaseOnlyStrategy** | Minimal strategy for an empty, disposable database. | Tests requiring no custom schema or data setup. |
| **DatabaseCreationWithPostScriptExecutionStrategy** | Creates a database and runs additional SQL initialization scripts. | Tests requiring pre-configured schemas, seeded data, or advanced setup. |

### Final Notes on Cleanup
Disposable databases are cleaned up automatically when they go out of scope (e.g., after exiting the `using` block). This ensures no lingering databases remain on your server, facilitating clean, isolated, and conflict-free testing environments.

## Debugging

To troubleshoot failed tests, you can preserve the database by calling the `PreserveDatabase` method on the disposable
database instance. This ensures that the database is not disposed of at the end of its lifecycle, allowing you to
perform
post-test analysis on the database state.

```csharp
disposableDatabase.PreserveDatabase();
```

By following these steps, you can efficiently manage disposable databases in your integration tests while ensuring that
your tests run in a controlled and isolated environment.

## Troubleshooting

### Common Issues

**Issue**: "Failed to create database due to missing permissions."

- **Cause**: The connection string is connected to a database user without `CREATE DATABASE` rights.
- **Solution**: Ensure the connection user has the necessary SQL permissions to create and drop databases.

Example script for MS SQL Server:

```sql
GRANT CREATE DATABASE TO [YourUsername];
```

**Issue**: "Database is deleted unexpectedly."

- **Cause**: The disposable database is removed once the `Dispose` method is called or the `using` block ends.
- **Solution**: Set `PreserveDatabase` to `true` to retain the database for further debugging:
  ```csharp
  disposableDatabase.PreserveDatabase = true;
  ```
