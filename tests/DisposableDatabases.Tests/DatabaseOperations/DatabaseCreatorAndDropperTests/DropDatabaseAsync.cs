// <copyright file="DropDatabaseAsync.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces.DatabaseOperations;
using NSubstitute;

namespace DisposableDatabases.Tests.DatabaseOperations.DatabaseCreatorAndDropperTests;

[TestFixture]
public class DropDatabaseAsync
{
	[Test]
	public async Task DelegatesToDatabaseDropperDropDatabaseAsync()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		const string databaseName = "ValidDatabaseName";
		IDatabaseCreator databaseCreator = Substitute.For<IDatabaseCreator>();
		IDatabaseDropper databaseDropper = Substitute.For<IDatabaseDropper>();
		var databaseCreatorAndDropper = new TestDatabaseCreatorAndDropper(databaseCreator, databaseDropper);

		// Act
		await databaseCreatorAndDropper.DropDatabaseAsync(connectionString, databaseName);

		// Assert
		await databaseDropper.Received(1).DropDatabaseAsync(connectionString, databaseName);
	}

	[Test]
	public async Task DelegatesToDatabaseDropperDropDatabaseAsyncWithCancellationToken()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		const string databaseName = "ValidDatabaseName";
		CancellationToken cancellationToken = CancellationToken.None;
		IDatabaseCreator databaseCreator = Substitute.For<IDatabaseCreator>();
		IDatabaseDropper databaseDropper = Substitute.For<IDatabaseDropper>();
		var databaseCreatorAndDropper = new TestDatabaseCreatorAndDropper(databaseCreator, databaseDropper);

		// Act
		await databaseCreatorAndDropper.DropDatabaseAsync(connectionString, databaseName, cancellationToken);

		// Assert
		await databaseDropper.Received(1).DropDatabaseAsync(connectionString, databaseName, cancellationToken);
	}
}
