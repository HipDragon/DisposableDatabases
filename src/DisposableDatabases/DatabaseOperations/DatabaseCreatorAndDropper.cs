// <copyright file="DatabaseCreatorAndDropper.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Diagnostics;
using DisposableDatabases.Interfaces.DatabaseOperations;

namespace DisposableDatabases.DatabaseOperations;

/// <summary>
/// Represents an abstract base class that provides functionality for creating and dropping databases.
/// </summary>
public abstract class DatabaseCreatorAndDropper : IDatabaseCreatorAndDropper
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DatabaseCreatorAndDropper" /> class with the specified database creator and dropper.
	/// </summary>
	/// <param name="databaseCreator">The <see cref="IDatabaseCreator" /> instance responsible for creating databases.</param>
	/// <param name="databaseDropper">The <see cref="IDatabaseDropper" /> instance responsible for dropping databases.</param>
	/// <exception cref="ArgumentNullException">Thrown if either <paramref name="databaseCreator" /> or <paramref name="databaseDropper" /> is <c>null</c>.</exception>
	protected DatabaseCreatorAndDropper([NotNull] IDatabaseCreator? databaseCreator, [NotNull] IDatabaseDropper? databaseDropper)
	{
		Guard.IsNotNull(databaseCreator);
		Guard.IsNotNull(databaseDropper);

		DatabaseCreator = databaseCreator;
		DatabaseDropper = databaseDropper;
	}

	/// <summary>
	/// Gets the <see cref="IDatabaseCreator" /> instance responsible for creating databases.
	/// </summary>
	protected IDatabaseCreator DatabaseCreator { get; }

	/// <summary>
	/// Gets the <see cref="IDatabaseDropper" /> instance responsible for dropping databases.
	/// </summary>
	protected IDatabaseDropper DatabaseDropper { get; }

	/// <inheritdoc cref="IDatabaseCreator.CreateDatabaseAsync(string, string)" />
	public Task<string> CreateDatabaseAsync(string connectionString, string databaseName)
	{
		return DatabaseCreator.CreateDatabaseAsync(connectionString, databaseName);
	}

	/// <inheritdoc cref="IDatabaseCreator.CreateDatabaseAsync(string, string, CancellationToken)" />
	public Task<string> CreateDatabaseAsync(string connectionString, string databaseName, CancellationToken cancellationToken)
	{
		return DatabaseCreator.CreateDatabaseAsync(connectionString, databaseName, cancellationToken);
	}

	/// <inheritdoc cref="IDatabaseDropper.DropDatabaseAsync(string, string)" />
	public Task DropDatabaseAsync(string connectionString, string databaseName)
	{
		return DatabaseDropper.DropDatabaseAsync(connectionString, databaseName);
	}

	/// <inheritdoc cref="IDatabaseDropper.DropDatabaseAsync(string, string, CancellationToken)" />
	public Task DropDatabaseAsync(string connectionString, string databaseName, CancellationToken cancellationToken)
	{
		return DatabaseDropper.DropDatabaseAsync(connectionString, databaseName, cancellationToken);
	}
}
