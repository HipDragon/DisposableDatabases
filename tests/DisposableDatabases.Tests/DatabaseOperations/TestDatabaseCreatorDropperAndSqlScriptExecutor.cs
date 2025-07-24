// <copyright file="TestDatabaseCreatorDropperAndSqlScriptExecutor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.DatabaseOperations;
using DisposableDatabases.Interfaces.DatabaseOperations;

namespace DisposableDatabases.Tests.DatabaseOperations;

public class TestDatabaseCreatorDropperAndSqlScriptExecutor : DatabaseCreatorDropperAndSqlScriptExecutor
{
	public TestDatabaseCreatorDropperAndSqlScriptExecutor(IDatabaseCreator? databaseCreator, IDatabaseDropper? databaseDropper, ISqlScriptExecutor? sqlScriptExecutor)
		: base(databaseCreator, databaseDropper, sqlScriptExecutor)
	{
	}
}
