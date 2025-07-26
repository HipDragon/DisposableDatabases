// <copyright file="Constructor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;

namespace DisposableDatabases.Tests.TemporaryFileTests;

[TestFixture]
public class Constructor
{
	[Test]
	public void ThrowsArgumentNullExceptionGivenNullExtension()
	{
		// Act
		Action action = () => _ = new TemporaryFile(extension: null);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptyExtension")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceExtension")]
	public void ThrowsArgumentExceptionGivenEmptyOrWhitespaceExtension(string invalidExtension)
	{
		// Act
		Action action = () => _ = new TemporaryFile(invalidExtension);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentException>());
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenNullLogger()
	{
		// Act
		Action action = () => _ = new TemporaryFile(logger: null);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenExtensionAndNullLogger()
	{
		// Arrange
		const string extension = "tmp";

		// Act
		Action action = () => _ = new TemporaryFile(extension, null);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public void ProvidesDefaultNullLoggerGivenNoLogger()
	{
		// Act
		using (var temporaryFile = new TemporaryFile())
		{
			// Assert
			object? actualLogger = typeof(TemporaryFile).GetField("_logger", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(temporaryFile);
			Assert.That(actualLogger, Is.TypeOf<NullLogger<TemporaryFile>>());
		}
	}

	[Test]
	public void ProvidesDefaultNullLoggerGivenExtensionAndNoLogger()
	{
		// Arrange
		const string extension = "tmp";

		// Act
		using (var temporaryFile = new TemporaryFile(extension))
		{
			// Assert
			object? actualLogger = typeof(TemporaryFile).GetField("_logger", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(temporaryFile);
			Assert.That(actualLogger, Is.TypeOf<NullLogger<TemporaryFile>>());
		}
	}

	[Test]
	public void CreatesFileInTempDirectory()
	{
		// Act
		using (var temporaryFile = new TemporaryFile())
		{
			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(File.Exists(temporaryFile.FilePath), Is.True);
				Assert.That(Path.GetDirectoryName(temporaryFile.FilePath), Is.EqualTo(Path.GetDirectoryName(Path.GetTempPath())));
			}
		}
	}

	[Test]
	public void CreatesFileInTempDirectoryGivenExtension()
	{
		// Arrange
		const string extension = "tmp";
		const string expectedExtension = ".tmp";

		// Act
		using (var temporaryFile = new TemporaryFile(extension))
		{
			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(File.Exists(temporaryFile.FilePath), Is.True);
				Assert.That(Path.GetExtension(temporaryFile.FilePath), Is.EqualTo(expectedExtension));
				Assert.That(Path.GetDirectoryName(temporaryFile.FilePath), Is.EqualTo(Path.GetDirectoryName(Path.GetTempPath())));
			}
		}
	}
}
