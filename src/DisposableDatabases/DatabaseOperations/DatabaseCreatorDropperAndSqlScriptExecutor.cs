// <copyright file="DatabaseCreatorDropperAndSqlScriptExecutor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Diagnostics;
using DisposableDatabases.Interfaces.DatabaseOperations;

namespace DisposableDatabases.DatabaseOperations;

/// <summary>
/// Represents an abstract base class that provides functionality for creating, dropping, and executing SQL scripts on a databases.
/// </summary>
public abstract class DatabaseCreatorDropperAndSqlScriptExecutor : DatabaseCreatorAndDropper, IDatabaseCreatorDropperAndSqlScriptExecutor
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DatabaseCreatorDropperAndSqlScriptExecutor"/> class.
	/// </summary>
	/// <param name="databaseCreator">The database creator object used for creating databases.</param>
	/// <param name="databaseDropper">The database dropper object used for dropping databases.</param>
	/// <param name="sqlScriptExecutor">The SQL script executor object used for executing SQL scripts.</param>
	/// <exception cref="ArgumentNullException">
	/// Thrown if either <paramref name="databaseCreator" />, <paramref name="databaseDropper" />, or <paramref name="sqlScriptExecutor" /> is <c>null</c>.
	/// </exception>
	protected DatabaseCreatorDropperAndSqlScriptExecutor([NotNull] IDatabaseCreator? databaseCreator, [NotNull] IDatabaseDropper? databaseDropper, [NotNull] ISqlScriptExecutor? sqlScriptExecutor)
		: base(databaseCreator, databaseDropper)
	{
		Guard.IsNotNull(sqlScriptExecutor);

		SqlScriptExecutor = sqlScriptExecutor;
	}

	/// <summary>
	/// Gets the <see cref="ISqlScriptExecutor" /> instance responsible for executing SQL scripts.
	/// </summary>
	protected ISqlScriptExecutor SqlScriptExecutor { get; }

	/// <inheritdoc cref="ISqlScriptExecutor.ExecuteSqlScriptAsync(string, string, string)" />
	public Task ExecuteSqlScriptAsync(string connectionString, string databaseName, string sqlScriptFilePath)
	{
		return SqlScriptExecutor.ExecuteSqlScriptAsync(connectionString, databaseName, sqlScriptFilePath);
	}

	/// <inheritdoc cref="ISqlScriptExecutor.ExecuteSqlScriptAsync(string, string, string)" />
	public Task ExecuteSqlScriptAsync(string connectionString, string databaseName, string sqlScriptFilePath, CancellationToken cancellationToken)
	{
		return SqlScriptExecutor.ExecuteSqlScriptAsync(connectionString, databaseName, sqlScriptFilePath, cancellationToken);
	}
}
