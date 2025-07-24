// <copyright file="Constructor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces.Strategies;
using DisposableDatabases.Strategies.Naming.Decorators;
using NSubstitute;

namespace DisposableDatabases.Tests.Strategies.Naming.Decorators.PrefixNamingStrategyTests;

[TestFixture]
public class Constructor
{
	[Test]
	public void ThrowsArgumentNullExceptionGivenNullDatabaseNamingStrategy()
	{
		// Arrange
		const string prefix = "Prefix-";

		// Act
		Action action = () => _ = new PrefixNamingStrategy(null!, prefix);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenNullPrefix()
	{
		// Arrange
		INamingStrategy substituteNamingStrategy = Substitute.For<INamingStrategy>();

		// Act
		Action action = () => _ = new PrefixNamingStrategy(substituteNamingStrategy, null!);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptyPrefix")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespacePrefix")]
	public void ThrowsArgumentExceptionGivenEmptyOrWhitespacePrefix(string invalidPrefix)
	{
		// Arrange
		INamingStrategy substituteNamingStrategy = Substitute.For<INamingStrategy>();

		// Act
		Action action = () => _ = new PrefixNamingStrategy(substituteNamingStrategy, invalidPrefix);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentException>());
	}
}
