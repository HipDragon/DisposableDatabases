// <copyright file="GenerateName.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Strategies.Naming;

namespace DisposableDatabases.Tests.Strategies.Naming.ConstantNamingStrategyTests;

[TestFixture]
public class GenerateName
{
	[Test]
	public void ReturnsName()
	{
		// Arrange
		const string expectedName = "ConstantTest";
		var namingStrategy = new ConstantNamingStrategy(expectedName);

		// Act
		string actualDatabaseName = namingStrategy.GenerateName();

		// Assert
		Assert.That(actualDatabaseName, Is.EqualTo(expectedName));
	}
}
