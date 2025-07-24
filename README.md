# Disposable Databases
A simple and flexible solution for creating disposable databases for integration testing.

## Introduction
Using a disposable database for integration testing ensures that tests run in a controlled environment without impacting
other databases. Each test or test suite can create a new, isolated database, execute the tests, and then automatically
clean up afterward by disposing of (dropping) the database. This approach helps developers by allowing them to enable
parallel test execution, alleviating one concern in managing multiple threads, although developers still need to ensure
their code can handle concurrent execution.

### Available Packages

| Package                        | Supports       | Description                                  | Version                                                                                                                                                   |
|--------------------------------|----------------|----------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------|
| DisposableDatabases            | Core           | Core functionality for disposable databases. | [![Nuget](https://img.shields.io/nuget/v/DisposableDatabases.svg?colorB=green)](https://www.nuget.org/packages/DisposableDatabases)                       |
| DisposableDatabases.PostgreSql | PostgreSQL     | Support for PostgreSQL databases.            | [![Nuget](https://img.shields.io/nuget/v/DisposableDatabases.PostgreSql.svg?colorB=green)](https://www.nuget.org/packages/DisposableDatabases.PostgreSql) |
| DisposableDatabases.SqlServer  | MS SQL Server  | Support for Microsoft SQL Server databases.  | [![Nuget](https://img.shields.io/nuget/v/DisposableDatabases.SqlServer.svg?colorB=green)](https://www.nuget.org/packages/DisposableDatabases.SqlServer)   |

## Writing Integration Tests
The **Disposable Databases** library enables the creation of isolated databases specifically for integration tests.
By configuring database naming and creation strategies, you can generate temporary databases tailored to your testing needs.
These disposable databases ensure clean and isolated test environments and are automatically deleted after use.
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
**Connection String**: The connection string must point to a database server with permissions to create and drop databases.
Typically, this would allow using a shared testing server while ensuring disposable databases are isolated and temporary.
For example:
``` csharp
const string connectionString = "Server=localhost;Integrated Security=true;Trusted Connection=True;";
```
**Database Naming Strategy**:
The **Database Naming Strategy** defines how the names for disposable databases will be generated. You can choose from
predefined strategies and extend them with decorators to customize database names with prefixes, suffixes, or other modifiers.
- **Available Naming Strategies**:
	- **`GuidNamingStrategy`**: Generates unique names based on GUIDs (e.g., `"{GUID}"`), ensuring no collisions.
	- **`ConstantNamingStrategy`**: Produces the same user-defined name (e.g., `"MyTestDatabase"`) for consistent identification.

- **Available Decorators**:
	- **`PrefixNamingStrategy`**: Adds a prefix to database names (e.g., `"Prefix-{Name}"`).
	- **`SuffixNamingStrategy`**: Adds a suffix to database names (e.g., `"{Name}-Suffix"`).

**Example Configurations**:
- To create unique database names like `"MyTests-{GUID}"`:
``` csharp
  INamingStrategy namingStrategy = new PrefixNamingStrategy(new GuidNamingStrategy(), "MyTests-");
```
- To produce a fixed name with a suffix, such as `"MyTestDatabase-Suffix"`:
``` csharp
  INamingStrategy namingStrategy = new SuffixNamingStrategy(new ConstNamingStrategy("MyTestDatabase"), "-Suffix");
```
- To customize names with both a prefix and suffix, e.g., `"Prefix-{GUID}-Suffix"`:
``` csharp
  INamingStrategy namingStrategy =
      new PrefixNamingStrategy(
          new SuffixNamingStrategy(new GuidNamingStrategy(), "-Suffix"),
          "Prefix-"
      );
```
### Step 3: Configure the Database Creation Strategy
The **Database Creation Strategy** defines how disposable databases are created and optionally initialized.
These strategies determine what happens when a disposable database is created, whether it's just an empty database or
fully initialized with a schema and data.
- **Available Strategies**:
	- **`CreateDatabaseOnlyStrategy`**: Creates an empty database without any initialization.
	- **`DatabaseCreationWithPostScriptExecutionStrategy`**: Creates a database and runs an SQL script for initialization, thus allowing schema setup and/or data seeding.

The connection string and naming strategy (defined in Step 2) are required to configure the database creation strategy.
#### Example 1: Creating a Basic Disposable Database
This example combines a **connection string**, a GUID-based naming strategy with a prefix, and a lightweight creation strategy:
``` csharp
const string connectionString = "Server=localhost;Integrated Security=true;Trusted Connection=True;";
const string prefix = "MyTests-";

// Define the naming strategy: "MyTests-{GUID}"
INamingStrategy namingStrategy = new PrefixNamingStrategy(new GuidNamingStrategy(), prefix);

// Configure the database creation strategy
var disposableDatabaseCreationStrategy =
    new CreateDatabaseOnlyStrategy(
        connectionString,
        new SqlServerDatabaseService(),
        namingStrategy
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
- The **Naming Strategy** ensures unique names like `"MyTests-{GUID}"`.
- The **Database Creation Strategy** simply creates an empty, disposable database.

#### Example 2: Initializing a Database with a Post-Script
This example initializes a disposable database with a fixed name, appends a suffix, and executes an SQL script to prepare
the database (e.g., setting up schema and seeding test data):
``` csharp
const string connectionString = "Server=localhost;Integrated Security=true;Trusted Connection=True;";
const string baseName = "MyFixedTestDB";
const string suffix = "-Suffix";
const string sqlScriptPath = "path_to_schema_setup.sql";

// Define the naming strategy: "MyFixedTestDB-Suffix"
INamingStrategy namingStrategy = new SuffixNamingStrategy(new ConstantNamingStrategy(baseName), suffix);

// Configure the creation strategy to execute a script after creating the database
var disposableDatabaseCreationStrategy =
    new DatabaseCreationWithPostScriptExecutionStrategy(
        connectionString,
        new SqlServerDatabaseService(),
        namingStrategy,
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
- The **Naming Strategy** fixes the name with a suffix (e.g., `"MyFixedTestDB-Suffix"`).
- The **Database Creation Strategy** sets up the schema and relevant data based on the SQL script passed to it.

### Choosing the Right Configuration

| Component                                           | Purpose                                                                                       | Examples                                                                                                |
|-----------------------------------------------------|-----------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------|
| **Connection String**                               | Directs the creation strategy to the appropriate server. Default server permissions required. | Examples: `"Server=localhost;Integrated Security=True;"`.                                               |
| **Naming Strategy**                                 | Determines how disposable databases are named during creation.                                | Use GUID naming for uniqueness (`{GUID}`), or decorators for customizable names like `Prefix-{GUID}`.   |
| **Decorators**                                      | Extend naming strategies by adding prefixes, suffixes, or other modifiers for readability.    | Names like `"Integration-{GUID}"`, `"MyTests-2023-{GUID}`.                                              |
| **CreateDatabaseOnlyStrategy**                      | Minimal strategy for an empty, disposable database.                                           | Tests requiring no custom schema or data setup.                                                         |
| **DatabaseCreationWithPostScriptExecutionStrategy** | Creates a database and runs additional SQL initialization scripts.                            | Tests requiring pre-configured schemas, seeded data, or advanced setup.                                 |

### Final Notes on Cleanup
Disposable databases are cleaned up automatically when they go out of scope (e.g., after exiting the `using` block).
This guarantees that no lingering databases remain on your server, promoting clean, isolated, and conflict-free testing environments.

## Debugging

To troubleshoot failed tests, you can preserve the database by calling the `PreserveDatabase` method on the disposable
database instance. This method allows you to retain the database for further inspection and troubleshooting,
ensuring that you can analyze the database state after a test failure.

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
