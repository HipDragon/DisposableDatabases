// <copyright file="TestDisposableDatabase.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using DisposableDatabases.Interfaces.Strategies;
using Microsoft.Extensions.Logging;

namespace DisposableDatabases.Tests;

internal sealed class TestDisposableDatabase : DisposableDatabase
{
	internal TestDisposableDatabase([NotNull] string? connectionString, [NotNull] string? databaseName, [NotNull] IDisposableDatabaseCreationStrategy? databaseCreationStrategy)
		: base(connectionString, databaseName, databaseCreationStrategy)
	{
	}

	internal TestDisposableDatabase([NotNull] string? connectionString,
	                                [NotNull] string? databaseName,
	                                [NotNull] IDisposableDatabaseCreationStrategy? databaseCreationStrategy,
	                                [NotNull] ILogger<DisposableDatabase>? logger)
		: base(connectionString, databaseName, databaseCreationStrategy, logger)
	{
	}
}
