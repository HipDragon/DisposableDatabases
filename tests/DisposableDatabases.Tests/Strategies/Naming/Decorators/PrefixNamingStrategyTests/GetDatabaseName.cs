// <copyright file="GenerateName.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces.Strategies;
using DisposableDatabases.Strategies.Naming.Decorators;
using NSubstitute;

namespace DisposableDatabases.Tests.Strategies.Naming.Decorators.PrefixNamingStrategyTests;

[TestFixture]
public class GetDatabaseName
{
	[Test]
	public void PrependsPrefixToDatabaseNamingStrategyGeneratedDatabaseName()
	{
		// Arrange
		const string prefix = "Prefix-";
		const string baseName = "BaseName";
		INamingStrategy substituteNamingStrategy = Substitute.For<INamingStrategy>();
		substituteNamingStrategy.GenerateName().Returns(baseName);
		var databaseNamingStrategyPrefixDecorator = new PrefixNamingStrategy(substituteNamingStrategy, prefix);

		// Act
		string result = databaseNamingStrategyPrefixDecorator.GenerateName();

		// Assert
		Assert.That(result, Is.EqualTo($"{prefix}{baseName}"));
	}
}
