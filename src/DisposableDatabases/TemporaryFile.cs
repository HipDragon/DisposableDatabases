// <copyright file="TemporaryFile.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using CommunityToolkit.Diagnostics;
using DisposableDatabases.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DisposableDatabases;

/// <summary>
/// Represents a temporary file that is automatically created and can be disposed of, ensuring proper cleanup.
/// </summary>
/// <remarks>
/// The <see cref="TemporaryFile"/> class provides functionality for creating temporary files with or without specific extensions.
/// It also ensures the automatic deletion of the file upon disposal, protecting against residual temporary files.
/// This class uses the .NET garbage collection mechanism to suppress finalization and ensure proper cleanup of resources.
/// </remarks>
public class TemporaryFile : IDisposable
{
	/// <summary>
	/// Stores the full path of the temporary file created by the instance.
	/// </summary>
	/// <remarks>
	/// This path is generated during object construction and is unique for each instance.
	/// It points to a physical file created in the system's temporary directory.
	/// </remarks>
	private readonly string _filePath;

	/// <summary>
	/// Provides logging capabilities for the <see cref="TemporaryFile"/> class.
	/// </summary>
	/// <remarks>
	/// This logger is used to record events and errors that occur during operations involving a temporary file.
	/// It can report failed attempts to delete the temporary file and other notable behaviors,
	/// offering insights into operations and aiding error diagnosis.
	/// </remarks>
	private readonly ILogger<TemporaryFile> _logger;

	/// <summary>
	/// Indicates whether the current instance has already been disposed of.
	/// </summary>
	private bool _disposed;

	/// <summary>
	/// Manages the creation and lifecycle of a temporary file, ensuring it is properly cleaned up when no longer needed.
	/// </summary>
	/// <remarks>
	/// This class supports the creation of temporary files, optionally with specific extensions.
	/// By implementing <see cref="IDisposable"/>, it ensures the file is deleted automatically upon disposal.
	/// It also includes mechanisms to prevent usage after being disposed of.
	/// </remarks>
	/// <exception cref="ArgumentNullException">Thrown when required arguments such as the logger or extension are null.</exception>
	/// <exception cref="ObjectDisposedException">Thrown if methods or properties of a disposed <see cref="TemporaryFile"/> instance are accessed.</exception>
	public TemporaryFile() : this(new NullLogger<TemporaryFile>())
	{
	}

	/// <summary>
	/// Represents a temporary file that is automatically managed during its lifecycle, with guaranteed cleanup upon disposal.
	/// </summary>
	/// <remarks>
	/// The <see cref="TemporaryFile"/> class allows for the creation of temporary files with specified extensions, ensuring they are deleted when the instance is disposed.
	/// This class leverages <see cref="IDisposable"/> for resource management and provides safeguards against accessing disposed instances.
	/// It can also be used with dependency injection by providing a logger instance for enhanced traceability of file operations.
	/// </remarks>
	/// <exception cref="ArgumentNullException">Thrown if required arguments such as the file extension or logger are null.</exception>
	/// <exception cref="ObjectDisposedException">Thrown when attempting to access properties or methods of a disposed <see cref="TemporaryFile"/> instance.</exception>
	public TemporaryFile(string? extension) : this(extension, new NullLogger<TemporaryFile>())
	{
	}

	/// <summary>
	/// Manages the lifecycle of a temporary file, ensuring it is created and properly cleaned up when no longer needed.
	/// </summary>
	/// <remarks>
	/// This class allows the creation of temporary files, optionally with specific extensions, and guarantees their deletion upon disposal.
	/// It integrates with .NET's resource management functionality through <see cref="IDisposable"/> and uses a logging mechanism to track its operations.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown when a null logger instance is provided to the constructor.
	/// </exception>
	/// <exception cref="IOException">
	/// Thrown during the creation of the temporary file if the underlying file system encounters an error.
	/// </exception>
	/// <exception cref="ObjectDisposedException">
	/// Thrown when attempting to access methods or properties of a disposed <see cref="TemporaryFile"/> instance.
	/// </exception>
	public TemporaryFile(ILogger<TemporaryFile>? logger)
	{
		Guard.IsNotNull(logger);

		_logger = logger;
		_filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

		File.Create(_filePath).Dispose();
	}

