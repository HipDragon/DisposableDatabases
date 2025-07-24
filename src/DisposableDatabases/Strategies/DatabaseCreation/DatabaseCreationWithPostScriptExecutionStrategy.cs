// <copyright file="DatabaseCreationWithPostScriptExecutionStrategy.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using CommunityToolkit.Diagnostics;
using DisposableDatabases.Exceptions;
using DisposableDatabases.Interfaces;
using DisposableDatabases.Interfaces.DatabaseOperations;
using DisposableDatabases.Interfaces.Strategies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DisposableDatabases.Strategies.DatabaseCreation;

/// <summary>
/// Represents a strategy for creating a database and running an SQL script against it.
/// </summary>
public class DatabaseCreationWithPostScriptExecutionStrategy : IDisposableDatabaseCreationStrategy
{
	/// <summary>
	/// Stores the connection string used to connect to the database during the creation and disposal process.
	/// </summary>
	private readonly string _connectionString;

	/// <summary>
	/// A service responsible for creating databases, dropping databases,
	/// and executing SQL scripts as part of the database management process.
	/// This is a core dependency for the strategy used to handle database creation
	/// and script execution within the lifecycle of disposable databases.
	/// </summary>
	private readonly IDatabaseCreatorDropperAndSqlScriptExecutor _databaseCreatorDropperAndSqlScriptExecutor;

	/// <summary>
	/// Represents the strategy used to generate database names for the creation process.
	/// </summary>
	private readonly INamingStrategy _namingStrategy;

	/// <summary>
	/// A factory used to create logger instances for the strategy.
	/// This allows for logging behavior to be customized or replaced as necessary,
	/// supporting diagnostic and debugging capabilities during database creation
	/// and SQL script execution.
	/// </summary>
	private readonly ILoggerFactory _loggerFactory;

	/// <summary>
	/// Represents the file path to the SQL script that will be executed as part
	/// of the database creation process. This is a required input to ensure
	/// the script is applied to the newly created database.
	/// </summary>
	private readonly string _sqlScriptFilePath;

	/// <summary>
	/// Represents a strategy for creating a database and running an SQL script against it.
	/// </summary>
	public DatabaseCreationWithPostScriptExecutionStrategy(string? connectionString,
	                                                       IDatabaseCreatorDropperAndSqlScriptExecutor? databaseCreatorDropperAndSqlScriptExecutor,
	                                                       INamingStrategy? namingStrategy,
	                                                       string? sqlScriptFilePath)
		: this(connectionString, databaseCreatorDropperAndSqlScriptExecutor, namingStrategy, sqlScriptFilePath, NullLoggerFactory.Instance)
	{
	}

	/// <summary>
	/// Represents a strategy for creating a database and running an SQL script against it.
	/// </summary>
	public DatabaseCreationWithPostScriptExecutionStrategy(string? connectionString,
	                                                       IDatabaseCreatorDropperAndSqlScriptExecutor? databaseCreatorDropperAndSqlScriptExecutor,
	                                                       INamingStrategy? namingStrategy,
	                                                       string? sqlScriptFilePath,
	                                                       ILoggerFactory? loggerFactory)
	{
		Guard.IsNotNullOrWhiteSpace(connectionString);
		Guard.IsNotNull(databaseCreatorDropperAndSqlScriptExecutor);
		Guard.IsNotNull(namingStrategy);
		Guard.IsNotNullOrWhiteSpace(sqlScriptFilePath);
		Guard.IsNotNull(loggerFactory);

		if (!File.Exists(sqlScriptFilePath))
		{
			throw new FileNotFoundException($"SQL Script file does not exist at path {sqlScriptFilePath}.", sqlScriptFilePath);
		}

		_connectionString = connectionString;
		_databaseCreatorDropperAndSqlScriptExecutor = databaseCreatorDropperAndSqlScriptExecutor;
		_namingStrategy = namingStrategy;
		_loggerFactory = loggerFactory;
		_sqlScriptFilePath = sqlScriptFilePath;
	}

	/// <summary>
	/// Asynchronously creates a disposable database using the specified connection string and database naming strategy.
	/// </summary>
	/// <returns>A task that represents the asynchronous database creation operation. The task result contains the instance of the disposable database.</returns>
	public Task<IDisposableDatabase> CreateDatabaseAsync()
	{
		return CreateDatabaseAsync(CancellationToken.None);
	}

	/// <summary>
	/// Asynchronously creates a disposable database using the provided connection string and database naming strategy.
	/// </summary>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task representing the asynchronous operation, with a result of a disposable database instance.</returns>
	public Task<IDisposableDatabase> CreateDatabaseAsync(CancellationToken cancellationToken)
	{
		return CreateDatabaseInternalAsync(cancellationToken);
	}

	/// <inheritdoc />
	public Task DisposeDatabaseAsync(IDisposableDatabase? disposableDatabase)
	{
		Guard.IsNotNull(disposableDatabase);

#pragma warning disable CA1062 // Validate arguments of public methods - Done by Guard.IsNotNull
		return DisposeDatabaseInternalAsync(disposableDatabase);
#pragma warning restore CA1062
	}

	/// <inheritdoc cref="CreateDatabaseAsync(CancellationToken)" />
	private async Task<IDisposableDatabase> CreateDatabaseInternalAsync(CancellationToken cancellationToken)
	{
		string databaseName = _namingStrategy.GenerateName();
		string newDatabaseConnectionString = await _databaseCreatorDropperAndSqlScriptExecutor.CreateDatabaseAsync(_connectionString, databaseName, cancellationToken).ConfigureAwait(false);

		try
		{
			await _databaseCreatorDropperAndSqlScriptExecutor.ExecuteSqlScriptAsync(newDatabaseConnectionString, databaseName, _sqlScriptFilePath, cancellationToken).ConfigureAwait(false);
		}
		catch (Exception sqlScriptException) when (sqlScriptException is DisposableDatabasesException)
		{
			await TryDropDatabaseAsync(databaseName, sqlScriptException, cancellationToken).ConfigureAwait(false);
		}

		return new DisposableDatabase(newDatabaseConnectionString, databaseName, this, _loggerFactory.CreateLogger<DisposableDatabase>());
	}

	/// <inheritdoc cref="DisposeDatabaseAsync(IDisposableDatabase)" />
	private Task DisposeDatabaseInternalAsync(IDisposableDatabase disposableDatabase)
	{
		return _databaseCreatorDropperAndSqlScriptExecutor.DropDatabaseAsync(_connectionString, disposableDatabase.DatabaseName);
	}

	private async Task TryDropDatabaseAsync(string databaseName, Exception scriptExecutionException, CancellationToken cancellationToken)
	{
		try
		{
			await _databaseCreatorDropperAndSqlScriptExecutor.DropDatabaseAsync(_connectionString, databaseName, cancellationToken).ConfigureAwait(false);
		}
		catch (Exception dropDatabaseException) when (dropDatabaseException is DisposableDatabasesException)
		{
			throw new AggregateException("An error occurred while executing the SQL script and dropping the database.", scriptExecutionException, dropDatabaseException);
		}

		throw scriptExecutionException;
	}
}
