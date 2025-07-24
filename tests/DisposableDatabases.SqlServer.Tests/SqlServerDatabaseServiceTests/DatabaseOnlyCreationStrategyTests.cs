// <copyright file="DatabaseOnlyCreationStrategyTests.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces;
using DisposableDatabases.Interfaces.Strategies;
using DisposableDatabases.Strategies.DatabaseCreation;
using DisposableDatabases.Strategies.Naming;
using DisposableDatabases.Strategies.Naming.Decorators;

namespace DisposableDatabases.SqlServer.Tests.SqlServerDatabaseServiceTests;

[TestFixture]
public class DatabaseOnlyCreationStrategyTests
{
	[Test]
	public async Task ReturnsDisposableDatabase()
	{
		// Arrange
		string connectionString = ConfigurationHelper.GetRequiredValue("SqlServerConnectionString");
		const string defaultPrefix = "DatabaseOnlyCreationStrategyTest-";
		INamingStrategy namingStrategy = new PrefixNamingStrategy(new GuidNamingStrategy(), defaultPrefix);
		var disposableDatabaseCreationStrategy = new DatabaseOnlyCreationStrategy(connectionString, new SqlServerDatabaseService(), namingStrategy);

		// Act
		IDisposableDatabase disposableDatabase = await disposableDatabaseCreationStrategy.CreateDatabaseAsync();

		// Assert
		Assert.That(disposableDatabase, Is.Not.Null);
		using (Assert.EnterMultipleScope())
		{
			Assert.That(disposableDatabase.DatabaseName, Is.Not.Empty);
			string databaseName = disposableDatabase.DatabaseName;
			Assert.That(disposableDatabase.ConnectionString, Is.Not.Empty);
			await Assert.ThatAsync(() => SqlServerDatabaseUtilities.TestDatabaseConnectionAsync(disposableDatabase.ConnectionString), Is.True);
			await disposableDatabase.DisposeAsync();
			await Assert.ThatAsync(() => SqlServerDatabaseUtilities.DatabaseExistsAsync(connectionString, databaseName), Is.False);
		}
	}
}