	/// <summary>
	/// Provides functionality to create and manage a temporary file that is automatically cleaned up when disposed of.
	/// </summary>
	/// <remarks>
	/// The <see cref="TemporaryFile"/> class allows for the creation of temporary files with optional file extensions.
	/// It ensures proper cleanup of the file when disposed, preventing leftover temporary files.
	/// The class uses resource management techniques to avoid file system clutter and supports extension validation when provided.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown when null or invalid arguments, such as a null logger or an empty file extension, are passed to the constructor.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	/// Thrown when an attempt is made to reuse or access resources of a disposed <see cref="TemporaryFile"/> instance.
	/// </exception>
	public TemporaryFile(string? extension, ILogger<TemporaryFile>? logger)
	{
		Guard.IsNotNullOrWhiteSpace(extension);
		Guard.IsNotNull(logger);

		_logger = logger;
		_filePath = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Path.GetRandomFileName(), extension));

		File.Create(_filePath).Dispose();
	}

	/// <summary>
	/// Gets the full path of the temporary file created by the instance.
	/// </summary>
	/// <remarks>
	/// This property provides the unique path to the temporary file managed by the instance.
	/// It is generated during the construction of the object and used for file operations throughout the instance's lifecycle.
	/// The file is automatically deleted upon disposal to prevent residual temporary files.
	/// </remarks>
	public string FilePath
	{
		get
		{
			CheckDisposed();

			return _filePath;
		}
	}

	/// <summary>
	/// Releases the resources used by the <see cref="TemporaryFile"/> instance.
	/// </summary>
	/// <remarks>
	/// This method ensures that the temporary file associated with the <see cref="TemporaryFile"/> instance is deleted
	/// and that any other unmanaged resources are properly released. It can be called explicitly or via
	/// the garbage collector during finalization through the dispose pattern implementation.
	/// </remarks>
	/// <example>
	/// It is recommended to call this method explicitly or use the <see langword="using"/> statement to ensure immediate cleanup of the temporary file.
	/// </example>
	/// <exception cref="ObjectDisposedException">Thrown if this method is called on an already disposed <see cref="TemporaryFile"/> instance.</exception>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Releases the resources used by the <see cref="TemporaryFile"/> instance and optionally deletes the associated temporary file.
	/// </summary>
	/// <param name="disposing">A boolean value indicating whether managed resources should be released.
	/// Set to <c>true</c> to release both managed and unmanaged resources, or <c>false</c> to release only unmanaged resources.</param>
	/// <remarks>
	/// This method is called by <see cref="Dispose()"/> or by a finalizer.
	/// When called with <paramref name="disposing"/> set to <c>true</c>, the method ensures proper cleanup of any associated resources,
	/// including the deletion of the temporary file created by the <see cref="TemporaryFile"/> instance.
	/// </remarks>
	/// <exception cref="ObjectDisposedException">Thrown if the method is accessed after the object has already been disposed of.</exception>
	protected virtual void Dispose(bool disposing)
	{
		if (_disposed)
		{
			return;
		}

		if (disposing)
		{
			DeleteFile();
		}

		_disposed = true;
	}

	/// <summary>
	/// Ensures that the current instance of the object has not been disposed of.
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

		ThrowHelper.ThrowObjectDisposedException(nameof(TemporaryFile));
	}

	/// <summary>
	/// Deletes the temporary file associated with the current instance, if it exists, ensuring proper cleanup of resources.
	/// </summary>
	/// <remarks>
	/// This method attempts to delete the file specified by the <see cref="FilePath"/> property.
	/// If an error occurs during deletion, such as insufficient permissions or IO constraints,
	/// the exception is caught, and a log entry is created to indicate the failure without throwing the exception further.
	/// </remarks>
	private void DeleteFile()
	{
		try
		{
			if (File.Exists(FilePath))
			{
				File.Delete(FilePath);
			}
		}
		catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
		{
			_logger.LogDeleteTemporaryFileFailed(FilePath, e);
		}
	}
}
