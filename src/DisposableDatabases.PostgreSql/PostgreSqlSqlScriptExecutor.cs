// <copyright file="PostgreSqlSqlScriptExecutor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Diagnostics;
using DisposableDatabases.Exceptions;
using DisposableDatabases.Interfaces.DatabaseOperations;
using Npgsql;

namespace DisposableDatabases.PostgreSql;

/// <summary>
/// Represents an executor for running SQL scripts against PostgreSQL databases.
/// </summary>
public class PostgreSqlSqlScriptExecutor : ISqlScriptExecutor
{
	/// <summary>
	/// Executes the specified SQL script against the given PostgreSQL database using the provided connection string.
	/// </summary>
	/// <param name="connectionString">The connection string to the PostgreSQL database server.</param>
	/// <param name="databaseName">The name of the database where the script will be executed.</param>
	/// <param name="sqlScriptFilePath">The path to the SQL script file to be executed.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	/// <exception cref="DisposableDatabasesException">Thrown when there is an error executing the SQL script.</exception>
	/// <exception cref="FileNotFoundException">Thrown when the SQL script file is not found at the specified path.</exception>
	public Task ExecuteSqlScriptAsync(string? connectionString, string? databaseName, string? sqlScriptFilePath)
	{
		return ExecuteSqlScriptAsync(connectionString, databaseName, sqlScriptFilePath, CancellationToken.None);
	}

	/// <summary>
	/// Executes the specified SQL script against the given PostgreSQL database using the provided connection string.
	/// </summary>
	/// <param name="connectionString">The connection string to the PostgreSQL database server.</param>
	/// <param name="databaseName">The name of the database where the script will be executed.</param>
	/// <param name="sqlScriptFilePath">The path to the SQL script file to be executed.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
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
	/// Executes the specified SQL script against the provided database using the given connection string.
	/// </summary>
	/// <param name="connectionString">The connection string to the PostgreSQL database server.</param>
	/// <param name="databaseName">The name of the database where the script will be executed.</param>
	/// <param name="sqlScriptFilePath">The file path to the SQL script to be executed.</param>
	/// <param name="cancellationToken">The cancellation token used to propagate notifications that the operation should be canceled.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
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

			using (var connection = new NpgsqlConnection(connectionString))
			{
#pragma warning disable CA2100
				using (var command = new NpgsqlCommand(sqlScript, connection))
#pragma warning restore CA2100
				{
					await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
					await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
				}
			}
		}
		catch (Exception e) when (e is NpgsqlException || e is ArgumentException)
		{
			throw new DisposableDatabasesException($"PostgreSQL: Failed to execute SQL script at path {sqlScriptFilePath} against database {databaseName}.", e);
		}
	}
}
