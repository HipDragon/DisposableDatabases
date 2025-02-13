// <copyright file="PostgreSqlDatabaseService.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.DatabaseOperations;

namespace DisposableDatabases.PostgreSql;

/// <summary>
/// Service for creating and dropping PostgreSQL databases.
/// </summary>
public class PostgreSqlDatabaseService : DatabaseCreatorDropperAndSqlScriptExecutor
{
	/// <summary>
	/// Service for creating and dropping PostgreSQL databases.
	/// </summary>
	public PostgreSqlDatabaseService()
		: base(new PostgreSqlDatabaseCreator(), new PostgreSqlDatabaseDropper(), new PostgreSqlSqlScriptExecutor())
	{
	}
}
