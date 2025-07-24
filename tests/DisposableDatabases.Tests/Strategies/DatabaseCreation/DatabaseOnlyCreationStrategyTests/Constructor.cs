// <copyright file="Constructor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Reflection;
using DisposableDatabases.Interfaces.DatabaseOperations;
using DisposableDatabases.Interfaces.Strategies;
using DisposableDatabases.Strategies.DatabaseCreation;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace DisposableDatabases.Tests.Strategies.DatabaseCreation.DatabaseOnlyCreationStrategyTests;

[TestFixture]
public class Constructor
{
	[Test]
	public void ThrowsArgumentNullExceptionGivenNullConnectionString()
	{
		// Arrange
		IDatabaseCreatorAndDropper substituteDatabaseCreatorAndDropper = Substitute.For<IDatabaseCreatorAndDropper>();
		INamingStrategy substituteNamingStrategy = Substitute.For<INamingStrategy>();

		// Act
		Action action = () => _ = new DatabaseOnlyCreationStrategy(null, substituteDatabaseCreatorAndDropper, substituteNamingStrategy);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptyConnectionString")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceConnectionString")]
	public void ThrowsArgumentNullExceptionGivenEmptyOrWhitespaceConnectionString(string invalidConnectionString)
	{
		// Arrange
		IDatabaseCreatorAndDropper substituteDatabaseCreatorAndDropper = Substitute.For<IDatabaseCreatorAndDropper>();
		INamingStrategy substituteNamingStrategy = Substitute.For<INamingStrategy>();

		// Act
		Action action = () => _ = new DatabaseOnlyCreationStrategy(invalidConnectionString, substituteDatabaseCreatorAndDropper, substituteNamingStrategy);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentException>());
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenNullDatabaseCreatorAndDropper()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		INamingStrategy substituteNamingStrategy = Substitute.For<INamingStrategy>();

		// Act
		Action action = () => _ = new DatabaseOnlyCreationStrategy(connectionString, null, substituteNamingStrategy);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenNullDatabaseNamingStrategy()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		IDatabaseCreatorAndDropper substituteDatabaseCreatorAndDropper = Substitute.For<IDatabaseCreatorAndDropper>();

		// Act
		Action action = () => _ = new DatabaseOnlyCreationStrategy(connectionString, substituteDatabaseCreatorAndDropper, null);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenNullILoggerFactory()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		IDatabaseCreatorAndDropper substituteDatabaseCreatorAndDropper = Substitute.For<IDatabaseCreatorAndDropper>();
		INamingStrategy substituteNamingStrategy = Substitute.For<INamingStrategy>();

		// Act
		Action action = () => _ = new DatabaseOnlyCreationStrategy(connectionString, substituteDatabaseCreatorAndDropper, substituteNamingStrategy, null);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public void ProvidesDefaultNullLoggerFactoryGivenNoILoggerFactory()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		IDatabaseCreatorAndDropper substituteDatabaseCreatorAndDropper = Substitute.For<IDatabaseCreatorAndDropper>();
		INamingStrategy substituteNamingStrategy = Substitute.For<INamingStrategy>();
		NullLoggerFactory expectedLogger = NullLoggerFactory.Instance;

		// Act
		var databaseOnlyCreationStrategy = new DatabaseOnlyCreationStrategy(connectionString, substituteDatabaseCreatorAndDropper, substituteNamingStrategy);

		// Assert
		object? actualLogger = typeof(DatabaseOnlyCreationStrategy).GetField("_loggerFactory", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(databaseOnlyCreationStrategy);
		Assert.That(actualLogger, Is.SameAs(expectedLogger));
	}
}
