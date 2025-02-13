// <copyright file="DisposeDatabaseAsync.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces;
using DisposableDatabases.Interfaces.DatabaseOperations;
using DisposableDatabases.Interfaces.Strategies;
using DisposableDatabases.Strategies.DatabaseCreation;
using NSubstitute;

namespace DisposableDatabases.Tests.Strategies.DatabaseCreation.DatabaseCreationWithPostScriptExecutionStrategyTests;

[TestFixture]
public class DisposeDatabaseAsync
{
	[Test]
	public async Task ThrowsArgumentNullExceptionGivenNullDisposableDatabase()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		IDatabaseCreatorDropperAndSqlScriptExecutor substituteDatabaseCreatorDropperAndSqlScriptExecutor = Substitute.For<IDatabaseCreatorDropperAndSqlScriptExecutor>();
		IDatabaseNamingStrategy substituteDatabaseNamingStrategy = Substitute.For<IDatabaseNamingStrategy>();
		DatabaseCreationWithPostScriptExecutionStrategy databaseCreationWithPostScriptExecutionStrategy;

		using (var temporarySqlFile = new TemporaryFile(".sql"))
		{
			databaseCreationWithPostScriptExecutionStrategy = new DatabaseCreationWithPostScriptExecutionStrategy(connectionString,
			                                                                                                      substituteDatabaseCreatorDropperAndSqlScriptExecutor,
			                                                                                                      substituteDatabaseNamingStrategy,
			                                                                                                      temporarySqlFile.FilePath);
		}

		// Act
		AsyncTestDelegate action = () => databaseCreationWithPostScriptExecutionStrategy.DisposeDatabaseAsync(null!);

		// Assert
		await Assert.ThatAsync(action, Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public async Task DelegatesToDatabaseCreatorAndDropper()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		IDatabaseCreatorDropperAndSqlScriptExecutor substituteDatabaseCreatorDropperAndSqlScriptExecutor = Substitute.For<IDatabaseCreatorDropperAndSqlScriptExecutor>();
		IDatabaseNamingStrategy substituteDatabaseNamingStrategy = Substitute.For<IDatabaseNamingStrategy>();
		IDisposableDatabase substituteDisposableDatabase = Substitute.For<IDisposableDatabase>();
		substituteDisposableDatabase.DatabaseName.Returns("ValidDatabaseName");
		DatabaseCreationWithPostScriptExecutionStrategy databaseCreationWithPostScriptExecutionStrategy;

		using (var temporarySqlFile = new TemporaryFile(".sql"))
		{
			databaseCreationWithPostScriptExecutionStrategy = new DatabaseCreationWithPostScriptExecutionStrategy(connectionString,
			                                                                                                      substituteDatabaseCreatorDropperAndSqlScriptExecutor,
			                                                                                                      substituteDatabaseNamingStrategy,
			                                                                                                      temporarySqlFile.FilePath);
		}

		// Act
		await databaseCreationWithPostScriptExecutionStrategy.DisposeDatabaseAsync(substituteDisposableDatabase);

		// Assert
		await substituteDatabaseCreatorDropperAndSqlScriptExecutor.Received(1).DropDatabaseAsync(connectionString, substituteDisposableDatabase.DatabaseName);
	}
}
