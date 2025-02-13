// <copyright file="Constructor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Strategies.DatabaseNaming;

namespace DisposableDatabases.Tests.Strategies.DatabaseNaming.GuidDatabaseNamingStrategyTests;

[TestFixture]
public class Constructor
{
	private const string DefaultGuidFormatPattern = NFormatPattern;
	private const string NFormatPattern = "^[0-9a-fA-F]{32}$";

	[Test]
	public void ProvidesDefaultGuidFormatGivenNoGuidFormat()
	{
		// Arrange
		var namingStrategy = new GuidDatabaseNamingStrategy();

		// Act
		string result = namingStrategy.GenerateDatabaseName();

		// Assert
		Assert.That(result, Does.Match(DefaultGuidFormatPattern));
	}

	[TestCase("N")]
	[TestCase("D")]
	[TestCase("B")]
	[TestCase("P")]
	[TestCase("X")]
	public void ThrowsNothingGivenValidGuidFormat(string validGuidFormat)
	{
		// Arrange & Act
		Action action = () => _ = new GuidDatabaseNamingStrategy(validGuidFormat);

		// Assert
		Assert.That(action, Throws.Nothing);
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenNullGuidFormat()
	{
		// Act
		Action action = () => _ = new GuidDatabaseNamingStrategy(null);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptySqlScriptFilePath")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceSqlScriptFilePath")]
	public void ThrowsArgumentExceptionGivenEmptyOrWhitespaceGuidFormat(string invalidGuidFormat)
	{
		// Act
		Action action = () => _ = new GuidDatabaseNamingStrategy(invalidGuidFormat);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentException>());
	}

	[TestCase("Z")]
	public void ThrowsArgumentExceptionGivenInvalidGuidFormat(string invalidGuidFormat)
	{
		// Act
		Action action = () => _ = new GuidDatabaseNamingStrategy(invalidGuidFormat);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentException>().With.Message.StartsWith("Invalid GUID format specified."));
	}
}
