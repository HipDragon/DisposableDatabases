// <copyright file="PostgreSqlDatabaseUtilities.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using Npgsql;

namespace DisposableDatabases.PostgreSql.Tests;

/// <summary>
/// A utility class for managing PostgreSQL databases used in the context of tests.
/// Provides methods to create, check existence, and drop databases.
/// </summary>
internal static class PostgreSqlDatabaseUtilities
{
	/// <summary>
	/// The SQL command used to check if a database exists.
	/// Usage: This constant is used in methods that need to execute an SQL command to verify the existence of a database.
	/// The database name parameter must be provided when executing this command.
	/// </summary>
	private const string DatabaseExistsSql = "SELECT 1 FROM pg_database WHERE datname = @databaseName;";

	private const string TableExistsSql = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName";

	/// <summary>
	/// Creates a new database with the specified name on the PostgreSQL instance defined by the connection string.
	/// </summary>
	/// <param name="connectionString">The connection string to the PostgreSQL instance.</param>
	/// <param name="databaseName">The name of the database to create.</param>
	internal static async Task CreateDatabaseAsync(string connectionString, string databaseName)
	{
		try
		{
			using (var connection = new NpgsqlConnection(connectionString))
			{
				await connection.OpenAsync();

#pragma warning disable CA2100
				using (var command = new NpgsqlCommand($"CREATE DATABASE \"{databaseName}\"", connection))
#pragma warning restore CA2100
				{
					await command.ExecuteNonQueryAsync();
				}
			}
		}
		catch (Exception e) when (e is NpgsqlException || e is ArgumentException)
		{
			await TestContext.Out.WriteLineAsync($"Failed to create database \"{databaseName}\".");
		}
	}

	/// <summary>
	/// Checks if a database with the specified name exists on the PostgreSQL instance defined by the connection string.
	/// </summary>
	/// <param name="connectionString">The connection string to the PostgreSQL instance.</param>
	/// <param name="databaseName">The name of the database to check for existence.</param>
	/// <returns>True if the database exists, otherwise false.</returns>
	internal static async Task<bool> DatabaseExistsAsync(string connectionString, string databaseName)
	{
		try
		{
			using (var connection = new NpgsqlConnection(connectionString))
			{
				await connection.OpenAsync();

				using (var command = new NpgsqlCommand(DatabaseExistsSql, connection))
				{
					command.Parameters.AddWithValue("@databaseName", databaseName);
					object? result = await command.ExecuteScalarAsync();

					return result != null;
				}
			}
		}
		catch (Exception e) when (e is NpgsqlException || e is ArgumentException)
		{
			return false;
		}
	}

	/// <summary>
	/// Drops the specified database on the PostgreSQL instance defined by the connection string.
	/// </summary>
	/// <param name="connectionString">The connection string to the PostgreSQL instance.</param>
	/// <param name="databaseName">The name of the database to drop.</param>
	internal static async Task DropDatabaseAsync(string connectionString, string databaseName)
	{
		try
		{
			using (var connection = new NpgsqlConnection(connectionString))
			{
				await connection.OpenAsync();

#pragma warning disable CA2100
				using (var command = new NpgsqlCommand($"DROP DATABASE \"{databaseName}\" WITH (FORCE)", connection))
#pragma warning restore CA2100
				{
					await command.ExecuteNonQueryAsync();
				}
			}
		}
		catch (Exception e) when (e is NpgsqlException || e is ArgumentException)
		{
			await TestContext.Out.WriteLineAsync($"Database was not dropped. Please drop the \"{databaseName}\" database manually.");
		}
	}

	/// <summary>
	/// Checks if a table with the specified name exists in the PostgreSQL database defined by the connection string.
	/// </summary>
	/// <param name="connectionString">The connection string to the PostgreSQL database.</param>
	/// <param name="tableName">The name of the table to check for existence.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains <c>true</c> if the table exists, otherwise <c>false</c>.</returns>
	internal static async Task<bool> TableExistsAsync(string connectionString, string tableName)
	{
		try
		{
			await using (var connection = new NpgsqlConnection(connectionString))
			{
				using (var command = new NpgsqlCommand(TableExistsSql, connection))
				{
					command.Parameters.AddWithValue("@tableName", tableName);

					await connection.OpenAsync();
					object? result = await command.ExecuteScalarAsync();

					return result != null;
				}
			}
		}
		catch (Exception e) when (e is NpgsqlException || e is ArgumentException)
		{
			return false;
		}
	}

	/// <summary>
	/// Tests the connectivity to a PostgreSQL database using the provided connection string.
	/// </summary>
	/// <param name="connectionString">The connection string to the PostgreSQL database.</param>
	/// <returns>True if the connection is successfully established, otherwise false.</returns>
	internal static async Task<bool> TestDatabaseConnectionAsync(string connectionString)
	{
		try
		{
			await using (var connection = new NpgsqlConnection(connectionString))
			{
				await connection.OpenAsync();
			}

			return true;
		}
		catch (Exception e) when (e is NpgsqlException || e is ArgumentException)
		{
			return false;
		}
	}
}
