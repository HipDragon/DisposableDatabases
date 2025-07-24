// <copyright file="SqlServerSqlScriptExecutorExecuteSqlScriptAsync.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Reflection;
using DisposableDatabases.Exceptions;
using Microsoft.Data.SqlClient;

namespace DisposableDatabases.SqlServer.Tests;

[TestFixture]
public class SqlServerSqlScriptExecutorExecuteSqlScriptAsync
{
	private readonly SqlServerSqlScriptExecutor _sqlServerSqlScriptExecutor = new();

	internal static async Task WriteTestSqlScriptToFileAsync(string filePath)
	{
		Assembly assembly = typeof(SqlServerSqlScriptExecutorExecuteSqlScriptAsync).Assembly;

		using (Stream? stream = assembly.GetManifestResourceStream(EmbeddedResources.TestSqlScript))
		{
			if (stream is null)
			{
				throw new InvalidOperationException($"Embedded test SQL script '{EmbeddedResources.TestSqlScript}' not found.");
			}

			using (var streamReader = new StreamReader(stream))
			{
				using (StreamWriter streamWriter = File.CreateText(filePath))
				{
					await streamWriter.WriteAsync(await streamReader.ReadToEndAsync());
				}
			}
		}
	}

	[Test]
	public async Task ThrowsArgumentNullExceptionGivenNullConnectionString()
	{
		// Arrange
		using (var temporarySqlFile = new TemporaryFile(".sql"))
		{
			// Act
			AsyncTestDelegate function = () => _sqlServerSqlScriptExecutor.ExecuteSqlScriptAsync(null, "TestDatabase", temporarySqlFile.FilePath);

			// Assert
			await Assert.ThatAsync(function, Throws.TypeOf<ArgumentNullException>());
		}
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptyConnectionString")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceConnectionString")]
	public async Task ThrowsArgumentExceptionGivenEmptyOrWhitespaceConnectionString(string invalidConnectionString)
	{
		// Arrange
		using (var temporarySqlFile = new TemporaryFile(".sql"))
		{
			// Act
			AsyncTestDelegate function = () => _sqlServerSqlScriptExecutor.ExecuteSqlScriptAsync(invalidConnectionString, "TestDatabase", temporarySqlFile.FilePath);

			// Assert
			await Assert.ThatAsync(function, Throws.TypeOf<ArgumentException>());
		}
	}

	[Test]
	public async Task ThrowsArgumentNullExceptionGivenNullDatabaseName()
	{
		// Arrange
		using (var temporarySqlFile = new TemporaryFile(".sql"))
		{
			// Act
			AsyncTestDelegate function = () => _sqlServerSqlScriptExecutor.ExecuteSqlScriptAsync("ValidConnectionString", null, temporarySqlFile.FilePath);

			// Assert
			await Assert.ThatAsync(function, Throws.TypeOf<ArgumentNullException>());
		}
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptyDatabaseName")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceDatabaseName")]
	public async Task ThrowsArgumentExceptionGivenEmptyOrWhitespaceDatabaseName(string invalidDatabaseName)
	{
		// Arrange
		using (var temporarySqlFile = new TemporaryFile(".sql"))
		{
			// Act
			AsyncTestDelegate function = () => _sqlServerSqlScriptExecutor.ExecuteSqlScriptAsync("ValidConnectionString", invalidDatabaseName, temporarySqlFile.FilePath);

			// Assert
			await Assert.ThatAsync(function, Throws.TypeOf<ArgumentException>());
		}
	}

	[Test]
	public async Task ThrowsArgumentNullExceptionGivenNullSqlScriptFilePath()
	{
		// Act
		AsyncTestDelegate function = () => _sqlServerSqlScriptExecutor.ExecuteSqlScriptAsync("ValidConnectionString", "TestDatabase", null);

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptySqlScriptFilePath")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceSqlScriptFilePath")]
	public async Task ThrowsArgumentExceptionGivenEmptyOrWhitespaceSqlScriptFilePath(string invalidFilePath)
	{
		// Act
		AsyncTestDelegate function = () => _sqlServerSqlScriptExecutor.ExecuteSqlScriptAsync("ValidConnectionString", "TestDatabase", invalidFilePath);

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<ArgumentException>());
	}

	[Test]
	public async Task ThrowsFileNotFoundExceptionGivenSqlScriptFileDoesNotExist()
	{
		// Arrange
		var temporarySqlFile = new TemporaryFile(".sql");
		string invalidFilePath = temporarySqlFile.FilePath;
		temporarySqlFile.Dispose();

		// Act
		AsyncTestDelegate function = () => _sqlServerSqlScriptExecutor.ExecuteSqlScriptAsync("ValidConnectionString", "TestDatabase", invalidFilePath);

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<FileNotFoundException>().With.Message.EqualTo($"SQL Script file does not exist at path {invalidFilePath}."));
	}

	[Test]
	public async Task ThrowsDisposableDatabaseExceptionGivenInvalidConnectionString()
	{
		// Arrange
		const string databaseName = "TestDatabase";

		using (var temporarySqlFile = new TemporaryFile(".sql"))
		{
			// Act
			AsyncTestDelegate function = () => _sqlServerSqlScriptExecutor.ExecuteSqlScriptAsync("InvalidConnectionString", databaseName, temporarySqlFile.FilePath);

			// Assert
			await Assert.ThatAsync(function,
								   Throws.TypeOf<DisposableDatabasesException>()
										 .With.Message.EqualTo($"SQL Server: Failed to execute SQL script at path {temporarySqlFile.FilePath} against database {databaseName}."));
		}
	}

	[Test]
	public async Task ThrowsDisposableDatabaseExceptionGivenInvalidSqlScript()
	{
		// Arrange
		string sqlServerConnectionString = ConfigurationHelper.GetRequiredValue("SqlServerConnectionString");
		string databaseName = "TestExecuteSqlScriptAsync-" + Guid.NewGuid();
		await SqlServerDatabaseUtilities.CreateDatabaseAsync(sqlServerConnectionString, databaseName);
		Assume.That(await SqlServerDatabaseUtilities.DatabaseExistsAsync(sqlServerConnectionString, databaseName), Is.True, "Test database should exist before the test runs.");

		using (var temporarySqlFile = new TemporaryFile(".sql"))
		{
			using (StreamWriter streamWriter = File.CreateText(temporarySqlFile.FilePath))
			{
				await streamWriter.WriteLineAsync("Invalid SQL Script");
			}

			// Act
			AsyncTestDelegate function = () => _sqlServerSqlScriptExecutor.ExecuteSqlScriptAsync(sqlServerConnectionString, databaseName, temporarySqlFile.FilePath);

			// Assert
			try
			{
				await Assert.ThatAsync(function,
				                       Throws.TypeOf<DisposableDatabasesException>()
				                             .With.Message.EqualTo($"SQL Server: Failed to execute SQL script at path {temporarySqlFile.FilePath} against database {databaseName}."));
			}
			finally
			{
				await SqlServerDatabaseUtilities.DropDatabaseAsync(sqlServerConnectionString, databaseName);
			}
		}
	}

	[Test]
	public async Task ExecutesSqlScript()
	{
		// Arrange
		string expectedDatabaseName = "TestExecuteSqlScriptAsync-" + Guid.NewGuid();
		string sqlServerConnectionString = ConfigurationHelper.GetRequiredValue("SqlServerConnectionString");

		using (var temporarySqlFile = new TemporaryFile(".sql"))
		{
			Task writeSqlScriptTask = WriteTestSqlScriptToFileAsync(temporarySqlFile.FilePath);
			Task createSqlDatabase = SqlServerDatabaseUtilities.CreateDatabaseAsync(sqlServerConnectionString, expectedDatabaseName);
			await Task.WhenAll(writeSqlScriptTask, createSqlDatabase);
			Assume.That(await SqlServerDatabaseUtilities.DatabaseExistsAsync(sqlServerConnectionString, expectedDatabaseName), Is.True, "Test database should exist before the test runs.");

			string newConnectionString = new SqlConnectionStringBuilder(sqlServerConnectionString) { InitialCatalog = expectedDatabaseName }.ConnectionString;

			try
			{
				// Act
				await _sqlServerSqlScriptExecutor.ExecuteSqlScriptAsync(newConnectionString, expectedDatabaseName, temporarySqlFile.FilePath);

				// Assert
				await Assert.ThatAsync(() => SqlServerDatabaseUtilities.TableExistsAsync(newConnectionString, "SampleTestTable"), Is.True);
			}
			finally
			{
				// Cleanup
				await SqlServerDatabaseUtilities.DropDatabaseAsync(sqlServerConnectionString, expectedDatabaseName);
			}
		}
	}
}
