// <copyright file="Constructor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Strategies.Naming;

namespace DisposableDatabases.Tests.Strategies.Naming.ConstantNamingStrategyTests;

[TestFixture]
public class Constructor
{
	[Test]
	public void ThrowsArgumentNullExceptionGivenNullName()
	{
		// Act
		Action action = () => _ = new ConstantNamingStrategy(null);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptyName")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceName")]
	public void ThrowsArgumentExceptionGivenEmptyOrWhitespaceName(string invalidName)
	{
		// Act
		Action action = () => _ = new ConstantNamingStrategy(invalidName);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentException>());
	}
}
