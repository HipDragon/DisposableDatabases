// <copyright file="PostgreSqlDatabaseCreator.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Diagnostics;
using DisposableDatabases.Exceptions;
using DisposableDatabases.Interfaces.DatabaseOperations;
using Npgsql;

namespace DisposableDatabases.PostgreSql;

/// <summary>
/// PostgreSqlDatabaseCreator is responsible for creating PostgreSQL databases.
/// It implements the IDatabaseCreator interface and provides both synchronous
/// and asynchronous methods for creating a database.
/// </summary>
public class PostgreSqlDatabaseCreator : IDatabaseCreator
{
	/// <inheritdoc />
	public Task<string> CreateDatabaseAsync([NotNull] string? connectionString, [NotNull] string? databaseName)
	{
		return CreateDatabaseAsync(connectionString, databaseName, CancellationToken.None);
	}

	/// <inheritdoc />
	public Task<string> CreateDatabaseAsync([NotNull] string? connectionString, [NotNull] string? databaseName, CancellationToken cancellationToken)
	{
		Guard.IsNotNullOrWhiteSpace(connectionString);
		Guard.IsNotNullOrWhiteSpace(databaseName);

		return CreateDatabaseInternalAsync(connectionString, databaseName, cancellationToken);
	}

	private static async Task<string> CreateDatabaseInternalAsync(string connectionString, string databaseName, CancellationToken cancellationToken)
	{
		try
		{
			using (var npgsqlConnection = new NpgsqlConnection(connectionString))
			{
#pragma warning disable CA2100
				using (var npgsqlCommand = new NpgsqlCommand($"CREATE DATABASE \"{databaseName}\"", npgsqlConnection))
#pragma warning restore CA2100
				{
					await npgsqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
					await npgsqlCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
				}
			}

			return GenerateDatabaseConnectionString(connectionString, databaseName);
		}
		catch (Exception e) when (e is NpgsqlException || e is ArgumentException)
		{
			throw new DisposableDatabasesException($"PostgreSQL: Failed to create database {databaseName}.", e);
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
		var npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString) { Database = databaseName };

		return npgsqlConnectionStringBuilder.ConnectionString;
	}
}
