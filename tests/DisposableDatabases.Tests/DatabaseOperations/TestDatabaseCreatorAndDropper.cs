// <copyright file="TestDatabaseCreatorAndDropper.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using DisposableDatabases.DatabaseOperations;
using DisposableDatabases.Interfaces.DatabaseOperations;

namespace DisposableDatabases.Tests.DatabaseOperations;

public class TestDatabaseCreatorAndDropper : DatabaseCreatorAndDropper
{
	public TestDatabaseCreatorAndDropper(IDatabaseCreator? databaseCreator, IDatabaseDropper? databaseDropper)
		: base(databaseCreator, databaseDropper)
	{
	}
}
