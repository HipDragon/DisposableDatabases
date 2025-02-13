// <copyright file="GetDatabaseName.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Strategies.DatabaseNaming;

namespace DisposableDatabases.Tests.Strategies.DatabaseNaming.GuidDatabaseNamingStrategyTests;

[TestFixture]
public class GetDatabaseName
{
	private const string NFormatPattern = "^[0-9a-fA-F]{32}$";
	private const string DFormatPattern = "^[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12}$";
	private const string BFormatPattern = @"^\{[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12}\}$";
	private const string PFormatPattern = @"^\([0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12}\)$";
	private const string XFormatPattern = @"^\{0x[0-9a-fA-F]{8},0x[0-9a-fA-F]{4},0x[0-9a-fA-F]{4},\{(0x[0-9a-fA-F]{2},){7}0x[0-9a-fA-F]{2}\}\}$";

	[Test]
	public void GenerateUniqueNames()
	{
		// Arrange
		var namingStrategy = new GuidDatabaseNamingStrategy();

		// Act
		string firstResult = namingStrategy.GenerateDatabaseName();
		string secondResult = namingStrategy.GenerateDatabaseName();

		// Assert
		Assert.That(firstResult, Is.Not.EqualTo(secondResult));
	}

	[TestCase("N", NFormatPattern)]
	[TestCase("D", DFormatPattern)]
	[TestCase("B", BFormatPattern)]
	[TestCase("P", PFormatPattern)]
	[TestCase("X", XFormatPattern)]
	public void GenerateFormattedGuid(string format, string expectedPattern)
	{
		// Arrange
		var namingStrategy = new GuidDatabaseNamingStrategy(format);

		// Act
		string result = namingStrategy.GenerateDatabaseName();

		// Assert
		Assert.That(result, Does.Match(expectedPattern));
	}
}
