// <copyright file="Constructor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Strategies.DatabaseNaming;

namespace DisposableDatabases.Tests.Strategies.DatabaseNaming.UserDefinedDatabaseNamingStrategyTests;

[TestFixture]
public class Constructor
{
	[Test]
	public void ThrowsArgumentNullExceptionGivenNullDatabaseName()
	{
		// Act
		Action action = () => _ = new UserDefinedDatabaseNamingStrategy(null);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptyDatabaseName")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceDatabaseName")]
	public void ThrowsArgumentExceptionGivenEmptyOrWhitespaceDatabaseName(string invalidDatabaseName)
	{
		// Act
		Action action = () => _ = new UserDefinedDatabaseNamingStrategy(invalidDatabaseName);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentException>());
	}
}
