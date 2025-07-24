// <copyright file="SqlServerDatabaseDropper.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using CommunityToolkit.Diagnostics;
using DisposableDatabases.Exceptions;
using DisposableDatabases.Interfaces.DatabaseOperations;
using Microsoft.Data.SqlClient;

namespace DisposableDatabases.SqlServer;

/// <summary>
/// Provides implementation to drop an SQL Server database.
/// </summary>
public class SqlServerDatabaseDropper : IDatabaseDropper
{
	/// <inheritdoc />
	public Task DropDatabaseAsync(string? connectionString, string? databaseName)
	{
		return DropDatabaseAsync(connectionString, databaseName, CancellationToken.None);
	}

	/// <inheritdoc />
	public Task DropDatabaseAsync(string? connectionString, string? databaseName, CancellationToken cancellationToken)
	{
		Guard.IsNotNullOrWhiteSpace(connectionString);
		Guard.IsNotNullOrWhiteSpace(databaseName);

		return DropDatabaseInternalAsync(connectionString, databaseName, cancellationToken);
	}

	/// <summary>
	/// Asynchronously drops a specified SQL Server database using the provided connection string.
	/// </summary>
	/// <param name="connectionString">The connection string to connect to the SQL Server instance.</param>
	/// <param name="databaseName">The name of the database to be dropped.</param>
	/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <exception cref="DisposableDatabasesException">
	/// Thrown when there is an error in dropping the database, such as SQL or argument exceptions.
	/// </exception>
	private static async Task DropDatabaseInternalAsync(string connectionString, string databaseName, CancellationToken cancellationToken)
	{
		try
		{
			using (var connection = new SqlConnection(connectionString))
			{
#pragma warning disable CA2100
				using (var command = new SqlCommand($"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{databaseName}];", connection))
#pragma warning disable CA2100
				{
					await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
					await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
				}
			}
		}
		catch (Exception e) when (e is SqlException || e is ArgumentException)
		{
			throw new DisposableDatabasesException($"SQL Server: Failed to drop the database {databaseName}.", e);
		}
	}
}
