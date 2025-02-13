// <copyright file="IDatabaseCreationStrategy.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Exceptions;

namespace DisposableDatabases.Interfaces.Strategies;

/// <summary>
/// Interface for strategies to create a disposable database.
/// </summary>
public interface IDisposableDatabaseCreationStrategy
{
	/// <summary>
	/// Asynchronously creates the disposable database.
	/// </summary>
	/// <returns>A task representing the asynchronous operation of creating the disposable database.</returns>
	/// <exception cref="DisposableDatabasesException">Thrown when an error occurs while attempting to create the database.</exception>
	Task<IDisposableDatabase> CreateDatabaseAsync();

	/// <summary>
	/// Asynchronously creates the disposable database.
	/// </summary>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	/// <returns>A task representing the asynchronous operation of creating the disposable database.</returns>
	/// <exception cref="DisposableDatabasesException">Thrown when an error occurs while attempting to create the database.</exception>
	Task<IDisposableDatabase> CreateDatabaseAsync(CancellationToken cancellationToken);

	/// <summary>
	/// Asynchronously disposes of the specified database.
	/// </summary>
	/// <param name="disposableDatabase">The disposable database to be disposed.</param>
	/// <returns>A task representing the asynchronous operation of disposing the database.</returns>
	/// <exception cref="DisposableDatabasesException">Thrown if an error occurs while dropping the database.</exception>
	Task DisposeDatabaseAsync(IDisposableDatabase disposableDatabase);
}
