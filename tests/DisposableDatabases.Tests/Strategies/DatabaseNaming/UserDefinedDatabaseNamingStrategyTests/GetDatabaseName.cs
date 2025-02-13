// <copyright file="GetDatabaseName.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Strategies.DatabaseNaming;

namespace DisposableDatabases.Tests.Strategies.DatabaseNaming.UserDefinedDatabaseNamingStrategyTests;

[TestFixture]
public class GetDatabaseName
{
	[Test]
	public void ReturnsDatabaseName()
	{
		// Arrange
		const string expectedDatabaseName = "MyTestDatabase";
		var namingStrategy = new UserDefinedDatabaseNamingStrategy(expectedDatabaseName);

		// Act
		string actualDatabaseName = namingStrategy.GenerateDatabaseName();

		// Assert
		Assert.That(actualDatabaseName, Is.EqualTo(expectedDatabaseName));
	}
}
