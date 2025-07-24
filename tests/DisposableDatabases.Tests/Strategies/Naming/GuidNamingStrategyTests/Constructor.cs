// <copyright file="Constructor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Strategies.Naming;

namespace DisposableDatabases.Tests.Strategies.Naming.GuidNamingStrategyTests;

[TestFixture]
public class Constructor
{
	private const string DefaultGuidFormatPattern = NFormatPattern;
	private const string NFormatPattern = "^[0-9a-fA-F]{32}$";

	[Test]
	public void ProvidesDefaultGuidFormatGivenNoGuidFormat()
	{
		// Arrange
		var namingStrategy = new GuidNamingStrategy();

		// Act
		string result = namingStrategy.GenerateName();

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
		Action action = () => _ = new GuidNamingStrategy(validGuidFormat);

		// Assert
		Assert.That(action, Throws.Nothing);
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenNullGuidFormat()
	{
		// Act
		Action action = () => _ = new GuidNamingStrategy(null);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptyGuidFormat")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceGuidFormat")]
	public void ThrowsArgumentExceptionGivenEmptyOrWhitespaceGuidFormat(string invalidGuidFormat)
	{
		// Act
		Action action = () => _ = new GuidNamingStrategy(invalidGuidFormat);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentException>());
	}

	[TestCase("Z")]
	public void ThrowsArgumentExceptionGivenInvalidGuidFormat(string invalidGuidFormat)
	{
		// Act
		Action action = () => _ = new GuidNamingStrategy(invalidGuidFormat);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentException>().With.Message.StartsWith("Invalid GUID format specified."));
	}
}
