// <copyright file="Dispose.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace DisposableDatabases.Tests.TemporaryFileTests;

[TestFixture]
public class Dispose
{
	[Test]
	[SuppressMessage("Maintainability", "S3966:Objects should not be disposed more than once", Justification = "Testing to make sure dispose is idempotent")]
	public void IsIdempotent()
	{
		// Arrange
		var temporaryFile = new TemporaryFile();
		string filePath = temporaryFile.FilePath;
		temporaryFile.Dispose();

		// Act
		Action action = () => temporaryFile.Dispose();

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(action, Throws.Nothing);
			Assert.That(File.Exists(filePath), Is.False);
		});
	}

	[Test]
	public void LogsErrorMessageGivenFileDeleteException()
	{
		// Arrange
		var fakeLogger = new FakeLogger<TemporaryFile>();
		var temporaryFile = new TemporaryFile(fakeLogger);
		string filePath = temporaryFile.FilePath;
		var expectedEventId = new EventId(3, "DeleteTemporaryFileFailed");
		IReadOnlyList<KeyValuePair<string, string>> expectedStructuredState = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("TemporaryFile", filePath),
			new KeyValuePair<string, string>("{OriginalFormat}", "Deleting temporary file failed. You can safely delete the file '{TemporaryFile}'.")
		};

		using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
		{
			// Act
			temporaryFile.Dispose();
		}

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(fakeLogger.Collector.LatestRecord.Level, Is.EqualTo(LogLevel.Error));
			Assert.That(fakeLogger.Collector.LatestRecord.Id, Is.EqualTo(expectedEventId));
			Assert.That(fakeLogger.Collector.LatestRecord.Id.Name, Is.EqualTo(expectedEventId.Name));
			Assert.That(fakeLogger.Collector.LatestRecord.Message, Is.EqualTo($"Deleting temporary file failed. You can safely delete the file '{filePath}'."));
			Assert.That(fakeLogger.Collector.LatestRecord.StructuredState, Is.EqualTo(expectedStructuredState));
			Assert.That(fakeLogger.Collector.LatestRecord.Exception, Is.Not.Null);
		});
	}
}
