// <copyright file="PostgreSqlDatabaseDropperDropDatabaseAsync.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Globalization;
using DisposableDatabases.Exceptions;

namespace DisposableDatabases.PostgreSql.Tests;

[TestFixture]
public class PostgreSqlDatabaseDropperDropDatabaseAsync
{
	private readonly PostgreSqlDatabaseDropper _databaseDropper;

	public PostgreSqlDatabaseDropperDropDatabaseAsync()
	{
		_databaseDropper = new PostgreSqlDatabaseDropper();
	}

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
		await Assert.ThatAsync(function, Throws.TypeOf<DisposableDatabasesException>().With.Message.EqualTo("PostgreSQL: Failed to drop the database TestDatabase."));
	}

	[Test]
	public async Task ThrowsDisposableDatabaseExceptionGivenInvalidDatabaseName()
	{
		// Arrange
		string postgreSqlConnectionString = ConfigurationHelper.GetRequiredValue("PostgreSqlConnectionString");
		string invalidDatabaseName = "InvalidDatabaseName-" + Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
		Assume.That(await PostgreSqlDatabaseUtilities.DatabaseExistsAsync(postgreSqlConnectionString, invalidDatabaseName), Is.False, "Test database should not exist before the test runs.");

		// Act
		AsyncTestDelegate function = () => _databaseDropper.DropDatabaseAsync(postgreSqlConnectionString, invalidDatabaseName);

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<DisposableDatabasesException>().With.Message.EqualTo($"PostgreSQL: Failed to drop the database {invalidDatabaseName}."));
	}

	[Test]
	public async Task DropsDatabase()
	{
		// Arrange
		string databaseName = "TestDropDatabase-" + Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
		string postgreSqlConnectionString = ConfigurationHelper.GetRequiredValue("PostgreSqlConnectionString");
		await PostgreSqlDatabaseUtilities.CreateDatabaseAsync(postgreSqlConnectionString, databaseName);
		Assume.That(await PostgreSqlDatabaseUtilities.DatabaseExistsAsync(postgreSqlConnectionString, databaseName), "Test database should exist before the test runs.");

		// Act
		await _databaseDropper.DropDatabaseAsync(postgreSqlConnectionString, databaseName);

		// Assert
		await Assert.ThatAsync(() => PostgreSqlDatabaseUtilities.DatabaseExistsAsync(postgreSqlConnectionString, databaseName), Is.False);
	}
}
