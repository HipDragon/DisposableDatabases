// <copyright file="GetDatabaseName.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces.Strategies;
using DisposableDatabases.Strategies.DatabaseNaming.Decorators;
using NSubstitute;

namespace DisposableDatabases.Tests.Strategies.DatabaseNaming.Decorators.DatabaseNamingStrategySuffixDecoratorTests;

[TestFixture]
public class GetDatabaseName
{
	[Test]
	public void PrependsPrefixToDatabaseNamingStrategyGeneratedDatabaseName()
	{
		// Arrange
		const string suffix = "-suffix";
		const string baseName = "BaseName";
		IDatabaseNamingStrategy substituteDatabaseNamingStrategy = Substitute.For<IDatabaseNamingStrategy>();
		substituteDatabaseNamingStrategy.GenerateDatabaseName().Returns(baseName);
		var databaseNamingStrategySuffixDecorator = new DatabaseNamingStrategySuffixDecorator(substituteDatabaseNamingStrategy, suffix);

		// Act
		string result = databaseNamingStrategySuffixDecorator.GenerateDatabaseName();

		// Assert
		Assert.That(result, Is.EqualTo($"{baseName}{suffix}"));
	}
}
