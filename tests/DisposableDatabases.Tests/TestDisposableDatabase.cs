// <copyright file="TestDisposableDatabase.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.Interfaces.Strategies;
using Microsoft.Extensions.Logging;

namespace DisposableDatabases.Tests;

internal sealed class TestDisposableDatabase : DisposableDatabase
{
	internal TestDisposableDatabase(string? connectionString, string? databaseName, IDisposableDatabaseCreationStrategy? databaseCreationStrategy)
		: base(connectionString, databaseName, databaseCreationStrategy)
	{
	}

	internal TestDisposableDatabase(string? connectionString,
	                                string? databaseName,
	                                IDisposableDatabaseCreationStrategy? databaseCreationStrategy,
	                                ILogger<DisposableDatabase>? logger)
		: base(connectionString, databaseName, databaseCreationStrategy, logger)
	{
	}
}
