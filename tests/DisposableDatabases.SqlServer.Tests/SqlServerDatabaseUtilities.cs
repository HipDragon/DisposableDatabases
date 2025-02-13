// <copyright file="SqlServerDatabaseUtilities.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using Microsoft.Data.SqlClient;

namespace DisposableDatabases.SqlServer.Tests;

/// <summary>
/// A utility class for managing SQL Server databases used in the context of tests.
/// Provides methods to create, check existence, and drop databases.
/// </summary>
internal static class SqlServerDatabaseUtilities
{
	/// <summary>
	/// The SQL command used to check if a database exists.
	/// Usage: This constant is used in methods that need to execute an SQL command to verify the existence of a database.
	/// The database name parameter must be provided when executing this command.
	/// </summary>
	private const string DatabaseExistsSql = "SELECT database_id FROM sys.databases WHERE name = @databaseName";

	/// <summary>
	/// The SQL command used to check if a specific table exists in the database.
	/// Usage: This constant is used in methods that execute an SQL query to determine the existence of a table.
	/// The table name parameter must be provided when executing this command.
	/// </summary>
	private const string TableExistsSql = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName";

	/// <summary>
	/// Creates a new database with the specified name on the SQL Server instance defined by the connection string.
	/// </summary>
	/// <param name="connectionString">The connection string to the SQL Server instance.</param>
	/// <param name="databaseName">The name of the database to create.</param>
	internal static async Task CreateDatabaseAsync(string connectionString, string databaseName)
	{
		try
		{
			using (var connection = new SqlConnection(connectionString))
			{
#pragma warning disable CA2100
				using (var command = new SqlCommand($"CREATE DATABASE [{databaseName}]", connection))
#pragma warning restore CA2100
				{
					await connection.OpenAsync();
					await command.ExecuteNonQueryAsync();
				}
			}
		}
		catch (Exception e) when (e is SqlException || e is ArgumentException)
		{
			await TestContext.Out.WriteLineAsync($"Failed to create database \"{databaseName}\".");
		}
	}

	/// <summary>
	/// Checks if a database with the specified name exists on the SQL Server instance defined by the connection string.
	/// </summary>
	/// <param name="connectionString">The connection string to the SQL Server instance.</param>
	/// <param name="databaseName">The name of the database to check for existence.</param>
	/// <return>True if the database exists, otherwise false.</return>
	internal static async Task<bool> DatabaseExistsAsync(string connectionString, string databaseName)
	{
		try
		{
			using (var connection = new SqlConnection(connectionString))
			{
				using (var command = new SqlCommand(DatabaseExistsSql, connection))
				{
					command.Parameters.AddWithValue("@databaseName", databaseName);
					await connection.OpenAsync();
					object? result = await command.ExecuteScalarAsync();

					return result != null;
				}
			}
		}
		catch (Exception e) when (e is SqlException || e is ArgumentException)
		{
			return false;
		}
	}

	/// <summary>
	/// Drops the specified database on the SQL Server instance defined by the connection string.
	/// </summary>
	/// <param name="connectionString">The connection string to the SQL Server instance.</param>
	/// <param name="databaseName">The name of the database to drop.</param>
	internal static async Task DropDatabaseAsync(string connectionString, string databaseName)
	{
		try
		{
			using (var connection = new SqlConnection(connectionString))
			{
#pragma warning disable CA2100
				using (var command = new SqlCommand($"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{databaseName}];", connection))
#pragma warning restore CA2100
				{
					await connection.OpenAsync();
					await command.ExecuteNonQueryAsync();
				}
			}
		}
		catch (Exception e) when (e is SqlException || e is ArgumentException)
		{
			await TestContext.Out.WriteLineAsync($"Database was not dropped. Please drop the \"{databaseName}\" database manually.");
		}
	}

	/// <summary>
	/// Checks if a table with the specified name exists in the SQL Server database defined by the connection string.
	/// </summary>
	/// <param name="connectionString">The connection string to the SQL Server database.</param>
	/// <param name="tableName">The name of the table to check for existence.</param>
	/// <returns>True if the table exists, otherwise false.</returns>
	internal static async Task<bool> TableExistsAsync(string connectionString, string tableName)
	{
		try
		{
			using (var connection = new SqlConnection(connectionString))
			{
				using (var command = new SqlCommand(TableExistsSql, connection))
				{
					command.Parameters.AddWithValue("@tableName", tableName);

					await connection.OpenAsync();
					object? result = await command.ExecuteScalarAsync();

					return result != null;
				}
			}
		}
		catch (Exception e) when (e is SqlException || e is ArgumentException)
		{
			return false;
		}
	}

	/// <summary>
	/// Tests the connectivity to the SQL Server instance defined by the connection string.
	/// </summary>
	/// <param name="connectionString">The connection string to the SQL Server instance.</param>
	/// <return>True if the connection to the SQL Server instance is successful, otherwise false.</return>
	internal static async Task<bool> TestDatabaseConnectionAsync(string connectionString)
	{
		try
		{
			using (var connection = new SqlConnection(connectionString))
			{
				await connection.OpenAsync();
			}

			return true;
		}
		catch (Exception e) when (e is SqlException || e is ArgumentException)
		{
			return false;
		}
	}
}
