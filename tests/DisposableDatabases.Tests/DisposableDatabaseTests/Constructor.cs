// <copyright file="Constructor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Reflection;
using DisposableDatabases.Interfaces.Strategies;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace DisposableDatabases.Tests.DisposableDatabaseTests;

[TestFixture]
public class Constructor
{
	[Test]
	public void ThrowsArgumentNullExceptionGivenNullConnectionString()
	{
		// Arrange
		IDisposableDatabaseCreationStrategy disposableDatabaseCreationStrategy = Substitute.For<IDisposableDatabaseCreationStrategy>();

		// Act
		Action action = () => _ = new TestDisposableDatabase(null, "DatabaseName", disposableDatabaseCreationStrategy);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptyConnectionString")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceConnectionString")]
	public void ThrowsArgumentExceptionGivenEmptyOrWhitespaceConnectionString(string invalidConnectionString)
	{
		// Arrange
		IDisposableDatabaseCreationStrategy disposableDatabaseCreationStrategy = Substitute.For<IDisposableDatabaseCreationStrategy>();

		// Act
		Action action = () => _ = new TestDisposableDatabase(invalidConnectionString, "DatabaseName", disposableDatabaseCreationStrategy);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentException>());
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenNullDatabaseName()
	{
		// Arrange
		IDisposableDatabaseCreationStrategy disposableDatabaseCreationStrategy = Substitute.For<IDisposableDatabaseCreationStrategy>();

		// Act
		Action action = () => _ = new TestDisposableDatabase("ValidConnectionString", null, disposableDatabaseCreationStrategy);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[TestCase("", TestName = "ThrowsArgumentExceptionGivenEmptyDatabaseName")]
	[TestCase(" \t\n\r\f\v", TestName = "ThrowsArgumentExceptionGivenWhitespaceDatabaseName")]
	public void ThrowsArgumentExceptionGivenEmptyOrWhitespaceDatabaseName(string invalidDatabaseName)
	{
		// Arrange
		IDisposableDatabaseCreationStrategy disposableDatabaseCreationStrategy = Substitute.For<IDisposableDatabaseCreationStrategy>();

		// Act
		Action action = () => _ = new TestDisposableDatabase("ValidConnectionString", invalidDatabaseName, disposableDatabaseCreationStrategy);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentException>());
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenNullDatabaseCreationStrategy()
	{
		// Act
		Action action = () => _ = new TestDisposableDatabase("ValidConnectionString", "DatabaseName", null);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public void ThrowsArgumentNullExceptionGivenNullILogger()
	{
		// Arrange
		IDisposableDatabaseCreationStrategy disposableDatabaseCreationStrategy = Substitute.For<IDisposableDatabaseCreationStrategy>();

		// Act
		Action action = () => _ = new TestDisposableDatabase("ValidConnectionString", "DatabaseName", disposableDatabaseCreationStrategy, null);

		// Assert
		Assert.That(action, Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public void ProvidesDefaultNullLoggerGivenNoILogger()
	{
		// Arrange
		IDisposableDatabaseCreationStrategy disposableDatabaseCreationStrategy = Substitute.For<IDisposableDatabaseCreationStrategy>();
		NullLogger<DisposableDatabase> expectedLogger = NullLogger<DisposableDatabase>.Instance;

		// Act
		using (var disposableDatabase = new TestDisposableDatabase("ValidConnectionString", "DatabaseName", disposableDatabaseCreationStrategy))
		{
			// Assert
			object? actualLogger = typeof(DisposableDatabase).GetField("_logger", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(disposableDatabase);
			Assert.That(actualLogger, Is.SameAs(expectedLogger));
		}
	}

	[Test]
	public void AssignsPublicProperties()
	{
		// Arrange
		IDisposableDatabaseCreationStrategy disposableDatabaseCreationStrategy = Substitute.For<IDisposableDatabaseCreationStrategy>();

		// Act
		using (var testDisposableDatabase = new TestDisposableDatabase("ValidConnectionString", "ValidDatabaseName", disposableDatabaseCreationStrategy))
		{
			// Assert
			Assert.Multiple(() =>
			{
				Assert.That(testDisposableDatabase.ConnectionString, Is.EqualTo("ValidConnectionString"));
				Assert.That(testDisposableDatabase.DatabaseName, Is.EqualTo("ValidDatabaseName"));
			});
		}
	}
}
