// <copyright file="IDatabaseCreatorDropperAndSqlScriptExecutor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

namespace DisposableDatabases.Interfaces.DatabaseOperations;

/// <summary>
/// Provides methods to create, drop, and execute SQL scripts on a database.
/// </summary>
public interface IDatabaseCreatorDropperAndSqlScriptExecutor : IDatabaseCreator, IDatabaseDropper, ISqlScriptExecutor
{
}
