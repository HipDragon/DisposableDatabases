// <copyright file="ISqlScriptExecutor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Exceptions;

namespace DisposableDatabases.Interfaces.DatabaseOperations;

/// <summary>
/// Represents a service for executing SQL scripts against a specified database.
/// </summary>
public interface ISqlScriptExecutor
{
	/// <summary>
	/// Executes the specified SQL script against the given database.
	/// </summary>
	/// <param name="connectionString">The connection string used to connect to the database.</param>
	/// <param name="databaseName">The name of the database where the script will be executed.</param>
	/// <param name="sqlScriptFilePath">The file path of the SQL script to be executed.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <exception cref="DisposableDatabasesException">Thrown when there is an error executing the SQL script.</exception>
	/// <exception cref="FileNotFoundException">Thrown when the SQL script file is not found at the specified path.</exception>
	Task ExecuteSqlScriptAsync(string connectionString, string databaseName, string sqlScriptFilePath);

	/// <summary>
	/// Executes the specified SQL script against the given database.
	/// </summary>
	/// <param name="connectionString">The connection string used to connect to the database.</param>
	/// <param name="databaseName">The name of the database where the script will be executed.</param>
	/// <param name="sqlScriptFilePath">The file path of the SQL script to be executed.</param>
	/// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <exception cref="DisposableDatabasesException">Thrown when there is an error executing the SQL script.</exception>
	/// <exception cref="FileNotFoundException">Thrown when the SQL script file is not found at the specified path.</exception>
	Task ExecuteSqlScriptAsync(string connectionString, string databaseName, string sqlScriptFilePath, CancellationToken cancellationToken);
}
