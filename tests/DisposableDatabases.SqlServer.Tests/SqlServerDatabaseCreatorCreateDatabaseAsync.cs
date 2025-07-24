// <copyright file="SqlServerDatabaseCreatorCreateDatabaseAsync.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Globalization;
using DisposableDatabases.Exceptions;
using Microsoft.Data.SqlClient;

namespace DisposableDatabases.SqlServer.Tests;

[TestFixture]
public class SqlServerDatabaseCreatorCreateDatabaseAsync
{
	private readonly SqlServerDatabaseCreator _databaseCreator = new();

	[Test]
	public async Task ThrowsArgumentNullExceptionGivenNullConnectionString()
	{
		// Act
		Func<Task<string>> function = () => _databaseCreator.CreateDatabaseAsync(null, "TestDatabase");

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptyConnectionString")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceConnectionString")]
	public async Task ThrowsArgumentExceptionGivenEmptyOrWhitespaceConnectionString(string invalidConnectionString)
	{
		// Act
		Func<Task<string>> function = () => _databaseCreator.CreateDatabaseAsync(invalidConnectionString, "TestDatabase");

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<ArgumentException>());
	}

	[Test]
	public async Task ThrowsArgumentNullExceptionGivenNullDatabaseName()
	{
		// Act
		Func<Task<string>> function = () => _databaseCreator.CreateDatabaseAsync("ValidConnectionString", null);

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptyDatabaseName")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceDatabaseName")]
	public async Task ThrowArgumentExceptionGiveEmptyOrWhitespaceDatabaseName(string invalidDatabaseName)
	{
		// Act
		Func<Task<string>> function = () => _databaseCreator.CreateDatabaseAsync("ValidConnectionString", invalidDatabaseName);

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<ArgumentException>());
	}

	[Test]
	public async Task ThrowsDisposableDatabaseExceptionGivenInvalidConnectionString()
	{
		// Act
		Func<Task<string>> function = () => _databaseCreator.CreateDatabaseAsync("InvalidConnectionString", "TestDatabase");

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<DisposableDatabasesException>().With.Message.EqualTo("SQL Server: Failed to create database TestDatabase."));
	}

	[Test]
	public async Task ThrowsDisposableDatabaseExceptionGivenInvalidDatabaseName()
	{
		// Arrange
		string sqlServerConnectionString = ConfigurationHelper.GetRequiredValue("SqlServerConnectionString");
		string invalidDatabaseName = "]-InvalidDatabaseName-";

		// Act
		Func<Task<string>> function = () => _databaseCreator.CreateDatabaseAsync(sqlServerConnectionString, invalidDatabaseName);

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<DisposableDatabasesException>().With.Message.EqualTo($"SQL Server: Failed to create database {invalidDatabaseName}."));
	}

	[Test]
	public async Task CreatesDatabase()
	{
		// Arrange
		string expectedDatabaseName = "TestCreateDatabaseAsync-" + Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
		string sqlServerConnectionString = ConfigurationHelper.GetRequiredValue("SqlServerConnectionString");
		string expectedConnectionString = new SqlConnectionStringBuilder(sqlServerConnectionString) { InitialCatalog = expectedDatabaseName }.ConnectionString;

		try
		{
			// Act
			string connectionString = await _databaseCreator.CreateDatabaseAsync(sqlServerConnectionString, expectedDatabaseName);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(connectionString, Is.EqualTo(expectedConnectionString));
				await Assert.ThatAsync(() => SqlServerDatabaseUtilities.DatabaseExistsAsync(sqlServerConnectionString, expectedDatabaseName), Is.True);
			}
		}
		finally
		{
			// Cleanup
			if (await SqlServerDatabaseUtilities.DatabaseExistsAsync(sqlServerConnectionString, expectedDatabaseName))
			{
				await SqlServerDatabaseUtilities.DropDatabaseAsync(sqlServerConnectionString, expectedDatabaseName);
			}
		}
	}
}
