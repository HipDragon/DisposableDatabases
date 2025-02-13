// <copyright file="Constructor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces.DatabaseOperations;
using NSubstitute;

namespace DisposableDatabases.Tests.DatabaseOperations.DatabaseCreatorAndDropperTests;

[TestFixture]
public class Constructor
{
	[Test]
	public void ThrowsArgumentNullExceptionGivenNullDatabaseCreator()
	{
		// Arrange
		IDatabaseDropper databaseDropper = Substitute.For<IDatabaseDropper>();

		// Act
		Action action = () => _ = new TestDatabaseCreatorAndDropper(null, databaseDropper);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenNullDatabaseDropper()
	{
		// Arrange
		IDatabaseCreator databaseCreator = Substitute.For<IDatabaseCreator>();

		// Act
		Action action = () => _ = new TestDatabaseCreatorAndDropper(databaseCreator, null);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}
}
