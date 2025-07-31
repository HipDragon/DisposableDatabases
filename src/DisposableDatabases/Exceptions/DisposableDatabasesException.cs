// <copyright file="DisposableDatabasesException.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Runtime.Serialization;

namespace DisposableDatabases.Exceptions;

/// <summary>
/// Represents exceptions that occur during disposable database operations.
/// </summary>
#if NETFRAMEWORK
[Serializable]
#endif
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

	#if NETFRAMEWORK
	/// <inheritdoc />
	protected DisposableDatabasesException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
	#endif
}
