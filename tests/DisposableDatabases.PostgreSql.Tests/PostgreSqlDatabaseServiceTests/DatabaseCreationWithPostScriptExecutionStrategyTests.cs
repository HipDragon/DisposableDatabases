// <copyright file="DatabaseCreationWithPostScriptExecutionStrategyTests.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces;
using DisposableDatabases.Interfaces.Strategies;
using DisposableDatabases.Strategies.DatabaseCreation;
using DisposableDatabases.Strategies.DatabaseNaming;
using DisposableDatabases.Strategies.DatabaseNaming.Decorators;

namespace DisposableDatabases.PostgreSql.Tests.PostgreSqlDatabaseServiceTests;

public class DatabaseCreationWithPostScriptExecutionStrategyTests
{
	[Test]
	public async Task ReturnsDisposableDatabase()
	{
		// Arrange
		string connectionString = ConfigurationHelper.GetRequiredValue("PostgreSqlConnectionString");
		const string defaultPrefix = "test_";
		IDatabaseNamingStrategy databaseNamingStrategy = new DatabaseNamingStrategyPrefixDecorator(new GuidDatabaseNamingStrategy(), defaultPrefix);

		using (var temporarySqlFile = new TemporaryFile(".sql"))
		{
			await PostgreSqlSqlScriptExecutorExecuteSqlScriptAsync.WriteTestSqlScriptToFileAsync(temporarySqlFile.FilePath);
			var disposableDatabaseCreationStrategy = new DatabaseCreationWithPostScriptExecutionStrategy(connectionString, new PostgreSqlDatabaseService(), databaseNamingStrategy, temporarySqlFile.FilePath);

			// Act
			IDisposableDatabase disposableDatabase = await disposableDatabaseCreationStrategy.CreateDatabaseAsync();

			// Assert
			Assert.That(disposableDatabase, Is.Not.Null);
			await Assert.MultipleAsync(async () =>
			{
				Assert.That(disposableDatabase.DatabaseName, Is.Not.Empty);
				string databaseName = disposableDatabase.DatabaseName;
				Assert.That(disposableDatabase.ConnectionString, Is.Not.Empty);
				await Assert.ThatAsync(() => PostgreSqlDatabaseUtilities.TableExistsAsync(disposableDatabase.ConnectionString, "sample_test_table"), Is.True);
				await disposableDatabase.DisposeAsync();
				await Assert.ThatAsync(() => PostgreSqlDatabaseUtilities.DatabaseExistsAsync(connectionString, databaseName), Is.False);
			});
		}
	}
}
