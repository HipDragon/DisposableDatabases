// <copyright file="ConnectionString.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces.Strategies;
using NSubstitute;

namespace DisposableDatabases.Tests.DisposableDatabaseTests;

[TestFixture]
public class DatabaseName
{
	[Test]
	public void ThrowsObjectDisposedExceptionWhenDisposed()
	{
		// Arrange
		IDisposableDatabaseCreationStrategy disposableDatabaseCreationStrategy = Substitute.For<IDisposableDatabaseCreationStrategy>();
		var disposableDatabase = new TestDisposableDatabase("ValidConnectionString", "DatabaseName", disposableDatabaseCreationStrategy);
		disposableDatabase.Dispose();

		// Act
		Action action = () => _ = disposableDatabase.DatabaseName;

		// Assert
		Assert.That(action, Throws.TypeOf<ObjectDisposedException>());
	}

	[Test]
	public void ReturnsConnectionString()
	{
		// Arrange
		const string expectedDatabaseName = "DatabaseName";
		IDisposableDatabaseCreationStrategy disposableDatabaseCreationStrategy = Substitute.For<IDisposableDatabaseCreationStrategy>();
		string actualDatabaseName;

		using (var disposableDatabase = new TestDisposableDatabase("ValidConnectionString", expectedDatabaseName, disposableDatabaseCreationStrategy))
		{
			// Act
			actualDatabaseName = disposableDatabase.DatabaseName;
		}

		// Assert
		Assert.That(actualDatabaseName, Is.EqualTo(expectedDatabaseName));
	}
}
