// <copyright file="LoggerExtensions.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using Microsoft.Extensions.Logging;

namespace DisposableDatabases.Extensions;

internal static class LoggerExtensions
{
	private static readonly Action<ILogger, string, Exception?> s_preserveDatabase =
		LoggerMessage.Define<string>(LogLevel.Debug, new EventId(1, "PreserveDatabase"), "Preserving database '{DatabaseName}'.");

	private static readonly Action<ILogger, string, Exception?> s_dropDatabaseFailed =
		LoggerMessage.Define<string>(LogLevel.Error, new EventId(2, "DropDatabaseFailed"), "Dropping database failed. Manual cleanup is required for the database '{DatabaseName}'.");

	private static readonly Action<ILogger, string, Exception?> s_deleteTemporaryFileFailed =
		LoggerMessage.Define<string>(LogLevel.Error, new EventId(3, "DeleteTemporaryFileFailed"), "Deleting temporary file failed. You can safely delete the file '{TemporaryFile}'.");

	internal static void LogPreserveDatabase(this ILogger logger, string databaseName)
	{
		s_preserveDatabase(logger, databaseName, null);
	}

	internal static void LogDropDatabaseFailed(this ILogger logger, string databaseName, Exception exception)
	{
		s_dropDatabaseFailed(logger, databaseName, exception);
	}

	internal static void LogDeleteTemporaryFileFailed(this ILogger logger, string temporaryFilePath, Exception exception)
	{
		s_deleteTemporaryFileFailed(logger, temporaryFilePath, exception);
	}
}
