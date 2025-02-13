// <copyright file="DisposableDatabasesException.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

namespace DisposableDatabases.Exceptions;

/// <summary>
/// Represents exceptions that occur during disposable database operations.
/// </summary>
public class DisposableDatabasesException : Exception
{
	/// <inheritdoc />
	public DisposableDatabasesException()
	{
	}

	/// <inheritdoc />
	public DisposableDatabasesException(string message) : base(message)
	{
	}

	/// <inheritdoc />
	public DisposableDatabasesException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
