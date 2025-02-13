// <copyright file="CreateDatabaseAsync.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces.DatabaseOperations;
using NSubstitute;

namespace DisposableDatabases.Tests.DatabaseOperations.DatabaseCreatorDropperAndSqlScriptExecutorTests;

[TestFixture]
public class CreateDatabaseAsync
{
	[Test]
	public async Task DelegatesToDatabaseCreatorCreateDatabaseAsync()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		const string databaseName = "ValidDatabaseName";
		IDatabaseCreator databaseCreator = Substitute.For<IDatabaseCreator>();
		IDatabaseDropper databaseDropper = Substitute.For<IDatabaseDropper>();
		ISqlScriptExecutor sqlScriptExecutor = Substitute.For<ISqlScriptExecutor>();
		var databaseCreatorDropperAndSqlScriptExecutor = new TestDatabaseCreatorDropperAndSqlScriptExecutor(databaseCreator, databaseDropper, sqlScriptExecutor);

		// Act
		await databaseCreatorDropperAndSqlScriptExecutor.CreateDatabaseAsync(connectionString, databaseName);

		// Assert
		await databaseCreator.Received(1).CreateDatabaseAsync(connectionString, databaseName);
	}

	[Test]
	public async Task DelegatesToDatabaseCreatorCreateDatabaseAsyncWithCancellationToken()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		const string databaseName = "ValidDatabaseName";
		CancellationToken cancellationToken = CancellationToken.None;
		IDatabaseCreator databaseCreator = Substitute.For<IDatabaseCreator>();
		IDatabaseDropper databaseDropper = Substitute.For<IDatabaseDropper>();
		ISqlScriptExecutor sqlScriptExecutor = Substitute.For<ISqlScriptExecutor>();
		var databaseCreatorDropperAndSqlScriptExecutor = new TestDatabaseCreatorDropperAndSqlScriptExecutor(databaseCreator, databaseDropper, sqlScriptExecutor);

		// Act
		await databaseCreatorDropperAndSqlScriptExecutor.CreateDatabaseAsync(connectionString, databaseName, cancellationToken);

		// Assert
		await databaseCreator.Received(1).CreateDatabaseAsync(connectionString, databaseName, cancellationToken);
	}
}
