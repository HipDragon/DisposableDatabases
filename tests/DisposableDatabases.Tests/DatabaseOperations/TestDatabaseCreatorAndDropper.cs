// <copyright file="TestDatabaseCreatorAndDropper.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using DisposableDatabases.DatabaseOperations;
using DisposableDatabases.Interfaces.DatabaseOperations;

namespace DisposableDatabases.Tests.DatabaseOperations;

public class TestDatabaseCreatorAndDropper : DatabaseCreatorAndDropper
{
	public TestDatabaseCreatorAndDropper([NotNull] IDatabaseCreator? databaseCreator, [NotNull] IDatabaseDropper? databaseDropper)
		: base(databaseCreator, databaseDropper)
	{
	}
}
