// <copyright file="CreateDatabaseAsync.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Exceptions;
using DisposableDatabases.Interfaces;
using DisposableDatabases.Interfaces.DatabaseOperations;
using DisposableDatabases.Interfaces.Strategies;
using DisposableDatabases.Strategies.DatabaseCreation;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace DisposableDatabases.Tests.Strategies.DatabaseCreation.DatabaseCreationWithPostScriptExecutionStrategyTests;

[TestFixture]
public class CreateDatabaseAsync
{
	// TODO: Review below tests
	[Test]
	public async Task CreatesNewDatabaseAndExecuteScript()
	{
		// Arrange
		const string connectionString = "Server=myServerAddress;";
		const string expectedDatabaseName = "TestDatabase";
		const string expectedNewDatabaseConnectionString = "Server=myServerAddress;Database=TestDatabase;";
		CancellationToken cancellationToken = CancellationToken.None;

		IDatabaseCreatorDropperAndSqlScriptExecutor substituteDatabaseCreatorDropperAndSqlScriptExecutor = Substitute.For<IDatabaseCreatorDropperAndSqlScriptExecutor>();
		substituteDatabaseCreatorDropperAndSqlScriptExecutor.CreateDatabaseAsync(connectionString, expectedDatabaseName, cancellationToken).Returns(Task.FromResult(expectedNewDatabaseConnectionString));

		IDatabaseNamingStrategy substituteDatabaseNamingStrategy = Substitute.For<IDatabaseNamingStrategy>();
		substituteDatabaseNamingStrategy.GenerateDatabaseName().Returns(expectedDatabaseName);

		using (var temporarySqlFile = new TemporaryFile(".sql"))
		{
			var strategy = new DatabaseCreationWithPostScriptExecutionStrategy(connectionString,
			                                                                   substituteDatabaseCreatorDropperAndSqlScriptExecutor,
			                                                                   substituteDatabaseNamingStrategy,
			                                                                   temporarySqlFile.FilePath);

			// Act
			IDisposableDatabase result = await strategy.CreateDatabaseAsync(cancellationToken);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(result.ConnectionString, Is.EqualTo(expectedNewDatabaseConnectionString));
				Assert.That(result.DatabaseName, Is.EqualTo(expectedDatabaseName));
			});
			await substituteDatabaseCreatorDropperAndSqlScriptExecutor.Received(1).CreateDatabaseAsync(connectionString, expectedDatabaseName, cancellationToken);
			await substituteDatabaseCreatorDropperAndSqlScriptExecutor.Received(1)
			                                                          .ExecuteSqlScriptAsync(expectedNewDatabaseConnectionString, expectedDatabaseName, temporarySqlFile.FilePath, cancellationToken);
		}
	}

	[Test]
	public async Task DropsDatabaseGivenScriptExecutionFails()
	{
		// Arrange
		const string connectionString = "Server=myServerAddress;";
		const string expectedDatabaseName = "TestDatabase";
		CancellationToken cancellationToken = CancellationToken.None;

		IDatabaseCreatorDropperAndSqlScriptExecutor substituteDatabaseCreatorDropperAndSqlScriptExecutor = Substitute.For<IDatabaseCreatorDropperAndSqlScriptExecutor>();
		substituteDatabaseCreatorDropperAndSqlScriptExecutor.CreateDatabaseAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
															.Returns(Task.FromResult("new_connection_string"));
		substituteDatabaseCreatorDropperAndSqlScriptExecutor.ExecuteSqlScriptAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
															.ThrowsAsync(new DisposableDatabasesException("Execution failed"));

		IDatabaseNamingStrategy substituteDatabaseNamingStrategy = Substitute.For<IDatabaseNamingStrategy>();
		substituteDatabaseNamingStrategy.GenerateDatabaseName().Returns(expectedDatabaseName);

		using (var temporarySqlFile = new TemporaryFile(".sql"))
		{
			var strategy = new DatabaseCreationWithPostScriptExecutionStrategy(connectionString,
			                                                                   substituteDatabaseCreatorDropperAndSqlScriptExecutor,
			                                                                   substituteDatabaseNamingStrategy,
			                                                                   temporarySqlFile.FilePath);

			// Act
			Func<Task<IDisposableDatabase>> function = () => strategy.CreateDatabaseAsync(cancellationToken);

			// Assert
			await Assert.ThatAsync(function, Throws.TypeOf<DisposableDatabasesException>().With.Message.EqualTo("Execution failed"));
			await substituteDatabaseCreatorDropperAndSqlScriptExecutor.Received(1).DropDatabaseAsync(connectionString, expectedDatabaseName, cancellationToken);
		}
	}

	[Test]
	public async Task ThrowsAggregateExceptionGivenScriptExecutionFailsAndDropDatabaseFails()
	{
		// Arrange
		const string connectionString = "Server=myServerAddress;";
		const string expectedDatabaseName = "TestDatabase";
		CancellationToken cancellationToken = CancellationToken.None;

		IDatabaseCreatorDropperAndSqlScriptExecutor substituteDatabaseCreatorDropperAndSqlScriptExecutor = Substitute.For<IDatabaseCreatorDropperAndSqlScriptExecutor>();
		substituteDatabaseCreatorDropperAndSqlScriptExecutor.CreateDatabaseAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
															.Returns(Task.FromResult("new_connection_string"));
		substituteDatabaseCreatorDropperAndSqlScriptExecutor.ExecuteSqlScriptAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
															.ThrowsAsync(new DisposableDatabasesException("SQL Script execution failed"));
		substituteDatabaseCreatorDropperAndSqlScriptExecutor.DropDatabaseAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
															.ThrowsAsync(new DisposableDatabasesException("Drop database failed"));

		IDatabaseNamingStrategy substituteDatabaseNamingStrategy = Substitute.For<IDatabaseNamingStrategy>();
		substituteDatabaseNamingStrategy.GenerateDatabaseName().Returns(expectedDatabaseName);

		using (var temporarySqlFile = new TemporaryFile(".sql"))
		{
			var strategy = new DatabaseCreationWithPostScriptExecutionStrategy(connectionString,
			                                                                   substituteDatabaseCreatorDropperAndSqlScriptExecutor,
			                                                                   substituteDatabaseNamingStrategy,
			                                                                   temporarySqlFile.FilePath);

			// Act
			Func<Task<IDisposableDatabase>> function = () => strategy.CreateDatabaseAsync(cancellationToken);

			// Assert
			await Assert.ThatAsync(function, Throws.TypeOf<AggregateException>().With.Message.StartsWith("An error occurred while executing the SQL script and dropping the database."));
		}
	}
}
