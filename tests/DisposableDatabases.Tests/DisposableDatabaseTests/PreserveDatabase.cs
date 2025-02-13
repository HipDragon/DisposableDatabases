// <copyright file="PreserveDatabase.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces.Strategies;
using NSubstitute;

namespace DisposableDatabases.Tests.DisposableDatabaseTests;

[TestFixture]
public class PreserveDatabase
{
	[Test]
	public void ThrowsObjectDisposedExceptionWhenDisposed()
	{
		// Arrange
		IDisposableDatabaseCreationStrategy disposableDatabaseCreationStrategy = Substitute.For<IDisposableDatabaseCreationStrategy>();
		var disposableDatabase = new TestDisposableDatabase("ValidConnectionString", "DatabaseName", disposableDatabaseCreationStrategy);
		disposableDatabase.Dispose();

		// Act
		Action action = () => _ = disposableDatabase.PreserveDatabase;

		// Assert
		Assert.That(action, Throws.TypeOf<ObjectDisposedException>());
	}

	[Test]
	public void ReturnsPreserveDatabase()
	{
		// Arrange
		IDisposableDatabaseCreationStrategy disposableDatabaseCreationStrategy = Substitute.For<IDisposableDatabaseCreationStrategy>();
		bool actualPreserveDatabase;

		using (var disposableDatabase = new TestDisposableDatabase("ValidConnectionString", "DatabaseName", disposableDatabaseCreationStrategy))
		{
			Assume.That(disposableDatabase.PreserveDatabase, Is.False);
			disposableDatabase.PreserveDatabase = true;

			// Act
			actualPreserveDatabase = disposableDatabase.PreserveDatabase;
		}

		// Assert
		Assert.That(actualPreserveDatabase, Is.True);
	}

	[Test]
	public void SetThrowsObjectDisposedExceptionWhenDisposed()
	{
		// Arrange
		IDisposableDatabaseCreationStrategy disposableDatabaseCreationStrategy = Substitute.For<IDisposableDatabaseCreationStrategy>();
		var disposableDatabase = new TestDisposableDatabase("ValidConnectionString", "DatabaseName", disposableDatabaseCreationStrategy);
		disposableDatabase.Dispose();

		// Act
		Action action = () => _ = disposableDatabase.PreserveDatabase = true;

		// Assert
		Assert.That(action, Throws.TypeOf<ObjectDisposedException>());
	}
}
