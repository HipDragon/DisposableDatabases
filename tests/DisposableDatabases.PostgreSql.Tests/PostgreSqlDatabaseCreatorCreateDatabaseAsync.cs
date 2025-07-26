// <copyright file="PostgreSqlDatabaseCreatorCreateDatabaseAsync.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Globalization;
using DisposableDatabases.Exceptions;
using Npgsql;

namespace DisposableDatabases.PostgreSql.Tests;

[TestFixture]
public class PostgreSqlDatabaseCreatorCreateDatabaseAsync
{
	private readonly PostgreSqlDatabaseCreator _databaseCreator;

	public PostgreSqlDatabaseCreatorCreateDatabaseAsync()
	{
		_databaseCreator = new PostgreSqlDatabaseCreator();
	}

	[Test]
	public async Task ThrowsArgumentExceptionGiveNullConnectionString()
	{
		// Act
		Func<Task<string>> function = () => _databaseCreator.CreateDatabaseAsync(null, "TestDatabase");

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptyConnectionString")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceConnectionString")]
	public async Task ThrowsArgumentExceptionGiveEmptyOrWhitespaceConnectionString(string invalidConnectionString)
	{
		// Act
		Func<Task<string>> function = () => _databaseCreator.CreateDatabaseAsync(invalidConnectionString, "TestDatabase");

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<ArgumentException>());
	}

	[Test]
	public async Task ThrowsArgumentExceptionGiveNullDatabaseName()
	{
		// Act
		Func<Task<string>> function = () => _databaseCreator.CreateDatabaseAsync("ValidConnectionString", null);

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptyDatabaseName")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceDatabaseName")]
	public async Task ThrowsArgumentExceptionGiveEmptyOrWhitespaceDatabaseName(string invalidDatabaseName)
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
		await Assert.ThatAsync(function, Throws.TypeOf<DisposableDatabasesException>().With.Message.EqualTo("PostgreSQL: Failed to create database TestDatabase."));
	}

	[Test]
	public async Task ThrowsDisposableDatabaseExceptionGivenInvalidDatabaseName()
	{
		// Arrange
		string postgreSqlConnectionString = ConfigurationHelper.GetRequiredValue("PostgreSqlConnectionString");
		string invalidDatabaseName = "\"-InvalidDatabaseName-";

		// Act
		Func<Task<string>> function = () => _databaseCreator.CreateDatabaseAsync(postgreSqlConnectionString, invalidDatabaseName);

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<DisposableDatabasesException>().With.Message.EqualTo($"PostgreSQL: Failed to create database {invalidDatabaseName}."));
	}

	[Test]
	public async Task CreatesDatabase()
	{
		// Arrange
		string expectedDatabaseName = $"TestCreateDatabaseAsync-{Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)}";
		string postgreSqlConnectionString = ConfigurationHelper.GetRequiredValue("PostgreSqlConnectionString");
		string expectedConnectionString = new NpgsqlConnectionStringBuilder(postgreSqlConnectionString) { Database = expectedDatabaseName }.ConnectionString;

		// Act
		string connectionString = await _databaseCreator.CreateDatabaseAsync(postgreSqlConnectionString, expectedDatabaseName);

		// Assert
		using (Assert.EnterMultipleScope())
		{
			Assert.That(connectionString, Is.EqualTo(expectedConnectionString));
			await Assert.ThatAsync(() => PostgreSqlDatabaseUtilities.DatabaseExistsAsync(postgreSqlConnectionString, expectedDatabaseName), Is.True);

			// Cleanup
			await PostgreSqlDatabaseUtilities.DropDatabaseAsync(postgreSqlConnectionString, expectedDatabaseName);
		}
	}
}
