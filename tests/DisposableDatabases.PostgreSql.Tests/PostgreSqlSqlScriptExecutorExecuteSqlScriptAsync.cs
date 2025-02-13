// <copyright file="PostgreSqlSqlScriptExecutorExecuteSqlScriptAsync.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Reflection;
using DisposableDatabases.Exceptions;
using Npgsql;

namespace DisposableDatabases.PostgreSql.Tests;

[TestFixture]
public class PostgreSqlSqlScriptExecutorExecuteSqlScriptAsync
{
	private readonly PostgreSqlSqlScriptExecutor _postgreSqlSqlScriptExecutor;

	public PostgreSqlSqlScriptExecutorExecuteSqlScriptAsync()
	{
		_postgreSqlSqlScriptExecutor = new PostgreSqlSqlScriptExecutor();
	}

	internal static async Task WriteTestSqlScriptToFileAsync(string filePath)
	{
		Assembly assembly = typeof(PostgreSqlSqlScriptExecutorExecuteSqlScriptAsync).Assembly;

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
			AsyncTestDelegate function = () => _postgreSqlSqlScriptExecutor.ExecuteSqlScriptAsync(null, "TestDatabase", temporarySqlFile.FilePath);

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
			AsyncTestDelegate function = () => _postgreSqlSqlScriptExecutor.ExecuteSqlScriptAsync(invalidConnectionString, "TestDatabase", temporarySqlFile.FilePath);

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
			AsyncTestDelegate function = () => _postgreSqlSqlScriptExecutor.ExecuteSqlScriptAsync("ValidConnectionString", null, temporarySqlFile.FilePath);

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
			AsyncTestDelegate function = () => _postgreSqlSqlScriptExecutor.ExecuteSqlScriptAsync("ValidConnectionString", invalidDatabaseName, temporarySqlFile.FilePath);

			// Assert
			await Assert.ThatAsync(function, Throws.TypeOf<ArgumentException>());
		}
	}

	[Test]
	public async Task ThrowsArgumentNullExceptionGivenNullSqlScriptFilePath()
	{
		// Act
		AsyncTestDelegate function = () => _postgreSqlSqlScriptExecutor.ExecuteSqlScriptAsync("ValidConnectionString", "TestDatabase", null);

		// Assert
		await Assert.ThatAsync(function, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptySqlScriptFilePath")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceSqlScriptFilePath")]
	public async Task ThrowsArgumentExceptionGivenEmptyOrWhitespaceSqlScriptFilePath(string invalidFilePath)
	{
		// Act
		AsyncTestDelegate function = () => _postgreSqlSqlScriptExecutor.ExecuteSqlScriptAsync("ValidConnectionString", "TestDatabase", invalidFilePath);

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
		AsyncTestDelegate function = () => _postgreSqlSqlScriptExecutor.ExecuteSqlScriptAsync("ValidConnectionString", "TestDatabase", invalidFilePath);

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
			AsyncTestDelegate function = () => _postgreSqlSqlScriptExecutor.ExecuteSqlScriptAsync("InvalidConnectionString", databaseName, temporarySqlFile.FilePath);

			// Assert
			await Assert.ThatAsync(function,
								   Throws.TypeOf<DisposableDatabasesException>()
										 .With.Message.EqualTo($"PostgreSQL: Failed to execute SQL script at path {temporarySqlFile.FilePath} against database {databaseName}."));
		}
	}

	[Test]
	public async Task ThrowsDisposableDatabaseExceptionGivenInvalidSqlScript()
	{
		// Arrange
		string postgreSqlConnectionString = ConfigurationHelper.GetRequiredValue("PostgreSqlConnectionString");
		string databaseName = "TestExecuteSqlScriptAsync-" + Guid.NewGuid();
		await PostgreSqlDatabaseUtilities.CreateDatabaseAsync(postgreSqlConnectionString, databaseName);

		using (var temporarySqlFile = new TemporaryFile(".sql"))
		{
			using (StreamWriter streamWriter = File.CreateText(temporarySqlFile.FilePath))
			{
				await streamWriter.WriteLineAsync("Invalid SQL Script");
			}

			// Act
			AsyncTestDelegate function = () => _postgreSqlSqlScriptExecutor.ExecuteSqlScriptAsync(postgreSqlConnectionString, databaseName, temporarySqlFile.FilePath);

			// Assert
			try
			{
				await Assert.ThatAsync(function,
				                       Throws.TypeOf<DisposableDatabasesException>()
				                             .With.Message.EqualTo($"PostgreSQL: Failed to execute SQL script at path {temporarySqlFile.FilePath} against database {databaseName}."));
			}
			finally
			{
				await PostgreSqlDatabaseUtilities.DropDatabaseAsync(postgreSqlConnectionString, databaseName);
			}
		}
	}

	[Test]
	public async Task ExecutesSqlScript()
	{
		// Arrange
		string expectedDatabaseName = "TestExecuteSqlScriptAsync-" + Guid.NewGuid();
		string postgreSqlConnectionString = ConfigurationHelper.GetRequiredValue("PostgreSqlConnectionString");

		using (var temporarySqlFile = new TemporaryFile(".sql"))
		{
			Task writeSqlScriptTask = WriteTestSqlScriptToFileAsync(temporarySqlFile.FilePath);
			Task createSqlDatabase = PostgreSqlDatabaseUtilities.CreateDatabaseAsync(postgreSqlConnectionString, expectedDatabaseName);
			await Task.WhenAll(writeSqlScriptTask, createSqlDatabase);

			string newConnectionString = new NpgsqlConnectionStringBuilder(postgreSqlConnectionString) { Database = expectedDatabaseName }.ConnectionString;

			// Act
			await _postgreSqlSqlScriptExecutor.ExecuteSqlScriptAsync(newConnectionString, expectedDatabaseName, temporarySqlFile.FilePath);

			// Assert
			await Assert.ThatAsync(SampleTestTableExists, Is.True);

			async Task<bool> SampleTestTableExists()
			{
				bool result = await PostgreSqlDatabaseUtilities.TableExistsAsync(newConnectionString, "sample_test_table");

				// Cleanup
				await PostgreSqlDatabaseUtilities.DropDatabaseAsync(postgreSqlConnectionString, expectedDatabaseName);

				return result;
			}
		}
	}
}
