// <copyright file="CreateDatabaseAsync.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces;
using DisposableDatabases.Interfaces.DatabaseOperations;
using DisposableDatabases.Interfaces.Strategies;
using DisposableDatabases.Strategies.DatabaseCreation;
using NSubstitute;

namespace DisposableDatabases.Tests.Strategies.DatabaseCreation.DatabaseOnlyCreationStrategyTests;

[TestFixture]
public class CreateDatabaseAsync
{
	[Test]
	public async Task DelegatesCreateDatabaseToDatabaseCreatorAndDropperCreateDatabaseAsync()
	{
		// Arrange
		const string connectionString = "Server=myServerAddress;";
		const string expectedDatabaseName = "TestDatabase";
		const string expectedNewDatabaseConnectionString = "Server=myServerAddress;Database=TestDatabase;";

		IDatabaseCreatorAndDropper substituteDatabaseCreatorAndDropper = Substitute.For<IDatabaseCreatorAndDropper>();
		substituteDatabaseCreatorAndDropper.CreateDatabaseAsync(connectionString, expectedDatabaseName, Arg.Any<CancellationToken>()).Returns(Task.FromResult(expectedNewDatabaseConnectionString));

		INamingStrategy substituteNamingStrategy = Substitute.For<INamingStrategy>();
		substituteNamingStrategy.GenerateName().Returns(expectedDatabaseName);

		var createDatabaseOnlyStrategy = new DatabaseOnlyCreationStrategy(connectionString, substituteDatabaseCreatorAndDropper, substituteNamingStrategy);

		// Act
		IDisposableDatabase disposableDatabase = await createDatabaseOnlyStrategy.CreateDatabaseAsync();

		// Assert
		await substituteDatabaseCreatorAndDropper.Received(1).CreateDatabaseAsync(connectionString, expectedDatabaseName, Arg.Any<CancellationToken>());
		Assert.That(disposableDatabase, Is.Not.Null);
		using (Assert.EnterMultipleScope())
		{
			Assert.That(disposableDatabase.ConnectionString, Is.EqualTo(expectedNewDatabaseConnectionString));
			Assert.That(disposableDatabase.DatabaseName, Is.EqualTo(expectedDatabaseName));
		}
	}
}
