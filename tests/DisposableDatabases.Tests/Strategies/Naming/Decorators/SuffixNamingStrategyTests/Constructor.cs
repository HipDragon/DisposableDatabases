// <copyright file="Constructor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces.Strategies;
using DisposableDatabases.Strategies.Naming.Decorators;
using NSubstitute;

namespace DisposableDatabases.Tests.Strategies.Naming.Decorators.SuffixNamingStrategyTests;

[TestFixture]
public class Constructor
{
	[Test]
	public void ThrowsArgumentNullExceptionGivenNullDatabaseNamingStrategy()
	{
		// Arrange
		const string suffix = "-suffix";

		// Act
		Action action = () => _ = new SuffixNamingStrategy(null!, suffix);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenNullSuffix()
	{
		// Arrange
		INamingStrategy substituteNamingStrategy = Substitute.For<INamingStrategy>();

		// Act
		Action action = () => _ = new SuffixNamingStrategy(substituteNamingStrategy, null!);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptySuffix")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceSuffix")]
	public void ThrowsArgumentExceptionGivenEmptyOrWhitespaceSuffix(string invalidSuffix)
	{
		// Arrange
		INamingStrategy substituteNamingStrategy = Substitute.For<INamingStrategy>();

		// Act
		Action action = () => _ = new SuffixNamingStrategy(substituteNamingStrategy, invalidSuffix);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentException>());
	}
}
