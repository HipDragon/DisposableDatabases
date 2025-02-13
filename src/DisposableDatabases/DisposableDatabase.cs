// <copyright file="DisposableDatabase.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Diagnostics;
using DisposableDatabases.Exceptions;
using DisposableDatabases.Extensions;
using DisposableDatabases.Interfaces;
using DisposableDatabases.Interfaces.Strategies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DisposableDatabases;

/// <summary>
/// Manages a temporary, or "disposable" database, which can be created and disposed
/// on demand during tests or other short-lived operations.
/// </summary>
public class DisposableDatabase : IDisposableDatabase
{
	private readonly IDisposableDatabaseCreationStrategy _disposableDatabaseCreationStrategy;

	/// <summary>
	/// Provides logging functionality for the <see cref="DisposableDatabase" /> class,
	/// enabling the recording of diagnostic messages, warnings, and errors during
	/// the lifecycle of a disposable database and its operations.
	/// </summary>
	private readonly ILogger<DisposableDatabase> _logger;

	/// <summary>
	/// Indicates whether the current instance has already been disposed.
	/// </summary>
	private bool _disposed;

	/// <summary>
	/// Indicates whether the database should be preserved after the instance is disposed.
	/// </summary>
	private bool _preserveDatabase;

	private readonly string _connectionString;

	private readonly string _databaseName;

	/// <summary>
	/// Represents a disposable database instance that can be used for temporary purposes and then discarded.
	/// </summary>
	/// <remarks>
	/// This class ensures that the database instance is created with valid connection string and database name,
	/// and it relies on the provided IDatabaseDropper instance for database disposal operations.
	/// </remarks>
	protected internal DisposableDatabase([NotNull] string? connectionString, [NotNull] string? databaseName, [NotNull] IDisposableDatabaseCreationStrategy? disposableDatabaseCreationStrategy)
		: this(connectionString, databaseName, disposableDatabaseCreationStrategy, NullLogger<DisposableDatabase>.Instance)
	{
	}

	/// <summary>
	/// Represents a disposable database instance capable of being used temporarily and properly cleaned up upon disposal.
	/// </summary>
	/// <remarks>
	/// This class requires valid connection strings and a database name for instantiation. It depends on an IDatabaseDropper implementation
	/// for handling the process of database cleanup and disposal. This ensures resources are released correctly once the instance is no longer needed.
	/// </remarks>
	protected internal DisposableDatabase([NotNull] string? connectionString,
	                                      [NotNull] string? databaseName,
	                                      [NotNull] IDisposableDatabaseCreationStrategy? disposableDatabaseCreationStrategy,
	                                      [NotNull] ILogger<DisposableDatabase>? logger)
	{
		Guard.IsNotNullOrWhiteSpace(connectionString);
		Guard.IsNotNullOrWhiteSpace(databaseName);
		Guard.IsNotNull(disposableDatabaseCreationStrategy);
		Guard.IsNotNull(logger);

		_disposableDatabaseCreationStrategy = disposableDatabaseCreationStrategy;
		_logger = logger;
		_connectionString = connectionString;
		_databaseName = databaseName;
	}

	/// <inheritdoc />
	public string ConnectionString
	{
		get
		{
			CheckDisposed();

			return _connectionString;
		}
	}

	/// <inheritdoc />
	public string DatabaseName
	{
		get
		{
			CheckDisposed();

			return _databaseName;
		}
	}

	/// <inheritdoc />
	public bool PreserveDatabase
	{
		get
		{
			CheckDisposed();

			return _preserveDatabase;
		}

		set
		{
			CheckDisposed();

			_preserveDatabase = value;
		}
	}

	/// <summary>
	/// Disposes the resources used by the <see cref="DisposableDatabase" /> class.
	/// </summary>
	/// <remarks>
	/// This method releases all resources used by the current instance of the <see cref="DisposableDatabase" /> class. It calls the
	/// <c>Dispose(bool disposing)</c> method with the <c>disposing</c> parameter set to <c>true</c>, allowing for both managed and
	/// unmanaged resources to be disposed. The <see cref="GC.SuppressFinalize(object)" /> method is called to prevent the garbage
	/// collector from calling the finalizer of the object if one exists. This method ensures that the disposal process is idempotent
	/// and can be safely called multiple times without causing any exceptions or unexpected behavior.
	/// </remarks>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Asynchronously disposes of the database resources used by the instance.
	/// </summary>
	/// <returns>
	/// A ValueTask representing the asynchronous dispose operation.
	/// </returns>
	public async ValueTask DisposeAsync()
	{
		await DisposeAsyncCoreAsync().ConfigureAwait(false);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Releases all resources used by the DisposableDatabase class.
	/// </summary>
	/// <remarks>
	/// This method ensures that the resources are properly released and the object is disposed of,
	/// avoiding memory leaks and other potential issues. It calls the Dispose method with a true
	/// argument and then suppresses finalization to optimize garbage collection.
	/// </remarks>
	protected virtual void Dispose(bool disposing)
	{
		if (_disposed)
		{
			return;
		}

		if (disposing)
		{
			DisposeDatabaseAsync().ConfigureAwait(false).GetAwaiter().GetResult();
		}

		_disposed = true;
	}

	/// <summary>
	/// Asynchronously performs tasks associated with releasing resources used by the DisposableDatabase instance.
	/// </summary>
	/// <returns>A task that represents the asynchronous dispose operation.</returns>
	/// <remarks>
	/// This method drops the database using the specified IDatabaseDropper if the database
	/// is not marked to be preserved. It also handles any exceptions thrown during the
	/// database drop operation by logging the exception messages.
	/// </remarks>
	protected virtual async ValueTask DisposeAsyncCoreAsync()
	{
		if (_disposed)
		{
			return;
		}

		await DisposeDatabaseAsync().ConfigureAwait(false);

		_disposed = true;
	}

	/// <summary>
	/// Ensures that the current instance of the object has not been disposed.
	/// </summary>
	/// <remarks>
	/// Throws an <see cref="ObjectDisposedException" /> if the instance has already been disposed, preventing access to methods or properties of the disposed object.
	/// </remarks>
	/// <exception cref="ObjectDisposedException">Thrown when attempting to use the object after it has been disposed.</exception>
	private void CheckDisposed()
	{
		if (!_disposed)
		{
			return;
		}

		ThrowHelper.ThrowObjectDisposedException(nameof(DisposableDatabase));
	}

	/// <summary>
	/// Asynchronously drops the temporary database if it is not marked to be preserved.
	/// Uses the configured IDatabaseDropper strategy for performing the drop operation.
	/// </summary>
	/// <returns>
	/// A task that represents the asynchronous operation of dropping the database.
	/// If the database is set to be preserved, the task will complete immediately.
	/// </returns>
	private async Task DisposeDatabaseAsync()
	{
		if (_preserveDatabase)
		{
			_logger.LogPreserveDatabase(DatabaseName);

			return;
		}

		try
		{
			await _disposableDatabaseCreationStrategy.DisposeDatabaseAsync(this).ConfigureAwait(false);
		}
		catch (DisposableDatabasesException e)
		{
			_logger.LogDropDatabaseFailed(DatabaseName, e);
		}
	}
}
