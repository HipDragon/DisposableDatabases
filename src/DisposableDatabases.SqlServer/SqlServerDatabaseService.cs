// <copyright file="SqlServerDatabaseService.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.DatabaseOperations;

namespace DisposableDatabases.SqlServer;

/// <summary>
/// Provides services for creating, dropping, and executing SQL scripts against SQL Server databases.
/// </summary>
public class SqlServerDatabaseService : DatabaseCreatorDropperAndSqlScriptExecutor
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerDatabaseService" /> class.
	/// </summary>
	/// <remarks>
	/// This constructor sets up an SQL Server database creator, dropper, and script executor.
	/// </remarks>
	public SqlServerDatabaseService()
		: base(new SqlServerDatabaseCreator(), new SqlServerDatabaseDropper(), new SqlServerSqlScriptExecutor())
	{
	}
}
