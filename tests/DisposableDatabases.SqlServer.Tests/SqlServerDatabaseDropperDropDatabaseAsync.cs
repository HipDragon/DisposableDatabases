// <copyright file="SqlServerDatabaseDropperDropDatabaseAsync.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Globalization;
using DisposableDatabases.Exceptions;

namespace DisposableDatabases.SqlServer.Tests;

[TestFixture]
public class SqlServerDatabaseDropperDropDatabaseAsync
{
	private readonly SqlServerDatabaseDropper _databaseDropper = new();

	[Test]
	public async Task ThrowsArgumentNullExceptionGivenNullConnectionString()
	{
		// Act
		AsyncTestDelegate function = () => _databaseDropper.DropDatabaseAsync(null, "TestDatabase");

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptyConnectionString")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceConnectionString")]
	public async Task ThrowsArgumentExceptionGivenEmptyOrWhitespaceConnectionString(string invalidConnectionString)
	{
		// Act
		AsyncTestDelegate function = () => _databaseDropper.DropDatabaseAsync(invalidConnectionString, "TestDatabase");

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<ArgumentException>());
	}

	[Test]
	public async Task ThrowsArgumentNullExceptionGivenNullDatabaseName()
	{
		// Act
		AsyncTestDelegate function = () => _databaseDropper.DropDatabaseAsync("ValidConnectionString", null);

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptyDatabaseName")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceDatabaseName")]
	public async Task ThrowsArgumentExceptionGivenEmptyOrWhitespaceDatabaseName(string invalidDatabaseName)
	{
		// Act
		AsyncTestDelegate function = () => _databaseDropper.DropDatabaseAsync("ValidConnectionString", invalidDatabaseName);

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<ArgumentException>());
	}

	[Test]
	public async Task ThrowsDisposableDatabaseExceptionGivenInvalidConnectionString()
	{
		// Act
		AsyncTestDelegate function = () => _databaseDropper.DropDatabaseAsync("InvalidConnectionString", "TestDatabase");

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<DisposableDatabasesException>().With.Message.EqualTo("SQL Server: Failed to drop the database TestDatabase."));
	}

	[Test]
	public async Task ThrowsDisposableDatabaseExceptionGivenInvalidDatabaseName()
	{
		// Arrange
		string sqlServerConnectionString = ConfigurationHelper.GetRequiredValue("SqlServerConnectionString");
		string invalidDatabaseName = "InvalidDatabaseName-" + Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
		Assume.That(await SqlServerDatabaseUtilities.DatabaseExistsAsync(sqlServerConnectionString, invalidDatabaseName), Is.False, "Test database should not exist before the test runs.");

		// Act
		AsyncTestDelegate function = () => _databaseDropper.DropDatabaseAsync(sqlServerConnectionString, invalidDatabaseName);

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<DisposableDatabasesException>().With.Message.EqualTo($"SQL Server: Failed to drop the database {invalidDatabaseName}."));
	}

	[Test]
	public async Task DropsDatabase()
	{
		// Arrange
		string databaseName = "TestDropDatabase" + Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
		string sqlServerConnectionString = ConfigurationHelper.GetRequiredValue("SqlServerConnectionString");
		await SqlServerDatabaseUtilities.CreateDatabaseAsync(sqlServerConnectionString, databaseName);
		Assume.That(await SqlServerDatabaseUtilities.DatabaseExistsAsync(sqlServerConnectionString, databaseName), "Test database should exist before the test runs.");

		try
		{
			// Act
			await _databaseDropper.DropDatabaseAsync(sqlServerConnectionString, databaseName);

			// Assert
			await Assert.ThatAsync(() => SqlServerDatabaseUtilities.DatabaseExistsAsync(sqlServerConnectionString, databaseName), Is.False);
		}
		finally
		{
			// Cleanup
			if (await SqlServerDatabaseUtilities.DatabaseExistsAsync(sqlServerConnectionString, databaseName))
			{
				await SqlServerDatabaseUtilities.DropDatabaseAsync(sqlServerConnectionString, databaseName);
			}
		}
	}
}
