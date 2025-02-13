// <copyright file="DatabaseOnlyCreationStrategy.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Diagnostics;
using DisposableDatabases.Interfaces;
using DisposableDatabases.Interfaces.DatabaseOperations;
using DisposableDatabases.Interfaces.Strategies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DisposableDatabases.Strategies.DatabaseCreation;

/// <summary>
/// Strategy to create a database.
/// </summary>
public class DatabaseOnlyCreationStrategy : IDisposableDatabaseCreationStrategy
{
	/// <summary>
	/// Stores the connection string used to connect to the database during the creation and disposal process.
	/// </summary>
	private readonly string _connectionString;

	/// <summary>
	/// Holds the reference to an instance of <see cref="IDatabaseCreatorAndDropper" />
	/// used for creating and dropping databases according to the strategy.
	/// </summary>
	private readonly IDatabaseCreatorAndDropper _databaseCreatorAndDropper;

	/// <summary>
	/// Represents the strategy used to generate database names for the creation process.
	/// </summary>
	private readonly IDatabaseNamingStrategy _databaseNamingStrategy;

	/// <summary>
	/// Holds the reference to an instance of <see cref="ILoggerFactory" />
	/// used for creating loggers to log operations within the database creation strategy.
	/// </summary>
	private readonly ILoggerFactory _loggerFactory;

	/// <summary>
	/// Initializes a new instance of the <see cref="DatabaseOnlyCreationStrategy" /> class.
	/// </summary>
	/// <param name="connectionString">The connection string used to connect to the database server.</param>
	/// <param name="databaseCreatorAndDropper">The service instance that implements both IDatabaseCreator and IDatabaseDropper.</param>
	/// <param name="databaseNamingStrategy">The strategy used to generate database names for creation processes.</param>
	/// <exception cref="ArgumentException">Thrown when the service instance does not implement both interfaces.</exception>
	public DatabaseOnlyCreationStrategy([NotNull] string? connectionString, [NotNull] IDatabaseCreatorAndDropper? databaseCreatorAndDropper, [NotNull] IDatabaseNamingStrategy? databaseNamingStrategy)
		: this(connectionString, databaseCreatorAndDropper, databaseNamingStrategy, NullLoggerFactory.Instance)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DatabaseOnlyCreationStrategy" /> class.
	/// </summary>
	/// <param name="connectionString">The connection string used to connect to the database server.</param>
	/// <param name="databaseCreatorAndDropper">The service instance that implements both IDatabaseCreator and IDatabaseDropper.</param>
	/// <param name="databaseNamingStrategy">The strategy used to generate database names for creation processes.</param>
	/// <param name="loggerFactory"></param>
	/// <exception cref="ArgumentException">Thrown when the service instance does not implement both interfaces.</exception>
	public DatabaseOnlyCreationStrategy([NotNull] string? connectionString,
	                                    [NotNull] IDatabaseCreatorAndDropper? databaseCreatorAndDropper,
	                                    [NotNull] IDatabaseNamingStrategy? databaseNamingStrategy,
	                                    [NotNull] ILoggerFactory? loggerFactory)
	{
		Guard.IsNotNullOrWhiteSpace(connectionString);
		Guard.IsNotNull(databaseCreatorAndDropper);
		Guard.IsNotNull(databaseNamingStrategy);
		Guard.IsNotNull(loggerFactory);

		_connectionString = connectionString;
		_databaseCreatorAndDropper = databaseCreatorAndDropper;
		_databaseNamingStrategy = databaseNamingStrategy;
		_loggerFactory = loggerFactory;
	}

	/// <inheritdoc />
	public Task<IDisposableDatabase> CreateDatabaseAsync()
	{
		return CreateDatabaseAsync(CancellationToken.None);
	}

	/// <inheritdoc />
	public Task<IDisposableDatabase> CreateDatabaseAsync(CancellationToken cancellationToken)
	{
		return CreateDatabaseInternalAsync(cancellationToken);
	}

	/// <inheritdoc />
	public Task DisposeDatabaseAsync([NotNull] IDisposableDatabase? disposableDatabase)
	{
		Guard.IsNotNull(disposableDatabase);

		return DisposeDatabaseInternalAsync(disposableDatabase);
	}

	/// <inheritdoc cref="CreateDatabaseAsync(CancellationToken)" />
	private async Task<IDisposableDatabase> CreateDatabaseInternalAsync(CancellationToken cancellationToken)
	{
		string databaseName = _databaseNamingStrategy.GenerateDatabaseName();
		string connectionString = await _databaseCreatorAndDropper.CreateDatabaseAsync(_connectionString, databaseName, cancellationToken).ConfigureAwait(false);

		return new DisposableDatabase(connectionString, databaseName, this, _loggerFactory.CreateLogger<DisposableDatabase>());
	}

	/// <inheritdoc cref="DisposeDatabaseAsync(IDisposableDatabase)" />
	private Task DisposeDatabaseInternalAsync(IDisposableDatabase disposableDatabase)
	{
		return _databaseCreatorAndDropper.DropDatabaseAsync(_connectionString, disposableDatabase.DatabaseName);
	}
}
