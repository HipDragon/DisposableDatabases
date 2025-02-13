// <copyright file="Constructor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces.Strategies;
using DisposableDatabases.Strategies.DatabaseNaming.Decorators;
using NSubstitute;

namespace DisposableDatabases.Tests.Strategies.DatabaseNaming.Decorators.DatabaseNamingStrategySuffixDecoratorTests;

[TestFixture]
public class Constructor
{
	[Test]
	public void ThrowsArgumentNullExceptionGivenNullDatabaseNamingStrategy()
	{
		// Arrange
		const string suffix = "-suffix";

		// Act
		Action action = () => _ = new DatabaseNamingStrategySuffixDecorator(null, suffix);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenNullSuffix()
	{
		// Arrange
		IDatabaseNamingStrategy substituteDatabaseNamingStrategy = Substitute.For<IDatabaseNamingStrategy>();

		// Act
		Action action = () => _ = new DatabaseNamingStrategySuffixDecorator(substituteDatabaseNamingStrategy, null);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptySuffix")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceSuffix")]
	public void ThrowsArgumentExceptionGivenEmptyOrWhitespaceSuffix(string invalidSuffix)
	{
		// Arrange
		IDatabaseNamingStrategy substituteDatabaseNamingStrategy = Substitute.For<IDatabaseNamingStrategy>();

		// Act
		Action action = () => _ = new DatabaseNamingStrategySuffixDecorator(substituteDatabaseNamingStrategy, invalidSuffix);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentException>());
	}
}
