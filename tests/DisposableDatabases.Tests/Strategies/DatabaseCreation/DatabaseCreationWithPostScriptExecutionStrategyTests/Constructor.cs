// <copyright file="Constructor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Reflection;
using DisposableDatabases.Interfaces.DatabaseOperations;
using DisposableDatabases.Interfaces.Strategies;
using DisposableDatabases.Strategies.DatabaseCreation;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace DisposableDatabases.Tests.Strategies.DatabaseCreation.DatabaseCreationWithPostScriptExecutionStrategyTests;

[TestFixture]
public class Constructor
{
	[Test]
	public void ThrowsArgumentNullExceptionGivenNullConnectionString()
	{
		// Arrange
		IDatabaseCreatorDropperAndSqlScriptExecutor substituteDatabaseCreatorDropperAndSqlScriptExecutor = Substitute.For<IDatabaseCreatorDropperAndSqlScriptExecutor>();
		IDatabaseNamingStrategy substituteDatabaseNamingStrategy = Substitute.For<IDatabaseNamingStrategy>();

		// Act
		Action action = () => _ = new DatabaseCreationWithPostScriptExecutionStrategy(null, substituteDatabaseCreatorDropperAndSqlScriptExecutor, substituteDatabaseNamingStrategy, "ValidSqlScriptFilePath");

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptyConnectionString")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceConnectionString")]
	public void ThrowsArgumentNullExceptionGivenEmptyOrWhitespaceConnectionString(string invalidConnectionString)
	{
		// Arrange
		IDatabaseCreatorDropperAndSqlScriptExecutor substituteDatabaseCreatorDropperAndSqlScriptExecutor = Substitute.For<IDatabaseCreatorDropperAndSqlScriptExecutor>();
		IDatabaseNamingStrategy substituteDatabaseNamingStrategy = Substitute.For<IDatabaseNamingStrategy>();

		// Act
		Action action = () => _ = new DatabaseCreationWithPostScriptExecutionStrategy(invalidConnectionString,
		                                                                              substituteDatabaseCreatorDropperAndSqlScriptExecutor,
		                                                                              substituteDatabaseNamingStrategy,
		                                                                              "ValidSqlScriptFilePath");

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentException>());
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenNullDatabaseCreatorDropperAndSqlScriptExecutor()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		IDatabaseNamingStrategy substituteDatabaseNamingStrategy = Substitute.For<IDatabaseNamingStrategy>();

		// Act
		Action action = () => _ = new DatabaseCreationWithPostScriptExecutionStrategy(connectionString, null, substituteDatabaseNamingStrategy, "ValidSqlScriptFilePath");

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenNullSqlScriptFilePath()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		IDatabaseCreatorDropperAndSqlScriptExecutor substituteDatabaseCreatorDropperAndSqlScriptExecutor = Substitute.For<IDatabaseCreatorDropperAndSqlScriptExecutor>();
		IDatabaseNamingStrategy substituteDatabaseNamingStrategy = Substitute.For<IDatabaseNamingStrategy>();

		// Act
		Action action = () => _ = new DatabaseCreationWithPostScriptExecutionStrategy(connectionString, substituteDatabaseCreatorDropperAndSqlScriptExecutor, substituteDatabaseNamingStrategy, null);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptySqlScriptFilePath")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceSqlScriptFilePath")]
	public void ThrowsArgumentExceptionGivenEmptyOrWhitespaceSqlScriptFilePath(string invalidSqlScriptFilePath)
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		IDatabaseCreatorDropperAndSqlScriptExecutor substituteDatabaseCreatorDropperAndSqlScriptExecutor = Substitute.For<IDatabaseCreatorDropperAndSqlScriptExecutor>();
		IDatabaseNamingStrategy substituteDatabaseNamingStrategy = Substitute.For<IDatabaseNamingStrategy>();

		// Act
		Action action = () => _ = new DatabaseCreationWithPostScriptExecutionStrategy(connectionString,
																					  substituteDatabaseCreatorDropperAndSqlScriptExecutor,
																					  substituteDatabaseNamingStrategy,
																					  invalidSqlScriptFilePath);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentException>());
	}

	[Test]
	public void ThrowsFileNotFoundExceptionGivenNonExistentSqlScriptFilePath()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		IDatabaseCreatorDropperAndSqlScriptExecutor substituteDatabaseCreatorDropperAndSqlScriptExecutor = Substitute.For<IDatabaseCreatorDropperAndSqlScriptExecutor>();
		IDatabaseNamingStrategy substituteDatabaseNamingStrategy = Substitute.For<IDatabaseNamingStrategy>();
		var temporarySqlFile = new TemporaryFile(".sql");
		string invalidFilePath = temporarySqlFile.FilePath;
		temporarySqlFile.Dispose();

		// Act
		Action action = () => _ = new DatabaseCreationWithPostScriptExecutionStrategy(connectionString,
		                                                                              substituteDatabaseCreatorDropperAndSqlScriptExecutor,
		                                                                              substituteDatabaseNamingStrategy,
		                                                                              invalidFilePath);

		// Assert
		Assert.That(action, Throws.TypeOf<FileNotFoundException>().With.Message.EqualTo($"SQL Script file does not exist at path {invalidFilePath}."));
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenNullILoggerFactory()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		IDatabaseCreatorDropperAndSqlScriptExecutor substituteDatabaseCreatorDropperAndSqlScriptExecutor = Substitute.For<IDatabaseCreatorDropperAndSqlScriptExecutor>();
		IDatabaseNamingStrategy substituteDatabaseNamingStrategy = Substitute.For<IDatabaseNamingStrategy>();

		// Act
		Action action = () => _ = new DatabaseCreationWithPostScriptExecutionStrategy(connectionString,
		                                                                              substituteDatabaseCreatorDropperAndSqlScriptExecutor,
		                                                                              substituteDatabaseNamingStrategy,
		                                                                              "ValidSqlScriptFilePath",
		                                                                              null);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public void ProvidesDefaultNullLoggerFactoryGivenNoILoggerFactory()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		IDatabaseCreatorDropperAndSqlScriptExecutor substituteDatabaseCreatorDropperAndSqlScriptExecutor = Substitute.For<IDatabaseCreatorDropperAndSqlScriptExecutor>();
		IDatabaseNamingStrategy substituteDatabaseNamingStrategy = Substitute.For<IDatabaseNamingStrategy>();
		DatabaseCreationWithPostScriptExecutionStrategy databaseCreationWithPostScriptExecutionStrategy;
		NullLoggerFactory expectedLogger = NullLoggerFactory.Instance;

		using (var temporarySqlFile = new TemporaryFile(".sql"))
		{
			// Act
			databaseCreationWithPostScriptExecutionStrategy = new DatabaseCreationWithPostScriptExecutionStrategy(connectionString,
			                                                                                                      substituteDatabaseCreatorDropperAndSqlScriptExecutor,
			                                                                                                      substituteDatabaseNamingStrategy,
			                                                                                                      temporarySqlFile.FilePath);
		}

		// Assert
		object? actualLogger = typeof(DatabaseCreationWithPostScriptExecutionStrategy).GetField("_loggerFactory", BindingFlags.NonPublic | BindingFlags.Instance)
		                                                                              ?.GetValue(databaseCreationWithPostScriptExecutionStrategy);
		Assert.That(actualLogger, Is.SameAs(expectedLogger));
	}
}
