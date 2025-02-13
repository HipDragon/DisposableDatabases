// <copyright file="IDatabaseCreator.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Exceptions;

namespace DisposableDatabases.Interfaces.DatabaseOperations;

/// <summary>
/// Provides methods to create a database.
/// </summary>
public interface IDatabaseCreator
{
	/// <summary>
	/// Creates a new database asynchronously with the specified connection string and database name.
	/// </summary>
	/// <param name="connectionString">The connection string to the server where the database will be created.</param>
	/// <param name="databaseName">The name of the database to be created.</param>
	/// <returns>A task representing the asynchronous operation, which contains the connection string to the newly created database.</returns>
	/// <exception cref="ArgumentException">Thrown when the connection string or database name is null or whitespace.</exception>
	/// <exception cref="DisposableDatabasesException">Thrown when an error occurs while attempting to create the database.</exception>
	Task<string> CreateDatabaseAsync(string connectionString, string databaseName);

	/// <summary>
	/// Creates a new database asynchronously with the specified connection string and database name.
	/// </summary>
	/// <param name="connectionString">The connection string to the server where the database will be created.</param>
	/// <param name="databaseName">The name of the database to be created.</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
	/// <returns>A task representing the asynchronous operation, which contains the connection string to the newly created database.</returns>
	/// <exception cref="ArgumentException">Thrown when the connection string or database name is null or whitespace.</exception>
	/// <exception cref="DisposableDatabasesException">Thrown when an error occurs while attempting to create the database.</exception>
	Task<string> CreateDatabaseAsync(string connectionString, string databaseName, CancellationToken cancellationToken);
}
