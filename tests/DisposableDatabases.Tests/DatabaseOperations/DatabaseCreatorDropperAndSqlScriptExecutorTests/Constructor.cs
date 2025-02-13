// <copyright file="Constructor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces.DatabaseOperations;
using NSubstitute;

namespace DisposableDatabases.Tests.DatabaseOperations.DatabaseCreatorDropperAndSqlScriptExecutorTests;

[TestFixture]
public class Constructor
{
	[Test]
	public void ThrowsArgumentNullExceptionGivenNullDatabaseCreator()
	{
		// Arrange
		IDatabaseDropper databaseDropper = Substitute.For<IDatabaseDropper>();
		ISqlScriptExecutor sqlScriptExecutor = Substitute.For<ISqlScriptExecutor>();

		// Act
		Action action = () => _ = new TestDatabaseCreatorDropperAndSqlScriptExecutor(null, databaseDropper, sqlScriptExecutor);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenNullDatabaseDropper()
	{
		// Arrange
		IDatabaseCreator databaseCreator = Substitute.For<IDatabaseCreator>();
		ISqlScriptExecutor sqlScriptExecutor = Substitute.For<ISqlScriptExecutor>();

		// Act
		Action action = () => _ = new TestDatabaseCreatorDropperAndSqlScriptExecutor(databaseCreator, null, sqlScriptExecutor);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenNullSqlScriptExecutor()
	{
		// Arrange
		IDatabaseCreator databaseCreator = Substitute.For<IDatabaseCreator>();
		IDatabaseDropper databaseDropper = Substitute.For<IDatabaseDropper>();

		// Act
		Action action = () => _ = new TestDatabaseCreatorDropperAndSqlScriptExecutor(databaseCreator, databaseDropper, null);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}
}
