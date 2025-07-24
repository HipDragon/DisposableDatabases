// <copyright file="PostgreSqlDatabaseDropper.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using CommunityToolkit.Diagnostics;
using DisposableDatabases.Exceptions;
using DisposableDatabases.Interfaces.DatabaseOperations;
using Npgsql;

namespace DisposableDatabases.PostgreSql;

/// <summary>
/// Provides implementation to drop a PostgreSQL database.
/// </summary>
public class PostgreSqlDatabaseDropper : IDatabaseDropper
{
	/// <summary>
	/// Asynchronously drops a specified PostgreSQL database using the provided connection string.
	/// </summary>
	/// <param name="connectionString">The connection string used to establish a connection to the PostgreSQL server.</param>
	/// <param name="databaseName">The name of the database to be dropped.</param>
	/// <returns>A task representing the asynchronous operation of dropping the database.</returns>
	public Task DropDatabaseAsync(string? connectionString, string? databaseName)
	{
		return DropDatabaseAsync(connectionString, databaseName, CancellationToken.None);
	}

	/// <summary>
	/// Asynchronously drops a PostgreSQL database using a specified connection string and database name.
	/// </summary>
	/// <param name="connectionString">The connection string used to connect to PostgreSQL.</param>
	/// <param name="databaseName">The name of the database to be dropped.</param>
	/// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <exception cref="ArgumentException">Thrown when the connection string or database name is null, empty, or consists only of white-space characters.</exception>
	public Task DropDatabaseAsync(string? connectionString, string? databaseName, CancellationToken cancellationToken)
	{
		Guard.IsNotNullOrWhiteSpace(connectionString);
		Guard.IsNotNullOrWhiteSpace(databaseName);

		return DropDatabaseInternalAsync(connectionString, databaseName, cancellationToken);
	}

	/// <summary>
	/// Asynchronously executes the operation to drop a specified PostgreSQL database using the provided connection string.
	/// </summary>
	/// <param name="connectionString">The connection string used to establish a connection to the PostgreSQL server.</param>
	/// <param name="databaseName">The name of the database to be dropped.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task representing the asynchronous operation of dropping the database.</returns>
	private static async Task DropDatabaseInternalAsync(string connectionString, string databaseName, CancellationToken cancellationToken)
	{
		try
		{
			using (var npgsqlConnection = new NpgsqlConnection(connectionString))
			{
#pragma warning disable CA2100
				using (var dropCommand = new NpgsqlCommand($"DROP DATABASE \"{databaseName}\" WITH (FORCE)", npgsqlConnection))
#pragma warning restore CA2100
				{
					await npgsqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
					await dropCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
				}
			}
		}
		catch (Exception e) when (e is NpgsqlException || e is ArgumentException)
		{
			throw new DisposableDatabasesException($"PostgreSQL: Failed to drop the database {databaseName}.", e);
		}
	}
}
