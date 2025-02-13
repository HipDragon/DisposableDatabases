// <copyright file="ConnectionString.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces.Strategies;
using NSubstitute;

namespace DisposableDatabases.Tests.DisposableDatabaseTests;

[TestFixture]
public class ConnectionString
{
	[Test]
	public void ThrowsObjectDisposedExceptionWhenDisposed()
	{
		// Arrange
		IDisposableDatabaseCreationStrategy disposableDatabaseCreationStrategy = Substitute.For<IDisposableDatabaseCreationStrategy>();
		var disposableDatabase = new TestDisposableDatabase("ValidConnectionString", "DatabaseName", disposableDatabaseCreationStrategy);
		disposableDatabase.Dispose();

		// Act
		Action action = () => _ = disposableDatabase.ConnectionString;

		// Assert
		Assert.That(action, Throws.TypeOf<ObjectDisposedException>());
	}

	[Test]
	public void ReturnsConnectionString()
	{
		// Arrange
		const string expectedConnectionString = "ValidConnectionString";
		IDisposableDatabaseCreationStrategy disposableDatabaseCreationStrategy = Substitute.For<IDisposableDatabaseCreationStrategy>();
		string actualConnectionString;

		using (var disposableDatabase = new TestDisposableDatabase(expectedConnectionString, "DatabaseName", disposableDatabaseCreationStrategy))
		{
			// Act
			actualConnectionString = disposableDatabase.ConnectionString;
		}

		// Assert
		Assert.That(actualConnectionString, Is.EqualTo(expectedConnectionString));
	}
}
