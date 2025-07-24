// <copyright file="SqlServerSqlScriptExecutor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using CommunityToolkit.Diagnostics;
using DisposableDatabases.Exceptions;
using DisposableDatabases.Interfaces.DatabaseOperations;
using Microsoft.Data.SqlClient;

namespace DisposableDatabases.SqlServer;

/// <summary>
/// SqlServerSqlScriptExecutor is a class for executing SQL scripts against an SQL Server database.
/// </summary>
public class SqlServerSqlScriptExecutor : ISqlScriptExecutor
{
	/// <summary>
	/// Executes the specified SQL script against the given database.
	/// </summary>
	/// <param name="connectionString">The connection string used to connect to the database.</param>
	/// <param name="databaseName">The name of the database where the script will be executed.</param>
	/// <param name="sqlScriptFilePath">The file path of the SQL script to be executed.</param>
	/// <exception cref="DisposableDatabasesException">Thrown when there is an error executing the SQL script.</exception>
	/// <exception cref="FileNotFoundException">Thrown when the SQL script file is not found at the specified path.</exception>
	public Task ExecuteSqlScriptAsync(string? connectionString, string? databaseName, string? sqlScriptFilePath)
	{
		return ExecuteSqlScriptAsync(connectionString, databaseName, sqlScriptFilePath, CancellationToken.None);
	}

	/// <summary>
	/// Executes an SQL script asynchronously against a specified SQL Server database.
	/// </summary>
	/// <param name="connectionString">The connection string used to connect to the SQL Server database.</param>
	/// <param name="databaseName">The name of the database where the SQL script will be executed.</param>
	/// <param name="sqlScriptFilePath">The file path to the SQL script to be executed.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the operation if needed.</param>
	/// <exception cref="DisposableDatabasesException">Thrown when an error occurs while executing the SQL script.</exception>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <exception cref="DisposableDatabasesException">Thrown when there is an error executing the SQL script.</exception>
	/// <exception cref="FileNotFoundException">Thrown when the SQL script file is not found at the specified path.</exception>
	public Task ExecuteSqlScriptAsync(string? connectionString, string? databaseName, string? sqlScriptFilePath, CancellationToken cancellationToken)
	{
		Guard.IsNotNullOrWhiteSpace(connectionString);
		Guard.IsNotNullOrWhiteSpace(databaseName);
		Guard.IsNotNullOrWhiteSpace(sqlScriptFilePath);

		if (!File.Exists(sqlScriptFilePath))
		{
			throw new FileNotFoundException($"SQL Script file does not exist at path {sqlScriptFilePath}.", sqlScriptFilePath);
		}

		return ExecuteSqlScriptInternalAsync(connectionString, databaseName, sqlScriptFilePath, cancellationToken);
	}

	/// <summary>
	/// Executes the specified SQL script against the given database.
	/// </summary>
	/// <param name="connectionString">The connection string used to connect to the database.</param>
	/// <param name="databaseName">The name of the database where the script will be executed.</param>
	/// <param name="sqlScriptFilePath">The file path of the SQL script to be executed.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <exception cref="DisposableDatabasesException">Thrown when there is an error executing the SQL script.</exception>
	/// <exception cref="FileNotFoundException">Thrown when the SQL script file is not found at the specified path.</exception>
	private static async Task ExecuteSqlScriptInternalAsync(string connectionString, string databaseName, string sqlScriptFilePath, CancellationToken cancellationToken)
	{
		try
		{
#if NETCOREAPP2_0_OR_GREATER
				string sqlScript = await File.ReadAllTextAsync(sqlScriptFilePath, cancellationToken).ConfigureAwait(false);
#else
			string sqlScript = File.ReadAllText(sqlScriptFilePath);
#endif

			using (var connection = new SqlConnection(connectionString))
			{
#pragma warning disable CA2100
				using (var command = new SqlCommand(sqlScript, connection))
#pragma warning restore CA2100
				{
					await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
					await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
				}
			}
		}
		catch (Exception e) when (e is SqlException || e is ArgumentException)
		{
			throw new DisposableDatabasesException($"SQL Server: Failed to execute SQL script at path {sqlScriptFilePath} against database {databaseName}.", e);
		}
	}
}
