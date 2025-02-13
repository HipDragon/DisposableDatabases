// <copyright file="Dispose.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using DisposableDatabases.Exceptions;
using DisposableDatabases.Interfaces;
using DisposableDatabases.Interfaces.Strategies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace DisposableDatabases.Tests.DisposableDatabaseTests;

[TestFixture]
public class Dispose
{
	[Test]
	public void CallsDropDatabaseGivenPreserveDatabaseIsFalse()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		const string databaseName = "ValidDatabaseName";
		IDisposableDatabaseCreationStrategy substituteDisposableDatabaseCreationStrategy = Substitute.For<IDisposableDatabaseCreationStrategy>();
		var testDisposableDatabase = new TestDisposableDatabase(connectionString, databaseName, substituteDisposableDatabaseCreationStrategy);
		testDisposableDatabase.PreserveDatabase = false;

		// Act
		testDisposableDatabase.Dispose();

		// Assert
		substituteDisposableDatabaseCreationStrategy.Received(1).DisposeDatabaseAsync(testDisposableDatabase);
	}

	[Test]
	public void DoesNotCallDropDatabaseGivenPreserveDatabaseIsTrue()
	{
		// Arrange
		IDisposableDatabaseCreationStrategy substituteDisposableDatabaseCreationStrategy = Substitute.For<IDisposableDatabaseCreationStrategy>();
		var testDisposableDatabase = new TestDisposableDatabase("ValidConnectionString", "ValidDatabaseName", substituteDisposableDatabaseCreationStrategy);
		testDisposableDatabase.PreserveDatabase = true;

		// Act
		testDisposableDatabase.Dispose();

		// Assert
		substituteDisposableDatabaseCreationStrategy.DidNotReceive().DisposeDatabaseAsync(Arg.Any<IDisposableDatabase>());
	}

	[Test]
	public void LogsPreservingDatabaseGivenPreserveDatabaseIsTrue()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		const string databaseName = "ValidDatabaseName";
		var expectedEventId = new EventId(1, "PreserveDatabase");
		IReadOnlyList<KeyValuePair<string, string>> expectedStructuredState = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("DatabaseName", databaseName),
			new KeyValuePair<string, string>("{OriginalFormat}", "Preserving database '{DatabaseName}'.")
		};

		IDisposableDatabaseCreationStrategy substituteDisposableDatabaseCreationStrategy = Substitute.For<IDisposableDatabaseCreationStrategy>();
		var fakeLogger = new FakeLogger<DisposableDatabase>();

		var testDisposableDatabase = new TestDisposableDatabase(connectionString, databaseName, substituteDisposableDatabaseCreationStrategy, fakeLogger);
		testDisposableDatabase.PreserveDatabase = true;

		// Act
		testDisposableDatabase.Dispose();

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(fakeLogger.Collector.LatestRecord.Level, Is.EqualTo(LogLevel.Debug));
			Assert.That(fakeLogger.Collector.LatestRecord.Id, Is.EqualTo(expectedEventId));
			Assert.That(fakeLogger.Collector.LatestRecord.Id.Name, Is.EqualTo(expectedEventId.Name));
			Assert.That(fakeLogger.Collector.LatestRecord.Message, Is.EqualTo($"Preserving database '{databaseName}'."));
			Assert.That(fakeLogger.Collector.LatestRecord.StructuredState, Is.EqualTo(expectedStructuredState));
		});
	}

	[Test]
	[SuppressMessage("Maintainability", "S3966:Objects should not be disposed more than once", Justification = "Testing to make sure dispose is idempotent")]
	public void IsIdempotent()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		const string databaseName = "ValidDatabaseName";
		IDisposableDatabaseCreationStrategy substituteDisposableDatabaseCreationStrategy = Substitute.For<IDisposableDatabaseCreationStrategy>();
		var testDisposableDatabase = new TestDisposableDatabase(connectionString, databaseName, substituteDisposableDatabaseCreationStrategy);
		testDisposableDatabase.Dispose();

		// Act
		testDisposableDatabase.Dispose();

		// Assert
		substituteDisposableDatabaseCreationStrategy.Received(1).DisposeDatabaseAsync(testDisposableDatabase);
	}

	[Test]
	public void LogsErrorMessageGivenDatabaseCreationStrategyThrowsDisposableDatabaseException()
	{
		// Arrange
		const string connectionString = "ValidConnectionString";
		const string databaseName = "ValidDatabaseName";
		var expectedEventId = new EventId(2, "DropDatabaseFailed");
		IReadOnlyList<KeyValuePair<string, string>> expectedStructuredState = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("DatabaseName", databaseName),
			new KeyValuePair<string, string>("{OriginalFormat}", "Dropping database failed. Manual cleanup is required for the database '{DatabaseName}'.")
		};

		var fakeLogger = new FakeLogger<DisposableDatabase>();
		var expectedException = new DisposableDatabasesException("Failed to drop database.");
		IDisposableDatabaseCreationStrategy substituteDisposableDatabaseCreationStrategy = Substitute.For<IDisposableDatabaseCreationStrategy>();
		var testDisposableDatabase = new TestDisposableDatabase(connectionString, databaseName, substituteDisposableDatabaseCreationStrategy, fakeLogger);

		substituteDisposableDatabaseCreationStrategy.DisposeDatabaseAsync(testDisposableDatabase).ThrowsAsync(expectedException);

		// Act
		testDisposableDatabase.Dispose();

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(fakeLogger.Collector.LatestRecord.Level, Is.EqualTo(LogLevel.Error));
			Assert.That(fakeLogger.Collector.LatestRecord.Id, Is.EqualTo(expectedEventId));
			Assert.That(fakeLogger.Collector.LatestRecord.Id.Name, Is.EqualTo(expectedEventId.Name));
			Assert.That(fakeLogger.Collector.LatestRecord.Message, Is.EqualTo($"Dropping database failed. Manual cleanup is required for the database '{databaseName}'."));
			Assert.That(fakeLogger.Collector.LatestRecord.StructuredState, Is.EqualTo(expectedStructuredState));
			Assert.That(fakeLogger.Collector.LatestRecord.Exception, Is.EqualTo(expectedException));
		});
	}
}
