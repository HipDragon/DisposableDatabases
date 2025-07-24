// <copyright file="GenerateName.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces.Strategies;
using DisposableDatabases.Strategies.Naming.Decorators;
using NSubstitute;

namespace DisposableDatabases.Tests.Strategies.Naming.Decorators.SuffixNamingStrategyTests;

[TestFixture]
public class GetDatabaseName
{
	[Test]
	public void PrependsPrefixToDatabaseNamingStrategyGeneratedDatabaseName()
	{
		// Arrange
		const string suffix = "-suffix";
		const string baseName = "BaseName";
		INamingStrategy substituteNamingStrategy = Substitute.For<INamingStrategy>();
		substituteNamingStrategy.GenerateName().Returns(baseName);
		var databaseNamingStrategySuffixDecorator = new SuffixNamingStrategy(substituteNamingStrategy, suffix);

		// Act
		string result = databaseNamingStrategySuffixDecorator.GenerateName();

		// Assert
		Assert.That(result, Is.EqualTo($"{baseName}{suffix}"));
	}
}
