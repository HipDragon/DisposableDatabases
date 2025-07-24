// <copyright file="DisposeDatabaseAsync.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces;
using DisposableDatabases.Interfaces.DatabaseOperations;
using DisposableDatabases.Interfaces.Strategies;
using DisposableDatabases.Strategies.DatabaseCreation;
using NSubstitute;

namespace DisposableDatabases.Tests.Strategies.DatabaseCreation.DatabaseOnlyCreationStrategyTests;

[TestFixture]
public class DisposeDatabaseAsync
{
	[Test]
	public async Task ThrowsArgumentNullExceptionGivenNullDisposableDatabase()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		IDatabaseCreatorAndDropper substituteDatabaseCreatorAndDropper = Substitute.For<IDatabaseCreatorAndDropper>();
		INamingStrategy substituteNamingStrategy = Substitute.For<INamingStrategy>();
		var databaseOnlyCreationStrategy = new DatabaseOnlyCreationStrategy(connectionString, substituteDatabaseCreatorAndDropper, substituteNamingStrategy);

		// Act
		AsyncTestDelegate action = () => databaseOnlyCreationStrategy.DisposeDatabaseAsync(null!);

		// Assert
		await Assert.ThatAsync(action, Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public async Task DelegatesToDatabaseCreatorAndDropper()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		IDatabaseCreatorAndDropper substituteDatabaseCreatorAndDropper = Substitute.For<IDatabaseCreatorAndDropper>();
		INamingStrategy substituteNamingStrategy = Substitute.For<INamingStrategy>();
		IDisposableDatabase substituteDisposableDatabase = Substitute.For<IDisposableDatabase>();
		substituteDisposableDatabase.DatabaseName.Returns("ValidDatabaseName");
		var databaseOnlyCreationStrategy = new DatabaseOnlyCreationStrategy(connectionString, substituteDatabaseCreatorAndDropper, substituteNamingStrategy);

		// Act
		await databaseOnlyCreationStrategy.DisposeDatabaseAsync(substituteDisposableDatabase);

		// Assert
		await substituteDatabaseCreatorAndDropper.Received(1).DropDatabaseAsync(connectionString, substituteDisposableDatabase.DatabaseName);
	}
}
