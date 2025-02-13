// <copyright file="GetDatabaseName.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces.Strategies;
using DisposableDatabases.Strategies.DatabaseNaming.Decorators;
using NSubstitute;

namespace DisposableDatabases.Tests.Strategies.DatabaseNaming.Decorators.DatabaseNamingStrategyPrefixDecoratorTests;

[TestFixture]
public class GetDatabaseName
{
	[Test]
	public void PrependsPrefixToDatabaseNamingStrategyGeneratedDatabaseName()
	{
		// Arrange
		const string prefix = "Prefix-";
		const string baseName = "BaseName";
		IDatabaseNamingStrategy substituteDatabaseNamingStrategy = Substitute.For<IDatabaseNamingStrategy>();
		substituteDatabaseNamingStrategy.GenerateDatabaseName().Returns(baseName);
		var databaseNamingStrategyPrefixDecorator = new DatabaseNamingStrategyPrefixDecorator(substituteDatabaseNamingStrategy, prefix);

		// Act
		string result = databaseNamingStrategyPrefixDecorator.GenerateDatabaseName();

		// Assert
		Assert.That(result, Is.EqualTo($"{prefix}{baseName}"));
	}
}
