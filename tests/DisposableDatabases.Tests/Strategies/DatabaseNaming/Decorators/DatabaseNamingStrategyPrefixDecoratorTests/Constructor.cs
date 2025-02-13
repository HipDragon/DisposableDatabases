// <copyright file="Constructor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces.Strategies;
using DisposableDatabases.Strategies.DatabaseNaming.Decorators;
using NSubstitute;

namespace DisposableDatabases.Tests.Strategies.DatabaseNaming.Decorators.DatabaseNamingStrategyPrefixDecoratorTests;

[TestFixture]
public class Constructor
{
	[Test]
	public void ThrowsArgumentNullExceptionGivenNullDatabaseNamingStrategy()
	{
		// Arrange
		const string prefix = "Prefix-";

		// Act
		Action action = () => _ = new DatabaseNamingStrategyPrefixDecorator(null, prefix);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenNullPrefix()
	{
		// Arrange
		IDatabaseNamingStrategy substituteDatabaseNamingStrategy = Substitute.For<IDatabaseNamingStrategy>();

		// Act
		Action action = () => _ = new DatabaseNamingStrategyPrefixDecorator(substituteDatabaseNamingStrategy, null);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptyPrefix")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespacePrefix")]
	public void ThrowsArgumentExceptionGivenEmptyOrWhitespacePrefix(string invalidPrefix)
	{
		// Arrange
		IDatabaseNamingStrategy substituteDatabaseNamingStrategy = Substitute.For<IDatabaseNamingStrategy>();

		// Act
		Action action = () => _ = new DatabaseNamingStrategyPrefixDecorator(substituteDatabaseNamingStrategy, invalidPrefix);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentException>());
	}
}
