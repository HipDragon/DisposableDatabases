// <copyright file="ExecuteSqlScriptAsync.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces.DatabaseOperations;
using NSubstitute;

namespace DisposableDatabases.Tests.DatabaseOperations.DatabaseCreatorDropperAndSqlScriptExecutorTests;

[TestFixture]
public class ExecuteSqlScriptAsync
{
	[Test]
	public async Task DelegatesToDatabaseDropperDropDatabaseAsync()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		const string databaseName = "ValidDatabaseName";
		const string sqlScriptFilePath = "ValidSqlScriptFilePath";
		IDatabaseCreator databaseCreator = Substitute.For<IDatabaseCreator>();
		IDatabaseDropper databaseDropper = Substitute.For<IDatabaseDropper>();
		ISqlScriptExecutor sqlScriptExecutor = Substitute.For<ISqlScriptExecutor>();
		var databaseCreatorDropperAndSqlScriptExecutor = new TestDatabaseCreatorDropperAndSqlScriptExecutor(databaseCreator, databaseDropper, sqlScriptExecutor);

		// Act
		await databaseCreatorDropperAndSqlScriptExecutor.ExecuteSqlScriptAsync(connectionString, databaseName, sqlScriptFilePath);

		// Assert
		await sqlScriptExecutor.Received(1).ExecuteSqlScriptAsync(connectionString, databaseName, sqlScriptFilePath);
	}

	[Test]
	public async Task DelegatesToDatabaseDropperDropDatabaseAsyncWithCancellationToken()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		const string databaseName = "ValidDatabaseName";
		const string sqlScriptFilePath = "ValidSqlScriptFilePath";
		CancellationToken cancellationToken = CancellationToken.None;
		IDatabaseCreator databaseCreator = Substitute.For<IDatabaseCreator>();
		IDatabaseDropper databaseDropper = Substitute.For<IDatabaseDropper>();
		ISqlScriptExecutor sqlScriptExecutor = Substitute.For<ISqlScriptExecutor>();
		var databaseCreatorDropperAndSqlScriptExecutor = new TestDatabaseCreatorDropperAndSqlScriptExecutor(databaseCreator, databaseDropper, sqlScriptExecutor);

		// Act
		await databaseCreatorDropperAndSqlScriptExecutor.ExecuteSqlScriptAsync(connectionString, databaseName, sqlScriptFilePath, cancellationToken);

		// Assert
		await sqlScriptExecutor.Received(1).ExecuteSqlScriptAsync(connectionString, databaseName, sqlScriptFilePath, cancellationToken);
	}
}
