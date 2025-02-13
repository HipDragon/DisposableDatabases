// <copyright file="TestDatabaseCreatorDropperAndSqlScriptExecutor.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using DisposableDatabases.DatabaseOperations;
using DisposableDatabases.Interfaces.DatabaseOperations;

namespace DisposableDatabases.Tests.DatabaseOperations;

public class TestDatabaseCreatorDropperAndSqlScriptExecutor : DatabaseCreatorDropperAndSqlScriptExecutor
{
	public TestDatabaseCreatorDropperAndSqlScriptExecutor([NotNull] IDatabaseCreator? databaseCreator, [NotNull] IDatabaseDropper? databaseDropper, [NotNull] ISqlScriptExecutor? sqlScriptExecutor)
		: base(databaseCreator, databaseDropper, sqlScriptExecutor)
	{
	}
}
