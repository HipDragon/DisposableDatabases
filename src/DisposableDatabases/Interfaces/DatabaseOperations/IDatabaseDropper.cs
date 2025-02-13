// <copyright file="IDatabaseDropper.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Exceptions;

namespace DisposableDatabases.Interfaces.DatabaseOperations;

/// <summary>
/// Provides method to drop a database.
/// </summary>
public interface IDatabaseDropper
{
	/// <summary>
	/// Drops the database with the specified connection string and name.
	/// </summary>
	/// <param name="connectionString">The connection string to the database server.</param>
	/// <param name="databaseName">The name of the database to be dropped.</param>
	/// <exception cref="ArgumentException">Thrown when the connection string or database name is null or whitespace.</exception>
	/// <exception cref="DisposableDatabasesException">Thrown if an error occurs while dropping the database.</exception>
	/// <returns>A task that represents the asynchronous drop operation.</returns>
	Task DropDatabaseAsync(string connectionString, string databaseName);

	/// <summary>
	/// Drops the database with the specified connection string and name, supporting cancellation.
	/// </summary>
	/// <param name="connectionString">The connection string to the database server.</param>
	/// <param name="databaseName">The name of the database to be dropped.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <exception cref="ArgumentException">Thrown when the connection string or database name is null or whitespace.</exception>
	/// <exception cref="DisposableDatabasesException">Thrown if an error occurs while dropping the database.</exception>
	/// <returns>A task that represents the asynchronous drop operation.</returns>
	Task DropDatabaseAsync(string connectionString, string databaseName, CancellationToken cancellationToken);
}
