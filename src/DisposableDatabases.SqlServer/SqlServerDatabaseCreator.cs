// <copyright file="SqlServerDatabaseCreator.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using CommunityToolkit.Diagnostics;
using DisposableDatabases.Exceptions;
using DisposableDatabases.Interfaces.DatabaseOperations;
using Microsoft.Data.SqlClient;

namespace DisposableDatabases.SqlServer;

/// <summary>
/// Responsible for creating SQL Server databases.
/// </summary>
public class SqlServerDatabaseCreator : IDatabaseCreator
{
	/// <inheritdoc />
	public Task<string> CreateDatabaseAsync(string? connectionString, string? databaseName)
	{
		return CreateDatabaseAsync(connectionString, databaseName, CancellationToken.None);
	}

	/// <inheritdoc />
	public Task<string> CreateDatabaseAsync(string? connectionString, string? databaseName, CancellationToken cancellationToken)
	{
		Guard.IsNotNullOrWhiteSpace(connectionString);
		Guard.IsNotNullOrWhiteSpace(databaseName);

		return CreateDatabaseInternalAsync(connectionString, databaseName, cancellationToken);
	}

	/// <inheritdoc cref="CreateDatabaseAsync(string, string, CancellationToken)" />
	private static async Task<string> CreateDatabaseInternalAsync(string connectionString, string databaseName, CancellationToken cancellationToken)
	{
		try
		{
			using (var connection = new SqlConnection(connectionString))
			{
#pragma warning disable CA2100
				using (var command = new SqlCommand($"CREATE DATABASE [{databaseName}]", connection))
#pragma warning restore CA2100
				{
					await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
					await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
				}
			}

			return GenerateDatabaseConnectionString(connectionString, databaseName);
		}
		catch (Exception e) when (e is SqlException || e is ArgumentException)
		{
			throw new DisposableDatabasesException($"SQL Server: Failed to create database {databaseName}.", e);
		}
	}

	/// <summary>
	/// Generates a database connection string with the specified database name.
	/// </summary>
	/// <param name="connectionString">The base connection string.</param>
	/// <param name="databaseName">The name of the database to include in the connection string.</param>
	/// <returns>The generated database connection string.</returns>
	private static string GenerateDatabaseConnectionString(string connectionString, string databaseName)
	{
		var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = databaseName };

		return sqlConnectionStringBuilder.ConnectionString;
	}
}
