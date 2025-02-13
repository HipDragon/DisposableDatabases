// <copyright file="FilePath.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

namespace DisposableDatabases.Tests.TemporaryFileTests;

[TestFixture]
public class FilePath
{
	[Test]
	public void ThrowsObjectDisposedExceptionWhenDisposed()
	{
		// Arrange
		var temporaryFile = new TemporaryFile();
		temporaryFile.Dispose();

		// Act
		Action action = () => _ = temporaryFile.FilePath;

		// Assert
		Assert.That(action, Throws.TypeOf<ObjectDisposedException>());
	}

	/*[Test]
	public void ReturnsFilePath()
	{
		// Arrange
		const string expectedFilePath = "";
		string actualFilePath;

		using (var temporaryFile = new TemporaryFile())
		{
			// Act
			actualFilePath = temporaryFile.FilePath;
		}

		// Assert
		Assert.That(actualFilePath, Is.EqualTo(expectedFilePath));
	}*/
}
